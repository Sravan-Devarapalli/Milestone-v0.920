using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;

namespace PraticeManagement.Config
{
    public partial class SiteMap : PracticeManagementPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void Display()
        {
        }

        protected void menu_OnMenuItemDataBound(object sender, MenuEventArgs e)
        {
            var item = e.Item;
            if (item.NavigateUrl.Contains("Temp.aspx"))
            {
                item.NavigateUrl = string.Empty;
            }

            if (item.ToolTip.ToLower() == "show url")
            {
                item.ToolTip = item.Text;
            }
        }
    }
}
