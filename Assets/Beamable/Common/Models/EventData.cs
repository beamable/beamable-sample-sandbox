using System;
using Beamable.Common.Interfaces;
using Beamable.Common.Models;
using Beamable.Server;

namespace Beamable.Common.Models
{
    [Serializable]
    public class EventData : StorageDocument, ISetStorageDocument<EventData>
    {
        public string eventName;
        public string leaderboardId;
        public DateTime startTime;
        public DateTime endTime;
        public bool isActive;

        public void Set(EventData document)
        {
            eventName = document.eventName;
            leaderboardId = document.leaderboardId;
            startTime = document.startTime;
            endTime = document.endTime;
            isActive = document.isActive;
        }
    }
}