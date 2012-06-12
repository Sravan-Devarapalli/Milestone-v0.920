using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using DataTransferObjects.Reports;

namespace PraticeManagement.Controls.Reports.ByPerson
{
    public partial class GroupByProject : System.Web.UI.UserControl
    {
        public void DatabindRepepeaterPersonDetails(List<TimeEntriesGroupByClientAndProject> timeEntriesGroupByClientAndProjectList, string name)
        {
            lblPerson.Text = name;
            lblTotalHours.Text = GetDoubleFormat(timeEntriesGroupByClientAndProjectList.Sum(p => p.TotalHours));
            ucPersonDetailReport.DatabindRepepeaterPersonDetails(timeEntriesGroupByClientAndProjectList);
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }
    }
}

