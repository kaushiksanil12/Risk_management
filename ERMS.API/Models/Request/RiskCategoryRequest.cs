namespace ERMS.API.Models.Request
{
    public class RiskCategoryRequest
    {
        public string RiskCatName { get; set; } = string.Empty;
        public string RiskCatAlias { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
    }
}
