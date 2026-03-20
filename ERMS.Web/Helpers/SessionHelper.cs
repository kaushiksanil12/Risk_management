using Newtonsoft.Json;

namespace ERMS.Web.Helpers
{
    public static class SessionHelper
    {
        public const string UserId = "UserId";
        public const string UserName = "UserName";
        public const string FullName = "FullName";
        public const string AdminFlag = "AdminFlag";
        public const string LoginType = "LoginType";
        public const string Permissions = "UserPermissions";

        public static void SetUser(ISession session, dynamic user)
        {
            session.SetInt32(UserId, (int)user.userId);
            session.SetString(UserName, (string)(user.username ?? ""));
            session.SetString(FullName, (string)(user.fullName ?? ""));
            session.SetString(AdminFlag, (string)(user.adminFlag ?? "N"));
            session.SetString(LoginType, (string)(user.loginType ?? "Custom"));
        }

        public static void SetPermissions(ISession session, string permissionsJson)
        {
            session.SetString(Permissions, permissionsJson);
        }

        public static int GetUserId(ISession session)
        {
            return session.GetInt32(UserId) ?? 0;
        }

        public static bool IsAdmin(ISession session)
        {
            return session.GetString(AdminFlag) == "Y";
        }

        public static string GetRole(ISession session, string buId)
        {
            var json = session.GetString(Permissions);
            if (string.IsNullOrEmpty(json)) return "N";

            var perms = JsonConvert.DeserializeObject<List<dynamic>>(json);
            if (perms == null) return "N";

            var perm = perms.FirstOrDefault(p => (string)p.buId == buId || (string)p.BUId == buId);
            if (perm == null) return "N";

            return (string)(perm.role ?? perm.Role ?? "N");
        }

        public static bool IsLoggedIn(ISession session)
        {
            return GetUserId(session) > 0;
        }

        public static void Clear(ISession session)
        {
            session.Clear();
        }
    }
}
