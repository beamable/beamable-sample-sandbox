using System.Collections.Generic;
using Beamable.Common.Content;
using Beamable.Common.Inventory;

namespace Beamable.Microservices.ChaseWT
{
    [ContentType("transaction_content")]
    public class TRANSACTION_CONTENT : ItemContent
    {
        public static readonly string TRANSACTION_GUID_KEY = "transaction_guid";
        public static readonly string TRANSACTION_RESPONSE_KEY = "transaction_response";
        
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public TRANSACTION_CONTENT() { }
    }
}