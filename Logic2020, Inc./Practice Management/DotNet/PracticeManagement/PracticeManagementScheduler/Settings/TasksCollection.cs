using System.Configuration;

namespace PracticeManagementScheduler.Settings
{
    public class TasksCollection : ConfigurationElementCollection
    {
        #region Constructor

        #endregion

        #region Properties

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "task"; }
        }

        #endregion

        #region Indexers

        public TaskElement this[int index]
        {
            get { return (TaskElement) base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new TaskElement this[string name]
        {
            get { return (TaskElement) base.BaseGet(name); }
        }

        #endregion

        #region Methods

        public void Add(TaskElement item)
        {
            base.BaseAdd(item);
        }

        public void Remove(TaskElement item)
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
            return new TaskElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as TaskElement).Name;
        }

        #endregion

        internal int IndexOf(string path)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Name == path)
                {
                    return i;
                }
            }

            return -1;
        }

        internal void MoveTo(int newIndex, string path)
        {
            int oldIndex = IndexOf(path);

            TaskElement currentItem = this[oldIndex];
            BaseRemoveAt(oldIndex);
            BaseAdd(newIndex, currentItem);
        }
    }
}
