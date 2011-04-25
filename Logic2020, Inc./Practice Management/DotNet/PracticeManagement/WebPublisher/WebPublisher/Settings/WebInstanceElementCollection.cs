using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace WebPublisher.Settings
{
    public class WebInstanceElementCollection : ConfigurationElementCollection
    {
        #region Constructor
        public WebInstanceElementCollection()
		{
		}
		#endregion

        #region Properties
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }
        protected override string ElementName
        {
            get
            {
                return "website";
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return new ConfigurationPropertyCollection();
            }
        }
        #endregion

        #region Indexers
        public WebInstanceElement this[int index]
        {
            get
            {
                return (WebInstanceElement)base.BaseGet(index);
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new WebInstanceElement this[string name]
        {
            get
            {
                return (WebInstanceElement)base.BaseGet(name);
            }
        }
        #endregion

        #region Methods
        public void Add(WebInstanceElement item)
        {
            base.BaseAdd(item);
        }

        public void Remove(WebInstanceElement item)
        {
            base.BaseRemove(item);
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }
        #endregion

        #region Overrides
        protected override ConfigurationElement CreateNewElement()
        {
            return new WebInstanceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as WebInstanceElement).Name;
        }
        #endregion
    }
}

