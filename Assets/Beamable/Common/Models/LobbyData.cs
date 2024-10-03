using System;
using System.Collections.Generic;
using Beamable.Common.Interfaces;
using Beamable.Server;

namespace Beamable.Common.Models
{
    [Serializable]
    public class LobbyData : StorageDocument, ISetStorageDocument<LobbyData>
    {
        public string lobbyName;
        public List<long> memberIds = new List<long>();  // List of member IDs
        public long hostId;  // Host ID

        public void Set(LobbyData document)
        {
            lobbyName = document.lobbyName;
            memberIds = document.memberIds;
            hostId = document.hostId;
        }
    }
}