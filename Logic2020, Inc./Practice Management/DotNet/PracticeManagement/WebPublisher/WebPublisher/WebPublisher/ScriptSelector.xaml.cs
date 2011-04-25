using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WebPublisher.Bindings.Models;
using System.Collections.ObjectModel;

namespace WebPublisher
{
    /// <summary>
    /// Interaction logic for ScriptSelector.xaml
    /// </summary>
    public partial class ScriptSelector : Window
    {
        public List<string> CheckedFiles
        {
            get;
            private set;
        }

        public string CategoryName
        {
            get { return txtName.Text; }
        }

        public ScriptSelector()
        {
            InitializeComponent();
            BindScripts(txtSources.Text);
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            var dlg1 = new WebPublisher.Controls.FolderBrowserDialogEx();
            dlg1.Description = "Select a folder with scripts:";
            dlg1.ShowNewFolderButton = false;
            dlg1.ShowEditBox = true;
            //dlg1.NewStyle = false;
            dlg1.SelectedPath = txtSources.Text;
            dlg1.ShowFullPathInEditBox = true;
            dlg1.RootFolder = System.Environment.SpecialFolder.MyComputer;

            // Show the FolderBrowserDialog.
            var result = dlg1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtSources.Text = dlg1.SelectedPath;
            }
            BindScripts(txtSources.Text);
        }

        private void BindScripts(string path)
        {
            var model = new WebPublisher.Bindings.Models.FilesViewModel(path);
            scripts.DataContext = model;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            CheckedFiles = new List<string>();

            if (scripts.DataContext != null)
            {
                foreach (FileViewModel fileModel in (scripts.DataContext as FilesViewModel).Files)
                {
                    if (fileModel.IsChecked.HasValue && fileModel.IsChecked.Value)
                    {
                        CheckedFiles.Add(fileModel.FilePath);
                    }
                }
            }
            
            this.Close();
        }
    }
}

