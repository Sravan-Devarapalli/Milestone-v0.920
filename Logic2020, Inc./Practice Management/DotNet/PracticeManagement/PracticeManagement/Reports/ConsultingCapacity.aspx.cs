using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Reporting
{
    public partial class ConsultingCapacity : System.Web.UI.Page
    {
        public Controls.Reports.ConsultantsWeeklyReport ConsultantsControl
        {
            get
            {
                return repWeekly;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}
