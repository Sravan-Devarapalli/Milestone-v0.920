using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;

namespace PraticeManagement.Sandbox
{
    public partial class ScrollingDropdownTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.FillTimeEntryPersonList(cblPersons, Resources.Controls.AllPersons, null);
            }
        }
    }
}
