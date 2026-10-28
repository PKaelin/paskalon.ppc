// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Time.Testing;
using System.Net.Sockets;

namespace paskalON.Hosting.UnitTest
{
    [TestClass]
    public sealed class StartupRetryTest
    {
        private const string OperationName = "Initialize device client";


        [TestMethod]
        public async Task ExecuteAsyncFirstAttemptSucceedsTest()
        {
            FakeLogger<StartupRetry> logger = new FakeLogger<StartupRetry>();
            StartupRetry retry = new StartupRetry(logger, TimeProvider.System, new StartupRetryOptions(8, TimeSpan.Zero));
            FailingOperation operation = new FailingOperation(0, CreateConnectionError());

            await retry.ExecuteAsync(OperationName, operation.InvokeAsync, HttpTransientExceptionFilter.IsTransient);

            Assert.AreEqual(1, operation.Calls);
            Assert.AreEqual(0, logger.Collector.Count);
        }


        [TestMethod]
        [DataRow(1)]
        [DataRow(3)]
        [DataRow(8)]
        public async Task ExecuteAsyncTransientFailuresRetriedUntilSuccessTest(int failures)
        {
            FakeLogger<StartupRetry> logger = new FakeLogger<StartupRetry>();
            StartupRetry retry = new StartupRetry(logger, TimeProvider.System, new StartupRetryOptions(8, TimeSpan.Zero));
            FailingOperation operation = new FailingOperation(failures, CreateConnectionError());

            await retry.ExecuteAsync(OperationName, operation.InvokeAsync, HttpTransientExceptionFilter.IsTransient);

            Assert.AreEqual(failures + 1, operation.Calls);
            Assert.AreEqual(failures, logger.Collector.GetSnapshot().Count(r => r.Level == LogLevel.Warning));
            Assert.AreEqual(0, logger.Collector.GetSnapshot().Count(r => r.Level == LogLevel.Error));
        }


        [TestMethod]
        [DataRow(0)]
        [DataRow(3)]
        [DataRow(8)]
        public async Task ExecuteAsyncRetriesExhaustedRethrowsLastExceptionTest(int retryCount)
        {
            FakeLogger<StartupRetry> logger = new FakeLogger<StartupRetry>();
            StartupRetry retry = new StartupRetry(logger, TimeProvider.System, new StartupRetryOptions(retryCount, TimeSpan.Zero));
            HttpRequestException error = CreateConnectionError();
            FailingOperation operation = new FailingOperation(int.MaxValue, error);

            Func<Task> act = () => retry.ExecuteAsync(OperationName, operation.InvokeAsync, HttpTransientExceptionFilter.IsTransient);

            HttpRequestException thrown = await Assert.ThrowsExactlyAsync<HttpRequestException>(act);
            Assert.AreSame(error, thrown);
            Assert.AreEqual(retryCount + 1, operation.Calls);
            Assert.AreEqual(retryCount, logger.Collector.GetSnapshot().Count(r => r.Level == LogLevel.Warning));
            Assert.AreEqual(LogLevel.Error, logger.LatestRecord.Level);
        }


        [TestMethod]
        public async Task ExecuteAsyncNonTransientFailureRethrowsImmediatelyTest()
        {
            FakeLogger<StartupRetry> logger = new FakeLogger<StartupRetry>();
            StartupRetry retry = new StartupRetry(logger, TimeProvider.System, new StartupRetryOptions(8, TimeSpan.Zero));
            FailingOperation operation = new FailingOperation(int.MaxValue, new InvalidOperationException("Device server returned an empty DER data transfer object"));

            Func<Task> act = () => retry.ExecuteAsync(OperationName, operation.InvokeAsync, HttpTransientExceptionFilter.IsTransient);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(act);
            Assert.AreEqual(1, operation.Calls);
            Assert.AreEqual(0, logger.Collector.Count);
        }


        [TestMethod]
        public async Task ExecuteAsyncWaitsRetryIntervalBetweenAttemptsTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            StartupRetry retry = new StartupRetry(NullLogger.Instance, timeProvider, new StartupRetryOptions(8, TimeSpan.FromSeconds(15)));
            FailingOperation operation = new FailingOperation(1, CreateConnectionError());

