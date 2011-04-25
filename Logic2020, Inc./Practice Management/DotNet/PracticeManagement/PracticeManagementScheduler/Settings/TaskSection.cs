using System.Configuration;

namespace PracticeManagementScheduler.Settings
{
    public class TaskSection : ConfigurationSection
    {
        public const string SectionName = "schedule";

        public TaskSection()
        {
            SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToApplication;
        }

        [ConfigurationProperty("tasks")]
        public TasksCollection Tasks
        {
            get { return this["tasks"] as TasksCollection; }
            set { this["tasks"] = value; }
        }

        [ConfigurationProperty("tasks_period", DefaultValue=3600)]
        public int TasksPeriod
        {
            get { return (int)this["tasks_period"]; }
            set { this["tasks_period"] = value; }
        }
    }
}

