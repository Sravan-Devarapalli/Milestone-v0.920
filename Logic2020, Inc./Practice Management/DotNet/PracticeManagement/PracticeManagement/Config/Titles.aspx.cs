using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Config
{
    public partial class Titles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ServiceCallers.Custom.Title(t => t.GetAllTitles());
            ServiceCallers.Custom.Title(t => t.GetTitleById(1));
            //ServiceCallers.Custom.Title(t => t.GetTitleById(1));
        }
    }
}
