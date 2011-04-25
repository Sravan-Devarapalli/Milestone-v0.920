using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using GongSolutions.Wpf.DragDrop;
using WebPublisher.Settings;
using System.Collections.ObjectModel;

namespace WebPublisher.Bindings.Models
{
    public class SqlScriptsViewModel : INotifyPropertyChanged, IDropTarget
    {
        #region Data

        bool? _isChecked = false;
        SqlScriptsViewModel _parent;

        #endregion // Data

        #region Properties

        public List<SqlScriptsViewModel> Children { get; private set; }

        public bool IsInitiallySelected { get; private set; }

        public string Name { get; private set; }

        public string Path { get; set; }

        public string CategoryName { get; set; }

        #region IsChecked

        /// <summary>
        /// Gets/sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child FooViewModels.  Setting this property to true or false
        /// will set all children to the same check state, and setting it 
        /// to any value will cause the parent to verify its check state.
        /// </summary>
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { this.SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
                return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
                this.Children.ForEach(c => c.SetIsChecked(_isChecked, true, false));

            if (updateParent && _parent != null)
                _parent.VerifyCheckState();

            this.OnPropertyChanged("IsChecked");
        }

        internal void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                bool? current = this.Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            this.SetIsChecked(state, false, true);
        }

        #endregion // IsChecked

        #endregion // Properties

        #region Inititalization methods
        public SqlScriptsViewModel(string name)
        {
            this.Name = name;
            this.Children = new List<SqlScriptsViewModel>();
        }

        internal void Initialize()
        {
            foreach (SqlScriptsViewModel child in this.Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        private static List<SqlScriptsViewModel> LoadItems(string path, string resultName)
        {
            var currentPathItems = new List<SqlScriptsViewModel>();
            foreach (var folder in Directory.GetDirectories(path))
            {
                var dInfo = new DirectoryInfo(folder);
                currentPathItems.AddRange(LoadItems(dInfo.FullName, dInfo.Name));
            }

            foreach (var files in Directory.GetFiles(path))
            {
                currentPathItems.Add(new SqlScriptsViewModel(System.IO.Path.Combine(resultName, System.IO.Path.GetFileName(files))) { IsChecked = true });
            }
            return currentPathItems;
        }
        #endregion 

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region IDropTarget Members

        public void DragOver(DropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        public void Drop(DropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

