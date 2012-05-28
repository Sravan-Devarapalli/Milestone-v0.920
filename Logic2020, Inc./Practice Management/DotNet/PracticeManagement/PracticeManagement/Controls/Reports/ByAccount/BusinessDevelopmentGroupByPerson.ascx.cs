using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class BusinessDevelopmentGroupByPerson : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void PopulateData(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate)
        {
            var data = ServiceCallers.Custom.Report(r => r.AccountReportGroupByPerson(accountId, businessUnitIds, startDate, endDate));
        }
    }
}
