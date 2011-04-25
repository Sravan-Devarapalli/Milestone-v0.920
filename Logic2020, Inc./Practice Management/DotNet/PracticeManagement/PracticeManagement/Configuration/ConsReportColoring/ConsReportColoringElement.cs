using System.Configuration;
using System.Drawing;

namespace PraticeManagement.Configuration.ConsReportColoring
{
    public class ConsReportColoringElement : ConfigurationElement
    {
        #region Constants

        private const string TITLE_PROPERTY = "title";
        private const string COLOR_VALUE_PROPERTY = "colorValue";
        private const string MIN_VALUE_PROPERTY = "minValue";
        private const string MAX_VALUE_PROPERTY = "maxValue";

        private const string TITLE_DEFAULT_VALUE = "Color coding";
        private const string DEFAULT_COLOR_VALUE = "0.0.0";
        private const int DEFAULT_MIN_VALUE = 0;
        private const int DEFAULT_MAX_VALUE = 100;
        private const string SEPARATOR = ".";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a title.
        /// </summary>
        [ConfigurationProperty(TITLE_PROPERTY, IsRequired = true, DefaultValue=TITLE_DEFAULT_VALUE)]
        public string Title
        {
            get
            {
                return (string) this[TITLE_PROPERTY];
            }
        }

        /// <summary>
        /// Gets or sets a color value.
        /// </summary>
        [ConfigurationProperty(COLOR_VALUE_PROPERTY, IsRequired = true, DefaultValue=DEFAULT_COLOR_VALUE)]
        protected string ColorValue
        {
            get
            {
                return (string)this[COLOR_VALUE_PROPERTY];
            }
        }

        /// <summary>
        /// Item color
        /// </summary>
        public Color ItemColor
        {
            get
            {                
                return Utils.Coloring.StringToColor(ColorValue, SEPARATOR);
            }
        }

        /// <summary>
        /// Gets or sets a minimum value.
        /// </summary>
        [ConfigurationProperty(MIN_VALUE_PROPERTY, IsRequired = true, DefaultValue=DEFAULT_MIN_VALUE)]
        public int MinValue
        {
            get
            {
                return (int) this[MIN_VALUE_PROPERTY];
            }
        }

        /// <summary>
        /// Gets or sets a maximum value.
        /// </summary>
        [ConfigurationProperty(MAX_VALUE_PROPERTY, IsRequired = true, DefaultValue=DEFAULT_MAX_VALUE)]
        public int MaxValue
        {
            get
            {
                return (int)this[MAX_VALUE_PROPERTY];
            }
        }

        #endregion
    }
}

