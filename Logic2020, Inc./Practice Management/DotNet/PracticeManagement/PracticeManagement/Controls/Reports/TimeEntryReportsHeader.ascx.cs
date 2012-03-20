using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Reports
{

    public partial class TimeEntryReportsHeader : System.Web.UI.UserControl
    {
        private void ApplyCssHeader()
        {
            if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
            {
                thTimePeriod.Style["background-color"] = "#e2ebff";
                thProject.Style["background-color"]  = "White";
                thPerson.Style["background-color"]  = "White";

            }
            else if (Page is PraticeManagement.Reporting.ProjectSummaryReport)
            {
                thTimePeriod.Style["background-color"]  = "White";
                thProject.Style["background-color"] = "#e2ebff";
                thPerson.Style["background-color"]  = "White";
            }
            else if (Page is PraticeManagement.Reporting.PersonDetailTimeReport)
            {
                thTimePeriod.Style["background-color"]  = "White";
                thProject.Style["background-color"]  = "White";
                thPerson.Style["background-color"] = "#e2ebff";
            }
         }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ApplyCssHeader();
            }
        }
    }
}
