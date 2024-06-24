using Newtonsoft.Json;

namespace vesa.Core.Extensions;

public static class DictionaryExtensions
{
    public static void SaveToFile(this Dictionary<string, string> dictionary, string filePath)
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the file content and deserialize it into a dictionary
            var fileContent = File.ReadAllText(filePath);
            var existingDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent);

            // Merge the existing dictionary with the new dictionary
            foreach (var kvp in dictionary)
            {
                existingDictionary[kvp.Key] = kvp.Value;
            }

            // Serialize the merged dictionary back to JSON
            var updatedJson = JsonConvert.SerializeObject(existingDictionary, Formatting.Indented);

            // Save the updated JSON to the file
            File.WriteAllText(filePath, updatedJson);
        }
        else
        {
            // If the file does not exist, create it and save the dictionary to it
            var json = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}
