using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
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
        inventoryItems.Clear(); // Clear previous items before updating

        foreach (var item in items)
        {
            Debug.Log($"Item id=[{item.ItemId}] type=[{item.ContentId}]");
            inventoryItems.Add($"Item id=[{item.ItemId}], type=[{item.ContentId}]");
        }

        Debug.Log("Inventory fetched successfully.");
    }

}