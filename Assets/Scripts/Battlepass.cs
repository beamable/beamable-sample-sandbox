using System.Collections.Generic;
using Beamable.Common.Content;
using Beamable.Common.Inventory;

namespace DefaultNamespace
{
    [ContentType("battlepass")]
    public class Battlepass : ContentObject
    {
        public string Name;
        public List<Tier> Tiers;
    }
}