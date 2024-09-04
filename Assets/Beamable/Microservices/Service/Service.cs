using System;
using System.Threading.Tasks;
using Beamable.Common.Api.Groups;
using Beamable.Common.Api.Inventory;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        
        [ClientCallable]
        public async Task<InventoryView> GetCurrentItems(string contentId)
        {
            try
            {
                Debug.Log("Fetching current");
                var result = await Services.Inventory.GetCurrent(contentId);
                return result;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
        }
   
    }
}