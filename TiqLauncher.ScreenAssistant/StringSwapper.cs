using System;
using System.IO;
using System.Linq;
using System.Text;

namespace TiqLauncher.ScreenAssistant
{
    internal static class StringSwapper
    {
        public static void SwapFuzzyStrings(string exePath)
        {
            byte[] byteBuffer = File.ReadAllBytes(exePath);

            var swapSettings = SwappedStringsInfo.Load();
            var strings = swapSettings.GetStrings();
            for (var index = 0; index < strings.Count; index++)
            {
                var fuzzyString = strings[index];
                var foundIndex = -1;
                var stringBytes = Encoding.ASCII.GetBytes(fuzzyString.StringData);
                if (fuzzyString.Index < 0)
                {
                    for (var i = 0; i <= byteBuffer.Length - stringBytes.Length; i++)
                    {
                        if (byteBuffer[i] == stringBytes[0])
                        {
                            for (var j = 1; j < stringBytes.Length && byteBuffer[i + j] == stringBytes[j]; j++)
                            {
                                if (j == stringBytes.Length - 1)
                                {
                                    Console.WriteLine($@"'{fuzzyString.StringData}'          string was found at offset {i}");
                                    fuzzyString.Index = i;
                                    foundIndex = i;
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (var j = 0; j < stringBytes.Length && byteBuffer[fuzzyString.Index + j] == stringBytes[j]; j++)
                    {
                        if (j == stringBytes.Length - 1)
                        {
                            Console.WriteLine($@"'{fuzzyString.StringData}'          string offset was restored at {fuzzyString.Index}");
                            foundIndex = fuzzyString.Index;
                        }
                    }
                }

                if (foundIndex < 0)
                {
                    if (fuzzyString.Index >= 0)
                    {
                        fuzzyString.Index = -1;
                        index--;
                    }
                    else
                    {
                        Console.WriteLine(fuzzyString.StringData + @" was not found! Error with re-hashing!");
                    }
                }
            }

            foreach (var fuzzyString in strings.Where(x => x.Index >= 0))
            {
                var strBefore = fuzzyString.StringData;
                fuzzyString.MakeItFuzzy();
                var newBytes = Encoding.ASCII.GetBytes(fuzzyString.StringData);
                for (var i = 0; i < fuzzyString.StringData.Length; i++)
                {
                    byteBuffer[fuzzyString.Index + i] = newBytes[i];
                }

                Console.WriteLine($@"{strBefore} was shuffled into {fuzzyString.StringData}");
            }

            File.WriteAllBytes(exePath, byteBuffer);

            swapSettings.Save();
        }
    }
}
