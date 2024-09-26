using Beamable;
using Beamable.Common.Content;
using UnityEngine;

namespace DefaultNamespace
{
    public class BattlepassManager : MonoBehaviour
    {
        [SerializeField] private ContentRef<Battlepass> _battlepassRef;
        private Battlepass _battlepass;

        private async void Start()
        {
            // Get the Beamable context
            var beamContext = BeamContext.Default;
            await beamContext.OnReady;

            // Fetch the Battlepass content
            await _battlepassRef.Resolve()
                .Then(content =>
                {
                    _battlepass = content;
                    Debug.Log($"Fetched Battlepass: {_battlepass.Name}");
                    DisplayBattlepassDetails();
                })
                .Error(ex =>
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
                foreach (var reward in tier.Rewards)
                {
                    Debug.Log($"  Reward: {reward.RewardName}, Quantity: {reward.Quantity}");
                }
            }
        }
    }
}