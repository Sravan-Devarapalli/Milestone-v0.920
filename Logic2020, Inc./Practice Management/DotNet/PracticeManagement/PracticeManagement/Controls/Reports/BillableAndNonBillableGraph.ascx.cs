using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Reports
{
    public partial class BillableAndNonBillableGraph : System.Web.UI.UserControl
    {
        public string BillablValue
        {
            set 
            {
                hdnBillable.Value = value;
            }
        }

        public string NonBillablValue
        {
            set
            {
                hdnNonBillable.Value = value;
            }
        }
    }
}
