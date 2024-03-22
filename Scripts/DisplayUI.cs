using System;
using System.Threading.Tasks;





namespace RideDiary.Scripts
{
    internal class DisplayUI
    {
        internal static async Task RootMenu()
        {
            ResetConsole();



            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 ");

            string menuTitle = "RideDiary";

            foreach (char letter in menuTitle)
            {
                Console.Write(letter);
                await Task.Delay(50);
            }
            Console.WriteLine();



            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 ________________________________________");

            await Task.Delay(50);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ");
            Console.WriteLine("                 [1] Add new number plate");
            await Task.Delay(50);
            Console.WriteLine("                 [2] Add data to an existing number plate");
            await Task.Delay(50);
            Console.WriteLine("                 [3] View data for a number plate");
            await Task.Delay(50);
            Console.WriteLine("                 [4] Remove added number plate");
            await Task.Delay(50);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 ________________________________________");
            await Task.Delay(50);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ");
            Console.WriteLine("                 [5] Show Statistics");
            Console.WriteLine("                 ");
            Console.Write("                 > ");
        }

        internal static void ResetConsole()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 5);
        }

        internal static async Task DisplayError(string errorMessage, int timeInSeconds = 3)
        {
            ResetConsole();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);

            await Task.Delay(timeInSeconds * 1000);
        }

        internal static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
            Console.Write("\r" + new string(' ', Console.BufferWidth) + "\r");
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
        }
    }
}