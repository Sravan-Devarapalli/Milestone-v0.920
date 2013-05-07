using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace PraticeManagement.Controls.Generic
{
    public class AjaxValidationSummary : ValidationSummary
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), this.ClientID + "AjaxValidationSummary", ";", true);
        }

    }
}



