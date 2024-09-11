namespace Beamable.Common.Models
{
    public class HarvestResponseDto : BaseRewardResponseDto
    {
        public int FailedPlantedSeedId { get; set; }
        
        
        public new static HarvestResponseDto CreateError(string errorMessage, int plantedSeedId)
        {
            return new HarvestResponseDto
            {
                Success = false,
                ErrorMessage = errorMessage,
                FailedPlantedSeedId = plantedSeedId
            };
        }
        
        public new static HarvestResponseDto CreateSuccess(long totalPoints)
        {
            return new HarvestResponseDto
            {
                Success = true,
                TotalPoints = totalPoints
            };
        }
    }
    
    
}