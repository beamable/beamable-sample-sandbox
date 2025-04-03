#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api;
using Beamable.Common.Api.Groups;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Beamable.Common.MicroserviceResponses;
using Beamable.Common.Shop;
using Beamable.Microservices.ChaseWT;
using Beamable.Server;
using Newtonsoft.Json;
using UnityEngine;

namespace Beamable.Microservices
{
   [Microservice("Service")]
   public class Service : Microservice
   {
      private TRANSACTION_SERVICE<PURCHASE_VIRTUAL_LISTING_RESPONSE> purchase_virtual_listing_transaction_service;

      [ClientCallable]
      public async Task<PURCHASE_VIRTUAL_LISTING_RESPONSE> purchase_virtual_listing(
         PURCHASE_VIRTUAL_LISTING_PARAMETERS purchase_params, string transaction_guid)
      {
         const string TRANSACTION_CONTENT_ID = ITEMS.TRANSACTIONS_ID + ".purchase_virtual_listing";

         purchase_virtual_listing_transaction_service ??=
            new TRANSACTION_SERVICE<PURCHASE_VIRTUAL_LISTING_RESPONSE>(Services, new SERVER_SERVICES(this),
               TRANSACTION_CONTENT_ID);

         var store = await purchase_params.store_ref.Resolve();
         var listing =
            await store.listings.Find(x => x.Id == purchase_params.listing_id)
               .Resolve(); //(KA) listing could be null here if not in store! But hard to cope if transaction already responding
         var inventory_scope = $"{TRANSACTION_CONTENT_ID},{listing.price.symbol},{CURRENCIES.PLAYER_XP_ID}";
         //ToDo: (KA) return or error if listing.offer == null perhaps ?
         if (listing.offer != null && listing.offer.obtainCurrency != null && listing.offer.obtainCurrency.Count > 0)
         {
            foreach (var item in listing.offer.obtainCurrency)
            {
               inventory_scope += "," + item.symbol.Id;
            }
         }

         if (listing.offer != null && listing.offer.obtainItems != null && listing.offer.obtainItems.Count > 0)
         {
            foreach (var item in listing.offer.obtainItems)
            {
               inventory_scope += "," + item.contentId.Id;
            }
         }

         var inventory_view = await Services.Inventory.GetCurrent(inventory_scope);
         //var inventory_view = await Services.Inventory.GetCurrent(); //Get everything, since don't know what might be being bought yet?
         var inventory_update_builder = new InventoryUpdateBuilder();
         var response =
            await purchase_virtual_listing_transaction_service.get_response(inventory_view, transaction_guid);
         if (response != null)
         {
            return response;
         }

         response = new PURCHASE_VIRTUAL_LISTING_RESPONSE();
         if (listing == null || listing.offer == null)
         {
            Debug.Log(
               $"ERROR_LISTING_NOT_FOUND_IN_STORE listing or listing.offer is null for listing={purchase_params.listing_id}");
            response.code = PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.ERROR_LISTING_NOT_FOUND_IN_STORE;
            goto return_response;
         }

         //ToDo: (KA) Check affordable
         long price_total = listing.price.amount * (long)purchase_params.num_times;
         if (inventory_view.currencies[listing.price.symbol] < price_total)
         {
            Debug.Log(
               $"ERROR_UNAFFORDABLE listing.price.symbol{listing.price.symbol} {inventory_view.currencies[listing.price.symbol]}<{price_total}");
            response.code = PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.ERROR_UNAFFORDABLE;
            goto return_response;
         }

         long multiple_cases_price_increase = price_total - listing.price.amount;
         // if (multiple_cases_price_increase > 0)
         //    await check_and_change_currency(inventory_update_builder, listing.price.symbol, -multiple_cases_price_increase, inventory_view, do_currency_changes: false);
         // else
         //    await check_and_change_currency(inventory_update_builder, listing.price.symbol, -price_total, inventory_view, do_currency_changes: false);

         if (listing.scheduleInstancePurchaseLimit.HasValue || listing.purchaseLimit.HasValue ||
             listing.activeDurationPurchaseLimit.HasValue)
         {
            response.store_needs_updating = true;
            // foreach (var item in listing.offer.obtainCurrency)
            // {
            //    inventory_update_builder.CurrencyChange(item.symbol.Id, item.amount);
            // }
         }
         // if (listing.offer.obtainItems != null && listing.offer.obtainItems.Count > GLOBAL.ZERO)
         // {
         //    Dictionary<string, string> empty_properties = new Dictionary<string, string>();
         //    foreach (var item in listing.offer.obtainItems)
         //    {
         //       //ToDo:(KA) if item.properties!=null - create Dictionary from the properties and use that! Don't think we use ObtainItem.Properties though
         //       inventory_update_builder.AddItem(item.contentId.Id, empty_properties);
         //    }
         // }

         response.code = PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.SUCCESS;

         return_response:
         if (response.code != PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.SUCCESS)
         {
            inventory_update_builder = new InventoryUpdateBuilder();
         }
         else
         {
            //(KA) Use Beamable's Commerce system instead so can use limits for Special Offers
            string purchaseId = $"{listing.Id}:{store.Id}";
            string playerId = Context.UserId.ToString();
            var request = new CommercePurchaseRequest { purchaseId = purchaseId };

            try
            {
               var result = await Requester.Request(
                  Method.POST,
                  $"object/commerce/{playerId}/purchase",
                  body: request,
                  parser: s => s
               );
            }
            catch (Exception e)
            {
               Debug.Log($"ERROR Beamable CommercePurchaseRequest for purchaseId={purchaseId} - error={e.Message}");
               response.code = PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.ERROR_BEAMABLE_PURCHASE_REQUEST;
            }
         }

         await purchase_virtual_listing_transaction_service.store_response(inventory_view, transaction_guid, response,
            inventory_update_builder);
         await Services.Inventory.Update(inventory_update_builder, transaction_guid);
         return response;
      }

