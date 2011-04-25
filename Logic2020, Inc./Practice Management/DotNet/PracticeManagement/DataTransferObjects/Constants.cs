
namespace DataTransferObjects
{
    /// <summary>
    /// Determones a common constants
    /// </summary>
    public static class Constants
    {
        public static class RoleNames
        {
            public const string AdministratorRoleName = "Administrator";
            public const string PracticeManagerRoleName = "Practice Area Manager";
            public const string SalespersonRoleName = "Salesperson";
            public const string RecruiterRoleName = "Recruiter";
            public const string ConsultantRoleName = "Consultant";
            public const string ProjectLead = "Project lead";
            public const string DirectorRoleName = "Client Director";
            public const string HRRoleName = "HR";
        }

        public class Formatting
        {
            public const string ProjectDetailedNameFormat = "{0} - {1} - {2} - {3}";
            public const string ProjectNameNumberFormat = "{0} - {1}";
            public const string StringValueSeparator = ",";
        }

        public class GroupNames
        {
            public const string DefaultGroupName = "Default Group";
        }

        public static class ResourceKeys
        {
            # region Report

            public const string StartDateKey = "StartDateKey";
            public const string GranularityKey = "GranularityKey";
            public const string PeriodKey = "PeriodKey";
            public const string ProjectedPersonsKey = "ProjectedPersonsKey";
            public const string ProjectedProjectsKey = "ProjectedProjectsKey";
            public const string ActivePersonsKey = "ActivePersonsKey";
            public const string ActiveProjectsKey = "ActiveProjectsKey";
            public const string ExperimentalProjectsKey = "ExperimentalProjectsKey";
            public const string TimescaleIdListKey = "TimescaleIdListKey";
            public const string PracticeIdListKey = "PracticeIdListKey";
            public const string ExcludeInternalPracticesKey = "ExcludeInternalPracticesKey";
            public const string InternalProjectsKey = "InternalProjectsKey";
            public const string SortIdKey = "SortIdKey";
            public const string SortDirectionKey = "SortDirectionKey";
            public const string AvgUtilKey = "AvgUtilKey";
            public const string EndDateKey = "EndDateKey";

            # endregion Report

            # region SMTP

            public const string MailServerKey = "MailServer";
            public const string PortNumberKey = "PortNumber";
            public const string SSLEnabledKey = "SSLEnabled";
            public const string SMTPAuthRequiredKey = "SMTPAuthRequired";
            public const string UserNameKey = "UserName";
            public const string PasswordKey = "Password";
            public const string PMSupportEmailAddressKey = "PMSupportEmailAddress";

            # endregion SMTP

        }
    }
}

