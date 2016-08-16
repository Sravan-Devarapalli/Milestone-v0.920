using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using System.Web.Services;
using System.Web.Script.Services;
using PraticeManagement.Controls.Projects;

namespace PraticeManagement.Config
{
    public partial class ProjectsReport : PracticeManagementSearchPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void Display()
        {
            projectSummary.Display();
        }

        protected override void RedirectWithBack(string redirectUrl, string backUrl)
        {
            projectSummary.RedirectWithBack(redirectUrl, backUrl);
            base.RedirectWithBack(redirectUrl, backUrl);
        }

        public override string SearchText
        {
            get
            {
                Session["Filters"] = projectSummary.GetFilterSettings();
                return projectSummary.SearchText;
            }
        }

        [WebMethod]
        [ScriptMethod]
        public static string RenderMonthMiniReport(string contextKey)
        {
            return ProjectSummary.RenderMonthMiniReport(contextKey);
        }
    }
}

