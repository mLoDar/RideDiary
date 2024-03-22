using System;
using System.Linq;
using System.Threading.Tasks;

using RideDiary.Scripts;
using RideDiary.Resources;

using Newtonsoft.Json.Linq;





namespace RideDiary.Commands
{
    internal class AddNewPlate
    {
        private static JObject _rideDiaryData = new();





        internal static async Task Start()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "RideDiary | Add new number plate";

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



            string carPlate = ReadCarPlate();
            string carMaker = ReadCarMaker();
            string carModel = ReadCarModel();



            JObject newPlate = new()
            {
                [carPlate] = new JObject()
                {
                    ["Car_Maker"] = carMaker,
                    ["Car_Model"] = carModel,
                    ["Collection_Trips"] = new JArray(),
                    ["Collection_Refuels"] = new JArray(),
                    ["Collection_Expenses"] = new JArray()
                }
            };



            JArray numberPlates = (_rideDiaryData["NumberPlates"] as JArray) ?? new JArray();
            numberPlates.Add(newPlate);

            _rideDiaryData["NumberPlates"] = numberPlates;



            JObject saveFileResult = await SaveFileHandler.SaveDataToFile(_rideDiaryData);

            if (saveFileResult.ContainsKey("error"))
            {
                await DisplayUI.DisplayError($"                 {saveFileResult["error"]}");
                return;
            }



            DisplayUI.ResetConsole();



            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                  ___________________________");
            Console.WriteLine("                 |                           |");

            Console.Write("                 |  ");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("The new plate was added");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  |");
            Console.WriteLine("                 |___________________________|");
            Console.WriteLine("                 ");
            Console.WriteLine("                 ");

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

        private static string ReadCarPlate()
        {
        LabelMethodBeginning:

            DisplayUI.ResetConsole();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ENTER INFORMATION");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 _________________");
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ");
            Console.Write("                 Enter the car's plate: ");



            Console.ForegroundColor = ConsoleColor.Cyan;
            string carPlate = Console.ReadLine() ?? string.Empty;



            if (RegexPatterns.AllWhitespaces().Replace(carPlate, string.Empty).Equals(string.Empty))
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelMethodBeginning;
            }



            JArray numberPlates = _rideDiaryData["NumberPlates"] as JArray ?? new JArray();

            foreach (JObject plate in numberPlates.Cast<JObject>())
            {
                if (plate.ContainsKey(carPlate))
                {
                    DisplayUI.DisplayError("                 The entered plate was already added").Wait();

                    Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                    DisplayUI.ClearLine();

                    goto LabelMethodBeginning;
                }
            }



            return carPlate;
        }

        private static string ReadCarMaker()
        {
        LabelMethodBeginning:

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 Enter the car's maker: ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string carMaker = Console.ReadLine() ?? string.Empty;

            if (RegexPatterns.AllWhitespaces().Replace(carMaker, string.Empty).Equals(string.Empty))
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelMethodBeginning;
            }

            return carMaker;
        }

        private static string ReadCarModel()
        {
        LabelMethodBeginning:

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 Enter the car model: ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string carModel = Console.ReadLine() ?? string.Empty;

            if (RegexPatterns.AllWhitespaces().Replace(carModel, string.Empty).Equals(string.Empty))
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelMethodBeginning;
            }

            return carModel;
        }
    }
}