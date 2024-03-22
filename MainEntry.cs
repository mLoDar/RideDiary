using System;

using RideDiary.Scripts;
using RideDiary.Commands;





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
                    AddNewPlate.Start();
                    break;

                case '2':
                    AddDataToPlate.Start();
                    break;

                case '3':
                    ViewDataForPlate.Start();
                    break;

                case '4':
                    ShowStatistics.Start();
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