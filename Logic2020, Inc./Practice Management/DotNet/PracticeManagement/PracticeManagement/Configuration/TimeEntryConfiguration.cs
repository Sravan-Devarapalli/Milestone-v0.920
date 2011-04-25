using System.Configuration;

namespace PraticeManagement.Configuration
{
    public class TimeEntryConfiguration : ConfigurationSection
    {
        #region Constants

        private const string CLIENT_ID_PROPERTY = "clientId";
        private const string PROJECT_ID_PROPERTY = "projectId";
        private const string MILESTONE_ID_PROPERTY = "milestoneId";
        private const string TimeEntrySectionName = "practiceManagement/timeEntryConfig";

        #endregion

        #region Properties

        public static TimeEntryConfiguration Current
        {
            get
            {
                return ConfigurationManager.GetSection(TimeEntrySectionName) as TimeEntryConfiguration;
            }
        }

        [ConfigurationProperty(CLIENT_ID_PROPERTY)]
        public int ClientId
        {
            get
            {
                return (int)this[CLIENT_ID_PROPERTY];
            }
            set
            {
                this[CLIENT_ID_PROPERTY] = value;
            }
        }

        [ConfigurationProperty(PROJECT_ID_PROPERTY)]
        public int ProjectId
        {
            get
            {
                return (int)this[PROJECT_ID_PROPERTY];
            }
            set
            {
                this[PROJECT_ID_PROPERTY] = value;
            }
        }

        [ConfigurationProperty(MILESTONE_ID_PROPERTY)]
        public int MilestoneId
        {
            get
            {
                return (int)this[MILESTONE_ID_PROPERTY];
            }
            set
            {
                this[MILESTONE_ID_PROPERTY] = value;
            }
        }

        #endregion
    }
}

