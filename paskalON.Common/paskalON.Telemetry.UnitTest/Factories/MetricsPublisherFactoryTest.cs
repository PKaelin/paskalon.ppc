// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Telemetry.Factories;

namespace paskalON.Telemetry.UnitTest.Factories
{
    [TestClass]
    public sealed class MetricsPublisherFactoryTest
    {
        [TestMethod]
        public void MetricsPublisherFactoryNullServiceProviderThrowsTest()
        {
            Action act = () => new MetricsPublisherFactory(null!);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        public void CreateReturnsPublisherFromServiceProviderTest()
        {
            using MetricsPublisher expectedPublisher = new MetricsPublisher(NullLogger<MetricsPublisher>.Instance);
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(provider => provider.GetService(typeof(IMetricsPublisher))).Returns(expectedPublisher);
            MetricsPublisherFactory factory = new MetricsPublisherFactory(serviceProviderMock.Object);

            IMetricsPublisher publisher = factory.Create();

            Assert.AreSame(expectedPublisher, publisher);
            serviceProviderMock.Verify(provider => provider.GetService(typeof(IMetricsPublisher)), Times.Once);
        }


        [TestMethod]
        public void CreatePublisherNotRegisteredThrowsTest()
        {
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(provider => provider.GetService(typeof(IMetricsPublisher))).Returns(null!);
            MetricsPublisherFactory factory = new MetricsPublisherFactory(serviceProviderMock.Object);

            Action act = () => factory.Create();

            Assert.ThrowsExactly<InvalidOperationException>(act);
        }


        [TestMethod]
        public void CreateTransientRegistrationReturnsNewUninitializedPublisherPerCallTest()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ILogger<MetricsPublisher>>(NullLogger<MetricsPublisher>.Instance);
            services.AddTransient<IMetricsPublisher, MetricsPublisher>();
            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            MetricsPublisherFactory factory = new MetricsPublisherFactory(serviceProvider);

            IMetricsPublisher firstPublisher = factory.Create();
            IMetricsPublisher secondPublisher = factory.Create();

            Assert.AreNotSame(firstPublisher, secondPublisher);
            Assert.IsInstanceOfType<MetricsPublisher>(firstPublisher);
            Assert.IsNull(firstPublisher.Meter);
            Assert.IsNull(secondPublisher.Meter);
        }
    }
}
