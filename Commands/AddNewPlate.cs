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
        private static JObject rideDiaryData = new();





        internal static async Task Start()
        {
            Console.Title = "RideDiary | Add new number plate";
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



            string car_Plate = ReadCarPlate();
            string car_Maker = ReadCarMaker();
            string car_Model = ReadCarModel();



            JObject newPlate = new()
            {
                [car_Plate] = new JObject()
                {
                    ["Car_Maker"] = car_Maker,
                    ["Car_Model"] = car_Model,
                    ["Collection_Trips"] = new JArray(),
                    ["Collection_Expenses"] = new JArray()
                }
            };



            JArray numberPlates = (rideDiaryData["numberPlates"] as JArray) ?? new JArray();
            numberPlates.Add(newPlate);

            rideDiaryData["numberPlates"] = numberPlates;



            JObject result = await SaveFileHandler.SaveDataToFile(rideDiaryData);

            if (result.ContainsKey("error"))
            {
                await DisplayUI.DisplayError($"                 {result["error"]}");
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
            string car_Plate = Console.ReadLine() ?? string.Empty;



            if (RegexPatterns.AllWhitespaces().Replace(car_Plate, string.Empty).Equals(string.Empty))
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelMethodBeginning;
            }



            JArray numberPlates = rideDiaryData["numberPlates"] as JArray ?? new JArray();

            foreach (JObject plate in numberPlates.Cast<JObject>())
            {
                if (plate.ContainsKey(car_Plate))
                {
                    DisplayUI.DisplayError("                 The entered plate was already added").Wait();

                    Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                    DisplayUI.ClearLine();

                    goto LabelMethodBeginning;
                }
            }



            return car_Plate;
        }

        private static string ReadCarMaker()
        {
        LabelMethodBeginning:

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 Enter the car's maker: ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string car_Maker = Console.ReadLine() ?? string.Empty;

            if (RegexPatterns.AllWhitespaces().Replace(car_Maker, string.Empty).Equals(string.Empty))
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelMethodBeginning;
            }

            return car_Maker;
        }

        private static string ReadCarModel()
        {
        LabelMethodBeginning:

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 Enter the car model: ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string car_Model = Console.ReadLine() ?? string.Empty;

            if (RegexPatterns.AllWhitespaces().Replace(car_Model, string.Empty).Equals(string.Empty))
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelMethodBeginning;
            }

            return car_Model;
        }
    }
}