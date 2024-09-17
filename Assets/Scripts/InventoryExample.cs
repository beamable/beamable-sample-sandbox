using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using UnityEngine;

public class InventoryExample : MonoBehaviour
{
    private BeamContext _beamContext;

    private async void Start()
    {
        _beamContext = await BeamContext.Default.Instance;
        Debug.Log($"Beamable initialized, PlayerId: {_beamContext.PlayerId}");

        await GetInventory();
        await DeleteItem();
        await GetInventory();
    }

    public async Task GetInventory()
    {
        Debug.Log("Fetching inventory...");
        var items = _beamContext.Inventory.GetItems();

        await items.Refresh();
        foreach (var item in items)
        {
            Debug.Log($"Item id=[{item.ItemId}] type=[{item.ContentId}]");
        }

        Debug.Log("Inventory fetched successfully.");
    }

    public async Task DeleteItem()
    {
        Debug.Log("Deleting an item...");
        var items = _beamContext.Inventory.GetItems();
        await items.Refresh();

        if (items.Count > 0)
        {
            var itemToDelete = items[0];
            await _beamContext.Inventory.Update(builder => builder.DeleteItem(itemToDelete.ContentId, itemToDelete.ItemId));
            Debug.Log($"Deleted item id=[{itemToDelete.ItemId}] type=[{itemToDelete.ContentId}]");
        }
    }
}
