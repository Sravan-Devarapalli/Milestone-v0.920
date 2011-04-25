using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WebPublisher.Bindings
{
    [Serializable]
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        [NonSerialized]
        private List<PropertyChangedEventHandler> propertyChangedList;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (this.propertyChangedList == null)
                    this.propertyChangedList = new List<PropertyChangedEventHandler>();

                this.propertyChangedList.Add(value);
            }

            remove
            {
                this.propertyChangedList.Remove(value);
            }
        }

        protected void Notify(params string[] propertyNames)
        {
            if (this.propertyChangedList == null)
                return;

            foreach (var propertyName in propertyNames)
            {
                foreach (var propertyChanged in this.propertyChangedList.ToArray())
                {
                    try
                    {
                        propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    }

                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}

