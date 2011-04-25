using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PraticeManagement.Controls.Generic.Filtering
{
    [Serializable]
    public class OpportunityFilterObject
    {
        public bool ActiveClientsOnly { get; set; }
        public string SearchText { get; set; }
        public int? ClientId { get; set; }
        public int? SalespersonId { get; set; }

        public void ResetProperties()
        {
            ActiveClientsOnly = true;
            ClientId = null;
            SalespersonId = null;
            SearchText = string.Empty;
        }

        public OpportunityFilterObject()
        {
            ResetProperties();
        }
    }
}

