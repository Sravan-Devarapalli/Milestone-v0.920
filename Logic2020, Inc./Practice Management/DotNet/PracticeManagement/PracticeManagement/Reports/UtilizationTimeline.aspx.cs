using System;

namespace PraticeManagement.Reporting
{
    public partial class UtilizationTimeline : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void RegisterClientScripts()
        {
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "EnableDisableRadioButtons();", true);
        }
    }
}

