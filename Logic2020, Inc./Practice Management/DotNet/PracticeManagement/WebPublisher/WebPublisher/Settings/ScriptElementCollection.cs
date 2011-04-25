using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace WebPublisher.Settings
{
    public class ScriptElementCollection : ConfigurationElementCollection
    {
         #region Constructor
        public ScriptElementCollection()
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
                return "script";
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
        public ScriptElement this[int index]
        {
            get
            {
                return (ScriptElement)base.BaseGet(index);
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

        public new ScriptElement this[string name]
        {
            get
            {
                return (ScriptElement)base.BaseGet(name);
            }
        }
        #endregion

        #region Methods
        public void Add(ScriptElement item)
        {
            base.BaseAdd(item);
        }

        public void Remove(ScriptElement item)
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
            return new ScriptElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ScriptElement).Path;
        }
        #endregion

        internal int IndexOf(string path)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Path == path)
                {
                    return i;
                }
            }

            return -1;
        }

        internal void MoveTo(int newIndex, string path)
        {
            int oldIndex = IndexOf(path);

            ScriptElement currentItem = this[oldIndex];
            BaseRemoveAt(oldIndex);
            BaseAdd(newIndex, currentItem);
        }
    }
}

