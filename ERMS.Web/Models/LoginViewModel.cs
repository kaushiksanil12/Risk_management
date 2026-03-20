using System.ComponentModel.DataAnnotations;

namespace ERMS.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        public string LoginType { get; set; } = "Custom";

        public bool RememberMe { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public class DashboardViewModel { }
    public class UserViewModel { public int UserId { get; set; } public string Username { get; set; } = ""; public string FullName { get; set; } = ""; public string Email { get; set; } = ""; public string? Mobile { get; set; } public string LoginType { get; set; } = "Custom"; public string Status { get; set; } = "Active"; public string AdminFlag { get; set; } = "N"; public string? Password { get; set; } public int IsLocked { get; set; } }
    public class BUViewModel { public string BUId { get; set; } = ""; public string BUName { get; set; } = ""; public string BUShortName { get; set; } = ""; public string Status { get; set; } = "Active"; }
    public class RiskCategoryViewModel { public int RiskCatId { get; set; } public string RiskCatName { get; set; } = ""; public string RiskCatAlias { get; set; } = ""; public string Status { get; set; } = "Active"; }
    public class RiskSubCategoryViewModel { public int RiskSubCatId { get; set; } public int RiskCatId { get; set; } public string RiskSubCatName { get; set; } = ""; public string RiskCatAlias { get; set; } = ""; public string Status { get; set; } = "Active"; }
    public class FunctionViewModel { public int FunctionId { get; set; } public string FunctionName { get; set; } = ""; public string Status { get; set; } = "Active"; }
    public class UserPermissionViewModel { public string BUId { get; set; } = ""; public string BUName { get; set; } = ""; public string Status { get; set; } = ""; public int? PermissionId { get; set; } public string Role { get; set; } = "N"; }
    public class RiskViewModel { public int RiskId { get; set; } public string BUId { get; set; } = ""; public string FYId { get; set; } = ""; public int RiskCatId { get; set; } public int RiskSubCatId { get; set; } public int FunctionId { get; set; } public string RiskTitle { get; set; } = ""; public string Description { get; set; } = ""; public string ImpactLevel { get; set; } = ""; public string Likelihood { get; set; } = ""; public string Status { get; set; } = ""; }
    public class RiskReviewViewModel { public string Action { get; set; } = ""; public string? Remarks { get; set; } }
}
