// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.Modbus.Dispatchers
{
    /// <summary>
    /// Modbus TCP allows only one in-flight request per connection, and the interface exposes a
    /// non-standard "priority" on every read/write. This dispatcher makes that meaningful: reads/writes are
    /// queued and executed one at a time, in priority order (lower value serviced first, default 3),
    /// FIFO within the same priority. Each caller's Task completes when their write has actually
    /// been sent and acknowledged (or failed).
    /// </summary>
    internal class PriorityDispatcher
    {
        /// <summary>
        /// Maximum queue size.
        /// </summary>
        private const int MaxQueueSize = 5000;


        /// <summary>
        /// Data lock object
        /// </summary>
        protected readonly object _dataLock = new();


        /// <summary>
        /// Work key structure to eliminate duplicates in the queue.
        /// </summary>
        protected readonly record struct WorkKey(ModbusOperation Operation, ushort Address);


        /// <summary>
        /// Work item structure for the queue.
        /// </summary>
        protected readonly record struct WorkItem(WorkKey Key, Func<Task<object?>> Action, TaskCompletionSource<object?> Completion, CancellationToken CancellationToken);


        /// <summary>
        /// Priority queue of work items.
        /// </summary>
        protected readonly PriorityQueue<WorkItem, short> _queue = new PriorityQueue<WorkItem, short>();


        /// <summary>
        /// List if queue keys that are in the priority queue for performance access.
        /// </summary>
        protected readonly HashSet<WorkKey> _queueKeys = new HashSet<WorkKey>();


        /// <summary>
        /// Signal for the dispatch worker. 
        /// </summary>
        /// <remarks>
        /// This is in combination with MaxQueueSize.
        /// </remarks>
        protected readonly SemaphoreSlim _signal = new(0, int.MaxValue);


        /// <summary>
        /// Cancellation token source for the loop.
        /// </summary>
        protected CancellationTokenSource? _loopCts;


        /// <summary>
        /// Loop task.
        /// </summary>
        protected Task? _loopTask;


        /// <summary>
        /// Starts the dispatcher loop.
        /// </summary>
        public void Start()
        {
            if (_loopTask is null)
            {
                _loopCts = new CancellationTokenSource();
                _loopTask = Task.Run(() => RunAsync(_loopCts.Token));
            }
        }


        /// <summary>
        /// Stops dispatcher loop.
        /// </summary>
        public async Task StopAsync()
        {
            if (_loopCts is not null)
            {
                _loopCts.Cancel();
                _signal.Release();

                if (_loopTask is not null)
                {
                    try { await _loopTask.ConfigureAwait(false); }
                    catch (OperationCanceledException) { /* expected */ }
                }

                // Fail anything left in the queue so callers don't hang forever.
                lock (_dataLock)
                {
                    while (_queue.TryDequeue(out var pending, out _))
                    {
                        pending.Completion.TrySetCanceled();
                    }

                    _queueKeys.Clear();
                }

                _loopCts = null;
                _loopTask = null;
            }
        }


        /// <summary>
        /// Enqueues a message onto the priority queue.
        /// </summary>
        /// <param name="operation">Modbus operation for key.</param>
        /// <param name="address">Modbus address.</param>
        /// <param name="priority">Priority of the message.</param>
        /// <param name="action">Action to execute by the queue.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task</returns>
        public Task EnqueueAsync(ModbusOperation operation, ushort address, short priority, Func<Task> action, CancellationToken cancellationToken)
        {
            return EnqueueCoreAsync(operation, address, priority, async () => { await action().ConfigureAwait(false); return null; }, cancellationToken);
        }


        /// <summary>
        /// Enqueues a message onto the priority queue.
        /// </summary>
        /// <typeparam name="T">Type of the return values.</typeparam>
        /// <param name="operation">Modbus operation for key.</param>
        /// <param name="address">Modbus address.</param>
        /// <param name="priority">Priority of the message.</param>
        /// <param name="action">Action to execute by the queue.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Generic Task</returns>
        public async Task<T> EnqueueAsync<T>(ModbusOperation operation, ushort address, short priority, Func<Task<T>> action, CancellationToken cancellationToken)
        {
            object? result = await EnqueueCoreAsync(operation, address, priority, async () => await action().ConfigureAwait(false), cancellationToken).ConfigureAwait(false);

            return (T)result!;
        }


        /// <summary>
        /// Enqueues a message onto the priority queue.
        /// </summary>
        /// <param name="operation">Modbus operation for key.</param>
        /// <param name="address">Modbus address.</param>
        /// <param name="priority">Priority of the message.</param>
        /// <param name="action">Action to execute by the queue.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Generic Task</returns>
        private Task<object?> EnqueueCoreAsync(ModbusOperation operation, ushort address, short priority, Func<Task<object?>> action, CancellationToken cancellationToken)
        {
            TaskCompletionSource<object?>? tcs = null;
            WorkKey key = new WorkKey(operation, address);
            bool added = false;

            if (_loopTask is not null)
            {
                lock (_dataLock)
                {
                    if (_queueKeys.Contains(key) == false && _queueKeys.Count < MaxQueueSize)
                    {
                        tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
                        WorkItem item = new WorkItem(key, action, tcs, cancellationToken);
                        _queue.Enqueue(item, priority);
                        _queueKeys.Add(key);
                        added = true;
                    }
                }

                if (added)
                {
                    _signal.Release();
                }
            }

            return tcs != null ? tcs.Task : Task.FromException<object?>(new InvalidOperationException("The dispatcher is not running."));
        }


        /// <summary>
        /// Runs the dispatcher in a loop.
        /// </summary>
        /// <param name="loopToken">Loop token.</param>
        /// <returns>Task</returns>
        private async Task RunAsync(CancellationToken loopToken)
        {
            while (true)
            {
                WorkItem item;
                await _signal.WaitAsync(loopToken).ConfigureAwait(false);

                if (loopToken.IsCancellationRequested)
                {
                    return;
                }

                lock (_dataLock)
                {
                    if (_queue.TryDequeue(out item, out _) == false)
                    {
                        continue;
                    }
                }

                lock (_dataLock)
                {
                    _queueKeys.Remove(item.Key);
                }

                if (item.CancellationToken.IsCancellationRequested)
                {
                    item.Completion.TrySetCanceled(item.CancellationToken);
                    continue;
                }

                try
                {
                    object? result = await item.Action().ConfigureAwait(false);
                    item.Completion.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    item.Completion.TrySetException(ex);
                }
            }
        }
    }
}