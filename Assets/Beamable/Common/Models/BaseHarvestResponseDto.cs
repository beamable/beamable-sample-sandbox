namespace Beamable.Common.Models
{
    public class BaseRewardResponseDto
    {
        public bool Success { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;
        public long TotalPoints { get; set; }
    
    
        public static BaseRewardResponseDto CreateError(string errorMessage)
        {
            return new BaseRewardResponseDto
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    
        public static BaseRewardResponseDto CreateSuccess(long totalPoints)
        {
            return new BaseRewardResponseDto
            {
                Success = true,
                TotalPoints = totalPoints
            };
        }
    }
}