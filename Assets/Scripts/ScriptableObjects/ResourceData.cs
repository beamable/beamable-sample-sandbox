using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData", menuName = "Battlepass/Resource Data")]
public class ResourceData : ScriptableObject
{
    public string resourceName;
    public int resourceValue;
}