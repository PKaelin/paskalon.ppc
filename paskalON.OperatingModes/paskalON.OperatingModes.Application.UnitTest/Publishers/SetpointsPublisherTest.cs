// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Messaging;
using paskalON.OperatingModes.Application.Publishers;
using paskalON.OperatingModes.Dto.Powers;
using paskalON.PhysicalUnits.Electricals.Powers;
using System.Text.Json;

namespace paskalON.OperatingModes.Application.UnitTest.Publishers
{
    /// <summary>
    /// Tests <see cref="SetpointsPublisher"/>.
    /// </summary>
    [TestClass]
    public sealed class SetpointsPublisherTest
    {
        /// <summary>
        /// The mocked message publisher.
        /// </summary>
        private Mock<IMessagePublisher>? _messagePublisher;


        /// <summary>
        /// The topic of the last published message.
        /// </summary>
        private string? _publishedTopic;


        /// <summary>
        /// The json of the last published message.
        /// </summary>
        private string? _publishedJson;


        /// <summary>
        /// Creates the mocked message publisher recording the last published message.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _publishedTopic = null;
            _publishedJson = null;
            _messagePublisher = new Mock<IMessagePublisher>();
            _messagePublisher.Setup(publisher => publisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((topic, json) =>
                {
                    _publishedTopic = topic;
                    _publishedJson = json;
                })
                .Returns(Task.CompletedTask);
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void SetpointsPublisherConstructorNullLoggerTest()
        {
            PublisherTopic topics = CreateTopics();

            Assert.ThrowsExactly<ArgumentNullException>(() => new SetpointsPublisher(null!, _messagePublisher!.Object, topics));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void SetpointsPublisherConstructorNullPublisherTest()
        {
            PublisherTopic topics = CreateTopics();

            Assert.ThrowsExactly<ArgumentNullException>(() => new SetpointsPublisher(NullLogger<SetpointsPublisher>.Instance, null!, topics));
        }



        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void SetpointsPublisherConstructorNullTopicsTest()
        {
            PublisherTopic topics = CreateTopics();

            Assert.ThrowsExactly<ArgumentNullException>(() => new SetpointsPublisher(NullLogger<SetpointsPublisher>.Instance, _messagePublisher!.Object, null!));
        }


        /// <summary>
        /// Confirms the setpoints are published as current setpoints DTO to the current setpoint topic.
        /// </summary>
        [TestMethod]
        public void SetpointsPublisherPublishSendsCurrentSetpointsTest()
        {
            SetpointsPublisher setpointsPublisher = new SetpointsPublisher(NullLogger<SetpointsPublisher>.Instance, _messagePublisher!.Object, CreateTopics());

            setpointsPublisher.Publish(ActivePower.FromMega(5), new ReactivePower(-1200000));

            CurrentSetpointsDto? dto = JsonSerializer.Deserialize<CurrentSetpointsDto>(_publishedJson!);
            Assert.AreEqual("ppc:om:setpoints", _publishedTopic);
            Assert.IsNotNull(dto);
            Assert.AreEqual(5000000, dto.SetpointActivePowerWatts);
            Assert.AreEqual(-1200000, dto.SetpointReactivePowerVars);
        }


        /// <summary>
        /// Confirms the DER unit setpoints are published as current DER unit setpoints DTO to the current DER unit setpoint topic.
        /// </summary>
        [TestMethod]
        public void SetpointsPublisherPublishUnitSendsCurrentDerUnitSetpointsTest()
        {
            SetpointsPublisher setpointsPublisher = new SetpointsPublisher(NullLogger<SetpointsPublisher>.Instance, _messagePublisher!.Object, CreateTopics());

            setpointsPublisher.PublishUnit("Bess1", ActivePower.FromMega(2), new ReactivePower(500000));

            CurrentDerUnitSetpointsDto? dto = JsonSerializer.Deserialize<CurrentDerUnitSetpointsDto>(_publishedJson!);
            Assert.AreEqual("ppc:om:unitsetpoints", _publishedTopic);
            Assert.IsNotNull(dto);
            Assert.AreEqual("Bess1", dto.DerUnitName);
            Assert.AreEqual(2000000, dto.SetpointActivePowerWatts);
            Assert.AreEqual(500000, dto.SetpointReactivePowerVars);
        }


        /// <summary>
        /// Confirms the DER unit name is validated.
        /// </summary>
        /// <param name="derUnitName">The invalid DER unit name.</param>
        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        public void SetpointsPublisherPublishUnitInvalidNameThrowsTest(string derUnitName)
        {
            SetpointsPublisher setpointsPublisher = new SetpointsPublisher(NullLogger<SetpointsPublisher>.Instance, _messagePublisher!.Object, CreateTopics());

            Assert.ThrowsExactly<ArgumentException>(() => setpointsPublisher.PublishUnit(derUnitName, ActivePower.FromMega(2), new ReactivePower(0)));
        }


        /// <summary>
        /// Confirms nothing is published if the topics are not configured.
        /// </summary>
        [TestMethod]
        public void SetpointsPublisherPublishWithoutTopicsDoesNotPublishTest()
        {
            SetpointsPublisher setpointsPublisher = new SetpointsPublisher(NullLogger<SetpointsPublisher>.Instance, _messagePublisher!.Object, new PublisherTopic());

            setpointsPublisher.Publish(ActivePower.FromMega(5), new ReactivePower(0));
            setpointsPublisher.PublishUnit("Bess1", ActivePower.FromMega(2), new ReactivePower(0));

            _messagePublisher!.Verify(publisher => publisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        /// <summary>
        /// Confirms a failing message publisher does not throw to the calculation cycle.
        /// </summary>
        [TestMethod]
        public void SetpointsPublisherPublishFailureDoesNotThrowTest()
        {
            _messagePublisher!.Setup(publisher => publisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromException(new InvalidOperationException("Broker not reachable.")));
            SetpointsPublisher setpointsPublisher = new SetpointsPublisher(NullLogger<SetpointsPublisher>.Instance, _messagePublisher!.Object, CreateTopics());

            setpointsPublisher.Publish(ActivePower.FromMega(5), new ReactivePower(0));
            setpointsPublisher.PublishUnit("Bess1", ActivePower.FromMega(2), new ReactivePower(0));

            _messagePublisher!.Verify(publisher => publisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }


        /// <summary>
        /// Creates the publisher topics of the setpoints.
        /// </summary>
        /// <returns>The publisher topics.</returns>
        private PublisherTopic CreateTopics()
        {
            return new PublisherTopic
            {
                CurrentSetpointTopic = "ppc:om:setpoints",
                CurrentDerUnitSetpointTopic = "ppc:om:unitsetpoints"
            };
        }
    }
}
