using System.IO;
using System.Threading.Tasks;

using RideDiary.Resources;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;





namespace RideDiary.Scripts
{
    internal class SaveFileHandler
    {
        private static readonly string path_SaveFile = ApplicationValues.path_SaveFile;





        internal static async Task<JObject> LoadDataFromFile()
        {
            if (File.Exists(path_SaveFile) == false)
            {
                try
                {
                    await File.WriteAllTextAsync(path_SaveFile, "{}");
                }
                catch
                {
                    return JObject.Parse("{error: 'Failed to create new save file.'}");
                }
            }



            string jsonDataFromSaveFile = await File.ReadAllTextAsync(path_SaveFile);

            if (ApplicationValues.AllWhitespaces().Replace(jsonDataFromSaveFile, string.Empty).Equals(string.Empty))
            {
                try
                {
                    await File.WriteAllTextAsync(path_SaveFile, "{}");
                }
                catch
                {
                    return JObject.Parse("{error: 'Failed to overwrite existing save file.'}");
                }

                jsonDataFromSaveFile = "{}";
            }



            JObject rideDiaryData;

            try
            {
                rideDiaryData = JObject.Parse(jsonDataFromSaveFile);
            }
            catch
            {
                return JObject.Parse("{error: 'Failed to load data from save file. (Received malformed data)'}");
            }

            return rideDiaryData;
        }

        internal static async Task<JObject> SaveDataToFile(JObject rideDiaryData)
        {
            try
            {
                string jsonDataToSave = rideDiaryData.ToString(Formatting.None) ?? "{}";

                await File.WriteAllTextAsync(path_SaveFile, jsonDataToSave);

                return JObject.Parse("{success: 'true'}");
            }
            catch
            {
                return JObject.Parse("{error: 'Failed to write data to the save file.'}");
            }
        }
    }
}