using System;
using System.Collections.Generic;
using Beamable.Common.Interfaces;
using Beamable.Server;


namespace Beamable.Common.Models
{
    [Serializable]
    public class AuthorizedDeviceData: StorageDocument, ISetStorageDocument<AuthorizedDeviceData>
    {
        public string Email; 
        public List<string> AuthorizedDeviceIds;

        public void Set(AuthorizedDeviceData document)
        {
            Email = document.Email;
            AuthorizedDeviceIds = document.AuthorizedDeviceIds;
        }
    }
}