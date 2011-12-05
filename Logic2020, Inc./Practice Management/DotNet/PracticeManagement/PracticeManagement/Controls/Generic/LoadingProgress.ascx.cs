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

        public string AssociatedUpdatePanelID
        {
            set 
            {
                upTimeEntries.AssociatedUpdatePanelID = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(DisplayText))
            {
                DisplayText = "Please Wait..";
            }            
        }
    }
}
