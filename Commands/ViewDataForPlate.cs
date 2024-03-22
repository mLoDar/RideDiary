using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;

using RideDiary.Scripts;

using Newtonsoft.Json.Linq;





namespace RideDiary.Commands
{
    internal class ViewDataForPlate
    {
        private static JObject _rideDiaryData = new();





        internal static async Task Start()
        {
        LabelMethodBeginnging:

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "RideDiary | Displaying data of plate";

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
            Console.WriteLine("                 Enter the number of the number plate whose data should be displayed");



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



            JToken carMaker = selectedPlate[plateProperty.Name]?["Car_Maker"];
            JToken carModel = selectedPlate[plateProperty.Name]?["Car_Model"];

            if (carMaker == null || carModel == null)
            {
                await DisplayUI.DisplayError("                 An unexpected error appeared, please try again");
                return;
            }

            JArray plateTrips = selectedPlate[plateProperty.Name]?["Collection_Trips"] as JArray ?? new JArray();
            JArray plateRefuels = selectedPlate[plateProperty.Name]?["Collection_Refuels"] as JArray ?? new JArray();
            JArray plateExpenses = selectedPlate[plateProperty.Name]?["Collection_Expenses"] as JArray ?? new JArray();
        


        LabelDisplayMainData:

            DisplayDataMain(plateProperty, carMaker, carModel, plateTrips, plateRefuels, plateExpenses);


            
        LabelKeyRead:

            Console.ForegroundColor = ConsoleColor.Cyan;
            char pressedKey = Console.ReadKey(true).KeyChar;

            switch (pressedKey)
            {
                case '1':
                    Console.Title = $"RideDiary | Displaying trip data for {plateProperty.Name}";
                    DisplayDataTrips(plateTrips);
                    goto LabelDisplayMainData;

                case '2':
                    Console.Title = $"RideDiary | Displaying expenses data for {plateProperty.Name}";
                    DisplayDataRefuels(plateRefuels);
                    goto LabelDisplayMainData;

                case '3':
                    Console.Title = $"RideDiary | Displaying refueling for {plateProperty.Name}";
                    DisplayDataExpenses(plateExpenses);
                    goto LabelDisplayMainData;

                case '4':
                    goto LabelMethodBeginnging;

                case (char)ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;

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

        private static void DisplayDataMain(JProperty plateProperty, JToken carMaker, JToken carModel, JArray plateTrips, JArray plateRefuels, JArray plateExpenses)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = $"RideDiary | Data of {plateProperty.Name}";



            DisplayUI.ResetConsole();

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 Selected plate: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(plateProperty.Name);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 Car: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{carMaker} {carModel}");

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 [1] Total trips: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(plateTrips.Count);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 [2] Total refuels:  ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(plateRefuels.Count);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 [3] Total expenses:  ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(plateExpenses.Count);

            Console.WriteLine("                 ");
            Console.WriteLine("                 ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 __________________________");
            Console.WriteLine("                 ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 [4] Return to number plate selection");
            Console.WriteLine("                 ");
            Console.Write("                 > ");
        }

        private static void DisplayDataTrips(JArray plateTrips)
        {
            if (plateTrips.Count <= 0)
            {
                return;
            }

            int dataEntriesPerPage = 15;

            int arraySizeHelper = (int)Math.Ceiling(Convert.ToDecimal(plateTrips.Count) / dataEntriesPerPage);
            string[,] tripPages = new string[arraySizeHelper, dataEntriesPerPage];



            int arrayHelperX = 0;
            int arrayHelperY = 0;
            int lengthHelper = 0;



            foreach (JObject trip in plateTrips.Cast<JObject>())
            {
                int trip_KilometersStart = Convert.ToInt32(trip["Trip_KilometersStart"]);
                int trip_KilometersEnd = Convert.ToInt32(trip["Trip_KilometersEnd"]);
                int trip_KilometersTraveled = trip_KilometersEnd - trip_KilometersStart;

                if (trip_KilometersTraveled.ToString().Length > lengthHelper)
                {
                    lengthHelper = trip_KilometersTraveled.ToString().Length;
                }
            }



            plateTrips = new JArray(plateTrips.OrderByDescending(obj => DateTime.Parse(obj["Trip_Date"].ToString())));



            foreach (JObject trip in plateTrips.Cast<JObject>())
            {
                int trip_KilometersStart = Convert.ToInt32(trip["Trip_KilometersStart"]);
                int trip_KilometersEnd = Convert.ToInt32(trip["Trip_KilometersEnd"]);
                int trip_KilometersTraveled = trip_KilometersEnd - trip_KilometersStart;

                string trip_CombinedInformation = $"{trip["Trip_Date"]} | {trip_KilometersTraveled.ToString().PadLeft(lengthHelper)} km | {trip["Trip_Description"]}";

                tripPages[arrayHelperY, arrayHelperX] = trip_CombinedInformation;

                arrayHelperX++;

                if (arrayHelperX >= dataEntriesPerPage)
                {
                    arrayHelperY++;
                    arrayHelperX = 0;
                }
            }



            int currentPageIndex = 0;


            
        LabelDisplayPage:
            
            DisplayUI.ResetConsole();
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < tripPages.GetLength(1); i++)
            {
                string singleTrip = tripPages[currentPageIndex, i];

                if (singleTrip != null && singleTrip.Equals(string.Empty) == false)
                {
                    string[] splittedInfo = singleTrip.Split('|');

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"                 {splittedInfo[0]}");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" | ");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"{splittedInfo[1]}");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" | ");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{splittedInfo[2]}");
                }
            }

            Console.WriteLine("                 ");

            if (plateTrips.Count - dataEntriesPerPage >= 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("                 ______________________");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (currentPageIndex - 1 >= 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("                 <-");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" previuos page   ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("(A)");
            }
            
            if (currentPageIndex + 1 <= tripPages.GetLength(0) - 1)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("                 (D)");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("       next page ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("->");
            }
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 __________");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 [ESC] Exit");
            Console.WriteLine("                 ");
            Console.Write("                 > ");



        LabelKeyRead:

            char pressedKey = Console.ReadKey(true).KeyChar;

            switch (pressedKey)
            {
                case 'a':

                    if (currentPageIndex - 1 < 0)
                    {
                        goto LabelKeyRead;
                    }

                    currentPageIndex--;
                    goto LabelDisplayPage;

                case 'd':

                    if (currentPageIndex + 1 > tripPages.GetLength(0) - 1)
                    {
                        goto LabelKeyRead;
                    }

                    currentPageIndex++;
                    goto LabelDisplayPage;

                case (char)ConsoleKey.Escape:
                    return;

                default:
                    goto LabelKeyRead;
            }
        }

        private static void DisplayDataRefuels(JArray plateRefuels)
        {
            if (plateRefuels.Count <= 0)
            {
                return;
            }

            int dataEntriesPerPage = 15;

            int arraySizeHelper = (int)Math.Ceiling(Convert.ToDecimal(plateRefuels.Count) / dataEntriesPerPage);
            string[,] refuelsPages = new string[arraySizeHelper, dataEntriesPerPage];



            int arrayHelperX = 0;
            int arrayHelperY = 0;

            int lengthHelperEuro = plateRefuels
                .Select(obj => Convert.ToDecimal(obj["Refuel_PaidInEuro"].ToString().Replace('.', ',')))
                .Select(number => number.ToString().Length)
                .Max();

            int lengthHelperLiter = plateRefuels
                .Select(obj => Convert.ToDecimal(obj["Refuel_AmountAsLiter"].ToString().Replace('.', ',')))
                .Select(number => number.ToString().Length)
                .Max();


            plateRefuels = new JArray(plateRefuels.OrderByDescending(obj => DateTime.Parse(obj["Refuel_Date"].ToString())));



            foreach (JObject refuel in plateRefuels.Cast<JObject>())
            {
                decimal.TryParse(Convert.ToString(refuel["Refuel_PaidInEuro"])?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal refuelAmountEuro);
                string refuelEuroFormatted = string.Format("{0:F2}", refuelAmountEuro);

                decimal.TryParse(Convert.ToString(refuel["Refuel_AmountAsLiter"])?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal refuelLiter);
                string refuelLiterFormatted = string.Format("{0:F2}", refuelLiter);

                string refuelCombinedInformation = $"{refuel["Refuel_Date"]} | {refuelEuroFormatted.PadRight(lengthHelperEuro)} € | {refuelLiterFormatted.PadRight(lengthHelperEuro)} Liters";

                refuelsPages[arrayHelperY, arrayHelperX] = refuelCombinedInformation;

                arrayHelperX++;

                if (arrayHelperX >= dataEntriesPerPage)
                {
                    arrayHelperY++;
                    arrayHelperX = 0;
                }
            }



            int currentPageIndex = 0;


            
        LabelDisplayPage:

            DisplayUI.ResetConsole();
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < refuelsPages.GetLength(1); i++)
            {
                string singleRefuel = refuelsPages[currentPageIndex, i];

                if (singleRefuel != null && singleRefuel.Equals(string.Empty) == false)
                {
                    string[] splittedInfo = singleRefuel.Split('|');

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"                 {splittedInfo[0]}");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" | ");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"{splittedInfo[1]}");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" | ");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{splittedInfo[2]}");
                }
            }

            Console.WriteLine("                 ");

            if (plateRefuels.Count - dataEntriesPerPage >= 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("                 ______________________");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (currentPageIndex - 1 >= 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("                 <-");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" previuos page   ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("(A)");
            }

            if (currentPageIndex + 1 <= refuelsPages.GetLength(0) - 1)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("                 (D)");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("       next page ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("->");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 __________");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 [ESC] Exit");
            Console.WriteLine("                 ");
            Console.Write("                 > ");



        LabelKeyRead:

            char pressedKey = Console.ReadKey(true).KeyChar;

            switch (pressedKey)
            {
                case 'a':

                    if (currentPageIndex - 1 < 0)
                    {
                        goto LabelKeyRead;
                    }

                    currentPageIndex--;
                    goto LabelDisplayPage;

                case 'd':

                    if (currentPageIndex + 1 > refuelsPages.GetLength(0) - 1)
                    {
                        goto LabelKeyRead;
                    }

                    currentPageIndex++;
                    goto LabelDisplayPage;

                case (char)ConsoleKey.Escape:
                    return;

                default:
                    goto LabelKeyRead;
            }
        }

        private static void DisplayDataExpenses(JArray plateExpenses)
        {
            if (plateExpenses.Count <= 0)
            {
                return;
            }

            int dataEntriesPerPage = 15;

            int arraySizeHelper = (int)Math.Ceiling(Convert.ToDecimal(plateExpenses.Count) / dataEntriesPerPage);
            string[,] expensePages = new string[arraySizeHelper, dataEntriesPerPage];



            int arrayHelperX = 0;
            int arrayHelperY = 0;

            int lengthHelper = plateExpenses
                .Select(obj => Convert.ToDecimal(obj["Expense_PaidInEuro"].ToString().Replace('.',',')))
                .Select(number => number.ToString().Length)
                .Max();



            plateExpenses = new JArray(plateExpenses.OrderByDescending(obj => DateTime.Parse(obj["Expense_Date"].ToString())));



            foreach (JObject expense in plateExpenses.Cast<JObject>())
            {
                decimal.TryParse(Convert.ToString(expense["Expense_PaidInEuro"])?.Replace(',','.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal expense_AmountEuro);
                string expenseFormatted = string.Format("{0:F2}", expense_AmountEuro);

                string expenseCombinedInformation = $"{expense["Expense_Date"]} | {expenseFormatted.PadLeft(lengthHelper)} € | {expense["Expense_Description"]}";

                expensePages[arrayHelperY, arrayHelperX] = expenseCombinedInformation;

                arrayHelperX++;

                if (arrayHelperX >= dataEntriesPerPage)
                {
                    arrayHelperY++;
                    arrayHelperX = 0;
                }
            }



            int currentPageIndex = 0;



        LabelDisplayPage:

            DisplayUI.ResetConsole();
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < expensePages.GetLength(1); i++)
            {
                string singleExpense = expensePages[currentPageIndex, i];

                if (singleExpense != null && singleExpense.Equals(string.Empty) == false)
                {
                    string[] splittedInfo = singleExpense.Split('|');

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"                 {splittedInfo[0]}");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" | ");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"{splittedInfo[1]}");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" | ");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{splittedInfo[2]}");
                }
            }

            Console.WriteLine("                 ");

            if (plateExpenses.Count - dataEntriesPerPage >= 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("                 ______________________");
                Console.ForegroundColor = ConsoleColor.White;
            }
            
            if (currentPageIndex - 1 >= 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("                 <-");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" previuos page   ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("(A)");
            }

            if (currentPageIndex + 1 <= expensePages.GetLength(0) - 1)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("                 (D)");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("       next page ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("->");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 __________");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 [ESC] Exit");
            Console.WriteLine("                 ");
            Console.Write("                 > ");



        LabelKeyRead:

            char pressedKey = Console.ReadKey(true).KeyChar;

            switch (pressedKey)
            {
                case 'a':

                    if (currentPageIndex - 1 < 0)
                    {
                        goto LabelKeyRead;
                    }

                    currentPageIndex--;
                    goto LabelDisplayPage;

                case 'd':

                    if (currentPageIndex + 1 > expensePages.GetLength(0) - 1)
                    {
                        goto LabelKeyRead;
                    }

                    currentPageIndex++;
                    goto LabelDisplayPage;

                case (char)ConsoleKey.Escape:
                    return;

                default:
                    goto LabelKeyRead;
            }
        }
    }
}