      [ClientCallable]
      public async Task<PURCHASE_VIRTUAL_LISTING_RESPONSE> purchase_virtual_listing_tracked(
         PURCHASE_VIRTUAL_LISTING_TRACKED_PARAMETERS purchase_parameters, string transaction_guid)
      {
         if (purchase_parameters.parameters.num_times != 1)
         {
            if (purchase_parameters.parameters.num_times != 0)
            {
               Debug.Log($"Unexpected num_times={purchase_parameters.parameters.num_times}");
            }

            purchase_parameters.parameters.num_times = 1;
         }

         return await purchase_virtual_listing_body(purchase_parameters, transaction_guid, use_achievements: false);
      }

      [ClientCallable]
      public async Task<PURCHASE_VIRTUAL_LISTING_RESPONSE> purchase_virtual_listing_1_7_5(
         PURCHASE_VIRTUAL_LISTING_TRACKED_PARAMETERS purchase_parameters, string transaction_guid)
      {
         return await purchase_virtual_listing_body(purchase_parameters, transaction_guid, use_achievements: true);
      }

      private async Task<PURCHASE_VIRTUAL_LISTING_RESPONSE> purchase_virtual_listing_body(
         PURCHASE_VIRTUAL_LISTING_TRACKED_PARAMETERS purchase_parameters, string transaction_guid,
         bool use_achievements)
      {
         var purchase_params = purchase_parameters.parameters;
         Debug.Log(
            $"[MA] MS - purchase_virtual_listing_tracked {purchase_params.store_ref}:{purchase_params.listing_id}");
         const string TRANSACTION_CONTENT_ID = ITEMS.TRANSACTIONS_ID + ".purchase_virtual_listing";

         purchase_virtual_listing_transaction_service ??=
            new TRANSACTION_SERVICE<PURCHASE_VIRTUAL_LISTING_RESPONSE>(Services, new SERVER_SERVICES(this),
               TRANSACTION_CONTENT_ID);

         var store = await purchase_params.store_ref.Resolve();
         var listing = await store.listings.Find(x => x.Id == purchase_params.listing_id).Resolve();
         //(KA) listing could be null here if not in store! But hard to cope if transaction already responding
         var inventory_scope =
            $"{TRANSACTION_CONTENT_ID},{listing.price.symbol},{CURRENCIES.PLAYER_XP_ID},{ITEMS.ACHIEVEMENTS_ID}";
         var inventory_view = await Services.Inventory.GetCurrent(inventory_scope);
         var response =
            await purchase_virtual_listing_transaction_service.get_response(inventory_view, transaction_guid);
         if (response != null)
         {
            return response;
         }

         response = new PURCHASE_VIRTUAL_LISTING_RESPONSE();
         var transaction_update_builder = new InventoryUpdateBuilder();
         long price_amount = listing.price.amount * (long)purchase_parameters.parameters.num_times;
         if (listing == null || listing.offer == null)
         {
            Debug.Log(
               $"ERROR_LISTING_NOT_FOUND_IN_STORE listing or listing.offer is null for listing={purchase_params.listing_id}");
            response.code = PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.ERROR_LISTING_NOT_FOUND_IN_STORE;
            goto return_response;
         }

         //ToDo: (KA) Check affordable
         if (inventory_view.currencies[listing.price.symbol] < price_amount)
         {
            Debug.Log(
               $"ERROR_UNAFFORDABLE listing.price.symbol{listing.price.symbol} {inventory_view.currencies[listing.price.symbol]}<{price_amount}");
            response.code = PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.ERROR_UNAFFORDABLE;
            goto return_response;
         }

         response.code = PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.SUCCESS;

         return_response:
         var error_message = response.code.ToString();
         if (response.code == PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.SUCCESS)
         {
            try
            {
               bool success = true;
               {
                  //(KA) Use Beamable's Commerce system instead so can use limits for Special Offers
                  string purchase_id = $"{listing.Id}:{store.Id}";
                  success = await process_commerce_purchase_request(purchase_id, Context.UserId.ToString());

                  if (success)
                  {
                     Debug.Log("Logged virtual purchase");
                  }
               }
            }
            catch (Exception e)
            {
               response.code = PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.ERROR_BEAMABLE_PURCHASE_REQUEST;
               error_message = e.Message;
            }
         }

         await purchase_virtual_listing_transaction_service.store_response(inventory_view, transaction_guid, response,
            transaction_update_builder);
         await Services.Inventory.Update(transaction_update_builder, transaction_guid);
         if (response.code != PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.SUCCESS)
         {
            Debug.Log(
               $"ERROR Beamable CommercePurchaseRequest for purchase={JsonConvert.SerializeObject(purchase_parameters)} - error={error_message}");
         }

         return response;
      }

