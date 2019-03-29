using System;
using System.Collections.Generic;

namespace TiqSoft.ScreenAssistant.Core
{
    internal static class StringHelper
    {
        /// <summary>
        /// Levenshtein distance -> https://en.wikipedia.org/wiki/Levenshtein_distance
        /// </summary>
        /// <param name="str1">String 1</param>
        /// <param name="str2">String 2</param>
        /// <returns>How many single character edits required to make string1 into string2.</returns>
        internal static int HowClose(this string str1, string str2)
        {
            int n = str1.Length, m = str2.Length;
            var d = new int[n + 1, m + 1];

            if (n == 0)
                return m;

            if (m == 0)
                return n;


            for (var i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (var j = 0; j <= m; d[0, j] = j++)
            {
            }

            for (var i = 1; i <= n; i++)
            for (var j = 1; j <= m; j++)
            {
                var cost = (str2[j - 1] == str1[i - 1]) ? 0 : 1;

                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
            return d[n, m];
        }

        public static string FindMostSimilar(this string searchFor, IEnumerable<string> words)
        {
            var result = "";
            var bestDistance = searchFor.Length / 2 + 1;
            foreach (var word in words)
            {
                var distance = word.HowClose(searchFor);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    result = word;
                }
            }

            return result;
        }
    }
}