using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class VendorFilters
    {
        public bool ShowActive
        {
            get;
            set;
        }

        public bool ShowInactive
        {
            get;
            set;
        }

        public string VendorTypeIds
        {
            get;
            set;
        }

        public string SearchText
        {
            get;
            set;
        }
    }
}

