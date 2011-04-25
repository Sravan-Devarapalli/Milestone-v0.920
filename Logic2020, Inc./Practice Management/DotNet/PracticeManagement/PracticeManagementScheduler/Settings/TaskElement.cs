using System;
using System.Configuration;

namespace PracticeManagementScheduler.Settings
{
    public class TaskElement : ConfigurationElement
    {
        private const string TemplateIdAttribute = "templateId";
        private const string SpAttribute = "sprocName";
        private const string NameAttribute = "name";
        private const string EnabledAttribute = "enabled";
        private const string PeriodAttribute = "period";

        #region Constructors

        public TaskElement()
        {
            UpdateLastRun();
        }

        #endregion

        #region Configuration Properties

        [ConfigurationProperty(NameAttribute, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) base[NameAttribute]; }
            set { base[NameAttribute] = value; }
        }

        [ConfigurationProperty(SpAttribute, IsRequired = true, IsKey = true)]
        public string StoredProcName
        {
            get { return (string)base[SpAttribute]; }
            set { base[SpAttribute] = value; }
        }

        [ConfigurationProperty(EnabledAttribute, IsRequired = true)]
        public bool Enabled
        {
            get { return (bool) base[EnabledAttribute]; }
            set { base[EnabledAttribute] = value; }
        }

        [ConfigurationProperty(PeriodAttribute, IsRequired = true)]
        public int Period
        {
            get { return (int) base[PeriodAttribute]; }
            set { base[PeriodAttribute] = value; }
        }

        [ConfigurationProperty(TemplateIdAttribute, IsRequired = true)]
        public int TemplateId
        {
            get { return (int)base[TemplateIdAttribute]; }
            set { base[TemplateIdAttribute] = value; }
        }

        #endregion

        #region Properties

        protected DateTime LastRun { get; set; }

        #endregion

        #region Methods

        public void UpdateLastRun()
        {
            LastRun = DateTime.Now;
        }

        public bool IsTimeToTrigger()
        {
            return DateTime.Now.Subtract(LastRun).Seconds >= Period;
        }

        #endregion

    }
}
