#nullable enable
using System;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Server;
using Newtonsoft.Json;
using UnityEngine;

namespace Beamable.Microservices.ChaseWT
{
   public class TRANSACTION_SERVICE<TRESPONSE> where TRESPONSE : class
   {
      private readonly IBeamableServices services;
      private readonly ISERVER_SERVICES server_services;
      private readonly string transaction_content_id;

      public TRANSACTION_SERVICE(IBeamableServices services, ISERVER_SERVICES server_services, string transaction_content_id)
      {
         this.services = services;
         this.server_services = server_services;
         this.transaction_content_id = transaction_content_id;
      }

      public async Task<TRESPONSE?> get_response(InventoryView inventory_view, string transaction_guid)
      {
         var transaction_items = await server_services.get_items<TRANSACTION_CONTENT>(inventory_view, transaction_content_id);

         foreach (var item in transaction_items)
         {
            if (item.Properties[TRANSACTION_CONTENT.TRANSACTION_GUID_KEY] == transaction_guid)
            {
               var result = JsonConvert.DeserializeObject<TRESPONSE>(item.Properties[TRANSACTION_CONTENT.TRANSACTION_RESPONSE_KEY]);
               return result;
            }
         }

         Debug.Log($"Start new transaction {transaction_guid}");
         return null;
      }

   public async Task store_response(InventoryView inventory_view, string transaction_guid, TRESPONSE response, InventoryUpdateBuilder inventory_update_builder)
{
   Debug.Log($"[store_response] Start - transaction_guid: {transaction_guid}");

   var transaction_items = await server_services.get_items<TRANSACTION_CONTENT>(inventory_view, transaction_content_id);
   Debug.Log($"[store_response] Retrieved {transaction_items.Count} transaction_items for content_id: {transaction_content_id}");

   INVENTORY_OBJECT<TRANSACTION_CONTENT>? transaction_item;

   if (transaction_items.Count >= 1)
   {
      transaction_item = transaction_items[0];
      Debug.Log($"[store_response] Using existing transaction item with ID: {transaction_item.Id}");

      for (var item_index = 1; item_index < transaction_items.Count; item_index++)
      {
         var item = transaction_items[item_index];
         Debug.Log($"[store_response] Deleting duplicate item with ID: {item.Id}");
         inventory_update_builder.DeleteItem(item.ItemContent.Id, item.Id);
      }
   }
   else
   {
      // Debug.Log($"[store_response] No existing transaction item, creating new from content: {transaction_content_id}");
      // try
      // {
      //    transaction_item = new INVENTORY_OBJECT<TRANSACTION_CONTENT>(
      //       await services.Content.GetContent<TRANSACTION_CONTENT>(
      //          new ContentRef(typeof(TRANSACTION_CONTENT), transaction_content_id)));
      // }
      // catch (Exception ex)
      // {
      //    Debug.LogError($"[store_response] Failed to load content: {transaction_content_id}, Exception: {ex.Message}");
      //    throw;
      // }
   }

   // transaction_item.Properties[TRANSACTION_CONTENT.TRANSACTION_GUID_KEY] = transaction_guid;
   // transaction_item.Properties[TRANSACTION_CONTENT.TRANSACTION_RESPONSE_KEY] = JsonConvert.SerializeObject(response);
   //
   // if (transaction_item.is_in_inventory)
   // {
   //    Debug.Log($"[store_response] Updating existing inventory item ID: {transaction_item.Id}");
   //    inventory_update_builder.UpdateItem(transaction_item.ItemContent.Id, transaction_item.Id, transaction_item.Properties);
   // }
   // else
   // {
   //    Debug.Log($"[store_response] Adding new inventory item with content ID: {transaction_item.ItemContent.Id}");
   //    inventory_update_builder.AddItem(transaction_item.ItemContent.Id, transaction_item.Properties);
   // }

   Debug.Log("[store_response] End");
}

   }
}
