using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Api.Announcements;
using Beamable.Common.Content;
using UnityEngine;

public class CustomAnnouncementManager : MonoBehaviour
{
    private BeamContext _beamContext;

    private async void Start()
    {
        _beamContext = BeamContext.Default;
        await _beamContext.OnReady;

        Debug.Log($"Player ID: {_beamContext.PlayerId}");

        // Retrieve default announcements
        var announcements = await GetAnnouncements();
        PrintAnnouncements(announcements);

        // Retrieve custom announcements
        var customAnnouncements = await GetCustomAnnouncements();
        PrintCustomAnnouncements(customAnnouncements);

        // Example: Mark the first default announcement as read
        if (announcements.Count > 0)
        {
            await MarkAnnouncementAsRead(announcements[0].id);
        }
    }

    private async Task<List<AnnouncementView>> GetAnnouncements()
    {
        var response = await _beamContext.Api.AnnouncementService.GetCurrent();
        return response?.announcements ?? new List<AnnouncementView>();
    }

    private void PrintAnnouncements(List<AnnouncementView> announcements)
    {
        foreach (var announcement in announcements)
        {
            Debug.Log($"Title: {announcement.title}");
            Debug.Log($"Body: {announcement.body}");
            Debug.Log($"Is Read: {announcement.isRead}");
            Debug.Log($"Has Claims Available: {announcement.HasClaimsAvailable()}");
        }
    }

    private async Task<List<LocalizedAnnouncementContent>> GetCustomAnnouncements()
    {
        // Example of retrieving custom announcements from Beamable's content service
        var customAnnouncementRef = new ContentRef<LocalizedAnnouncementContent>("customAnnouncement");
        var customAnnouncement = await customAnnouncementRef.Resolve();

        return new List<LocalizedAnnouncementContent> { customAnnouncement }; // Adapt as needed
    }

    private void PrintCustomAnnouncements(List<LocalizedAnnouncementContent> customAnnouncements)
    {
        foreach (var customAnnouncement in customAnnouncements)
        {
            Debug.Log($"Custom Field: {customAnnouncement.AnnouncementImage}");
        }
    }

    private async Task MarkAnnouncementAsRead(string announcementId)
    {
        try
        {
            await _beamContext.Api.AnnouncementService.MarkRead(announcementId);
            Debug.Log($"Marked announcement {announcementId} as read.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to mark announcement as read: {ex.Message}");
        }
    }

    private async Task ClaimAnnouncementReward(string announcementId)
    {
        try
        {
            await _beamContext.Api.AnnouncementService.Claim(announcementId);
            Debug.Log($"Claimed rewards for announcement {announcementId}.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to claim rewards: {ex.Message}");
        }
    }
}
