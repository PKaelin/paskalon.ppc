// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.OperatingModes.Domain.Transitions;

namespace paskalON.OperatingModes.Domain.UnitTest.Transitions
{
    /// <summary>
    /// Tests the segmented bumpless transition of <see cref="AxisTransition"/>.
    /// </summary>
    [TestClass]
    public class AxisTransitionTest
    {
        /// <summary>
        /// Confirms a single request starts one segment with its step and its own ramp.
        /// </summary>
        /// <param name="offset">The step and required offset in Watts.</param>
        [TestMethod]
        [DataRow(-2000000)]
        [DataRow(5000000)]
        public void AxisTransitionStartSingleRequestTest(double offset)
        {
            AxisTransition transition = new AxisTransition();
            Mock<IRampController> rampController = new Mock<IRampController>();

            transition.Start(offset, new[] { CreateRequest("ActivePowerMode", TransitionKind.ModeReplacement, offset, rampController, true) });

            Assert.HasCount(1, transition.Segments);
            Assert.AreEqual(offset, transition.Offset);
            Assert.AreEqual(TransitionKind.ModeReplacement, transition.Kind);
            Assert.AreEqual("ActivePowerMode", transition.OperatingModeName);
            rampController.Verify(controller => controller.Start(offset, 0), Times.Once);
        }


