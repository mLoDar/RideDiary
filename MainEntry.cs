using System;





namespace RideDiary
{
    internal class MainEntry
    {
        internal static void Main()
        {
        LabelMethodBeginning:

            Console.Title = "RideDiary | Root";
            DisplayUI.RootMenu();



        LabelKeyRead:
            char pressedKey = Console.ReadKey(true).KeyChar;

            switch (pressedKey)
            {
                case '1':
                    break;

                case '2':
                    break;

                case '3':
                    break;

                case '4':
                    break;

                case (char)ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;

                default:
                    goto LabelKeyRead;
            }



            goto LabelMethodBeginning;
        }
    }
}