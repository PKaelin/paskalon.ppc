// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Messaging;
using paskalON.Messaging.Sequences;
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
        /// The source instance of the published setpoints.
        /// </summary>
        private const string SourceInstance = "OperatingMode-ppc-core-node1-5d4c3b2a19f84e7d8c6b5a4f3e2d1c0b";


        /// <summary>
        /// The creation time of the published setpoints.
        /// </summary>
        private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 10, 29, 8, 15, 0, TimeSpan.Zero);


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

            Assert.ThrowsExactly<ArgumentNullException>(() => new SetpointsPublisher(null!, _messagePublisher!.Object, topics, CreateSequencer()));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void SetpointsPublisherConstructorNullPublisherTest()
        {
            PublisherTopic topics = CreateTopics();

            Assert.ThrowsExactly<ArgumentNullException>(() => new SetpointsPublisher(NullLogger<SetpointsPublisher>.Instance, null!, topics, CreateSequencer()));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void SetpointsPublisherConstructorNullTopicsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new SetpointsPublisher(NullLogger<SetpointsPublisher>.Instance, _messagePublisher!.Object, null!,
                CreateSequencer()));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void SetpointsPublisherConstructorNullSequencerTest()
        {
            PublisherTopic topics = CreateTopics();

            Assert.ThrowsExactly<ArgumentNullException>(() => new SetpointsPublisher(NullLogger<SetpointsPublisher>.Instance, _messagePublisher!.Object, topics, null!));
        }


        /// <summary>
        /// Confirms the setpoints are published as stamped current setpoints DTO to the current setpoint topic.
        /// </summary>
        [TestMethod]
        public void SetpointsPublisherPublishSendsCurrentSetpointsTest()
        {
            SetpointsPublisher setpointsPublisher = CreatePublisher(CreateTopics());

            setpointsPublisher.Publish(ActivePower.FromMega(5), new ReactivePower(-1200000));

            CurrentSetpointsDto? dto = JsonSerializer.Deserialize<CurrentSetpointsDto>(_publishedJson!);
            Assert.AreEqual("ppc:om:setpoints", _publishedTopic);
            Assert.IsNotNull(dto);
            Assert.AreEqual(5000000, dto.SetpointActivePowerWatts);
            Assert.AreEqual(-1200000, dto.SetpointReactivePowerVars);
            Assert.AreEqual(SourceInstance, dto.SourceInstance);
            Assert.AreEqual(1, dto.Sequence);
            Assert.AreEqual(Now, dto.Timestamp);
        }


        /// <summary>
        /// Confirms the DER unit setpoints are published as stamped current DER unit setpoints DTO to the current DER unit setpoint topic.
        /// </summary>
        [TestMethod]
        public void SetpointsPublisherPublishUnitSendsCurrentDerUnitSetpointsTest()
        {
            SetpointsPublisher setpointsPublisher = CreatePublisher(CreateTopics());

            setpointsPublisher.PublishUnit("Bess1", ActivePower.FromMega(2), new ReactivePower(500000));

            CurrentDerUnitSetpointsDto? dto = JsonSerializer.Deserialize<CurrentDerUnitSetpointsDto>(_publishedJson!);
            Assert.AreEqual("ppc:om:unitsetpoints", _publishedTopic);
            Assert.IsNotNull(dto);
            Assert.AreEqual("Bess1", dto.DerUnitName);
            Assert.AreEqual(2000000, dto.SetpointActivePowerWatts);
            Assert.AreEqual(500000, dto.SetpointReactivePowerVars);
            Assert.AreEqual(SourceInstance, dto.SourceInstance);
            Assert.AreEqual(1, dto.Sequence);
            Assert.AreEqual(Now, dto.Timestamp);
        }


        /// <summary>
        /// Confirms every published message gets the next sequence number.
        /// </summary>
        [TestMethod]
        public void SetpointsPublisherPublishIncreasesSequenceTest()
        {
            List<string> messages = new List<string>();
            _messagePublisher!.Setup(publisher => publisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((_, json) => messages.Add(json))
                .Returns(Task.CompletedTask);
            SetpointsPublisher setpointsPublisher = CreatePublisher(CreateTopics());

            setpointsPublisher.Publish(ActivePower.FromMega(5), new ReactivePower(0));
            setpointsPublisher.PublishUnit("Bess1", ActivePower.FromMega(2), new ReactivePower(0));
            setpointsPublisher.Publish(ActivePower.FromMega(6), new ReactivePower(0));

            Assert.HasCount(3, messages);
            Assert.AreEqual(1, JsonSerializer.Deserialize<CurrentSetpointsDto>(messages[0])!.Sequence);
            Assert.AreEqual(2, JsonSerializer.Deserialize<CurrentDerUnitSetpointsDto>(messages[1])!.Sequence);
            Assert.AreEqual(3, JsonSerializer.Deserialize<CurrentSetpointsDto>(messages[2])!.Sequence);
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
            SetpointsPublisher setpointsPublisher = CreatePublisher(CreateTopics());

            Assert.ThrowsExactly<ArgumentException>(() => setpointsPublisher.PublishUnit(derUnitName, ActivePower.FromMega(2), new ReactivePower(0)));
        }


        /// <summary>
        /// Confirms nothing is published if the topics are not configured.
        /// </summary>
        [TestMethod]
        public void SetpointsPublisherPublishWithoutTopicsDoesNotPublishTest()
        {
            SetpointsPublisher setpointsPublisher = CreatePublisher(new PublisherTopic());

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
            SetpointsPublisher setpointsPublisher = CreatePublisher(CreateTopics());

            setpointsPublisher.Publish(ActivePower.FromMega(5), new ReactivePower(0));
            setpointsPublisher.PublishUnit("Bess1", ActivePower.FromMega(2), new ReactivePower(0));

            _messagePublisher!.Verify(publisher => publisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }


        /// <summary>
        /// Creates the setpoints publisher with the mocked message publisher.
        /// </summary>
        /// <param name="topics">The publisher topics.</param>
        /// <returns>The setpoints publisher.</returns>
        private SetpointsPublisher CreatePublisher(PublisherTopic topics)
        {
            return new SetpointsPublisher(NullLogger<SetpointsPublisher>.Instance, _messagePublisher!.Object, topics, CreateSequencer());
        }


        /// <summary>
        /// Creates the sequencer stamping the setpoints at a fixed time.
        /// </summary>
        /// <returns>The message sequencer.</returns>
        private MessageSequencer CreateSequencer()
        {
            return new MessageSequencer(SourceInstance, new FakeTimeProvider(Now));
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
