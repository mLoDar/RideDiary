using System;
using System.IO;





namespace RideDiary.Resources
{
    internal partial class ApplicationValues
    {
        internal static readonly string pathFolderAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        internal static readonly string pathSaveFile = Path.Combine(pathFolderAppData, "RideDiaryData.json");
    }
}