using System;





namespace RideDiary.Scripts
{
    internal class DisplayUI
    {
        internal static void RootMenu()
        {
            ResetUI();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 RideDiary");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 __________________");

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

        internal static void ResetUI()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 5);
        }
    }
}