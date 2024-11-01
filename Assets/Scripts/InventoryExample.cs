using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Api.Inventory;
using UnityEngine;

public class InventoryExample : MonoBehaviour
{
    private BeamContext _beamContext;

    [SerializeField]
    private List<string> inventoryItems;

    private async void Start()
    {
        _beamContext = await BeamContext.Default.Instance;
        Debug.Log($"Beamable initialized, PlayerId: {_beamContext.PlayerId}");
        

        await GetInventory();
    }

    private async Task GetInventory()
    {
        Debug.Log("Fetching inventory...");
        var items = _beamContext.Inventory.GetItems();
    
        await items.Refresh();
        inventoryItems.Clear();
    
        foreach (var item in items)
        {
            Debug.Log($"Item id=[{item.ItemId}] type=[{item.ContentId}]");
            inventoryItems.Add($"Item id=[{item.ItemId}], type=[{item.ContentId}]");
        }
    
        Debug.Log("Inventory fetched successfully.");
    }
    
    // private async Task GetInventory()
    // {
    //     Debug.Log("Fetching inventory...");
    //     var inventoryView = await _beamContext.Api.InventoryService.GetCurrent("");
    //
    //     foreach (var item in inventoryView.items)
    //     {
    //         List<ItemView> itemList = item.Value; // Access the List<ItemView> directly
    //         foreach (var itemView in itemList)
    //         {
    //             Debug.Log($"Item id=[{itemView.id}] type=[{itemView.contentId}]");
    //             inventoryItems.Add($"Item id=[{itemView.id}], type=[{itemView.contentId}]");
    //         }
    //     }
    //
    //     Debug.Log("Inventory fetched successfully.");
    // }



    private void AddToInventory(string itemRef, long amount, string itemAmountProperty)
    {
        var itemProperties = new Dictionary<string, string>
        {
            { itemAmountProperty, amount.ToString() }
        };

        _beamContext.Api.InventoryService.AddItem(itemRef, itemProperties).Then(_ =>
        {
            Debug.Log($"Success adding {amount} of {itemRef}");
        }).Error(ex =>
        {
            Debug.LogError($"Error adding {amount} of {itemRef}: {ex.Message}");
        });
    }
}