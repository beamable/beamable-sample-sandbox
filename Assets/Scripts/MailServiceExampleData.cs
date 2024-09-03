using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    [Serializable]
    public class MailServiceExampleData
    {
        public int UnreadMailCount;
        public List<string> UnreadMailLogs = new List<string>();
        public List<string> MailMessageLogs = new List<string>();
        public bool IsBeamableSetup;
        public List<string> RewardLogs = new List<string>(); // New: To store reward information
        public long Dbid { get; set; }
    }
}