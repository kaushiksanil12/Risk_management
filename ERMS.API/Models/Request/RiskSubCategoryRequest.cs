namespace ERMS.API.Models.Request
{
    public class RiskSubCategoryRequest
    {
        public int RiskCatId { get; set; }
        public string RiskSubCatName { get; set; } = string.Empty;
        public string RiskCatAlias { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
    }
}
