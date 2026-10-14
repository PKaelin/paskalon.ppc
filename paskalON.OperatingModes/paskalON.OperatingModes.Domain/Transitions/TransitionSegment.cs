// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain.Transitions
{
    /// <summary>
    /// Running transition of the step caused by one operating mode, ramped to zero with the ramp of that operating mode.
    /// </summary>
    public sealed class TransitionSegment
    {
        /// <summary>
        /// Offsets smaller than this value in the base unit finish the segment.
        /// </summary>
        internal const double FinishTolerance = 3;


        /// <summary>
        /// Ramp controller ramping the offset to zero.
        /// </summary>
        private IRampController _rampController;


        /// <summary>
        /// Gets the name of the operating mode causing the segment.
        /// </summary>
        public string OperatingModeName { get; }


        /// <summary>
        /// Gets the kind of transition.
        /// </summary>
        public TransitionKind Kind { get; private set; }


        /// <summary>
        /// Gets the current offset in base unit.
        /// </summary>
        public double Offset { get; private set; }


        /// <summary>
        /// Gets whether the segment reached zero.
        /// </summary>
        public bool IsFinished { get => Math.Abs(Offset) < FinishTolerance; }



        /// <summary>
        /// Constructor of <see cref="TransitionSegment"/>. Starts ramping the offset to zero.
        /// </summary>
        /// <param name="operatingModeName">The name of the operating mode causing the segment.</param>
        /// <param name="kind">The kind of transition.</param>
        /// <param name="offset">The initial offset in base unit.</param>
        /// <param name="rampController">The ramp controller of the operating mode causing the segment.</param>
        internal TransitionSegment(string operatingModeName, TransitionKind kind, double offset, IRampController rampController)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(operatingModeName);
            ArgumentNullException.ThrowIfNull(rampController);

            OperatingModeName = operatingModeName;
            Kind = kind;
            _rampController = rampController;
            Restart(offset);
        }


        /// <summary>
        /// Advances the ramp to the current time.
        /// </summary>
        /// <returns>The current offset.</returns>
        internal double Advance()
        {
            if (IsFinished == true)
            {
                return 0;
            }

            Offset = _rampController.Calculate();

            if (IsFinished == true)
            {
                Stop();
            }

            return Offset;
        }


        /// <summary>
        /// Merges a further step of the same operating mode into this segment and continues with the given ramp.
        /// </summary>
        /// <param name="kind">The kind of the latest transition.</param>
        /// <param name="offset">The additional offset in base unit.</param>
        /// <param name="rampController">The ramp controller of the operating mode.</param>
        internal void Merge(TransitionKind kind, double offset, IRampController rampController)
        {
            ArgumentNullException.ThrowIfNull(rampController);

            _rampController.Stop();
            _rampController = rampController;
            Kind = kind;
            Restart(Offset + offset);
        }


        /// <summary>
        /// Scales the offset, e.g. after a mandatory constraint limited the applied target.
        /// </summary>
        /// <param name="factor">The scale factor between zero and one.</param>
        internal void Scale(double factor)
        {
            Restart(Offset * factor);
        }


        /// <summary>
        /// Stops the segment and resets the offset.
        /// </summary>
        internal void Stop()
        {
            _rampController.Stop();
            Offset = 0;
        }


        /// <summary>
        /// Restarts the ramp from the offset to zero.
        /// </summary>
        /// <param name="offset">The offset in base unit.</param>
        private void Restart(double offset)
        {
            Offset = offset;

            if (IsFinished == true)
            {
                Stop();

                return;
            }

            _rampController.Start(offset, 0);
        }
    }
}
