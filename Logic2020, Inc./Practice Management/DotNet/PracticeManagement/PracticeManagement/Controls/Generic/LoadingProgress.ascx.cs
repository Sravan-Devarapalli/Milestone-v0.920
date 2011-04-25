using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Generic
{
    public partial class LoadingProgress : System.Web.UI.UserControl
    {
        public string DisplayText { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Title == "Practice Management - Opportunity Details")
            {
                DisplayText = "Saving ...";
            }
            else
            {
                DisplayText = "Please Wait..";
            }            
        }
    }
}
