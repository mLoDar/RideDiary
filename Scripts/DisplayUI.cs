﻿using System;
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
            Console.WriteLine("                 [4] Remove added number plate");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 __________________");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ");
            Console.WriteLine("                 [5] Show Statistics");

            Console.Write("                 ");
        }

        internal static void ResetConsole()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 5);
        }

        internal static async Task DisplayError(string errorMessage)
        {
            ResetConsole();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);

            await Task.Delay(3000);
        }

        internal static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
            Console.Write("\r" + new string(' ', Console.BufferWidth) + "\r");
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
        }
    }
}