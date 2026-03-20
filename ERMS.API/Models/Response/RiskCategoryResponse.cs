namespace ERMS.API.Models.Response
{
    public class RiskCategoryResponse
    {
        public int RiskCatId { get; set; }
        public string RiskCatName { get; set; } = string.Empty;
        public string RiskCatAlias { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
