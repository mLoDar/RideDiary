using System;
using System.Linq;
using System.Threading.Tasks;

using RideDiary.Scripts;
using RideDiary.Resources;

using Newtonsoft.Json.Linq;





namespace RideDiary.Commands
{
    internal class RemovePlate
    {
        private static JObject rideDiaryData = new();





        internal static async Task Start()
        {
        LabelMethodBeginning:

            Console.Title = "RideDiary | Remove added number plate";
            DisplayUI.ResetConsole();

            LoadDataFromFile();

            if (rideDiaryData.ContainsKey("error"))
            {
                await DisplayUI.DisplayError($"                 {rideDiaryData["error"]}");
                return;
            }

            if (rideDiaryData.ContainsKey("numberPlates") == false)
            {
                rideDiaryData["numberPlates"] = new JArray();
            }



            DisplayUI.ResetConsole();



            JArray numberPlates = rideDiaryData["numberPlates"] as JArray ?? new();

            if (numberPlates.Count <= 0)
            {
                await DisplayUI.DisplayError("                 No number plates have been added yet");
                return;
            }


            NumberPlateExtras.DisplayNumberPlates(numberPlates);
            


            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"                 ");
            Console.WriteLine($"                 Enter the number of the plate to remove");



        LabelNumberInput:

            Console.Write($"                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string enteredNumber = Console.ReadLine() ?? string.Empty;

            if (NumberPlateExtras.ValidNumberPlateSelected(enteredNumber, numberPlates) == false)
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelNumberInput;
            }



            int plateIndex = Convert.ToInt32(enteredNumber) - 1;
            JObject plateToRemove = numberPlates.ElementAtOrDefault(plateIndex) as JObject ?? new JObject();
            JProperty? plateProperty = plateToRemove.Properties().FirstOrDefault();

            if (plateProperty == null)
            {
                await DisplayUI.DisplayError($"                 An unexpected error appeared, please try again");
                return;
            }



            DisplayUI.ResetConsole();



            DisplayConfirmationMenu(plateToRemove, plateProperty);

        LabelKeyRead:

            char pressedKey = Console.ReadKey(true).KeyChar;

            switch (pressedKey)
            {
                case 'y':
                    break;

                case 'n':
                    goto LabelMethodBeginning;

                case (char)ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;

                default:
                    goto LabelKeyRead;
            }



            numberPlates.ElementAtOrDefault(plateIndex)?.Remove();
            rideDiaryData["numberPlates"] = numberPlates;



            JObject result = await SaveFileHandler.SaveDataToFile(rideDiaryData);

            if (result.ContainsKey("error"))
            {
                await DisplayUI.DisplayError($"                 {result["error"]}");
                return;
            }



            DisplayUI.ResetConsole();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 Successfully removed the selected plate");
            await Task.Delay(3000);
        }

        private static void LoadDataFromFile()
        {
            DisplayUI.ResetConsole();

            Console.Write("                 Loading save file");



            bool saveFileLoaded = false;



            Task task_SaveFileLoading = new(async () =>
            {
                rideDiaryData = await SaveFileHandler.LoadDataFromFile();

                saveFileLoaded = true;
            });

            Task task_LoadingAnimation = new(async () =>
            {
                while (saveFileLoaded == false)
                {
                    Console.Write(" .");
                    await Task.Delay(1000);
                }
            });



            task_SaveFileLoading.Start();
            task_LoadingAnimation.Start();

            while (saveFileLoaded == false)
            {

            }
        }

        private static void DisplayConfirmationMenu(JObject plateToRemove, JProperty plateProperty)
        {
            string plate_Name = plateProperty.Name;
            string plate_Maker = $"{plateToRemove?[plate_Name]?["Car_Maker"]}";
            string plate_Model = $"{plateToRemove?[plate_Name]?["Car_Model"]}";

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"                 Number plate: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"'{plate_Name}'");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"                 Car maker: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"'{plate_Maker}'");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"                 Car model: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"'{plate_Model}'");


            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"                 ");
            Console.Write($"                 Are you sure you want to ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"REMOVE");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($" the selected plate?");

            Console.WriteLine($"                 ");
            Console.Write($"                 > (y/n): ");
        }
    }
}