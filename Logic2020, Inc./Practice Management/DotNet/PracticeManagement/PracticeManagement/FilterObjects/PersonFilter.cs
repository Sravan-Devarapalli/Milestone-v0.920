using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace PraticeManagement.FilterObjects
{
    [Serializable]
    public class PersonFilter
    {
        public bool ShowActive
        {
            set;
            get;
        }
        public bool ShowInactive
        {
            set;
            get;
        }
        public bool ShowProjected
        {
            set;
            get;
        }
        public bool ShowTerminated
        {
            set;
            get;
        }
        public string SelectedPracticeIds
        {
            get;
            set;
        }
        public string SelectedPayTypeIds
        {
            get;
            set;
        }
        public string SearchText
        {
            get;
            set;
        }
        public string SelectedRecruiterIds
        {
            get;
            set;
        }
        public int SelectedPageSizeIndex
        {
            get;
            set;
        }
        public char? Alphabet
        {
            get;
            set;
        }
        public string SortBy
        {
            set;
            get;
        }
        public SortDirection SortOrder
        {
            set;
            get;
        }
        public int? CurrentIndex
        {
            get;
            set;
        }
        public bool DisplaySearchResuts
        {
            set;
            get;
        }
    }
}
