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
      PURCHASE_VIRTUAL_LISTING_PARAMETERS purchase_params, string transaction_guid, long UserId)
   {
      Debug.Log("[purchase_virtual_listing] Start method execution");
      Debug.Log($"Params: store_ref={purchase_params.store_ref}, listing_id={purchase_params.listing_id}, num_times={purchase_params.num_times}");

      const string TRANSACTION_CONTENT_ID = ITEMS.TRANSACTIONS_ID + ".purchase_virtual_listing";

      purchase_virtual_listing_transaction_service ??=
         new TRANSACTION_SERVICE<PURCHASE_VIRTUAL_LISTING_RESPONSE>(Services, new SERVER_SERVICES(this),
            TRANSACTION_CONTENT_ID);

      var store = await purchase_params.store_ref.Resolve();
      Debug.Log($"Resolved store: {store?.Id}");

      var listing =
         await store.listings.Find(x => x.Id == purchase_params.listing_id)
            .Resolve();
      Debug.Log($"Resolved listing: {listing?.Id}");

      var inventory_scope = $"{TRANSACTION_CONTENT_ID},{listing?.price?.symbol},{CURRENCIES.PLAYER_XP_ID}";

      if (listing?.offer?.obtainCurrency != null && listing.offer.obtainCurrency.Count > 0)
      {
         foreach (var item in listing.offer.obtainCurrency)
         {
            inventory_scope += "," + item.symbol.Id;
         }
      }

      if (listing?.offer?.obtainItems != null && listing.offer.obtainItems.Count > 0)
      {
         foreach (var item in listing.offer.obtainItems)
         {
            inventory_scope += "," + item.contentId.Id;
         }
      }

      Debug.Log($"Inventory scope: {inventory_scope}");
      var inventory_view = await Services.Inventory.GetCurrent(inventory_scope);
      Debug.Log("Retrieved current inventory view");

      var inventory_update_builder = new InventoryUpdateBuilder();
      var response =
         await purchase_virtual_listing_transaction_service.get_response(inventory_view, transaction_guid);

      if (response != null)
      {
         Debug.Log("Cached transaction response found, returning");
         return response;
      }

      response = new PURCHASE_VIRTUAL_LISTING_RESPONSE();
      if (listing == null || listing.offer == null)
      {
         Debug.Log($"ERROR_LISTING_NOT_FOUND_IN_STORE: listing or offer is null - listing_id={purchase_params.listing_id}");
         response.code = PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.ERROR_LISTING_NOT_FOUND_IN_STORE;
         goto return_response;
      }

      long price_total = listing.price.amount * (long)purchase_params.num_times;
      Debug.Log($"Total price: {price_total}, Available: {inventory_view.currencies[listing.price.symbol]}");

      if (inventory_view.currencies[listing.price.symbol] < price_total)
      {
         Debug.Log($"ERROR_UNAFFORDABLE: Not enough currency for listing {listing.Id}");
         response.code = PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.ERROR_UNAFFORDABLE;
         goto return_response;
      }

      if (listing.scheduleInstancePurchaseLimit.HasValue || listing.purchaseLimit.HasValue ||
          listing.activeDurationPurchaseLimit.HasValue)
      {
         Debug.Log("Detected listing with purchase limit, store update needed");
         response.store_needs_updating = true;
      }

      response.code = PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.SUCCESS;

      return_response:
      if (response.code != PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.SUCCESS)
      {
         Debug.Log($"Returning with error code: {response.code}");
         inventory_update_builder = new InventoryUpdateBuilder();
      }
      else
      {
         string purchaseId = $"{listing.Id}:{store.Id}";
         string playerId = UserId.ToString();
         Debug.Log($"Attempting Beamable CommercePurchaseRequest: playerId={playerId}, purchaseId={purchaseId}");
         var request = new CommercePurchaseRequest { purchaseId = purchaseId };

         try
         {
            var result = await Requester.Request(
               Method.POST,
               $"object/commerce/{playerId}/purchase",
               body: request,
               parser: s => s
            );
            Debug.Log("Beamable CommercePurchaseRequest success");
         }
         catch (Exception e)
         {
            Debug.Log($"ERROR Beamable CommercePurchaseRequest failed: {e.Message}");
            response.code = PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.ERROR_BEAMABLE_PURCHASE_REQUEST;
         }
      }

      Debug.Log("Storing transaction response and updating inventory");
      await purchase_virtual_listing_transaction_service.store_response(inventory_view, transaction_guid, response,
         inventory_update_builder);
      await Services.Inventory.Update(inventory_update_builder, transaction_guid);

      Debug.Log($"Returning response with code: {response.code}");
      return response;
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