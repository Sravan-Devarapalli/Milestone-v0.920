using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebPublisher.Bindings.Models
{
    public class FileViewModel : TreeViewItemViewModel
    {
        public string FilePath { get; set; }

        public FileViewModel(string path, TreeViewItemViewModel parent)
            : base(parent, false)
        {
            FilePath = path;
            Name = System.IO.Path.GetFileName(FilePath);
        }

        public FileViewModel(string path)
            : base(null, false)
        {
            FilePath = path;
            Name = System.IO.Path.GetFileName(FilePath);
        }

        protected override string GetImageUri()
        {
            return base.GetImageUri(); 
        }
    }
}

