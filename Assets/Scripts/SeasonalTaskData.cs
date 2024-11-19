using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    [Serializable]
    public class SeasonalTaskData
    {
        public string TaskTitle;
        public string TaskDescription;
        public int CheckedValue;
        public List<SeasonalTaskRewardItem> TaskRewards; // Rewards at each step
    }
}