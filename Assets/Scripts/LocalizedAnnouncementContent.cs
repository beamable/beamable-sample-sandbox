using Beamable.Common.Announcements;
using Beamable.Common.Content;
using UnityEngine;
using UnityEngine.AddressableAssets;

[ContentType("customAnnouncement")]
public class LocalizedAnnouncementContent : AnnouncementContent
{
    public AssetReferenceSprite AnnouncementImage; // Unity's AssetReference for images
    public string LocalizedTextKey; // Key for localized text
}