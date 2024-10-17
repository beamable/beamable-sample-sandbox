using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Groups;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        public Promise<Unit> UpdateInventory(string itemRef, int itemId, Dictionary<string, string> itemProperties)
        {
            try
            {
                var result = Services.Inventory.Update(builder =>
                {
                    builder.UpdateItem(itemRef, itemId, itemProperties);
                    Debug.Log($"Item {itemRef} updated");
                });

                return result;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
        }
        
        [ClientCallable]
        public Promise<Unit> AddToInventory(string itemRef, Dictionary<string, string> itemProperties)
        {
            try
            {
                var result = Services.Inventory.AddItem(itemRef, itemProperties);

                return result;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
        }
    }
}