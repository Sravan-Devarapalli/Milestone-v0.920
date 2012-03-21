using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;

namespace PraticeManagement.Controls.Reports
{
    public partial class ProjectSummaryByResource : System.Web.UI.UserControl
    {



        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void repResource_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

            }
        }

        public void DataBindResource(PersonLevelGroupedHours[] reportData)
        {
            repResource.DataSource = reportData;
            repResource.DataBind();
        }
    }
}
