using System;

namespace CanasUvighi
{
    /// <summary>
    /// AI class. Currently not really implemented AI.
    /// </summary>
    public static class AI
    {
        private static Random RNG = new Random();

        /// <summary>
        /// Simplest way to move NPCs. (TESTING)
        /// </summary>
        /// <returns>Random int [0-8) to be parsed to Cardinal Direction.</returns>
        public static int DrunkardWalk()
        {
            return RNG.Next(0, 8);
        }
    }
}
