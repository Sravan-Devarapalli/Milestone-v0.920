using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class PersonsByProjectFilters
    {
        public string ClientIds
        {
            get;
            set;
        }

        public string PayTypeIds
        {
            get;
            set;
        }
        public string PersonStatusIds
        {
            get;
            set;
        }
        public string ProjectStatusIds
        {
            get;
            set;
        }
        public string PracticeIds
        {
            get;
            set;
        }
        public bool ExcludeInternalPractices
        {
            get;
            set;
        }
    }
}

