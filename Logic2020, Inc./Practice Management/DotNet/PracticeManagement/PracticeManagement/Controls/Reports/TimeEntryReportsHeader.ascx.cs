using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace PraticeManagement.Controls.Reports
{

    public partial class TimeEntryReportsHeader : System.Web.UI.UserControl
    {

        public int Count { get; set; }

        private void ApplyCssHeader()
        {
            if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
            {
                thTimePeriod.Style["background-color"] = "#e2ebff";
                thProject.Style["background-color"] = "White";
                thPerson.Style["background-color"] = "White";

            }
            else if (Page is PraticeManagement.Reporting.ProjectSummaryReport)
            {
                thTimePeriod.Style["background-color"] = "White";
                thProject.Style["background-color"] = "#e2ebff";
                thPerson.Style["background-color"] = "White";
            }
            else if (Page is PraticeManagement.Reporting.PersonDetailTimeReport)
            {
                thTimePeriod.Style["background-color"] = "White";
                thProject.Style["background-color"] = "White";
                thPerson.Style["background-color"] = "#e2ebff";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ApplyCssHeader();
            }
            var count = 0;

            thTimePeriod.Visible = UrlAuthorizationModule.CheckUrlAccessForPrincipal(Constants.ApplicationPages.TimePeriodSummaryReport, new RolePrincipal(HttpContext.Current.User.Identity), "GET");
            count = thTimePeriod.Visible ? count + 1 : count;
            thProject.Visible = UrlAuthorizationModule.CheckUrlAccessForPrincipal(Constants.ApplicationPages.ProjectSummaryReport, new RolePrincipal(HttpContext.Current.User.Identity), "GET");
            count = thProject.Visible ? count + 1 : count;
            thPerson.Visible = UrlAuthorizationModule.CheckUrlAccessForPrincipal(Constants.ApplicationPages.PersonDetailReport, new RolePrincipal(HttpContext.Current.User.Identity), "GET");
            count = thPerson.Visible ? count + 1 : count;

            if (count == 1)
            {
                tdFirst.Style["width"] = "35%";
                tdSecond.Style["width"] = "30%";
                tdThird.Style["width"] = "35%";
            }
            else if(count == 2)
            {
                tdFirst.Style["width"] = "20%";
                tdSecond.Style["width"] = "60%";
                tdThird.Style["width"] = "20%";
            }

            Count = count;

        }
    }
}
