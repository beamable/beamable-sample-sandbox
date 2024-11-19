using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Content;
using Beamable.Server.Clients;
using DefaultNamespace;
using UnityEngine;

public class BattlepassManager : MonoBehaviour
{
    [SerializeField] private ContentRef<Battlepass> battlepassRef;
    private Battlepass _battlepass;

    private async void Start()
    {
        var beamContext = await BeamContext.Default.Instance;
        Debug.Log("start" + beamContext.PlayerId);

        // Fetch the Battlepass content
        await battlepassRef.Resolve()
            .Then(content =>
            {
                _battlepass = content;
                Debug.Log($"Fetched Battlepass: {_battlepass.Name}");
                DisplayBattlepassDetails();
                TestSeasonalTaskRewardItemSerialization();
            })
            .Error(_ =>
            {
                Debug.LogError("Failed to fetch the Battlepass content.");
            });
    }

    private void DisplayBattlepassDetails()
    {
        Debug.Log($"Battlepass: {_battlepass.Name}");
        foreach (var tier in _battlepass.Tiers)
        {
            Debug.Log($"Tier {tier.Level}");
            foreach (var task in tier.Tasks)
            {
                Debug.Log($"  Task: {task.TaskTitle}");
                Debug.Log($"    Description: {task.TaskDescription}");
                Debug.Log($"    Checked Value: {task.CheckedValue}");
                foreach (var reward in task.TaskRewards)
                {
                    DisplayRewardDetails(reward);
                }
            }
        }
    }

    private void DisplayRewardDetails(SeasonalTaskRewardItem reward)
    {
        Debug.Log($"    Reward Name: {reward.RewardName}");
        Debug.Log($"    Quantity: {reward.Quantity}");

        if (reward.ResourceData != null)
        {
            Debug.Log($"      Resource Name: {reward.ResourceData.resourceName}");
            Debug.Log($"      Resource Value: {reward.ResourceData.resourceValue}");
        }

        if (reward.ItemResourceData != null)
        {
            Debug.Log($"      Item Name: {reward.ItemResourceData.itemName}");
            Debug.Log($"      Item Description: {reward.ItemResourceData.itemDescription}");
        }

        if (reward.Skin != null)
        {
            Debug.Log($"      Skin Name: {reward.Skin.SkinName}");
        }

        if (reward.EquipmentResourceData != null)
        {
            Debug.Log($"      Equipment Resource Name: {reward.EquipmentResourceData.ResourceName}");
            Debug.Log($"      Equipment Resource Amount: {reward.EquipmentResourceData.ResourceAmount}");
        }

        Debug.Log($"      Equipment Reward Grade: {reward.EquipmentRewardGrade}");
    }

    private void TestSeasonalTaskRewardItemSerialization()
    {
        Debug.Log("Testing serialization for SeasonalTaskRewardItem...");

        // Create a sample reward structure
        var sampleReward = new SeasonalTaskRewardItem
        {
            RewardName = "Test Reward",
            Quantity = 1,
            ResourceData = new ResourceData { resourceName = "Gold", resourceValue = 100 },
            ItemResourceData = new ItemResourceData { itemName = "Sword", itemDescription = "A sharp blade" },
            Skin = ScriptableObject.CreateInstance<SkinDataSO>(),
            EquipmentResourceData = ScriptableObject.CreateInstance<EquipmentResourceData>(),
            EquipmentRewardGrade = ItemGradeEnums.Legendary
        };

        // Set properties for ScriptableObjects
        sampleReward.Skin.SkinName = "Hero Skin";
        sampleReward.EquipmentResourceData.ResourceName = "Armor";
        sampleReward.EquipmentResourceData.ResourceAmount = 1;

        // Simulate serialization and deserialization
        try
        {
            string json = JsonUtility.ToJson(sampleReward, true); // Serialize to JSON
            Debug.Log($"Serialized SeasonalTaskRewardItem: {json}");

            var deserializedReward = JsonUtility.FromJson<SeasonalTaskRewardItem>(json); // Deserialize back
            Debug.Log("Deserialization successful!");
            Debug.Log($"Reward Name: {deserializedReward.RewardName}, Quantity: {deserializedReward.Quantity}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Exception during serialization test: {ex.Message}");
        }
    }

}
