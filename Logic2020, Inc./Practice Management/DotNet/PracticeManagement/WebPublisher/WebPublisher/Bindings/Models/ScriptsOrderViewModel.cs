using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GongSolutions.Wpf.DragDrop;
using WebPublisher.Bindings;
using System.Collections.ObjectModel;
using WebPublisher.Settings;
using System.Collections;

namespace WebPublisher.Bindings.Models
{
    public class ScriptsOrderViewModel : NotifyPropertyChanged, IDropTarget
    {
        public ScriptsOrderViewModel()
        {
            PopulateScripts();
        }

        public void PopulateScripts()
        {
            //load from settings
            if (Scripts == null)
            {
                Scripts = new ObservableCollection<SqlScriptsViewModel>();
            }
            else
            {
                Scripts.Clear();
            }

            foreach (ScriptElement script in SettingsManager.Instance.SqlScripts.Scripts)
            {
                AddModel(script.Category, script.Path, script.Include);
            }
        }

        public ObservableCollection<SqlScriptsViewModel> Scripts
        {
            get;
            private set;
        }

        private SqlScriptsViewModel AddModel(string name)
        {
            SqlScriptsViewModel model = new SqlScriptsViewModel(name);
            Scripts.Add(model);
            
            return model;
        }

        internal void AddModel(string categoryName, string file, bool isChecked)
        {
            SqlScriptsViewModel script = null;
            foreach (SqlScriptsViewModel model in Scripts)
            {
                if (model.Path == file)
                {
                    script = model;
                    break;
                }
            }

            if (script == null)
            {
                script = new SqlScriptsViewModel(System.IO.Path.GetFileName(file)) { Path = file, IsChecked = isChecked};
                Scripts.Add(script);
                
            }
            else 
            {
                script.IsChecked = isChecked;
            }

            Notify("Scripts");
        }

        #region IDropTarget Members

        public void DragOver(DropInfo dropInfo)
        {
           //ignored
        }

        public void Drop(DropInfo dropInfo)
        {
            SqlScriptsViewModel data = dropInfo.Data as SqlScriptsViewModel;

            if (dropInfo.DragInfo.VisualSource == dropInfo.VisualTarget)
            {
                SettingsManager.Instance.SqlScripts.Scripts.MoveTo(dropInfo.InsertIndex, data.Path);
                PopulateScripts();
            }
            Notify("Scripts");
        }

        #endregion

        internal void RemoveModel(SqlScriptsViewModel selected)
        {
            SettingsManager.Instance.SqlScripts.Scripts.RemoveAt(SettingsManager.Instance.SqlScripts.Scripts.IndexOf(selected.Path));
            PopulateScripts();
            Notify("Scripts");
        }
    }
}

