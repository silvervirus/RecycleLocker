using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nautilus.Handlers;
using RecycleCan;

public class RecycleLockerHandler : MonoBehaviour
{
    private StorageContainer storageContainer;

    private void Start()
    {
        // Load the banned items
        RecyclotronDataLoader.LoadBannedItems();

        // Get the StorageContainer component from this GameObject
        storageContainer = GetComponent<StorageContainer>();

        if (storageContainer != null)
        {
            // Subscribe to the OnAddItem event
            storageContainer.container.onAddItem += OnItemAdded;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the OnAddItem event
        if (storageContainer != null && storageContainer.container != null)
        {
            storageContainer.container.onAddItem -= OnItemAdded;
        }
    }

    private void OnItemAdded(InventoryItem item)
    {
        if (item == null || item.item == null)
        {
            return;
        }

        // Start recycling item
        StartCoroutine(RecycleItemAsync(item));
    }

    private IEnumerator RecycleItemAsync(InventoryItem item)
    {
        // Get the TechType of the item
        var techType = CraftData.GetTechType(item.item.gameObject);

        // Fetch the recipe for the item
        var recipe = CraftDataHandler.GetRecipeData(techType);

        if (recipe?.Ingredients != null)
        {
            Debug.Log($"Attempting to recycle item {techType}. Ingredients: {recipe.Ingredients.Count}");

            bool anyRecycled = false;

            foreach (var ingredient in recipe.Ingredients)
            {
                // Skip banned items
                if (RecyclotronDataLoader.BannedTechFromJson.Contains(ingredient.techType))
                {
                    Debug.LogWarning($"Banned ingredient {ingredient.techType} skipped.");
                    continue;
                }

                // Check if the ingredient is a battery, and if the item doesn't have one, skip it
                if (ingredient.techType == TechType.Battery)
                {
                    // Check if the item actually has a battery
                    EnergyMixin energyMixin = item.item.gameObject.GetComponent<EnergyMixin>();
                    if (energyMixin != null && energyMixin.GetBattery() == null)
                    {
                        Debug.Log($"Item {techType} doesn't have a battery, skipping battery recycling.");
                        continue; // Skip adding the battery if the item doesn't have one
                    }
                }

                // Add the ingredient directly to the player's inventory
                for (int i = 0; i < ingredient.amount; i++)
                {
                    CraftData.AddToInventory(ingredient.techType);
                    Debug.Log($"Recycled {ingredient.techType} added to inventory.");
                    anyRecycled = true;
                }
            }

            // If no items were recycled, inform the player and stop recycling
            if (!anyRecycled)
            {
                Debug.LogWarning(
                    $"Item {techType} contains only banned ingredients or no valid ingredients to recycle.");
                ErrorMessage.AddMessage(Language.main.Get("RecycleLockerErrorNoValidItems"));
                yield break;
            }
        }
        else
        {
            // No recipe found for the item
            Debug.LogWarning($"No recipe found for {techType}. Item cannot be recycled.");
            ErrorMessage.AddMessage(Language.main.Get("RecycleLockerErrorNoRecipe"));
            yield break;
        }

        // Remove the original item from the storage container only if some items were recycled
        storageContainer.container.RemoveItem(item.item, forced: true);
        Destroy(item.item.gameObject);
    }
}
