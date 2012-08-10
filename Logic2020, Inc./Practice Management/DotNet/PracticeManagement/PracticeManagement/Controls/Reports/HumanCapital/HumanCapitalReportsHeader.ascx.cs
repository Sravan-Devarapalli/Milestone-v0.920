using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace PraticeManagement.Controls.Reports.HumanCapital
{
    public partial class HumanCapitalReportsHeader : System.Web.UI.UserControl
    {
        public int Count { get; set; }

        private void ApplyCssHeader()
        {
            if (Page is PraticeManagement.Reporting.NewHireReport)
            {
                thNewHireReport.Attributes["class"] = "bgcolorE2EBFF";
                thTerminationReport.Attributes["class"] = "bgcolorwhite";
            }
            else if (Page is PraticeManagement.Reporting.TerminationReport)
            {
                thNewHireReport.Attributes["class"] = "bgcolorwhite";
                thTerminationReport.Attributes["class"] = "bgcolorE2EBFF";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ApplyCssHeader();
            }
            var count = 0;

            thNewHireReport.Visible = UrlAuthorizationModule.CheckUrlAccessForPrincipal(Constants.ApplicationPages.NewHireReport, new RolePrincipal(HttpContext.Current.User.Identity), "GET");
            count = thNewHireReport.Visible ? count + 1 : count;

            thTerminationReport.Visible = UrlAuthorizationModule.CheckUrlAccessForPrincipal(Constants.ApplicationPages.TerminationReport, new RolePrincipal(HttpContext.Current.User.Identity), "GET");
            count = thTerminationReport.Visible ? count + 1 : count;

            if (count == 1)
            {
                tdFirst.Attributes["class"] = "width30P";
                tdSecond.Attributes["class"] = "Width40P";
                tdThird.Attributes["class"] = "width30P";
            }
            else if (count == 2)
            {
                tdFirst.Attributes["class"] = "Width10Percent";
                tdSecond.Attributes["class"] = "Width80Percent";
                tdThird.Attributes["class"] = "Width10Percent";
            }

            Count = count;

        }
    }
}
