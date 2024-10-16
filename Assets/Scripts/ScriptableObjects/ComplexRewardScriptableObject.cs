using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ComplexRewardScriptableObject", menuName = "Battlepass/ComplexReward")]
    public class ComplexRewardScriptableObject: RewardScriptableObject
    {
        public List<RewardScriptableObject> nestedRewards;
    }
}