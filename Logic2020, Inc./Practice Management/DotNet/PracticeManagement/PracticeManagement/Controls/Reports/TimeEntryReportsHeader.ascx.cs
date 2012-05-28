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
                thByAccount.Style["background-color"] = "White";
                thProject.Style["background-color"] = "White";
                thPerson.Style["background-color"] = "White";
            }
            else if (Page is PraticeManagement.Reporting.AccountSummaryReport)
            {
                thTimePeriod.Style["background-color"] = "White";
                thByAccount.Style["background-color"] = "#e2ebff";
                thProject.Style["background-color"] = "White";
                thPerson.Style["background-color"] = "White";
            }
            else if (Page is PraticeManagement.Reporting.ProjectSummaryReport)
            {
                thTimePeriod.Style["background-color"] = "White";
                thByAccount.Style["background-color"] = "White";
                thProject.Style["background-color"] = "#e2ebff";
                thPerson.Style["background-color"] = "White";
                
            }
            else if (Page is PraticeManagement.Reporting.PersonDetailTimeReport)
            {
                thTimePeriod.Style["background-color"] = "White";
                thByAccount.Style["background-color"] = "White";
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

            thByAccount.Visible = UrlAuthorizationModule.CheckUrlAccessForPrincipal(Constants.ApplicationPages.AccountSummaryReport, new RolePrincipal(HttpContext.Current.User.Identity), "GET");
            count = thPerson.Visible ? count + 1 : count;

            if (count == 1)
            {
                tdFirst.Style["width"] = "37%";
                tdSecond.Style["width"] = "26%";
                tdThird.Style["width"] = "37%";
            }
            else if(count == 2)
            {
                tdFirst.Style["width"] = "25%";
                tdSecond.Style["width"] = "50%";
                tdThird.Style["width"] = "25%";
            }
            else if (count == 3)
            {
                tdFirst.Style["width"] = "12%";
                tdSecond.Style["width"] = "76%";
                tdThird.Style["width"] = "12%";
            }

            Count = count;

        }
    }
}
