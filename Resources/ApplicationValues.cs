using System;
using System.IO;





namespace RideDiary.Resources
{
    internal class ApplicationValues
    {
        internal static readonly string path_AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        internal static readonly string path_SaveFile = Path.Combine(path_AppData, "RideDiaryData.json");
    }
}