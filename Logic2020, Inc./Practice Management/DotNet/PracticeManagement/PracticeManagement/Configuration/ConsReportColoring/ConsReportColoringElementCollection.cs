using System.Configuration;

namespace PraticeManagement.Configuration.ConsReportColoring
{
    public class ConsReportColoringElementCollection : ConfigurationElementCollection
    {
        public ConsReportColoringElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as ConsReportColoringElement;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ConsReportColoringElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConsReportColoringElement)element).Title;
        }
    }
}

