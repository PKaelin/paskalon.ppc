// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.UnitTest
{
    /// <summary>
    /// Tests the final power calculation of <see cref="OperatingModeSignals"/>.
    /// </summary>
    [TestClass]
    public class OperatingModeSignalsTest
    {
        /// <summary>
        /// Confirms new signals have zero contributions and keep the supervisory setpoints.
        /// </summary>
        [TestMethod]
        public void OperatingModeSignalsInitialValuesTest()
        {
            OperatingModeSignals signals = new OperatingModeSignals { SetpointActivePower = new ActivePower(5000000), SetpointReactivePower = new ReactivePower(1000000) };

            Assert.AreEqual(5000000, signals.SetpointActivePower.Watts);
            Assert.AreEqual(1000000, signals.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, signals.FinalActivePower.Watts);
            Assert.AreEqual(0, signals.FinalReactivePower.VoltAmperesReactive);
            Assert.IsNull(signals.LimitedActivePower);
            Assert.IsNull(signals.LimitedReactivePower);
        }


        /// <summary>
        /// Confirms the final power is the base plus the additive deltas without a limit.
        /// </summary>
        [TestMethod]
        public void OperatingModeSignalsFinalPowerWithoutLimitTest()
        {
            OperatingModeSignals signals = CreateSignals();
            signals.BaseActivePower = new ActivePower(5000000);
            signals.BaseReactivePower = new ReactivePower(1000000);
            signals.AdditiveActivePowerDelta += new ActivePower(300000);
            signals.AdditiveActivePowerDelta += new ActivePower(-100000);
            signals.AdditiveReactivePowerDelta += new ReactivePower(200000);

            Assert.AreEqual(5200000, signals.FinalActivePower.Watts);
            Assert.AreEqual(1200000, signals.FinalReactivePower.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms chained limiting modes limit the already limited value.
        /// </summary>
        [TestMethod]
        public void OperatingModeSignalsFinalPowerChainedLimitsTest()
        {
            OperatingModeSignals signals = CreateSignals();
            signals.BaseActivePower = new ActivePower(5000000);
            signals.AdditiveActivePowerDelta = new ActivePower(1000000);
            signals.LimitedActivePower = new ActivePower(Math.Min(signals.FinalActivePower.Watts, 5500000));
            signals.LimitedActivePower = new ActivePower(Math.Min(signals.FinalActivePower.Watts, 5800000));
            signals.LimitedReactivePower = new ReactivePower(Math.Min(signals.FinalReactivePower.VoltAmperesReactive, -200000));

            Assert.AreEqual(5500000, signals.FinalActivePower.Watts);
            Assert.AreEqual(-200000, signals.FinalReactivePower.VoltAmperesReactive);
        }


        /// <summary>
        /// Creates signals with realistic supervisory setpoints.
        /// </summary>
        /// <returns>The operating mode signals.</returns>
        private OperatingModeSignals CreateSignals()
        {
            return new OperatingModeSignals { SetpointActivePower = new ActivePower(5000000), SetpointReactivePower = new ReactivePower(1000000) };
        }
    }
}
