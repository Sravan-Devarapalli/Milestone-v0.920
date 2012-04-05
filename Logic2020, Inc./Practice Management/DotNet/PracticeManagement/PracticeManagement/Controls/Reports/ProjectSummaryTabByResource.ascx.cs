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
    public partial class ProjectSummaryTabByResource : System.Web.UI.UserControl
    {
        private PraticeManagement.Reporting.ProjectSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.ProjectSummaryReport)Page.Page); }
        }

        public List<PersonLevelGroupedHours> TimeEntriesGroupByPersonSummaryList
        {
            get
            {
                return ViewState["ProjectSummaryByResourceTimeEntries"] as List<PersonLevelGroupedHours>;
            }
            set
            {
                ViewState["ProjectSummaryByResourceTimeEntries"] = value;
            }
        }

        protected void repResource_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

            }
        }

        public void DataBindByResourceSummary(PersonLevelGroupedHours[] reportData)
        {
            TimeEntriesGroupByPersonSummaryList = reportData.ToList();

            if (reportData.Count() > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repResource.Visible = true;
                repResource.DataSource = reportData;
                repResource.DataBind();

            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repResource.Visible = false;
            }
        }

        public string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {

            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber));


            StringBuilder sb = new StringBuilder();
            sb.Append(project.Client.Name);
            sb.Append("\t");
            sb.Append(project.Group.Name);
            sb.Append("\t");
            sb.AppendLine();
            //P081003 - [ProjectName]
            sb.Append(string.Format("{0} - {1}", project.ProjectNumber, project.Name));
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(project.Status.Name);
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(HostingPage.ProjectRange);
            sb.Append("\t");
            sb.AppendLine();
            sb.AppendLine();

            if (TimeEntriesGroupByPersonSummaryList.Count > 0)
            {
                //Header
                sb.Append("Resource");
                sb.Append("\t");
                sb.Append("Project Role");
                sb.Append("\t");
                sb.Append("Billable");
                sb.Append("\t");
                sb.Append("Non-Billable");
                sb.Append("\t");
                sb.Append("Total");
                sb.Append("\t");
                sb.Append("Person Variance (in Hours)");
                sb.Append("\t");
                sb.AppendLine();

                var list = TimeEntriesGroupByPersonSummaryList.OrderBy(p => p.Person.PersonLastFirstName);

                //Data
                foreach (var byPerson in list)
                {
                    sb.Append(byPerson.Person.PersonLastFirstName);
                    sb.Append("\t");
                    sb.Append(byPerson.Person.ProjectRoleName);
                    sb.Append("\t");
                    sb.Append(GetDoubleFormat(byPerson.BillableHours));
                    sb.Append("\t");
                    sb.Append(GetDoubleFormat(byPerson.NonBillableHours));
                    sb.Append("\t");
                    sb.Append(GetDoubleFormat(byPerson.TotalHours));
                    sb.Append("\t");
                    sb.Append(byPerson.Variance);
                    sb.Append("\t");
                    sb.AppendLine();
                }
            }
            else
            {
                sb.Append("There are no Time Entries towards this project.");
            }

            var filename = string.Format("{0}_{1}_{2}.xls", project.ProjectNumber, project.Name, "_ByResourceSummary");
            filename = filename.Replace(' ', '_');
            GridViewExportUtil.Export(filename, sb);

        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

    }
}
