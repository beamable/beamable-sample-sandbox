using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "RewardScriptableObject", menuName = "Battlepass/Reward")]
    public class RewardScriptableObject : ScriptableObject
    {
        public string rewardName;
        public int quantity;
    }
}