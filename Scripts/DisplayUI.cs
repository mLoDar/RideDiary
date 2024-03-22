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
            Console.Write("                 ");

            string menuFirstBar = "__________________";

            foreach (char dash in menuFirstBar)
            {
                Console.Write(dash);
                await Task.Delay(25);
            }
            Console.WriteLine();



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ");
            Console.WriteLine("                 [1] Add new number plate");
            Console.WriteLine("                 [2] Add data to an existing number plate");
            Console.WriteLine("                 [3] View data for a number plate");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 __________________");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ");
            Console.WriteLine("                 [4] Show Statistics");

            Console.Write("                 ");
        }

        internal static void ResetConsole()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 5);
        }
    }
}