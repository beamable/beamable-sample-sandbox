using System.Collections.Generic;
using Beamable;
using Beamable.Common;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    public Button updateButton;
    private BeamContext _ctx;

    private async void Start()
    {
        _ctx = await BeamContext.Default.Instance;
        updateButton.onClick.AddListener(OnUpdateButtonClick);
    }

    private void OnUpdateButtonClick()
    {
        Debug.Log("Update button clicked!");

        // Example item data, replace with real data in production
        var itemRef = "items.SwordItem01";
        long itemId = 1; // Example item ID
        long currentAmount = 500;
        long amountToRemove = 1;
        var itemAmountProperty = "Amount";

        // Simulating multiple rapid inventory updates
        for (var i = 0; i < 10; i++)
        {
            UpdateInventory(itemRef, itemId, currentAmount, amountToRemove, itemAmountProperty);
        }
    }

    private void UpdateInventory(string itemRef, long itemId, long currentAmount, long amountToRemove, string itemAmountProperty)
    {
        Promise<Unit> promise;

        if (currentAmount - amountToRemove == 0)
        {
            // Remove item if the new amount is zero
            promise = _ctx.Inventory.Update(builder =>
            {
                builder.DeleteItem(itemRef, itemId);
                Debug.Log($"Item {itemRef} marked for removal.");
            });
        }
        else
        {
            // Update item properties
            var newAmount = currentAmount - amountToRemove;
            var itemProperties = new Dictionary<string, string>
            {
                { itemAmountProperty, newAmount.ToString() }
            };

            promise = _ctx.Inventory.Update(builder =>
            {
                builder.UpdateItem(itemRef, itemId, itemProperties);
                Debug.Log($"Item {itemRef} updated to new amount: {newAmount}");
            });
        }

        // Promise handling for success or failure
        promise.Then(_ =>
        {
            Debug.Log($"Success updating/removing {amountToRemove} of {itemRef}");
        }).Error(ex =>
        {
            Debug.LogError($"Error updating/removing {amountToRemove} of {itemRef}: {ex.Message}");
        });
    }
}
