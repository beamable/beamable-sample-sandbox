using System.Collections;
using System.Collections.Generic;
using Beamable;
using Beamable.Common.Shop;
using UnityEngine;

public class purchasetest : MonoBehaviour
{
    private BeamContext _beamContext;
    [SerializeField] private StoreRef storeRef;
    [SerializeField] private ListingRef listingRef;
    private async void Start()
    {
        _beamContext = BeamContext.Default;
        await _beamContext.OnReady;
        Debug.Log($"User Id: {_beamContext.PlayerId}");
        Debug.Log($"store Id: {storeRef.Id}");
        Debug.Log($"listing Id: {listingRef.Id}");
    }
}