        /// <summary>
        /// Confirms two changes in the same cycle start two segments with their own ramps and the remainder goes to the axis owner.
        /// </summary>
        [TestMethod]
        public void AxisTransitionStartAssignsRemainderToOwnerTest()
        {
            AxisTransition transition = new AxisTransition();
            Mock<IRampController> additiveRamp = new Mock<IRampController>();
            Mock<IRampController> baseRamp = new Mock<IRampController>();
            TransitionRequest[] requests = new[]
            {
                CreateRequest("FrequencyWattMode", TransitionKind.InitialActivation, -1000000, additiveRamp, false),
                CreateRequest("ActivePowerMode", TransitionKind.InitialActivation, -5000000, baseRamp, true),
            };

            transition.Start(-4000000, requests);

            Assert.HasCount(2, transition.Segments);
            Assert.AreEqual(-4000000, transition.Offset);
            additiveRamp.Verify(controller => controller.Start(-1000000, 0), Times.Once);
            baseRamp.Verify(controller => controller.Start(-3000000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms the remainder goes to the request with the largest step without an axis owner.
        /// </summary>
        [TestMethod]
        public void AxisTransitionStartAssignsRemainderToLargestWithoutOwnerTest()
        {
            AxisTransition transition = new AxisTransition();
            Mock<IRampController> firstRamp = new Mock<IRampController>();
            Mock<IRampController> secondRamp = new Mock<IRampController>();
            TransitionRequest[] requests = new[]
            {
                CreateRequest("FrequencyWattMode", TransitionKind.ModeRemoved, 500000, firstRamp, false),
                CreateRequest("FrequencyDroopMode", TransitionKind.ModeRemoved, 1000000, secondRamp, false),
            };

            transition.Start(2000000, requests);

            firstRamp.Verify(controller => controller.Start(500000, 0), Times.Once);
            secondRamp.Verify(controller => controller.Start(1500000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms no segment is started when the remainder cancels the step, e.g. a base mode replaced under a binding limit.
        /// </summary>
        [TestMethod]
        public void AxisTransitionStartWithoutRequiredOffsetAddsNoSegmentTest()
        {
            AxisTransition transition = new AxisTransition();
            Mock<IRampController> rampController = new Mock<IRampController>();

            transition.Start(0, new[] { CreateRequest("MaximumPowerPointTrackingMode", TransitionKind.ModeReplacement, -2000000, rampController, true) });

            Assert.IsEmpty(transition.Segments);
            Assert.AreEqual(TransitionKind.None, transition.Kind);
            rampController.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms segments ramp independently and finished segments are removed.
        /// </summary>
        [TestMethod]
        public void AxisTransitionAdvanceRunsSegmentsIndependentlyTest()
        {
            AxisTransition transition = new AxisTransition();
            Mock<IRampController> firstRamp = new Mock<IRampController>();
            Mock<IRampController> secondRamp = new Mock<IRampController>();
            firstRamp.Setup(controller => controller.Calculate()).Returns(500000);
            secondRamp.Setup(controller => controller.Calculate()).Returns(0);
            transition.Start(3000000, new[]
            {
                CreateRequest("FrequencyWattMode", TransitionKind.ModeRemoved, 1000000, firstRamp, false),
                CreateRequest("MaximumActivePowerLimitMode", TransitionKind.ModeRemoved, 2000000, secondRamp, false),
            });

            double offset = transition.Advance();

            // Still some (half) left
            Assert.AreEqual(500000, offset);
            // MaximumActivePowerLimitMode is finished and removed, FrequencyWattMode is still running
            Assert.HasCount(1, transition.Segments);
            Assert.AreEqual("FrequencyWattMode", transition.Segments[0].OperatingModeName);
            firstRamp.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            secondRamp.Verify(controller => controller.Stop(), Times.Once);
        }


        [TestMethod]
        public void AxisTransitionAdvanceRunsSegmentsIndependentlyUnevenRequiredTest()
        {
            AxisTransition transition = new AxisTransition();
            Mock<IRampController> firstRamp = new Mock<IRampController>();
            Mock<IRampController> secondRamp = new Mock<IRampController>();
            firstRamp.Setup(controller => controller.Calculate()).Returns(500000);
            secondRamp.Setup(controller => controller.Calculate()).Returns(0);
            transition.Start(4000000, new[]
            {
                CreateRequest("FrequencyWattMode", TransitionKind.ModeRemoved, 1000000, firstRamp, false),
                // Last in the list gets the remainder, so it will be 3000000, not 2000000 as the transition start request says 4000000 - 1000000 = 3000000
                CreateRequest("MaximumActivePowerLimitMode", TransitionKind.ModeRemoved, 2000000, secondRamp, false),
            });

            double offset = transition.Advance();

            // Still some (half) left
            Assert.AreEqual(500000, offset);
            // MaximumActivePowerLimitMode is finished and removed, FrequencyWattMode is still running
            Assert.HasCount(1, transition.Segments);
            Assert.AreEqual("FrequencyWattMode", transition.Segments[0].OperatingModeName);
            firstRamp.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            secondRamp.Verify(controller => controller.Stop(), Times.Once);
        }



        /// <summary>
        /// Confirms a change while another segment is running keeps the running segment and its ramp.
        /// </summary>
        [TestMethod]
        public void AxisTransitionStartWhileRunningKeepsRunningSegmentTest()
        {
            AxisTransition transition = new AxisTransition();
            Mock<IRampController> firstRamp = new Mock<IRampController>();
            Mock<IRampController> secondRamp = new Mock<IRampController>();
            firstRamp.Setup(controller => controller.Calculate()).Returns(-600000);
            transition.Start(-1000000, new[] { CreateRequest("FrequencyWattMode", TransitionKind.ModeAdded, -1000000, firstRamp, false) });
            double runningOffset = transition.Advance();

            transition.Start(-2000000, new[] { CreateRequest("FrequencyDroopMode", TransitionKind.ModeAdded, -2000000, secondRamp, false) });

            Assert.AreEqual(-600000, runningOffset);
            Assert.HasCount(2, transition.Segments);
            Assert.AreEqual(-2600000, transition.Offset);
            Assert.AreEqual("FrequencyDroopMode", transition.OperatingModeName);
            firstRamp.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            secondRamp.Verify(controller => controller.Start(-2000000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms a further change of the same operating mode is merged into its running segment.
        /// </summary>
        [TestMethod]
        public void AxisTransitionStartSameModeMergesTest()
        {
            AxisTransition transition = new AxisTransition();
            Mock<IRampController> firstRamp = new Mock<IRampController>();
            Mock<IRampController> secondRamp = new Mock<IRampController>();
            transition.Start(-1000000, new[] { CreateRequest("FrequencyWattMode", TransitionKind.ModeAdded, -1000000, firstRamp, false) });

            transition.Start(1000000, new[] { CreateRequest("FrequencyWattMode", TransitionKind.ModeRemoved, 1000000, secondRamp, false) });

            Assert.IsEmpty(transition.Segments);
            firstRamp.Verify(controller => controller.Stop(), Times.Once);
            secondRamp.Verify(controller => controller.Stop(), Times.Once);
        }


        /// <summary>
        /// Confirms a constraint in the direction of the running offset scales the segments.
        /// </summary>
        [TestMethod]
        public void AxisTransitionConstrainScalesSegmentsTest()
        {
            AxisTransition transition = new AxisTransition();
            Mock<IRampController> firstRamp = new Mock<IRampController>();
            Mock<IRampController> secondRamp = new Mock<IRampController>();
            transition.Start(-6000000, new[]
            {
                CreateRequest("ActivePowerMode", TransitionKind.InitialActivation, -4000000, firstRamp, true),
                CreateRequest("FrequencyWattMode", TransitionKind.InitialActivation, -2000000, secondRamp, false),
            });

            transition.Constrain(-3000000);

            Assert.AreEqual(-3000000, transition.Offset);
            firstRamp.Verify(controller => controller.Start(-2000000, 0), Times.Once);
            secondRamp.Verify(controller => controller.Start(-1000000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms a constraint against the direction of the running offset cancels the segments.
        /// </summary>
        [TestMethod]
        public void AxisTransitionConstrainOppositeCancelsSegmentsTest()
        {
            AxisTransition transition = new AxisTransition();
            Mock<IRampController> rampController = new Mock<IRampController>();
            transition.Start(-6000000, new[] { CreateRequest("ActivePowerMode", TransitionKind.InitialActivation, -6000000, rampController, true) });

            transition.Constrain(1000000);

            Assert.IsEmpty(transition.Segments);
            Assert.AreEqual(0, transition.Offset);
            rampController.Verify(controller => controller.Stop(), Times.Once);
        }


        /// <summary>
        /// Confirms cancelling stops all segments.
        /// </summary>
        [TestMethod]
        public void AxisTransitionCancelStopsSegmentsTest()
        {
            AxisTransition transition = new AxisTransition();
            Mock<IRampController> firstRamp = new Mock<IRampController>();
            Mock<IRampController> secondRamp = new Mock<IRampController>();
            transition.Start(3000000, new[]
            {
                CreateRequest("FrequencyWattMode", TransitionKind.ModeRemoved, 1000000, firstRamp, false),
                CreateRequest("MaximumActivePowerLimitMode", TransitionKind.ModeRemoved, 2000000, secondRamp, false),
            });

            transition.Cancel();

            Assert.IsEmpty(transition.Segments);
            Assert.AreEqual(TransitionKind.None, transition.Kind);
            firstRamp.Verify(controller => controller.Stop(), Times.Once);
            secondRamp.Verify(controller => controller.Stop(), Times.Once);
        }


        /// <summary>
        /// Confirms an empty request list has no effect and null is rejected.
        /// </summary>
        [TestMethod]
        public void AxisTransitionStartInvalidRequestsTest()
        {
            AxisTransition transition = new AxisTransition();

            transition.Start(1000000, Array.Empty<TransitionRequest>());

            Assert.IsEmpty(transition.Segments);
            Assert.ThrowsExactly<ArgumentNullException>(() => transition.Start(0, null!));
        }


        /// <summary>
        /// Creates a transition request.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        /// <param name="kind">The kind of transition.</param>
        /// <param name="offset">The step in Watts.</param>
        /// <param name="rampController">The mocked ramp controller.</param>
        /// <param name="ownsAxis">Whether the operating mode owns the axis.</param>
        /// <returns>The transition request.</returns>
        private TransitionRequest CreateRequest(string name, TransitionKind kind, double offset, Mock<IRampController> rampController, bool ownsAxis)
        {
            return new TransitionRequest(name, kind, offset, rampController.Object, ownsAxis);
        }
    }
}
