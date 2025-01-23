using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace RecycleCan
{
    public static class RecyclotronDataLoader
    {
        private static readonly List<TechType> bannedTechFromJson = new List<TechType>();
        private static bool isInitialized = false;

        // Load banned items from the banneditems.json file
        public static void LoadBannedItems()
        {
            if (isInitialized)
            {
                Debug.Log("Banned items are already loaded.");
                return;
            }

            // Get the directory of the executing assembly (DLL)
            string dllDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(dllDirectory))
            {
                Debug.LogError("Failed to locate DLL directory.");
                return;
            }

            string filePath = Path.Combine(dllDirectory, "banneditems.json");

            if (!File.Exists(filePath))
            {
                Debug.LogError($"banneditems.json not found in {dllDirectory}");
                return;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                Debug.Log("JSON file read successfully.");

                var bannedItemsJson = JsonUtility.FromJson<BannedItemsJson>(json);

                if (bannedItemsJson?.bannedItems != null)
                {
                    foreach (var bannedItem in bannedItemsJson.bannedItems)
                    {
                        if (Enum.TryParse(bannedItem, out TechType techType))
                        {
                            bannedTechFromJson.Add(techType);
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid TechType in banneditems.json: {bannedItem}");
                        }
                    }

                    Debug.Log($"Banned items loaded successfully. Total: {bannedTechFromJson.Count}");
                }
                else
                {
                    Debug.LogWarning("banneditems.json is empty or malformed.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load banneditems.json: {ex.Message}");
            }
            isInitialized = true;
        }


        // Accessor method for banned TechTypes
        public static List<TechType> BannedTechFromJson => new List<TechType>(bannedTechFromJson);
    }
    
}
