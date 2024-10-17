using System.Collections.Generic;
using Beamable;
using Beamable.Common;
using Beamable.Server.Clients;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    public Button updateButton;
    public Button addButton;
    private BeamContext _ctx;
    private ServiceClient _service;

    private async void Start()
    {
        _ctx = await BeamContext.Default.Instance;
        _service = new ServiceClient();
        Debug.Log(_ctx.PlayerId);
        updateButton.onClick.AddListener(OnUpdateButtonClick);
        addButton.onClick.AddListener(OnAddButtonClick);
    }

    private void OnUpdateButtonClick()
    {
        Debug.Log("Update button clicked!");

        // Example item data, replace with real data in production
        var itemRef = "items.SwordItem01";
        int itemId = 1; // Example item ID
        long currentAmount = 600;
        long amountToRemove = 1;
        var itemAmountProperty = "Amount";
        
        UpdateInventory(itemRef, itemId, currentAmount, amountToRemove, itemAmountProperty);
    }
    private void OnAddButtonClick()
    {
        Debug.Log("Add button clicked!");

        // Example item data, replace with real data in production
        var itemRef = "items.SwordItem02";
        long currentAmount = 200;
        var itemAmountProperty = "Amount";
        
        AddToInventory(itemRef, currentAmount, itemAmountProperty);
    }

    private void UpdateInventory(string itemRef, int itemId, long currentAmount, long amountToRemove, string itemAmountProperty)
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

            promise = _service.UpdateInventory(itemRef, itemId, itemProperties);
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
    
    private void AddToInventory(string itemRef, long amount, string itemAmountProperty)
    {
        Promise<Unit> promise;
        
        // add item properties
        var itemProperties = new Dictionary<string, string>
        {
            { itemAmountProperty, amount.ToString() }
        };

        promise = _service.AddToInventory(itemRef, itemProperties);
    

        // Promise handling for success or failure
        promise.Then(_ =>
        {
            Debug.Log($"Success adding {amount} of {itemRef}");
        }).Error(ex =>
        {
            Debug.LogError($"Error adding {amount} of {itemRef}: {ex.Message}");
        });
    }
}
