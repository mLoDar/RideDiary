using System;
using System.Linq;
using System.Threading.Tasks;

using RideDiary.Scripts;

using Newtonsoft.Json.Linq;





namespace RideDiary.Commands
{
    internal class ShowStatistics
    {
        private static JObject rideDiaryData = new();





        internal static async Task Start()
        {
        LabelMethodBeginnging:

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "RideDiary | Statistics";

            DisplayUI.ResetConsole();



            LoadDataFromFile();

            if (rideDiaryData.ContainsKey("error"))
            {
                await DisplayUI.DisplayError($"                 {rideDiaryData["error"]}");
                return;
            }

            if (rideDiaryData.ContainsKey("NumberPlates") == false)
            {
                rideDiaryData["NumberPlates"] = new JArray();
            }



            DisplayUI.ResetConsole();



            JArray numberPlates = rideDiaryData["NumberPlates"] as JArray ?? new();

            if (numberPlates.Count <= 0)
            {
                await DisplayUI.DisplayError("                 No number plates have been added yet");
                return;
            }



            NumberPlateExtras.DisplayNumberPlates(numberPlates);


            
            /*if (numberPlates.Count >= 2)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("                 ");
                Console.Write("                 [A] ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Across all plates");
            }*/



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ");
            Console.WriteLine("                 Which statistics do you want to see?");



        LabelSelectionInput:

            Console.Write("                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string selectedOption = Console.ReadLine() ?? string.Empty;


            
            if (selectedOption.ToLower().Equals("a") == true)
            {
                // TODO: show statistics from all plates combined
            }
            
            
            
            if (NumberPlateExtras.ValidNumberPlateSelected(selectedOption, numberPlates) == false)
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelSelectionInput;
            }



            int plateIndex = Convert.ToInt32(selectedOption) - 1;
            JObject selectedPlate = numberPlates.ElementAtOrDefault(plateIndex) as JObject ?? new JObject();

            await ShowPlateStatistics(selectedPlate);



            Console.WriteLine("                 ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 __________________________________________");
            Console.WriteLine("                 ");


            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 [1]   Return to the number plate selection");
            Console.WriteLine("                 [ESC] Return to the root menu");
            Console.WriteLine("                 ");
            Console.Write("                 > ");



        LabelKeyRead:

            Console.ForegroundColor = ConsoleColor.Cyan;
            char pressedKey = Console.ReadKey(true).KeyChar;

            switch (pressedKey)
            {
                case '1':
                    goto LabelMethodBeginnging;

                case (char)ConsoleKey.Escape:
                    return;

                default:
                    goto LabelKeyRead;
            }
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

        private static async Task ShowPlateStatistics(JObject selectedPlate)
        {
            JProperty? plateProperty = selectedPlate.Properties().FirstOrDefault();

            if (plateProperty == null)
            {
                await DisplayUI.DisplayError("                 An unexpected error appeared, please try again");
                return;
            }



            Console.Title = $"RideDiary | Statistics for '{plateProperty.Name}'";
            DisplayUI.ResetConsole();



            JToken car_Maker = selectedPlate[plateProperty.Name]?["Car_Maker"] ?? "-";
            JToken car_Model = selectedPlate[plateProperty.Name]?["Car_Model"] ?? "-";

            if (car_Maker.Equals("-") || car_Model.Equals("-"))
            {
                await DisplayUI.DisplayError("                 An unexpected error appeared, please try again");
                return;
            }



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"                 Here are some stats for your {car_Maker} {car_Model}");
            Console.WriteLine("                 ");



            JArray plate_Trips = selectedPlate[plateProperty.Name]?["Collection_Trips"] as JArray ?? new JArray();
            JArray plate_Refuels = selectedPlate[plateProperty.Name]?["Collection_Refuels"] as JArray ?? new JArray();
            JArray plate_Expenses = selectedPlate[plateProperty.Name]?["Collection_Expenses"] as JArray ?? new JArray();
            


            if (plate_Trips.Count != 0)
            {
                int kilometersTotal = 0;

                foreach (JObject trip in plate_Trips.Cast<JObject>())
                {
                    _ = int.TryParse((string?)trip["Trip_KilometersStart"], out int trip_KilometersStart);
                    _ = int.TryParse((string?)trip["Trip_KilometersEnd"], out int trip_KilometersEnd);

                    int trip_TravelledKilometers = trip_KilometersEnd - trip_KilometersStart;

                    if (trip_TravelledKilometers > 0)
                    {
                        kilometersTotal += trip_TravelledKilometers;
                    }
                }

                int averageSpeedInKmhAsReference = 50;



                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("                 In total you travelled ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{kilometersTotal} kilometers");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" across ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{plate_Trips.Count} rides");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(".");



                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("                 When looked at, there is an average trip length of ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{decimal.Divide(kilometersTotal, plate_Trips.Count):0.00} kilometers");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(".");



                Console.WriteLine("                 ");



                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("                 Ever thought to yourself: 'That is a long trip I am on' ?");
                Console.Write("                 That could be because of the fact, that you spent about ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{decimal.Divide(kilometersTotal, averageSpeedInKmhAsReference):0.00} hours");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" in your car :o");

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("                 (NOTE: An average speed of ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{averageSpeedInKmhAsReference} km/h");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" was used as reference)");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("                 Sadly you have not saved any trips yet.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("                 How about you go on a ride? ;)");
            }



            Console.WriteLine("                 ");
            Console.WriteLine("                 ");



            if (plate_Refuels.Count != 0)
            {
                decimal refuel_PaidInTotal = 0;
                decimal refuel_LitersInTotal = 0;

                foreach (JObject refuel in plate_Refuels.Cast<JObject>())
                {
                    refuel_PaidInTotal += (decimal)refuel["Refuel_PaidInEuro"];
                    refuel_LitersInTotal += (decimal)refuel["Refuel_AmountAsLiter"];
                }



                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("                 As we all know, cars need fuel. According to the save file you refueled your car ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{plate_Refuels.Count} times");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(".");

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("                 When looking at it, you managed to get your hands on ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{refuel_LitersInTotal} liters");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" of fuel.");

                Console.WriteLine("                 An impressive amount, sadly everything has a price tag on it :/");
                Console.Write("                 You were charged ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{refuel_PaidInTotal}€");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" in total, which makes an average cost of ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{decimal.Divide(refuel_PaidInTotal, refuel_LitersInTotal):0.00}€");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" per liter.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("                 Sadly you have not saved any refuels yet.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("                 Is your car powered by air? *_*");
            }



            Console.WriteLine("                 ");
            Console.WriteLine("                 ");



            if (plate_Expenses.Count != 0)
            {
                decimal expenses_PaidInTotal = 0;

                foreach (JObject expense in plate_Expenses.Cast<JObject>())
                {
                    expenses_PaidInTotal += (decimal)expense["Expense_PaidInEuro"];
                }



                Console.WriteLine("                 When speaking about costs, there are some other expenses:");

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("                 Your ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{plate_Expenses.Count}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" different expenses sum up to ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{expenses_PaidInTotal}€");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(".");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("                 It seems you have not saved any expenses yet.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("                 Do you have a sponsor for that? ;)");
            }
        }
    }
}