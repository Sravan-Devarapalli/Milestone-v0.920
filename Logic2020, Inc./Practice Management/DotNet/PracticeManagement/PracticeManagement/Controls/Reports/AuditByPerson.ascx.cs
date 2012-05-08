using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;

namespace PraticeManagement.Controls.Reports
{
    public partial class AuditByPerson : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        public void PopulateByResourceData(PersonLevelTimeEntriesHistory[] reportDataByPerson)
        {
            var reportDataList = reportDataByPerson.ToList();
           
            if (reportDataList.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repPersons.Visible = true;
                repPersons.DataSource = reportDataList;
                repPersons.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repPersons.Visible = false;
            }
        }
       
    }
}
