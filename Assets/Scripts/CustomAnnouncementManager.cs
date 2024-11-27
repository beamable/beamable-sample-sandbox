using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Api.Announcements;
using Beamable.Common.Content;
using UnityEngine;

public class CustomAnnouncementManager : MonoBehaviour
{
    [SerializeField] private ContentRef<LocalizedAnnouncementContent> _customAnnouncementRef;
    private LocalizedAnnouncementContent _customAnnouncement;

    private BeamContext _beamContext;

    private async void Start()
    {
        _beamContext = BeamContext.Default;
        await _beamContext.OnReady;

        Debug.Log($"Player ID: {_beamContext.PlayerId}");

        // Fetch the custom announcement content
        await _customAnnouncementRef.Resolve()
            .Then(content =>
            {
                _customAnnouncement = content;
                Debug.Log("Fetched Custom Announcement");
                DisplayCustomAnnouncementDetails();
            })
            .Error(ex =>
            {
                Debug.LogError("Failed to fetch the custom announcement content: " + ex.Message);
            });

        // Retrieve the associated AnnouncementView
        var announcementViews = await GetAllAnnouncements();
        foreach (var view in announcementViews)
        {
            if (view.title == _customAnnouncement.title)
            {
                Debug.Log("Custom announcement found in AnnouncementView");
                await MarkAnnouncementAsRead(view.id);
            }
        }
    }

    private async Task<List<AnnouncementView>> GetAllAnnouncements()
    {
        var response = await _beamContext.Api.AnnouncementService.GetCurrent();
        return response?.announcements ?? new List<AnnouncementView>();
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

    private void DisplayCustomAnnouncementDetails()
    {
        if (_customAnnouncement == null)
        {
            Debug.LogWarning("No custom announcement to display.");
            return;
        }

        Debug.Log($"Title: {_customAnnouncement.title}");
        Debug.Log($"Body: {_customAnnouncement.body}");
        Debug.Log($"Custom Image: {_customAnnouncement.AnnouncementImage}");
        Debug.Log($"Localized Text Key: {_customAnnouncement.LocalizedTextKey}");
    }
}
