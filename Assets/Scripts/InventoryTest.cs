using System;
using Beamable;
using Beamable.Server.Clients;
using UnityEngine;

namespace DefaultNamespace
{
    public class InventoryTest: MonoBehaviour
    {
        private ServiceClient _service;
        private async void Start()
        {
            _service = new ServiceClient();
            // Immediately attempt to fetch the inventory again.
            Debug.Log("Fetching inventory");
             FetchInventory();
             FetchCurrencies();
        }
        
        private async void FetchInventory()
        {
           var ctx = await BeamContext.Default.Instance;
            Debug.Log("Initialize complete");
            var items = ctx.Inventory.GetItems();

            // Attempt to refresh the items and see if the list is empty.
            await items.Refresh();
            Debug.Log("got items");
            // Attempt to refresh the items and see if the list is empty.
            Debug.Log("refreshed");

            foreach (var item in items)
            {
                Debug.Log("in loop");
                Debug.Log($"Item ID: {item.ItemId}, Content ID: {item.ContentId}");
            }
            Debug.Log("End");
        }
        
        private async void FetchCurrencies()
        {
            var ctx = await BeamContext.Default.Instance;
            Debug.Log("Initialize complete");
            var currencies = ctx.Inventory.GetCurrencies();

            // Attempt to refresh the items and see if the list is empty.
            await currencies.Refresh();
            Debug.Log("got currencies");
            // Attempt to refresh the currencies and see if the list is empty.
            Debug.Log("refreshed");

            foreach (var currency in currencies)
            {
                Debug.Log("in loop");
                Debug.Log($"currency ID: {currency.CurrencyId}, amount: {currency.Amount}");
            }
            Debug.Log("End");
        }


    }
}