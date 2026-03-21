namespace ERMS.Web.Models
{
    public class RiskResponse
    {
        public int RiskId { get; set; }
        public string BUId { get; set; } = string.Empty;
        public string FYId { get; set; } = string.Empty;
        public int RiskCatId { get; set; }
        public int RiskSubCatId { get; set; }
        public int FunctionId { get; set; }
        public string RiskTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImpactLevel { get; set; } = string.Empty;
        public string Likelihood { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public int? ReviewedBy { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string BUName { get; set; } = string.Empty;
        public string FYName { get; set; } = string.Empty;
        public string RiskCatName { get; set; } = string.Empty;
        public string RiskSubCatName { get; set; } = string.Empty;
        public string FunctionName { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public string? ReviewedByName { get; set; }
    }
}
