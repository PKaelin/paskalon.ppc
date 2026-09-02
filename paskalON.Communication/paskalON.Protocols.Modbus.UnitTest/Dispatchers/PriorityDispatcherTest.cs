// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Protocols.Modbus.Dispatchers;

namespace paskalON.Protocols.Modbus.UnitTest.Dispatchers
{
    [TestClass]
    public sealed class PriorityDispatcherTest
    {
        [TestMethod]
        public async Task PriorityDispatcherEnqueueAsyncReturnsActualValueTest()
        {
            PriorityDispatcher dispatcher = new PriorityDispatcher();
            dispatcher.Start();

            try
            {
                Task<int> result = dispatcher.EnqueueAsync<int>(ModbusOperation.Read, 10, 3, () => Task.FromResult(1234), CancellationToken.None);

                Assert.AreEqual(1234, await result);
            }
            finally
            {
                await dispatcher.StopAsync();
            }
        }


        [TestMethod]
        public async Task PriorityDispatcherEnqueueAsyncUsesPriorityAndFifoOrderTest()
        {
            PriorityDispatcher dispatcher = new PriorityDispatcher();
            dispatcher.Start();

            try
            {
                TaskCompletionSource<bool> firstStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
                TaskCompletionSource<bool> releaseFirst = new(TaskCreationOptions.RunContinuationsAsynchronously);
                List<int> executionOrder = [];

                Task first = dispatcher.EnqueueAsync(ModbusOperation.Write, 1, 5, async () =>
                {
                    executionOrder.Add(1);
                    firstStarted.SetResult(true);
                    await releaseFirst.Task;
                }, CancellationToken.None);

                await firstStarted.Task;
                Task second = dispatcher.EnqueueAsync(ModbusOperation.Write, 2, 1, () =>
                {
                    executionOrder.Add(2);
                    return Task.CompletedTask;
                }, CancellationToken.None);
                Task third = dispatcher.EnqueueAsync(ModbusOperation.Write, 3, 1, () =>
                {
                    executionOrder.Add(3);
                    return Task.CompletedTask;
                }, CancellationToken.None);

                releaseFirst.SetResult(true);
                await Task.WhenAll(first, second, third);

                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, executionOrder);
            }
            finally
            {
                await dispatcher.StopAsync();
            }
        }


        [TestMethod]
        public async Task PriorityDispatcherEnqueueAsyncDuplicateTest()
        {
            PriorityDispatcherTestClass dispatcher = new PriorityDispatcherTestClass();
            dispatcher.SetLoopTask(Task.CompletedTask);
            Task first = dispatcher.EnqueueAsync(ModbusOperation.Write, 20, 1, () => Task.CompletedTask, CancellationToken.None);
            Task duplicate1 = dispatcher.EnqueueAsync(ModbusOperation.Write, 20, 1, () => Task.CompletedTask, CancellationToken.None);
            Task duplicate2 = dispatcher.EnqueueAsync(ModbusOperation.Write, 20, 1, () => Task.CompletedTask, CancellationToken.None);

            Assert.AreEqual(1, dispatcher.QueueCount);
        }


        [TestMethod]
        public async Task PriorityDispatcherEnqueueAsyncPriorityTest()
        {
            PriorityDispatcherTestClass dispatcher = new PriorityDispatcherTestClass();
            dispatcher.SetLoopTask(Task.CompletedTask);
            List<int> prioOrder = [];
            Task prio2 = dispatcher.EnqueueAsync(ModbusOperation.Write, 30, 2, () => { prioOrder.Add(2); return Task.CompletedTask; }, CancellationToken.None);
            Task prio3 = dispatcher.EnqueueAsync(ModbusOperation.Write, 10, 3, () => { prioOrder.Add(3); return Task.CompletedTask; }, CancellationToken.None);
            Task prio1 = dispatcher.EnqueueAsync(ModbusOperation.Write, 40, 1, () => { prioOrder.Add(1); return Task.CompletedTask; }, CancellationToken.None);

            Assert.AreEqual(3, dispatcher.QueueCount);
            await dispatcher.CallActions();
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, prioOrder);
        }


        [TestMethod]
        public async Task PriorityDispatcherEnqueueAsyncWhenNotRunningFailsTest()
        {
            PriorityDispatcher dispatcher = new PriorityDispatcher();

            Task result = dispatcher.EnqueueAsync(ModbusOperation.Read, 1, 3, () => Task.CompletedTask, CancellationToken.None);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await result);
        }


        class PriorityDispatcherTestClass : PriorityDispatcher
        {
            public void SetLoopTask(Task task)
            {
                _loopTask = task;
            }

            public int QueueCount { get => _queue.Count; }

            public async Task CallActions()
            {
                while (_queue.Count > 0)
                {
                    WorkItem item = _queue.Dequeue();
                    _queueKeys.Remove(item.Key);
                    object? result = await item.Action().ConfigureAwait(false);
                    item.Completion.TrySetResult(result);
                }
            }
        }
    }
}
