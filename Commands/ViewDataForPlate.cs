using System;
using System.Linq;
using System.Threading.Tasks;

using RideDiary.Scripts;
using RideDiary.Resources;

using Newtonsoft.Json.Linq;





namespace RideDiary.Commands
{
    internal class ViewDataForPlate
    {
        private static JObject rideDiaryData = new();





        internal static async Task Start()
        {
        LabelMethodBeginnging:
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
            Console.WriteLine($"                 Enter the number of the number plate whose data should be displayed");



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



            JToken car_Maker = selectedPlate[plateProperty.Name]?["Car_Maker"];
            JToken car_Model = selectedPlate[plateProperty.Name]?["Car_Model"];

            if (car_Maker == null || car_Model == null)
            {
                await DisplayUI.DisplayError($"                 An unexpected error appeared, please try again");
                return;
            }

            JArray plate_Trips = selectedPlate[plateProperty.Name]?["Collection_Trips"] as JArray ?? new JArray();
            JArray plate_Expenses = selectedPlate[plateProperty.Name]?["Collection_Expenses"] as JArray ?? new JArray();



        LabelDisplayMainData:

            DisplayData_Main(plateProperty, car_Maker, car_Model, plate_Trips, plate_Expenses);
            


        LabelKeyRead:

            Console.ForegroundColor = ConsoleColor.Cyan;
            char pressedKey = Console.ReadKey(true).KeyChar;

            switch (pressedKey)
            {
                case '1':
                    DisplayData_Trips(plate_Trips);
                    goto LabelDisplayMainData;

                case '2':
                    DisplayData_Expenses(plate_Expenses);
                    goto LabelDisplayMainData;

                case '3':
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

        private static void DisplayData_Main(JProperty plateProperty, JToken car_Maker, JToken car_Model, JArray plate_Trips, JArray plate_Expenses)
        {
            DisplayUI.ResetConsole();

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 Selected plate: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(plateProperty.Name);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 Car: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{car_Maker} {car_Model}");

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 Total trips: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(plate_Trips.Count);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 Total expenses:  ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(plate_Expenses.Count);

            Console.WriteLine("                 ");
            Console.WriteLine("                 ");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 [1] Display trips");
            Console.WriteLine("                 [2] Display expenses");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 __________________________");
            Console.WriteLine("                 ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 [3] Return to number plate selection");
            Console.WriteLine("                 ");
            Console.Write($"                 > ");
        }

        private static void DisplayData_Trips(JArray plate_Trips)
        {
            int tripsPerPage = 15;

            int arraySizeHelper = (int)Math.Ceiling(Convert.ToDecimal(plate_Trips.Count) / tripsPerPage);
            string[,] trip_Pages = new string[arraySizeHelper, tripsPerPage];



            int arrayHelperX = 0;
            int arrayHelperY = 0;
            int lengthHelper = 0;



            foreach (JObject trip in plate_Trips.Cast<JObject>())
            {
                int trip_KilometersStart = Convert.ToInt32(trip["Trip_KilometersStart"]);
                int trip_KilometersEnd = Convert.ToInt32(trip["Trip_KilometersEnd"]);
                int trip_KilometersTraveled = trip_KilometersEnd - trip_KilometersStart;

                if (trip_KilometersTraveled.ToString().Length > lengthHelper)
                {
                    lengthHelper = trip_KilometersTraveled.ToString().Length;
                }
            }



            foreach (JObject trip in plate_Trips.Cast<JObject>())
            {
                int trip_KilometersStart = Convert.ToInt32(trip["Trip_KilometersStart"]);
                int trip_KilometersEnd = Convert.ToInt32(trip["Trip_KilometersEnd"]);
                int trip_KilometersTraveled = trip_KilometersEnd - trip_KilometersStart;

                string trip_CombinedInformation = $"{trip["Trip_Date"]} | {trip_KilometersTraveled.ToString().PadLeft(lengthHelper)} km | {trip["Trip_Description"]}";

                trip_Pages[arrayHelperY, arrayHelperX] = trip_CombinedInformation;

                arrayHelperX++;

                if (arrayHelperX >= tripsPerPage)
                {
                    arrayHelperY++;
                    arrayHelperX = 0;
                }
            }



            int currentPageIndex = 0;


            
        LabelDisplayPage:

            DisplayUI.ResetConsole();
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < trip_Pages.GetLength(1); i++)
            {
                string singleTrip = trip_Pages[currentPageIndex, i];

                if (singleTrip != null && singleTrip.Equals(string.Empty) == false)
                {
                    string[] splittedInfo = singleTrip.Split('|');

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"                 {splittedInfo[0]}");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("|");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"{splittedInfo[1]}");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("|");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{splittedInfo[2]}");
                }
            }

            Console.WriteLine($"                 ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"                 ______________________");
            Console.ForegroundColor = ConsoleColor.White;

            if (currentPageIndex - 1 >= 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"                 <-");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" previuos page   ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"(A)");
            }
            
            if (currentPageIndex + 1 <= trip_Pages.GetLength(0) - 1)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"                 (D)");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"       next page ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"->");
            }
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"                 __________");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"                 [ESC] Exit");
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

                    if (currentPageIndex + 1 > trip_Pages.GetLength(0) - 1)
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

        private static void DisplayData_Expenses(JArray plate_Expenses)
        {

        }
    }
}