using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;

using RideDiary.Scripts;
using RideDiary.Resources;

using Newtonsoft.Json.Linq;





namespace RideDiary.Commands
{
    internal class AddDataToPlate
    {
        private static JObject rideDiaryData = new();





        internal static async Task Start()
        {
            Console.Title = "RideDiary | Add data to plate";
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


            DisplayNumberPlates(numberPlates);



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"                 ");
            Console.WriteLine($"                 Enter the number of the plate to add data to");



        LabelNumberInput:

            Console.Write($"                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string enteredNumber = Console.ReadLine() ?? string.Empty;

            if (ValidNumberPlateSelection(enteredNumber, numberPlates) == false)
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelNumberInput;
            }



            int plateIndex = Convert.ToInt32(enteredNumber) - 1;
            JObject selectedPlate = numberPlates.ElementAtOrDefault(plateIndex) as JObject ?? new JObject();
            JProperty? plateProperty = selectedPlate.Properties().FirstOrDefault();

            if (plateProperty == null)
            {
                await DisplayUI.DisplayError($"                 An unexpected error appeared, please try again");
                return;
            }



            DisplayUI.ResetConsole();



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 Which data do you want to add?");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 ______________________________");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ");
            Console.WriteLine("                 [1] New trip");
            Console.WriteLine("                 [2] New expenses");
            Console.WriteLine("                 ");
            Console.Write("                 > ");


        LabelKeyRead:

            Console.ForegroundColor = ConsoleColor.Cyan;
            char pressedKey = Console.ReadKey(true).KeyChar;

            switch (pressedKey)
            {
                case '1':
                    selectedPlate = AddNewTrip(selectedPlate, plateProperty.Name);
                    break;

                case '2':
                    selectedPlate = AddNewExpenses(selectedPlate, plateProperty.Name);
                    break;

                case (char)ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;

                default:
                    goto LabelKeyRead;
            }



            numberPlates[plateIndex] = selectedPlate;
            rideDiaryData["numberPlates"] = numberPlates;



            JObject result = await SaveFileHandler.SaveDataToFile(rideDiaryData);

            if (result.ContainsKey("error"))
            {
                await DisplayUI.DisplayError($"                 {result["error"]}");
                return;
            }



            DisplayUI.ResetConsole();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 Successfully added data to the selected plate");
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

        private static void DisplayNumberPlates(JArray numberPlates)
        {
            for (int i = 0; i < numberPlates.Count; i++)
            {
                JProperty? currentPlateProperty = (numberPlates.ElementAt(i) as JObject ?? new JObject()).Properties().FirstOrDefault();
                string numberPlate = currentPlateProperty?.Name ?? string.Empty;

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"                 [{i + 1}] ");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{numberPlate}");
            }
        }

        private static bool ValidNumberPlateSelection(string enteredNumber, JArray numberPlates)
        {
            if (RegexPatterns.AllWhitespaces().Replace(enteredNumber, string.Empty).Equals(string.Empty))
            {
                return false;
            }

            if (int.TryParse(enteredNumber, out int convertedNumber) == false)
            {
                return false;
            }

            if (Enumerable.Range(1, numberPlates.Count).Contains(convertedNumber) == false)
            {
                return false;
            }

            return true;
        }

        private static JObject AddNewTrip(JObject plateToAddDataTo, string numberPlate)
        {
            Console.Title = "RideDiary | Add new trip to plate";
            DisplayUI.ResetConsole();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ENTER INFORMATION");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 _________________");
            Console.WriteLine();


            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 Enter the kilometer reading at the beginning of the trip");
        LabelReadKilometersStart:
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string trip_KilometersStart = Console.ReadLine() ?? string.Empty;

            if (int.TryParse(trip_KilometersStart, out _) == false)
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelReadKilometersStart;
            }

            
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 Enter the kilometer reading at the end of the trip");
        LabelReadKilometersEnd:
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string trip_KilometersEnd = Console.ReadLine() ?? string.Empty;

            if (int.TryParse(trip_KilometersEnd, out int result) == false || result < Convert.ToInt32(trip_KilometersStart))
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelReadKilometersEnd;
            }



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 Enter the date of the trip (format: day.month.year)");
        LabelReadDate:
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string trip_Date = Console.ReadLine() ?? string.Empty;

            if (DateTime.TryParseExact(trip_Date, "dd.MM.yyyy", null, DateTimeStyles.None, out _) == false)
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelReadDate;
            }



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 Enter description for the trip");
        LabelReadDescription:
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string trip_Description = Console.ReadLine() ?? string.Empty;

            if (RegexPatterns.AllWhitespaces().Replace(trip_Description, string.Empty).Equals(string.Empty))
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelReadDescription;
            }



            JArray collection_Trips = plateToAddDataTo[numberPlate]["Collection_Trips"] as JArray ?? new JArray();

            collection_Trips.Add(
                new JObject()
                {
                    ["Trip_KilometersStart"] = trip_KilometersStart,
                    ["Trip_KilometersEnd"] = trip_KilometersEnd,
                    ["Trip_Date"] = trip_Date,
                    ["Trip_Description"] = trip_Description
                }
            );

            plateToAddDataTo[numberPlate]["Collection_Trips"] = collection_Trips;
            return plateToAddDataTo;
        }

        private static JObject AddNewExpenses(JObject plateToAddDataTo, string numberPlate)
        {
            Console.Title = "RideDiary | Add new expenses to plate";
            DisplayUI.ResetConsole();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ENTER INFORMATION");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 _________________");
            Console.WriteLine();


            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 Enter the amount of the expenses in Euro");
        LabelReadKilometersStart:
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string expenses_AmountEuro = Console.ReadLine() ?? string.Empty;
            expenses_AmountEuro.Replace("€", string.Empty);

            if (decimal.TryParse(expenses_AmountEuro, out _) == false)
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelReadKilometersStart;
            }



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 Enter the date of the expenses");
        LabelReadDate:
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string expenses_Date = Console.ReadLine() ?? string.Empty;

            if (DateTime.TryParseExact(expenses_Date, "dd.MM.yyyy", null, DateTimeStyles.None, out _) == false)
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelReadDate;
            }



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 Enter a description for the expenses");
        LabelReadDescription:
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string expenses_Description = Console.ReadLine() ?? string.Empty;

            if (RegexPatterns.AllWhitespaces().Replace(expenses_Description, string.Empty).Equals(string.Empty))
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelReadDescription;
            }



            JArray collection_Expenses = plateToAddDataTo[numberPlate]?["Collection_Expenses"] as JArray ?? new JArray();

            collection_Expenses.Add(
                new JObject()
                {
                    ["Expenses_Date"] = expenses_Date,
                    ["Expenses_AmountEuro"] = expenses_AmountEuro,
                    ["Expenses_Description"] = expenses_Description
                }
            );

            plateToAddDataTo[numberPlate]["Collection_Expenses"] = collection_Expenses;
            return plateToAddDataTo;
        }
    }
}