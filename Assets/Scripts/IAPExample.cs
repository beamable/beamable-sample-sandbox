using Beamable;
using Beamable.Common.Shop;
using JetBrains.Annotations;
using UnityEngine;

public class IAPExample : MonoBehaviour
{
    private BeamContext _context;

    private async void Start()
    {
        _context = BeamContext.Default;
        await _context.OnReady;
        Debug.Log($"Beamable finished initialization, PlayerId: {_context.PlayerId}");
    }

    [UsedImplicitly]
    public async void MakePurchase(string listingId)
    {
        // We're creating a new ListingRef and resolving it just so we can continue
        // using this function with a string parameter from the Unity inspector.
        // This could just as easily work passing in a ListingRef as well, and
        // skipping the construction of a new one.
        var listing = await new ListingRef {Id = listingId}.Resolve();
        var skuId = listing.price.symbol;

        // This validates the existence of the SKU. Since the content service won't allow you to assign an
        // invalid SKU to a Listing, this shouldn't really be necessary, but you can at least validate
        // that your content is set up properly.
        var skusResponse = await _context.Api.PaymentService.GetSKUs();
        var sku = skusResponse.skus.definitions.Find(i => i.name == skuId);
        if (sku == null)
        {
            Debug.LogError("Sku not found.");
            return;
        }

        // These are the two parameters necessary for purchasing using Beamable IAP.
        Debug.Log($"listingSymbol {listing.Id}, skuSymbol: {sku.name}");

        // In editor, BeamableIAP will be a test purchaser. On device, this will defer to whichever purchasing
        // service your platform uses (Android -> Google Play, Apple -> iTunes Store, etc)
        var purchaser = await _context.Api.BeamableIAP;
        var purchaseResult = await purchaser.StartPurchase(listing.Id, sku.name)
            .Error(Debug.LogError);
        
        if (string.IsNullOrEmpty(purchaseResult.Receipt)) return;
        Debug.Log(purchaseResult.Receipt);
        Debug.Log("Purchase successful!"); 
    }
}
