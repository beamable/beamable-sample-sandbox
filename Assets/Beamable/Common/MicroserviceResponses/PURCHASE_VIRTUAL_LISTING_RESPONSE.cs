using System;
using System.Collections.Generic;
using Beamable.Common.Api.Inventory;

namespace Beamable.Common.MicroserviceResponses
{

   public enum PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE
   {
      SUCCESS = 0,
      ERROR_LISTING_NOT_FOUND_IN_STORE = 1,
      ERROR_UNAFFORDABLE = 2,
      ERROR_BEAMABLE_PURCHASE_REQUEST=3
   }

   [Serializable]
   public record PURCHASE_VIRTUAL_LISTING_RESPONSE
   {
      public PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE code = PURCHASE_VIRTUAL_LISTING_RESPONSE_CODE.SUCCESS;
      public bool store_needs_updating;
   }
}

