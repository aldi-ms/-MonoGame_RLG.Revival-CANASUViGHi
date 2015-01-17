using System;

namespace CanasUvighi
{
    /// <summary>
    /// AI class. Currently just for testing/future purposes.
    /// </summary>
    public static class AI
    {
        private static Random RNG = new Random();

        /// <summary>
        /// Test-move NPCs with random direction.
        /// </summary>
        /// <returns>Random CardinalDirection.</returns>
        public static CardinalDirection DrunkardWalk()
        {
            return (CardinalDirection)RNG.Next(0, 8);
        }
    }
}
