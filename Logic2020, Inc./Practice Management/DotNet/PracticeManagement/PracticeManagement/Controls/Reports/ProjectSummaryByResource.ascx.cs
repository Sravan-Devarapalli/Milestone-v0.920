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

        private void SelectView(Control sender, int viewIndex)
        {
            mvProjectReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        public void LoadActiveTabInByResource()
        {
            if (mvProjectReport.ActiveViewIndex == 0)
            {
                PopulateByResourceSummaryReport();
            }
            else
            {
                PopulateByResourceDetailReport();
            }
        }

        private void PopulateByResourceSummaryReport()
        {
            var data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByResource(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.PeriodSelected == "0" ? HostingPage.StartDate : null, HostingPage.PeriodSelected == "0" ? HostingPage.EndDate : null));
            ucProjectSummaryReport.DataBindByResourceSummary(data);
            PopulateHeaderSection(data.ToList());
        }

        private void PopulateByResourceDetailReport()
        {
            var data = ServiceCallers.Custom.Report(r => r.ProjectDetailReportByResource(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.PeriodSelected == "0" ? HostingPage.StartDate : null, HostingPage.PeriodSelected == "0" ? HostingPage.EndDate : null));
            ucProjectDetailReport.DataBindByResourceDetail(data);
            PopulateHeaderSection(data.ToList());
        }

        private void PopulateHeaderSection(List<PersonLevelGroupedHours> personLevelGroupedHoursList)
        {
            if (personLevelGroupedHoursList.Count > 0)
            {
                tbHeader.Style["display"] = "";
                var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber));
                double billableHours = personLevelGroupedHoursList.Sum(p => p.DayTotalHours != null ?  p.DayTotalHours.Sum(d => d.BillableHours) : p.BillableHours);
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
            }
            else
            {
                tbHeader.Style["display"] = "none";
            }
        }

    }
}

