using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;

namespace WebPublisher.Bindings.Models
{
    public class FilesViewModel : NotifyPropertyChanged
    {
        public ObservableCollection<FileViewModel> Files
        {
            get;
            private set;
        }

        public FilesViewModel(string path) 
        {
            Files = new ObservableCollection<FileViewModel>();
            if (!string.IsNullOrEmpty(path))
            {
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    Files.Add(new FileViewModel(file) { IsChecked = true });
                }
            }
        }
    }
}

