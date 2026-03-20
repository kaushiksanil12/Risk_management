namespace ERMS.API.Helpers
{
    public static class ApiConstants
    {
        public const string HeaderUserId = "X-User-Id";
        public const string HeaderAdminFlag = "X-Admin-Flag";

        public static class Roles
        {
            public const string Owner = "O";
            public const string Champion = "C";
            public const string Viewer = "V";
            public const string None = "N";
        }

        public static class RiskStatuses
        {
            public const string Draft = "Draft";
            public const string Submitted = "Submitted";
            public const string UnderReview = "UnderReview";
            public const string Approved = "Approved";
            public const string Rejected = "Rejected";
            public const string RevisionRequired = "RevisionRequired";
            public const string Closed = "Closed";
        }
    }
}
