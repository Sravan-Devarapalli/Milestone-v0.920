using System;
using PraticeManagement.Utils;
using DataTransferObjects;
using DataTransferObjects.Filters;

namespace PraticeManagement.Reporting
{
    public partial class UtilizationTimeline : System.Web.UI.Page
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

        public void RegisterClientScripts()
        {
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "EnableDisableRadioButtons();", true);
        }
    }
}

