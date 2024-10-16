using System;
using System.Collections.Generic;
using ScriptableObjects;

namespace DefaultNamespace
{
    [Serializable]
    public class Tier
    {
        public int Level;
        public List<RewardScriptableObject> Rewards; 
        public List<ComplexRewardScriptableObject> ComplexRewards; 
    }
}