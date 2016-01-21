using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class ClientFilters
    {
        public string SearchText
        {
            get;
            set;
        }

        public string View
        {
            get;
            set;
        }

        public bool ShowActiveOnly
        {
            get;
            set;
        }
    }
}

