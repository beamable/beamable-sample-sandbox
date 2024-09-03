using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Api.Mail;
using Beamable.Server.Clients;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class RefreshedUnityEvent : UnityEvent<MailServiceExampleData> { }

public class MailServiceExample : MonoBehaviour
{
    [HideInInspector]
    public RefreshedUnityEvent OnRefreshed = new RefreshedUnityEvent();

    private BeamContext _beamContext;
    private const string MailCategory = "daily_login_reward";
    private MailServiceExampleData _data = new MailServiceExampleData();
    private ServiceClient _service;
    protected void Start()
    {
        Debug.Log("Starting MailServiceExample...");
        _service = new ServiceClient();
        SetupBeamable();
        Refresh();
    }

    private async void SetupBeamable()
    {
        Debug.Log("Setting up Beamable context...");
        _beamContext = await BeamContext.Default.Instance;

        if (_beamContext == null)
        {
            Debug.LogError("Beamable context could not be initialized.");
            return;
        }

        _data.Dbid = _beamContext.PlayerId;
        Debug.Log($"Beamable setup complete. Player ID: {_data.Dbid}");

        // Trigger sending an email via the microservice
        await _service.SendEmail(_data.Dbid);
        
        _beamContext.Api.MailService.Subscribe(async mailQueryResponse =>
        {
            Debug.Log("Mail subscription triggered. Fetching mail...");
            _data.UnreadMailLogs.Clear();
            _data.UnreadMailCount = mailQueryResponse.unreadCount;
            _data.UnreadMailLogs.Add($"unreadCount = {_data.UnreadMailCount}");

            await CollectMailAndRewards();  // Modified to collect rewards
            Refresh();
        });

        _data.IsBeamableSetup = _beamContext != null;
        Refresh();
    }

    private async Task CollectMailAndRewards()
    {
        Debug.Log("Collecting mail and rewards...");
        _data.MailMessageLogs.Clear();
        _data.RewardLogs.Clear(); // Clear previous rewards
            
        // Trigger mail collection and reward retrieval via the microservice
        var rewards = await _service.CollectAllMailAndReturnRewards(_data.Dbid);

        if (rewards != null && rewards.Count > 0)
        {
            foreach (var reward in rewards)
            {
                _data.RewardLogs.Add(reward);
                Debug.Log($"Collected reward: {reward}");
            }
        }
        else
        {
            Debug.Log("No rewards collected.");
        }

        Refresh();
    }

    private string ExtractRewardsFromMail(MailMessage mailMessage)
    {
        // Example: Parse rewards from mail body or custom field
        // Modify this method according to how rewards are structured in your mail
        string rewardInfo = $"Reward from {mailMessage.senderGamerTag}: {mailMessage.body}";
        Debug.Log($"Extracted rewards from mail ID {mailMessage.id}: {rewardInfo}");
        return rewardInfo;
    }

    public void Refresh()
    {
        Debug.Log("Refreshing UI with new data...");
        if (_data.IsBeamableSetup)
        {
            OnRefreshed?.Invoke(_data);
        }
    }
    
}
