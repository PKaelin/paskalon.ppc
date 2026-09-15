// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Numerics;

namespace paskalON.Maths.Randoms
{
    /// <summary>
    /// Random walker that walks around a home in steps with bounds.
    /// </summary>
    public class RandomWalker<T> where T : INumber<T>
    {
        /// <summary>
        /// Random instance.
        /// </summary>
        private readonly Random _random = new Random();


        /// <summary>
        /// Precision of the floating point walks.
        /// </summary>
        private readonly int _precision;

        /// <summary>
        /// Home value to walk around.
        /// </summary>
        public T Home { get; init; }


        /// <summary>
        /// Step size to walk up or down.
        /// </summary>
        public T StepSize { get; init; }


        /// <summary>
        /// Lower bound of the walk.
        /// </summary>
        public T LowerBound { get; init; }


        /// <summary>
        /// Upper bound of the walk.
        /// </summary>
        public T UpperBound { get; init; }


        /// <summary>
        /// Max distance to travel.
        /// </summary>
        public T MaxDistance { get; }


        /// <summary>
        /// Current walk value.
        /// </summary>
        public T CurrentValue { get; private set; }


        /// <summary>
        /// Constructor of <see cref="RandomWalker"/>.
        /// </summary>
        /// <param name="home">Home value to walk around.</param>
        /// <param name="stepSize">Step size to walk up or down.</param>
        /// <param name="lowerBound">Lower bound of the walk.</param>
        /// <param name="upperBound">Upper bound of the walk.</param>
        public RandomWalker(T home, T stepSize, T lowerBound, T upperBound, T maxDistance, int precision = 3)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(precision);

            if (lowerBound > home || upperBound < home)
            {
                throw new ArgumentException("Home must be within the specified lower and upper bounds.");
            }

            Home = home;
            StepSize = stepSize;
            LowerBound = lowerBound;
            UpperBound = upperBound;
            // Ensure distance limit is positive
            MaxDistance = T.Abs(maxDistance);
            CurrentValue = home;
            _precision = precision;
        }


        /// <summary>
        /// Execute next walk.
        /// </summary>
        /// <returns>Next random walk value.</returns>
        public T Next()
        {
            // Generate a random choice: 0 = Down, 1 = Stay at Home, 2 = Up
            int choice = _random.Next(0, 3);

            T newValue;

            if (choice == 0)
            {
                newValue = CurrentValue - StepSize;
            }
            else if (choice == 2)
            {
                newValue = CurrentValue + StepSize;
            }
            else
            {
                // Evaluate if we have strayed too far from home
                T currentDistanceFromHome = T.Abs(CurrentValue - Home);

                if (currentDistanceFromHome > MaxDistance)
                {
                    // Move closer to home instead of snapping
                    if (CurrentValue > Home)
                    {
                        newValue = CurrentValue - StepSize;
                    }
                    else
                    {
                        newValue = CurrentValue + StepSize;
                    }
                }
                else
                {
                    newValue = Home;
                }
            }

            // Check if the type is any floating-point type (float, double, decimal) and round
            if (typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(decimal))
            {
                double rounded = Math.Round(Convert.ToDouble(newValue), _precision);
                newValue = (T)Convert.ChangeType(rounded, typeof(T));
            }

            // Clamp the final value within bounds
            CurrentValue = T.Clamp(newValue, LowerBound, UpperBound);

            return CurrentValue;
        }
    }
}
