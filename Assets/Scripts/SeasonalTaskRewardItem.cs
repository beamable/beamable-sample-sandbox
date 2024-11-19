using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    [Serializable]
    public class SeasonalTaskRewardItem
    {
        public string RewardName;
        public int Quantity;

        // Fields for specific types of rewards
        public ResourceData ResourceData;
        public ItemResourceData ItemResourceData;
        public SkinDataSO Skin;
        public EquipmentResourceData EquipmentResourceData;
        public ItemGradeEnums EquipmentRewardGrade;
    }
}