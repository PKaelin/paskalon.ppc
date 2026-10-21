// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Maths.Randoms;

namespace paskalON.DeviceSimulator.Equipments.Grids
{
    /// <summary>
    /// Simulated grid shared by all simulated devices (e.g. one grid frequency for every PCS and meter).
    /// </summary>
    public sealed class SimulatedGrid
    {
        /// <summary>
        /// Default nominal grid frequency in hertz.
        /// </summary>
        public const double DefaultNominalFrequency = 60;


        /// <summary>
        /// Default nominal line to line voltage at the point of interconnection in volts.
        /// </summary>
        public const double DefaultNominalVoltageLineToLine = 34500;


        /// <summary>
        /// Custom configuration key of the nominal grid frequency in hertz (system power meter).
        /// </summary>
        public const string NominalFrequencyKey = "SimGridNominalFrequency";


        /// <summary>
        /// Custom configuration key of the nominal line to line voltage in volts (system power meter).
        /// </summary>
        public const string NominalVoltageLineToLineKey = "SimGridNominalVoltageLineToLine";


        /// <summary>
        /// Maximum frequency deviation of the random walk in hertz.
        /// </summary>
        private const double FrequencyDeviation = 0.03;


        /// <summary>
        /// Data lock object.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Random walker for the frequency.
        /// </summary>
        private RandomWalker<double> _frequencyWalker;


        /// <summary>
        /// Nominal grid frequency in hertz.
        /// </summary>
        public double NominalFrequency
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <summary>
        /// Nominal line to line voltage at the point of interconnection in volts.
        /// </summary>
        public double NominalVoltageLineToLine
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <summary>
        /// Current grid frequency in hertz.
        /// </summary>
        public double Frequency
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <summary>
        /// Constructor of <see cref="SimulatedGrid"/>.
        /// </summary>
        public SimulatedGrid()
        {
            _frequencyWalker = CreateFrequencyWalker(DefaultNominalFrequency);
            NominalFrequency = DefaultNominalFrequency;
            NominalVoltageLineToLine = DefaultNominalVoltageLineToLine;
            Frequency = DefaultNominalFrequency;
        }


        /// <summary>
        /// Configures the nominal grid values.
        /// </summary>
        /// <param name="nominalFrequency">Nominal grid frequency in hertz.</param>
        /// <param name="nominalVoltageLineToLine">Nominal line to line voltage in volts.</param>
        public void Configure(double nominalFrequency, double nominalVoltageLineToLine)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(nominalFrequency);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(nominalVoltageLineToLine);

            lock (_dataLock)
            {
                _frequencyWalker = CreateFrequencyWalker(nominalFrequency);
                NominalFrequency = nominalFrequency;
                NominalVoltageLineToLine = nominalVoltageLineToLine;
                Frequency = nominalFrequency;
            }
        }


        /// <summary>
        /// Advances the grid by one simulation interval.
        /// </summary>
        public void Tick()
        {
            lock (_dataLock)
            {
                Frequency = _frequencyWalker.Next();
            }
        }


        /// <summary>
        /// Creates the frequency random walker around the nominal frequency.
        /// </summary>
        private static RandomWalker<double> CreateFrequencyWalker(double nominalFrequency)
        {
            return new RandomWalker<double>(nominalFrequency, 0.005, nominalFrequency - FrequencyDeviation, nominalFrequency + FrequencyDeviation, 1);
        }
    }
}
