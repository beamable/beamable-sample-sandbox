using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    [Serializable]
    public class Tier
    {
        public int Level;
        public List<Reward> Rewards;
    }
}