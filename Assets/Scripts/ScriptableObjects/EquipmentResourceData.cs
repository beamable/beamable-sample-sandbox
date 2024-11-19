using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentResource", menuName = "GameData/EquipmentResource")]
public class EquipmentResourceData : ScriptableObject
{
    public string ResourceName;
    public int ResourceAmount;
}