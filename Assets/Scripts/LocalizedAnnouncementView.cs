using Beamable.Common.Api.Announcements;
using UnityEngine;

public class LocalizedAnnouncementView : AnnouncementView
{
    public Sprite AnnouncementImage { get; private set; }
    public string LocalizedText { get; private set; }

    public LocalizedAnnouncementView(LocalizedAnnouncementContent content)
    {
        // Load the image from AssetReference
        content.AnnouncementImage.LoadAssetAsync<Sprite>().Completed += handle =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                AnnouncementImage = handle.Result;
            }
        };

        // Resolve localization key to localized text (mocked for example)
        LocalizedText = Localize(content.LocalizedTextKey);
    }

    private string Localize(string key)
    {
        // Implement your localization system here
        return $"Localized text for key: {key}";
    }
}