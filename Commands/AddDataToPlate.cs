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



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ");
            Console.WriteLine("                 Enter the number of the plate to add data to");



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
            JObject selectedPlate = numberPlates.ElementAtOrDefault(plateIndex) as JObject ?? new JObject();
            JProperty? plateProperty = selectedPlate.Properties().FirstOrDefault();

            if (plateProperty == null)
            {
                await DisplayUI.DisplayError("                 An unexpected error appeared, please try again");
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
            Console.WriteLine("                 [2] New refuel");
            Console.WriteLine("                 [3] New expense");
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
                    selectedPlate = AddNewRefuel(selectedPlate, plateProperty.Name);
                    break;

                case '3':
                    selectedPlate = AddNewExpenses(selectedPlate, plateProperty.Name);
                    break;

                case (char)ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;

                default:
                    goto LabelKeyRead;
            }



            numberPlates[plateIndex] = selectedPlate;
            rideDiaryData["NumberPlates"] = numberPlates;



            JObject saveFileResult = await SaveFileHandler.SaveDataToFile(rideDiaryData);

            if (saveFileResult.ContainsKey("error"))
            {
                await DisplayUI.DisplayError($"                 {saveFileResult["error"]}");
                return;
            }



            DisplayUI.ResetConsole();



            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                  _________________________________________________");
            Console.WriteLine("                 |                                                 |");

            Console.Write("                 |  ");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Successfully added data to the selected plate");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  |");
            Console.WriteLine("                 |_________________________________________________|");
            Console.WriteLine("                 ");
            Console.WriteLine("                 ");

            await Task.Delay(2000);
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

        private static JObject AddNewTrip(JObject plateToAddDataTo, string numberPlate)
        {
            Console.Title = "RideDiary | Add new trip to plate";
            DisplayUI.ResetConsole();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ENTER INFORMATION");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 _________________");
            Console.WriteLine("                 ");



            string currentDate = DateTime.Now.ToShortDateString().Replace("/", ".");



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"                 Enter the date of the trip (day.month.year | e.g. {currentDate})");

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
            Console.WriteLine("                 Enter the kilometer reading at the beginning of the trip");

        LabelReadKilometersStart:

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string trip_KilometersStart = Console.ReadLine() ?? string.Empty;

            if (int.TryParse(trip_KilometersStart, out int parsed_KilometersStart) == false)
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

            if (int.TryParse(trip_KilometersEnd, out int parsed_KilometersEnd) == false || parsed_KilometersEnd < parsed_KilometersStart)
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelReadKilometersEnd;
            }



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 Enter a description for the trip");

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



            JArray collection_Trips = plateToAddDataTo[numberPlate]?["Collection_Trips"] as JArray ?? new JArray();

            collection_Trips.Add(
                new JObject()
                {
                    ["Trip_Date"] = trip_Date,
                    ["Trip_KilometersStart"] = parsed_KilometersStart,
                    ["Trip_KilometersEnd"] = parsed_KilometersEnd,
                    ["Trip_Description"] = trip_Description
                }
            );

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            plateToAddDataTo[numberPlate]["Collection_Trips"] = collection_Trips;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return plateToAddDataTo;
        }

        private static JObject AddNewRefuel(JObject plateToAddDataTo, string numberPlate)
        {
            Console.Title = "RideDiary | Add new refuel to plate";
            DisplayUI.ResetConsole();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ENTER INFORMATION");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 _________________");
            Console.WriteLine("                 ");



            string currentDate = DateTime.Now.ToShortDateString().Replace("/", ".");



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"                 Enter the date of the refuel (day.month.year | e.g. {currentDate})");

        LabelReadDate:

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string refuel_Date = Console.ReadLine() ?? string.Empty;

            if (DateTime.TryParseExact(refuel_Date, "dd.MM.yyyy", null, DateTimeStyles.None, out _) == false)
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelReadDate;
            }



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 Enter the amount paid in Euro");

        LabelReadEuro:

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string refuel_AmountEuro = Console.ReadLine() ?? string.Empty;
            refuel_AmountEuro = refuel_AmountEuro.Replace("€", string.Empty);

            if (decimal.TryParse(refuel_AmountEuro, out decimal parsed_AmountEuro) == false)
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelReadEuro;
            }



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 Enter the amount of liters of fuel");

        LabelReadLiters:

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string refuel_Liter = Console.ReadLine() ?? string.Empty;

            if (decimal.TryParse(refuel_Liter, out decimal parsed_Liters) == false)
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelReadLiters;
            }



            JArray collection_Expenses = plateToAddDataTo[numberPlate]?["Collection_Refuels"] as JArray ?? new JArray();

            collection_Expenses.Add(
                new JObject()
                {
                    ["Refuel_Date"] = refuel_Date,
                    ["Refuel_PaidInEuro"] = parsed_AmountEuro,
                    ["Refuel_AmountAsLiter"] = parsed_Liters
                }
            );

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            plateToAddDataTo[numberPlate]["Collection_Refuels"] = collection_Expenses;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return plateToAddDataTo;
        }

        private static JObject AddNewExpenses(JObject plateToAddDataTo, string numberPlate)
        {
            Console.Title = "RideDiary | Add new expense to plate";
            DisplayUI.ResetConsole();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 ENTER INFORMATION");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 _________________");
            Console.WriteLine("                 ");



            string currentDate = DateTime.Now.ToShortDateString().Replace("/", ".");



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"                 Enter the date of the expense (day.month.year | e.g. {currentDate})");

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
            Console.WriteLine("                 Enter the amount of the expense in Euro");

        LabelReadEuro:

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 > ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            string expenses_AmountEuro = Console.ReadLine() ?? string.Empty;
            expenses_AmountEuro = expenses_AmountEuro.Replace("€", string.Empty);

            if (decimal.TryParse(expenses_AmountEuro, out decimal parsed_AmountEuro) == false)
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);

                DisplayUI.ClearLine();

                goto LabelReadEuro;
            }



            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 Enter a description for the expense");

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
                    ["Expense_Date"] = expenses_Date,
                    ["Expense_PaidInEuro"] = parsed_AmountEuro,
                    ["Expense_Description"] = expenses_Description
                }
            );

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            plateToAddDataTo[numberPlate]["Collection_Expenses"] = collection_Expenses;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return plateToAddDataTo;
        }
    }
}