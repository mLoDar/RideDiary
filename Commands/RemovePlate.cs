using System;
using System.Linq;
using System.Threading.Tasks;

using RideDiary.Scripts;

using Newtonsoft.Json.Linq;





namespace RideDiary.Commands
{
    internal class RemovePlate
    {
        private static JObject _rideDiaryData = new();





        internal static async Task Start()
        {
        LabelMethodBeginning:

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "RideDiary | Remove added number plate";

            DisplayUI.ResetConsole();



            LoadDataFromFile();

            if (_rideDiaryData.ContainsKey("error"))
            {
                await DisplayUI.DisplayError($"                 {_rideDiaryData["error"]}");
                return;
            }

            if (_rideDiaryData.ContainsKey("NumberPlates") == false)
            {
                _rideDiaryData["NumberPlates"] = new JArray();
            }



            DisplayUI.ResetConsole();



            JArray numberPlates = _rideDiaryData["NumberPlates"] as JArray ?? new();

            if (numberPlates.Count <= 0)
            {
                await DisplayUI.DisplayError("                 No number plates have been added yet");
                return;
            }



            NumberPlateExtras.DisplayNumberPlates(numberPlates);
            


            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ");
            Console.WriteLine("                 Enter the number of the plate to remove");



        LabelNumberInput:

            Console.Write("                 > ");

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
            _rideDiaryData["NumberPlates"] = numberPlates;



            JObject saveFileResult = await SaveFileHandler.SaveDataToFile(_rideDiaryData);

            if (saveFileResult.ContainsKey("error"))
            {
                await DisplayUI.DisplayError($"                 {saveFileResult["error"]}");
                return;
            }



            DisplayUI.ResetConsole();



            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                  ___________________________________________");
            Console.WriteLine("                 |                                           |");
            Console.Write("                 |  ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Successfully removed the selected plate");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  |");
            Console.WriteLine("                 |___________________________________________|");

            await Task.Delay(2000);
        }

        private static void LoadDataFromFile()
        {
            DisplayUI.ResetConsole();

            Console.Write("                 Loading save file");



            bool saveFileLoaded = false;



            Task loadSaveFile = new(async () =>
            {
                _rideDiaryData = await SaveFileHandler.LoadDataFromFile();

                saveFileLoaded = true;
            });

            Task loadingAnimation = new(async () =>
            {
                while (saveFileLoaded == false)
                {
                    Console.Write(" .");
                    await Task.Delay(1000);
                }
            });



            loadSaveFile.Start();
            loadingAnimation.Start();

            while (saveFileLoaded == false)
            {

            }
        }

        private static void DisplayConfirmationMenu(JObject plateToRemove, JProperty plateProperty)
        {
            string plateName = plateProperty.Name;
            string plateMaker = $"{plateToRemove?[plateName]?["Car_Maker"]}";
            string plateModel = $"{plateToRemove?[plateName]?["Car_Model"]}";

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 Number plate: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"'{plateName}'");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 Car maker: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"'{plateMaker}'");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 Car model: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"'{plateModel}'");


            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ");
            Console.Write("                 Are you sure you want to ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("REMOVE");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" the selected plate?");

            Console.WriteLine("                 ");
            Console.Write("                 > (y/n): ");
        }
    }
}