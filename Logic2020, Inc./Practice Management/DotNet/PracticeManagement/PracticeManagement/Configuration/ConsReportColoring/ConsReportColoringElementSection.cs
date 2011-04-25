using System.Configuration;
using System.Drawing;

namespace PraticeManagement.Configuration.ConsReportColoring
{
    public class ConsReportColoringElementSection : ConfigurationSection
    {
        #region Constants

        private const string COLORS_PROPERTY = "colors";
        private const string COLORING_SECTION_NAME = "practiceManagement/consReportColoring";
        private const string DEFAULT_COLOR_PROPERTY = "defaultColorValue";
        private const string VACATION_COLOR_PROPERTY = "vacationColorValue";       
        private const string HIRED_COLOR_PROPERTY = "hiredColorValue";       
        private const string MILESTONE_COLOR_PROPERTY = "milestonesColorValue";
        private const string OPPORTUNITY_COLOR_PROPERTY = "opportunityColorValue";
        private const string VACATION_TITLE_PROPERTY = "vacationTitle";
        private const string SEPARATOR_PROPERTY = "separator";

        #endregion

        public static ConsReportColoringElementSection ColorSettings
        {
            get
            {
                return ConfigurationManager.GetSection(COLORING_SECTION_NAME) as ConsReportColoringElementSection;
            }
        }

        [ConfigurationProperty(COLORS_PROPERTY, IsDefaultCollection=true)]
        public ConsReportColoringElementCollection Colors
        {
            get
            {
                return this[COLORS_PROPERTY] as ConsReportColoringElementCollection;
            }
        }

        [ConfigurationProperty(DEFAULT_COLOR_PROPERTY)]
        protected string DefaultColorValue
        {
            get
            {
                return this[DEFAULT_COLOR_PROPERTY] as string;
            }
        }

        [ConfigurationProperty(VACATION_TITLE_PROPERTY)]
        public string VacationTitle
        {
            get
            {
                return this[VACATION_TITLE_PROPERTY] as string;
            }
        }

        [ConfigurationProperty(VACATION_COLOR_PROPERTY)]
        protected string VacationColorValue
        {
            get
            {
                return this[VACATION_COLOR_PROPERTY] as string;
            }
        }

        
        [ConfigurationProperty(HIRED_COLOR_PROPERTY)]
        protected string HiredColorValue
        {
            get
            {
                return this[HIRED_COLOR_PROPERTY] as string;
            }
        }

        [ConfigurationProperty(MILESTONE_COLOR_PROPERTY)]
        protected string MilestoneColorValue
        {
            get
            {
                return this[MILESTONE_COLOR_PROPERTY] as string;
            }
        }

        [ConfigurationProperty(OPPORTUNITY_COLOR_PROPERTY)]
        protected string OpportunityColorValue
        {
            get
            {
                return this[OPPORTUNITY_COLOR_PROPERTY] as string;
            }
        }

        [ConfigurationProperty(SEPARATOR_PROPERTY)]
        public string Separator
        {
            get
            {
                return this[SEPARATOR_PROPERTY] as string;
            }
        }

        public Color VacationColor
        {
            get
            {
                return Utils.Coloring.StringToColor(
                    VacationColorValue, Separator);
            }
        }

        public Color HiredColor
        {
            get
            {
                return Utils.Coloring.StringToColor(
                    HiredColorValue, Separator);
            }
        }

        public Color MilestoneColor
        {
            get
            {
                return Utils.Coloring.StringToColor(
                    MilestoneColorValue, Separator);
            }
        }

        public Color OpportunityColor
        {
            get
            {
                return Utils.Coloring.StringToColor(
                    OpportunityColorValue, Separator);
            }
        }
    }
}

