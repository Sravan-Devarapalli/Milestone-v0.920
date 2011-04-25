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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebPublisher.Settings;
using WebPublisher.Bindings.Models;
namespace WebPublisher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SettingsManager PubSettings
        {
            get { return SettingsManager.Instance; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveScripts();

            // TODO : find out why bidning isn't TwoWay!
            DatabaseSettings.Default.ServerName = tbServerName.Text;
            DatabaseSettings.Default.InstanceName = tbInstanceName.Text;
            DatabaseSettings.Default.DatabaseName = tbDatabaseName.Text;
            DatabaseSettings.Default.UserName = tbUserName.Text;
            DatabaseSettings.Default.Password = tbPasswordName.Text;
            DatabaseSettings.Default.SqlDataPath = tbSqlDataPath.Text;
            DatabaseSettings.Default.Save();
            PubSettings.SaveSettings();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BuildDB();
            PublishWebSite();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rtbLogArea.Document = new FlowDocument();
            rtbLogArea.Document.Blocks.Add(new Paragraph(new Run(string.Format("Config path : {0}", PubSettings.ConfigPath))));

            lbWebSites.ItemsSource = PubSettings.WebSites.Default;

            PopulateScripts();
        }

        private void PopulateScripts()
        {
            var viewModel = new ScriptsOrderViewModel();
           
            tree.DataContext = viewModel;
        }

        private void btnPathSelector_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".csproj";
            dlg.Filter = "C# Projects (.csproj)|*.csproj";

            dlg.InitialDirectory = txtSources.Text;
            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                txtSources.Text = filename;
            }
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            lbWebSites.SelectedIndex = -1;
        }

        private void lbWebSites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) // new item adding
            {
                txtSiteUrl.Text = string.Empty;
                txtSources.Text = string.Empty;
                txtVirtualPath.Text = "New Element";
                txtVirtualFolder.Text = string.Empty;
                txtSiteName.Text = "Default Web Site";
            }
            else
            {
                var wie = e.AddedItems[0] as WebInstanceElement;
                txtSiteUrl.Text = wie.SiteUrl;
                txtSources.Text = wie.SiteSources;
                txtVirtualFolder.Text = wie.SiteOutput;
                txtVirtualPath.Text = wie.Name;
                txtSiteName.Text = wie.SiteName;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (PubSettings.WebSites.Default == null)
            {
                PubSettings.WebSites.Default = new WebInstanceElementCollection();
            }
            if (lbWebSites.SelectedIndex < 0)
            {
                if (!string.IsNullOrEmpty(txtVirtualPath.Text) &&
                    !string.IsNullOrEmpty(txtSiteUrl.Text) && 
                    !string.IsNullOrEmpty(txtSources.Text)&& 
                    !string.IsNullOrEmpty(txtVirtualFolder.Text))
                {
                    var newInstance = new WebInstanceElement() { Name = txtVirtualPath.Text, SiteSources = txtSources.Text, SiteUrl = txtSiteUrl.Text, SiteOutput = txtVirtualFolder.Text, SiteName = txtSiteName.Text };
                    PubSettings.WebSites.Default.Add(newInstance);
                }
            }
            else
            {
                PubSettings.WebSites.Default[txtVirtualPath.Text].SiteSources = txtSources.Text;
                PubSettings.WebSites.Default[txtVirtualPath.Text].SiteUrl = txtSiteUrl.Text;
                PubSettings.WebSites.Default[txtVirtualPath.Text].SiteOutput = txtVirtualFolder.Text;
                PubSettings.WebSites.Default[txtVirtualPath.Text].SiteName = txtSiteName.Text;
            }
            PubSettings.SaveSettings();
        }

        private void BuildDB()
        {
            SaveScripts();
            var items = tree.ItemsSource as List<WebPublisher.Bindings.Models.SqlScriptsViewModel>;
            var scripts = new List<string>();
            string path = string.Empty;

            foreach (SqlScriptsViewModel ssvw in tree.ItemsSource)
            {
                if (ssvw.IsChecked == true)
                {
                    scripts.Add(ssvw.Path);
                }
            }
            DataBaseHelper dbHelper = new DataBaseHelper()
            {
                DBName = tbDatabaseName.Text,
                InstanceName = tbInstanceName.Text,
                ServerName = tbServerName.Text,
                Password = tbPasswordName.Text,
                UserName = tbUserName.Text,
                SqlDataPath = tbSqlDataPath.Text
            };

            dbHelper.OnLogMessage += new DataBaseHelper.LogMessage(dbHelper_OnLogMessage);
            dbHelper.BuildDataBase(scripts, false);
            
        }

        delegate void PrintMessage(string message);
        void dbHelper_OnLogMessage(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new PrintMessage(dbHelper_OnLogMessage), System.Windows.Threading.DispatcherPriority.Normal, new object[] { message });
            }
            else 
            {
                rtbLogArea.Document.Blocks.Add(new Paragraph(new Run(message)));
            }
        }

        private void PublishWebSite()
        {
            WebHelper wwwHelper = new WebHelper();
            wwwHelper.OnLogMessage += new WebHelper.LogMessage(dbHelper_OnLogMessage);
            try
            {
                foreach (WebInstanceElement wie in (lbWebSites.ItemsSource as WebInstanceElementCollection))
                {
                    if (wie.Include)
                    {
                        rtbLogArea.Document.Blocks.Add(new Paragraph(new Run(wwwHelper.PublishWebSite(wie.SiteUrl, wie.Name, wie.SiteSources, wie.SiteOutput, wie.SiteName))));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void btnAddScripts_Click(object sender, RoutedEventArgs e)
        {
            SaveScripts();
            var dlg = new ScriptSelector()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            dlg.ShowDialog();

            if (!string.IsNullOrEmpty(dlg.CategoryName) && dlg.CheckedFiles != null && dlg.CheckedFiles.Count > 0)
            {
                AddScripts(dlg.CheckedFiles, dlg.CategoryName);
                (tree.DataContext as ScriptsOrderViewModel).PopulateScripts();
            }
        }

        private void AddScripts(List<string> files, string categoryName)
        {
            foreach (var file in files)
            {
                string newName = System.IO.Path.GetFileName(file);
                bool exist = false;
                foreach (ScriptElement chld in SettingsManager.Instance.SqlScripts.Scripts)
                {
                    if (chld.Path == newName)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    SettingsManager.Instance.SqlScripts.Scripts.Add(new ScriptElement() { Path = file, Include = true, Category = categoryName });
                    (tree.DataContext as ScriptsOrderViewModel).AddModel(categoryName, file, true);
                }

            }
            
        }
        private void SaveScripts()
        {
            foreach (var category in (tree.DataContext as ScriptsOrderViewModel).Scripts)
            {
                foreach (SqlScriptsViewModel model in category.Children) // loop through files and include required
                {
                    if (SettingsManager.Instance.SqlScripts.Scripts[model.Name] != null)
                    {
                        SettingsManager.Instance.SqlScripts.Scripts[model.Name].Include = model.IsChecked.HasValue && model.IsChecked.Value;
                    }
                }
            }

        }

        private void btnDeleteScripts_Click(object sender, RoutedEventArgs e)
        {
            var selected = tree.SelectedItem as SqlScriptsViewModel;
            if (selected != null)
            {
                (tree.DataContext as ScriptsOrderViewModel).RemoveModel(selected);
            }
        }

        private void btnOutputPathSelector_Click(object sender, RoutedEventArgs e)
        {
            var dlg1 = new WebPublisher.Controls.FolderBrowserDialogEx();
            dlg1.Description = "Select a output folder: ";
            dlg1.ShowNewFolderButton = false;
            dlg1.ShowEditBox = true;
            //dlg1.NewStyle = false;
            dlg1.SelectedPath = txtVirtualFolder.Text;
            dlg1.ShowFullPathInEditBox = true;
            dlg1.RootFolder = System.Environment.SpecialFolder.MyComputer;

            // Show the FolderBrowserDialog.
            var result = dlg1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtVirtualFolder.Text = dlg1.SelectedPath;
            }
        }

        private void btnSqlPathSelector_Click(object sender, RoutedEventArgs e)
        {
            var dlg1 = new WebPublisher.Controls.FolderBrowserDialogEx();
            dlg1.Description = "Select a output folder: ";
            dlg1.ShowNewFolderButton = false;
            dlg1.ShowEditBox = true;
            //dlg1.NewStyle = false;
            dlg1.SelectedPath = tbSqlDataPath.Text;
            dlg1.ShowFullPathInEditBox = true;
            dlg1.RootFolder = System.Environment.SpecialFolder.MyComputer;

            // Show the FolderBrowserDialog.
            var result = dlg1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                tbSqlDataPath.Text = dlg1.SelectedPath;
            }
        }
    }
}