      private async Task<bool> process_commerce_purchase_request(string purchase_id, string player_id)
      {
         var request = new CommercePurchaseRequest { purchaseId = purchase_id };
         var result = await Requester.Request(
            Method.POST,
            $"object/commerce/{player_id}/purchase",
            body: request,
            parser: s => s
         );
         return (string)JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["result"] == "ok";
      }

      [Serializable]
      public class CommercePurchaseRequest
      {
         public string purchaseId;
      }


      public async Task<List<INVENTORY_OBJECT<TCONTENT>>> get_items<TCONTENT>(InventoryView inventory_view, string? content_id = null)
         where TCONTENT : ItemContent, new()
      {
         var full_content_type_prefix = $"{get_full_content_type<TCONTENT>()}.";

         var inventory_objects = new List<INVENTORY_OBJECT<TCONTENT>>();

         foreach (var items in inventory_view.items)
         {
            var is_match = content_id != null
               ? items.Key == content_id
               : items.Key.StartsWith(full_content_type_prefix);

            if (is_match)
            {
               foreach (var item_view in items.Value)
               {
                  var content = await Services.Content.GetContent<TCONTENT>(new ContentRef(typeof( TCONTENT ), item_view.contentId));
                  inventory_objects.Add(new INVENTORY_OBJECT<TCONTENT>(content, item_view));
               }
            }
         }

         return inventory_objects;
      }
      
      private static string get_full_content_type<TCONTENT>() where TCONTENT : ContentObject
      {
         var content_type_attributes =
            (ContentTypeAttribute[])typeof( TCONTENT ).GetCustomAttributes(typeof( ContentTypeAttribute ), inherit: false);

         if (content_type_attributes.Length == 0)
         {
            throw new ArgumentException($"Class {typeof( TCONTENT ).Name} has no {nameof(ContentTypeAttribute)}", nameof(TCONTENT));
         }

         GLOBAL.ASSERT(content_type_attributes.Length == 1);
         var full_content_type = content_type_attributes[0].TypeName;
         var base_class_type = typeof( TCONTENT ).BaseType;

         while (base_class_type != null)
         {
            content_type_attributes =
               (ContentTypeAttribute[])base_class_type.GetCustomAttributes(typeof( ContentTypeAttribute ), inherit: false);

            if (content_type_attributes.Length > 0)
            {
               GLOBAL.ASSERT(content_type_attributes.Length == 1);
               full_content_type = $"{content_type_attributes[0].TypeName}.{full_content_type}";
               base_class_type = base_class_type.BaseType;
            }
            else
            {
               break;
            }
         }

         return full_content_type;
      }
      
      public static class GLOBAL
      {
         public static void ASSERT(bool condition)
         {
            if (!condition)
            {
               throw new InvalidOperationException("Assertion failed!");
            }
         }
      }

   }
}