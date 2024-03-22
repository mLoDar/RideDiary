using System;
using System.Linq;
using System.Threading.Tasks;

using RideDiary.Scripts;

using Newtonsoft.Json.Linq;





namespace RideDiary.Commands
{
    internal class ShowStatistics
    {
        private static JObject _rideDiaryData = new();





        internal static async Task Start()
        {
        LabelMethodBeginnging:

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "RideDiary | Statistics";

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



            JToken carMaker = selectedPlate[plateProperty.Name]?["Car_Maker"] ?? "-";
            JToken carModel = selectedPlate[plateProperty.Name]?["Car_Model"] ?? "-";

            if (carMaker.Equals("-") || carModel.Equals("-"))
            {
                await DisplayUI.DisplayError("                 An unexpected error appeared, please try again");
                return;
            }



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"                 Here are some stats for your {carMaker} {carModel}");
            Console.WriteLine("                 ");



            JArray plateTrips = selectedPlate[plateProperty.Name]?["Collection_Trips"] as JArray ?? new JArray();
            JArray plateRefuels = selectedPlate[plateProperty.Name]?["Collection_Refuels"] as JArray ?? new JArray();
            JArray plateExpenses = selectedPlate[plateProperty.Name]?["Collection_Expenses"] as JArray ?? new JArray();
            


            if (plateTrips.Count != 0)
            {
                int kilometersTotal = 0;

                foreach (JObject trip in plateTrips.Cast<JObject>())
                {
                    _ = int.TryParse((string?)trip["Trip_KilometersStart"], out int tripKilometersStart);
                    _ = int.TryParse((string?)trip["Trip_KilometersEnd"], out int tripKilometersEnd);

                    int tripTravelledKilometers = tripKilometersEnd - tripKilometersStart;

                    if (tripTravelledKilometers > 0)
                    {
                        kilometersTotal += tripTravelledKilometers;
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
                Console.Write($"{plateTrips.Count} rides");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(".");



                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("                 When looked at, there is an average trip length of ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{decimal.Divide(kilometersTotal, plateTrips.Count):0.00} kilometers");
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



            if (plateRefuels.Count != 0)
            {
                decimal refuelPaidInTotal = 0;
                decimal refuelLitersInTotal = 0;

                foreach (JObject refuel in plateRefuels.Cast<JObject>())
                {
                    refuelPaidInTotal += (decimal)refuel["Refuel_PaidInEuro"];
                    refuelLitersInTotal += (decimal)refuel["Refuel_AmountAsLiter"];
                }



                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("                 As we all know, cars need fuel. According to the save file you refueled your car ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{plateRefuels.Count} times");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(".");

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("                 When looking at it, you managed to get your hands on ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{refuelLitersInTotal} liters");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" of fuel.");

                Console.Write("                 For refuelling you were charged ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{refuelPaidInTotal}€");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" in total, which makes an average cost of ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{decimal.Divide(refuelPaidInTotal, refuelLitersInTotal):0.00}€");
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



            if (plateExpenses.Count != 0)
            {
                decimal expensesPaidInTotal = 0;

                foreach (JObject expense in plateExpenses.Cast<JObject>())
                {
                    expensesPaidInTotal += (decimal)expense["Expense_PaidInEuro"];
                }



                Console.WriteLine("                 When speaking about costs, there are some other expenses:");

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("                 Your ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{plateExpenses.Count}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" different expenses sum up to ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{expensesPaidInTotal}€");
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