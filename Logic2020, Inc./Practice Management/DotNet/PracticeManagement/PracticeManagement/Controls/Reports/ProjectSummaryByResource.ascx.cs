using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;
using System.Text;

namespace PraticeManagement.Controls.Reports
{
    public partial class ProjectSummaryByResource : System.Web.UI.UserControl
    {

        public List<PersonLevelGroupedHours> TimeEntriesGroupByPerson
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

        private PraticeManagement.Reporting.ProjectSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.ProjectSummaryReport)Page); }
        }

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
            TimeEntriesGroupByPerson = reportData.ToList();

            if (reportData.Count() > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repResource.Visible = true;
                repResource.DataSource = reportData;
                repResource.DataBind();
                PopulateHeaderSection(reportData.ToList());
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repResource.Visible = false;
            }


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

            var list = TimeEntriesGroupByPerson.OrderBy(p => p.Person.PersonLastFirstName);

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


            var filename = string.Format("{0}_{1}_{2}.xls", project.ProjectNumber, project.Name, "_ByResource");
            GridViewExportUtil.Export(filename, sb);

        }

        public string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        private void PopulateHeaderSection(List<PersonLevelGroupedHours> personLevelGroupedHoursList)
        {

            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber)); 

            double billableHours = personLevelGroupedHoursList.Sum(p => p.BillableHours);
            double nonBillableHours = personLevelGroupedHoursList.Sum(p => p.NonBillableHours);
            double projectedHours = personLevelGroupedHoursList.Sum(p => p.ForecastedHours);

            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (billableHours != 0 || nonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(billableHours, nonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }

            ltrlAccount.Text = project.Client.Name;
            ltrlBusinessUnit.Text = project.Group.Name;
            ltrlProjectedHours.Text = projectedHours.ToString(Constants.Formatting.DoubleValue);
            ltrlProjectName.Text = project.Name;
            ltrlProjectNumber.Text = project.ProjectNumber;
            ltrlProjectStatus.Text = project.Status.Name;
            ltrlProjectRange.Text = HostingPage.ProjectRange;
            ltrlTotalHours.Text = (billableHours + nonBillableHours).ToString(Constants.Formatting.DoubleValueWithZeroPadding);
            ltrlBillableHours.Text = billableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlNonBillableHours.Text = nonBillableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlBillablePercent.Text = billablePercent.ToString();
            ltrlNonBillablePercent.Text = nonBillablePercent.ToString();

            if (billablePercent == 0 && nonBillablePercent == 0)
            {
                trBillable.Height = "1px";
                trNonBillable.Height = "1px";
            }
            else if (billablePercent == 100)
            {
                trBillable.Height = "80px";
                trNonBillable.Height = "1px";
            }
            else if (billablePercent == 0 && nonBillablePercent == 100)
            {
                trBillable.Height = "1px";
                trNonBillable.Height = "80px";
            }
            else
            {
                int billablebarHeight = (int)(((float)80 / (float)100) * billablePercent);
                trBillable.Height = billablebarHeight.ToString() + "px";
                trNonBillable.Height = (80 - billablebarHeight).ToString() + "px";
            }

        }

    }
}

