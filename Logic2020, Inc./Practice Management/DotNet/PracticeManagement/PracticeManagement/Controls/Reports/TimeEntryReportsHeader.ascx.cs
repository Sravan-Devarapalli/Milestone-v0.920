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
                thTimePeriod.Attributes["class"] = "bgcolorE2EBFF";
                thProject.Attributes["class"] = "bgcolorwhite";
                thPerson.Attributes["class"] = "bgcolorwhite";
            }
            else if (Page is PraticeManagement.Reporting.ProjectSummaryReport)
            {
                thTimePeriod.Attributes["class"] = "bgcolorwhite";
                thProject.Attributes["class"] = "bgcolorE2EBFF";
                thPerson.Attributes["class"] = "bgcolorwhite";
            }
            else if (Page is PraticeManagement.Reporting.PersonDetailTimeReport)
            {
                thTimePeriod.Attributes["class"] = "bgcolorwhite";
                thProject.Attributes["class"] = "bgcolorwhite";
                thPerson.Attributes["class"] = "bgcolorE2EBFF";
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
                tdFirst.Attributes["class"] = "Width35Percent";
                tdSecond.Attributes["class"] = "width30P";
                tdThird.Attributes["class"] = "Width35Percent";
            }
            else if (count == 2)
            {
                tdFirst.Attributes["class"] = "Width20Percent";
                tdSecond.Attributes["class"] = "width60P";
                tdThird.Attributes["class"] = "Width20Percent";
            }
            else if (count == 3)
            {
                tdFirst.Attributes["class"] = "Width1Percent";
                tdSecond.Attributes["class"] = "Width98Percent";
                tdThird.Attributes["class"] = "Width1Percent";
            }

            Count = count;

        }
    }
}

