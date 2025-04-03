using System;
using System.Collections.Generic;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;

namespace Beamable.Common.Shop
{
    [Serializable]
    public class PURCHASE_VIRTUAL_LISTING_PARAMETERS
    {
        public StoreRef store_ref;
        public string listing_id;
        public int num_times;
        public PURCHASE_VIRTUAL_LISTING_PARAMETERS(StoreRef store_ref, string listing_id, int num_times)
        {
            this.store_ref = store_ref;
            this.listing_id = listing_id;
            this.num_times = num_times;
        }
    }


    [Serializable]
    public class PURCHASE_VIRTUAL_LISTING_TRACKED_PARAMETERS
    {
        public PURCHASE_VIRTUAL_LISTING_PARAMETERS parameters;
    }

    public static class ITEMS
    {
        public const string TRANSACTIONS_ID = "transactions";
        public const string ACHIEVEMENTS_ID = "achievements";
        public const string NUM_ITEMS_KEY = "num_items";
    }

    public static class CURRENCIES
    {
        public const string PLAYER_XP_ID = "player_xp";
    }
}