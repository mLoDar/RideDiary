﻿using System;
using System.Threading.Tasks;

using RideDiary.Scripts;
using RideDiary.Commands;





namespace RideDiary
{
    internal class MainEntry
    {
        internal static async Task Main()
        {
        LabelMethodBeginning:

            Console.Title = "RideDiary | Root";
            await DisplayUI.RootMenu();



        LabelKeyRead:
            char pressedKey = Console.ReadKey(true).KeyChar;

            switch (pressedKey)
            {
                case '1':
                    await AddNewPlate.Start();
                    break;

                case '2':
                    await AddDataToPlate.Start();
                    break;

                case '3':
                    await ViewDataForPlate.Start();
                    break;

                case '4':
                    await ShowStatistics.Start();
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