using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Text;

namespace PraticeManagement.Controls.Reports
{
    public partial class PersonSummaryReport : System.Web.UI.UserControl
    {
        public List<TimeEntriesGroupByClientAndProject> TimeEntriesGroupByClientAndProjectList
        {
            get 
            {
                return ViewState["TimeEntriesGroupByClientAndProjectList_Key"] as List<TimeEntriesGroupByClientAndProject>;
            }
            set 
            {
                ViewState["TimeEntriesGroupByClientAndProjectList_Key"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void DatabindRepepeaterSummary(List<TimeEntriesGroupByClientAndProject> timeEntriesGroupByClientAndProjectList)
        {
            TimeEntriesGroupByClientAndProjectList = timeEntriesGroupByClientAndProjectList;

            if(timeEntriesGroupByClientAndProjectList.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repSummary.Visible = true;
                repSummary.DataSource = timeEntriesGroupByClientAndProjectList;
                repSummary.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repSummary.Visible = false;
            }
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
              //TimeEntriesGroupByClientAndProjectList
            StringBuilder sb = new StringBuilder();
            //sb.Append();

        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        
    }
}
