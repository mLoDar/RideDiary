using System;
using System.IO;
using System.Text.RegularExpressions;





namespace RideDiary.Resources
{
    internal partial class ApplicationValues
    {
        [GeneratedRegex("\\s+")]
        internal static partial Regex AllWhitespaces();



        internal static readonly string path_AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        internal static readonly string path_SaveFile = Path.Combine(path_AppData, "RideDiaryData.json");
    }
}