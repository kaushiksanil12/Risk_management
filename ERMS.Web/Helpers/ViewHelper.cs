namespace ERMS.Web.Helpers
{
    public static class ViewHelper
    {
        public static string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "Draft" => "bg-secondary",
                "Submitted" => "bg-primary",
                "UnderReview" => "bg-info",
                "Approved" => "bg-success",
                "Rejected" => "bg-danger",
                "RevisionRequired" => "bg-warning text-dark",
                "Closed" => "bg-dark",
                "Active" => "bg-success",
                "Inactive" => "bg-secondary",
                _ => "bg-secondary"
            };
        }

        public static string GetImpactBadgeClass(string impact)
        {
            return impact switch
            {
                "High" => "bg-danger",
                "Medium" => "bg-warning text-dark",
                "Low" => "bg-success",
                _ => "bg-secondary"
            };
        }

        public static string GetRoleName(string role)
        {
            return role switch
            {
                "O" => "Owner",
                "C" => "Champion",
                "V" => "Viewer",
                "N" => "None",
                _ => "None"
            };
        }
    }
}
