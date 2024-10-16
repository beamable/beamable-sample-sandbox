using ScriptableObjects;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceReward", menuName = "Battlepass/Resource Reward")]
public class ResourceReward : RewardScriptableObject
{
    public ResourceData resourceData;  // Field for resource-specific data
}

[CreateAssetMenu(fileName = "ItemReward", menuName = "Battlepass/Item Reward")]
public class ItemReward : RewardScriptableObject
{
    public ItemResourceData itemResourceData;  // Field for item-specific data
}

[CreateAssetMenu(fileName = "SkinReward", menuName = "Battlepass/Skin Reward")]
public class SkinReward : RewardScriptableObject
{
    public SkinData skinData;  // Field for skin-specific data
}

[CreateAssetMenu(fileName = "EquipmentReward", menuName = "Battlepass/Equipment Reward")]
public class EquipmentReward : RewardScriptableObject
{
    public EquipmentResourceData equipmentResourceData;  // Field for equipment-specific data
    public string equipmentRewardName;
}