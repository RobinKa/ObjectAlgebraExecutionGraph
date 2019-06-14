using System;
using System.Linq;

namespace ObjectAlgebraExecutionGraphs.Utility
{
    public static class RandomGenerator
    {
        private static Random random = new Random();

        const string lowerLetters = "abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Generates a sequence of lower-case letters of the specified length.
        /// </summary>
        /// <param name="length">Length of the resulting string.</param>
        /// <returns>Random sequence of lower-case letters of the specified length.</returns>
        public static string GetRandomLowerLetters(int length) => new string (Enumerable.Repeat(lowerLetters, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
