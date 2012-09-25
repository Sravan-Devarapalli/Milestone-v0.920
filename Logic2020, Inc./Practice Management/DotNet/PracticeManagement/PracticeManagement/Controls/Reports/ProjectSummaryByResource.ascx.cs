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
        private PraticeManagement.Reporting.ProjectSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.ProjectSummaryReport)Page); }
        }

        public FilteredCheckBoxList cblProjectRolesControl
        {
            get
            {
                return ucProjectSummaryReport.cblProjectRolesControl;
            }
        }

        public LinkButton LnkbtnSummaryObject
        {
            get
            {
                return lnkbtnSummary;
            }
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SwitchView((Control)sender, viewIndex);
        }

        private void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
            LoadActiveTabInByResource();
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblProjectViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        public void SelectView(Control sender, int viewIndex)
        {
            mvProjectReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        public void LoadActiveTabInByResource(bool isFirstTime = false)
        {
            if (mvProjectReport.ActiveViewIndex == 0)
            {
                PopulateByResourceSummaryReport(isFirstTime);
            }
            else
            {
                PopulateByResourceDetailReport();
            }
        }

        public void PopulateByResourceSummaryReport(bool isFirstTime = false)
        {
            PersonLevelGroupedHours[] data;

            if (isFirstTime)
            {
                data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByResource(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.PeriodSelected == "*" ? null : HostingPage.StartDate, HostingPage.PeriodSelected == "*" ? null : HostingPage.EndDate, null));
            }
            else
            {
                data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByResource(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.PeriodSelected == "*" ? null : HostingPage.StartDate, HostingPage.PeriodSelected == "*" ? null : HostingPage.EndDate, ucProjectSummaryReport.cblProjectRolesControl.SelectedItemsXmlFormat));
            }
            ucProjectSummaryReport.DataBindByResourceSummary(data, isFirstTime);
            PopulateHeaderSection(data.ToList());
        }

        public void PopulateByResourceDetailReport()
        {
            ucProjectDetailReport.hdnGroupByControl.Value = "Person";
            ucProjectDetailReport.btnGroupByControl.Text = ucProjectDetailReport.btnGroupByControl.ToolTip = "Group By Date";
            var data = ServiceCallers.Custom.Report(r => r.ProjectDetailReportByResource(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.PeriodSelected == "*" ? null : HostingPage.StartDate, HostingPage.PeriodSelected == "*" ? null : HostingPage.EndDate, ucProjectSummaryReport.cblProjectRolesControl.SelectedItemsXmlFormat)).ToList();
            ucProjectDetailReport.DataBindByResourceDetail(data);
            PopulateHeaderSection(data.ToList());
        }

        public void PopulateHeaderSection(List<PersonLevelGroupedHours> personLevelGroupedHoursList)
        {
            //if (personLevelGroupedHoursList.Count > 0)
            //{
            //tbHeader.Style["display"] = "";
            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.StartDate, HostingPage.EndDate));
            double billableHours = personLevelGroupedHoursList.Sum(p => p.DayTotalHours != null ? p.DayTotalHours.Sum(d => d.BillableHours) : p.BillableHours);
            double nonBillableHours = personLevelGroupedHoursList.Sum(p => p.NonBillableHours);
            double projectedHours = personLevelGroupedHoursList.Sum(p => p.ForecastedHours);

            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (billableHours != 0 || nonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(billableHours, nonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }

            ltrlAccount.Text = project.Client.HtmlEncodedName;
            ltrlBusinessUnit.Text = project.Group.HtmlEncodedName;
            ltrlProjectedHours.Text = projectedHours.ToString(Constants.Formatting.DoubleValue);
            ltrlProjectName.Text = project.HtmlEncodedName;
            ltrlProjectNumber.Text = project.ProjectNumber;
            ltrlProjectStatusAndBillingType.Text = string.IsNullOrEmpty(project.BillableType) ? project.Status.Name : project.Status.Name + ", " + project.BillableType;
            ltrlProjectRange.Text = HostingPage.ProjectRange;
            ltrlTotalHours.Text = (billableHours + nonBillableHours).ToString(Constants.Formatting.DoubleValue);
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
            //}
            //else
            //{
            //    tbHeader.Style["display"] = "none";
            //}
        }

    }
}

