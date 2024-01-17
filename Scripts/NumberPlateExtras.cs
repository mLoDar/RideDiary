using System;
using System.Linq;

using RideDiary.Resources;

using Newtonsoft.Json.Linq;






namespace RideDiary.Scripts
{
    internal class NumberPlateExtras
    {
        internal static bool ValidNumberPlateSelected(string enteredNumber, JArray numberPlates)
        {
            if (RegexPatterns.AllWhitespaces().Replace(enteredNumber, string.Empty).Equals(string.Empty))
            {
                return false;
            }

            if (int.TryParse(enteredNumber, out int convertedNumber) == false)
            {
                return false;
            }

            if (Enumerable.Range(1, numberPlates.Count).Contains(convertedNumber) == false)
            {
                return false;
            }

            return true;
        }

        internal static void DisplayNumberPlates(JArray numberPlates)
        {
            for (int i = 0; i < numberPlates.Count; i++)
            {
                JProperty? currentPlateProperty = (numberPlates.ElementAt(i) as JObject ?? new JObject()).Properties().FirstOrDefault();
                string numberPlate = currentPlateProperty?.Name ?? string.Empty;

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"                 [{i + 1}] ");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{numberPlate}");
            }
        }
    }
}