            Task execution = retry.ExecuteAsync(OperationName, operation.InvokeAsync, HttpTransientExceptionFilter.IsTransient);
            timeProvider.Advance(TimeSpan.FromSeconds(14));
            bool completedBeforeInterval = execution.IsCompleted;
            int callsBeforeInterval = operation.Calls;
            timeProvider.Advance(TimeSpan.FromSeconds(1));
            await execution;

            Assert.IsFalse(completedBeforeInterval);
            Assert.AreEqual(1, callsBeforeInterval);
            Assert.AreEqual(2, operation.Calls);
        }


        [TestMethod]
        public async Task ExecuteAsyncReturnsOperationResultTest()
        {
            StartupRetry retry = new StartupRetry(NullLogger.Instance, TimeProvider.System, new StartupRetryOptions(8, TimeSpan.Zero));
            FailingOperation operation = new FailingOperation(2, new TimeoutException("Timeout during reading attempt"));

            string result = await retry.ExecuteAsync(OperationName, async () =>
            {
                await operation.InvokeAsync();

                return "SystemConfig";
            }, DatabaseTransientExceptionFilter.IsTransient);

            Assert.AreEqual("SystemConfig", result);
            Assert.AreEqual(3, operation.Calls);
        }


        [TestMethod]
        public async Task ExecuteAsyncCanceledBeforeStartDoesNotExecuteTest()
        {
            StartupRetry retry = new StartupRetry(NullLogger.Instance, TimeProvider.System, new StartupRetryOptions(8, TimeSpan.Zero));
            FailingOperation operation = new FailingOperation(0, CreateConnectionError());
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            Func<Task> act = () => retry.ExecuteAsync(OperationName, operation.InvokeAsync, HttpTransientExceptionFilter.IsTransient, cancellation.Token);

            await Assert.ThrowsAsync<OperationCanceledException>(act);
            Assert.AreEqual(0, operation.Calls);
        }


        [TestMethod]
        public async Task ExecuteAsyncCanceledDuringWaitStopsRetryingTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            StartupRetry retry = new StartupRetry(NullLogger.Instance, timeProvider, new StartupRetryOptions(8, TimeSpan.FromSeconds(15)));
            FailingOperation operation = new FailingOperation(int.MaxValue, CreateConnectionError());
            using CancellationTokenSource cancellation = new CancellationTokenSource();

            Task execution = retry.ExecuteAsync(OperationName, operation.InvokeAsync, HttpTransientExceptionFilter.IsTransient, cancellation.Token);
            cancellation.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => execution);
            Assert.AreEqual(1, operation.Calls);
        }


        [TestMethod]
        public void ConstructorNullArgumentsThrowTest()
        {
            StartupRetryOptions options = new StartupRetryOptions(8, TimeSpan.FromSeconds(15));

            Action nullLogger = () => new StartupRetry(null!, TimeProvider.System, options);
            Action nullTimeProvider = () => new StartupRetry(NullLogger.Instance, null!, options);
            Action nullOptions = () => new StartupRetry(NullLogger.Instance, TimeProvider.System, null!);

            Assert.ThrowsExactly<ArgumentNullException>(nullLogger);
            Assert.ThrowsExactly<ArgumentNullException>(nullTimeProvider);
            Assert.ThrowsExactly<ArgumentNullException>(nullOptions);
        }


        /// <summary>
        /// Create the exception raised by the HttpClient when the remote service refuses the connection.
        /// </summary>
        private static HttpRequestException CreateConnectionError()
        {
            return new HttpRequestException(HttpRequestError.ConnectionError, "Connection refused (paskalon.devices.service:8080)",
                new SocketException((int)SocketError.ConnectionRefused));
        }


        /// <summary>
        /// Operation that fails a given number of times with the given exception before it succeeds.
        /// </summary>
        private sealed class FailingOperation
        {
            private readonly int _failures;


            private readonly Exception _exception;


            public int Calls { get; private set; }


            public FailingOperation(int failures, Exception exception)
            {
                _failures = failures;
                _exception = exception;
            }


            public Task InvokeAsync()
            {
                Calls++;

                if (Calls <= _failures)
                {
                    return Task.FromException(_exception);
                }

                return Task.CompletedTask;
            }
        }
    }
}
