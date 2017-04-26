using System;

namespace OffSite.Web.Utils.Constants
{
    public static class Constants
    {
        public static class Roles
        {
            public const string UserRole = "User";
            public const string AdminRole = "Admin";
            public const string WatcherRole = "Watcher";
            public const string ApproverRole = "Approver";
        }

        public static class Global
        {
            public const string CurrentUser = "CurrentUser";
            public const string CurrentUserRoles = "CurrentUserRoles";
            public const string Message = "Message";
            public const string ErrorMessage = "ErrorMessage";
            public const string IsCurrentUserIsInAdminRole = "IsCurrentUserIsInAdminRole";
            public const string PaidVacationName = "Vacation - paid";
            public const string NonPaidVacationName = "Vacation - non paid";
        }
    }
}
