using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;
using System.Text;
using DataTransferObjects.Reports.ByAccount;
using ByBusinessDevelopment = PraticeManagement.Controls.Reports.ByAccount.ByBusinessDevelopment;

namespace PraticeManagement.Controls.Reports
{
    public partial class TimePeriodSummaryByProject : System.Web.UI.UserControl
    {
        private string TimePeriodSummaryReportExport = "TimePeriod Summary Report By Project";
        private string ByProjectByResourceUrl = "ProjectSummaryReport.aspx?StartDate={0}&EndDate={1}&PeriodSelected={2}&ProjectNumber={3}";
        private string ShowPanel = "ShowPanel('{0}', '{1}','{2}');";
        private string HidePanel = "HidePanel('{0}');";
        private string OnMouseOver = "onmouseover";
        private string OnMouseOut = "onmouseout";

        private HtmlImage ImgClientFilter { get; set; }

        private HtmlImage ImgProjectStatusFilter { get; set; }

        private Label LblProjectedHours { get; set; }

        private Label LblBillable { get; set; }

        private Label LblNonBillable { get; set; }

        private Label LblActualHours { get; set; }

        private Label LblBillableHoursVariance { get; set; }

        public string SelectedProjectNumber
        {
            get
            {
                return (string)ViewState["SelectedProjectNumberForDetails"];
            }
            set
            {
                ViewState["SelectedProjectNumberForDetails"] = value;
            }
        }

        public AjaxControlToolkit.ModalPopupExtender MpeProjectDetailReport
        {
            get
            {
                return mpeProjectDetailReport;
            }
        }

        public ByBusinessDevelopment ByBusinessDevelopmentControl
        {
            get
            {
                return ucGroupByBusinessDevelopment;
            }
        }

        private PraticeManagement.Reporting.TimePeriodSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.TimePeriodSummaryReport)Page); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            cblClients.OKButtonId = cblProjectStatus.OKButtonId = btnFilterOK.ClientID;
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetCurrencyFormat(double value)
        {
            return value > 0 ? value.ToString(Constants.Formatting.CurrencyFormat) : "$0";
        }

        protected string GetVarianceSortValue(string variance)
        {
            if (variance.Equals("N/A"))
            {
                return int.MinValue.ToString();
            }
            return variance;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(TimePeriodSummaryReportExport);

            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByProject(HostingPage.StartDate.Value, HostingPage.EndDate.Value,
                    cblClients.SelectedItems, cblProjectStatus.SelectedItems));

                string filterApplied = "Filters applied to columns: ";
                List<string> filteredColoums = new List<string>();
                if (!cblClients.AllItemsSelected)
                {
                    filteredColoums.Add("Project");
                }
                if (!cblProjectStatus.AllItemsSelected)
                {
                    filteredColoums.Add("Status");
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("TimePeriod_ByProject Report");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(data.Length + " Projects");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(HostingPage.Range);
                sb.Append("\t");
                if (filteredColoums.Count > 0)
                {
                    sb.AppendLine();
                    for (int i = 0; i < filteredColoums.Count; i++)
                    {
                        if (i == filteredColoums.Count - 1)
                            filterApplied = filterApplied + filteredColoums[i] + ".";
                        else
                            filterApplied = filterApplied + filteredColoums[i] + ",";
                    }
                    sb.Append(filterApplied);
                    sb.Append("\t");
                }
                sb.AppendLine();
                sb.AppendLine();

                if (data.Length > 0)
                {
                    //Header
                    sb.Append("Account");
                    sb.Append("\t");
                    sb.Append("Account Name");
                    sb.Append("\t");
                    sb.Append("Business Unit");
                    sb.Append("\t");
                    sb.Append("Business Unit Name");
                    sb.Append("\t");
                    sb.Append("Project");
                    sb.Append("\t");
                    sb.Append("Project Name");
                    sb.Append("\t");
                    sb.Append("Status");
                    sb.Append("\t");
                    sb.Append("Billing");
                    sb.Append("\t");
                    sb.Append("Projected Hours");
                    sb.Append("\t");
                    sb.Append("Billable");
                    sb.Append("\t");
                    sb.Append("Non-Billable");
                    sb.Append("\t");
                    sb.Append("Actual Hours");
                    sb.Append("\t");
                    sb.Append("Billable Hours Variance");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var projectLevelGroupedHours in data)
                    {
                        sb.Append(projectLevelGroupedHours.Project.Client.Code);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.Client.HtmlEncodedName);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.Group.Code);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.Group.HtmlEncodedName);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.ProjectNumber);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.HtmlEncodedName);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.Status.Name);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.BillingType);
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.ForecastedHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.BillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.NonBillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.TotalHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.BillableHoursVariance));
                        sb.Append("\t");
                        sb.AppendLine();
                    }

                }
                else
                {
                    sb.Append("There are no Time Entries towards this range selected.");
                }
                //“TimePeriod_ByProject_DateRange.xls”.  
                var filename = string.Format("TimePeriod_ByProject_{0}-{1}.xls", HostingPage.StartDate.Value.ToString("MM/dd/yyyy"), HostingPage.EndDate.Value.ToString("MM/dd/yyyy"));
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        public void PopulateByProjectData(bool isPopulateFilters = true)
        {
            ProjectLevelGroupedHours[] data;
            if (isPopulateFilters)
            {
                data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByProject(HostingPage.StartDate.Value, HostingPage.EndDate.Value, null, null));
            }
            else
            {
                data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByProject(HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblClients.SelectedItems, cblProjectStatus.SelectedItems));
            }
            DataBindProject(data, isPopulateFilters);
        }

        protected void repProject_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgClientFilter = e.Item.FindControl("imgClientFilter") as HtmlImage;
                ImgProjectStatusFilter = e.Item.FindControl("imgProjectStatusFilter") as HtmlImage;

                LblProjectedHours = e.Item.FindControl("lblProjectedHours") as Label;
                LblBillable = e.Item.FindControl("lblBillable") as Label;
                LblNonBillable = e.Item.FindControl("lblNonBillable") as Label;
                LblActualHours = e.Item.FindControl("lblActualHours") as Label;
                LblBillableHoursVariance = e.Item.FindControl("lblBillableHoursVariance") as Label;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (ProjectLevelGroupedHours)e.Item.DataItem;
                var lnkProject = e.Item.FindControl("lnkProject") as LinkButton;
                var imgZoomIn = e.Item.FindControl("imgZoomIn") as HtmlImage;
                var LnkActualHours = e.Item.FindControl("lnkActualHours") as LinkButton;

                lnkProject.Attributes["onmouseover"] = string.Format("document.getElementById(\'{0}\').style.display='';", imgZoomIn.ClientID);
                lnkProject.Attributes["onmouseout"] = string.Format("document.getElementById(\'{0}\').style.display='none';", imgZoomIn.ClientID);
                LnkActualHours.Attributes["NavigationUrl"] = string.Format(ByProjectByResourceUrl, HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat),
                    HostingPage.EndDate.Value.Date.ToString(Constants.Formatting.EntryDateFormat), HostingPage.RangeSelected, dataItem.Project.ProjectNumber);
            }
        }

        protected void lnkProject_OnClick(object sender, EventArgs e)
        {
            var lnkProject = sender as LinkButton;
            SelectedProjectNumber = lnkProject.Attributes["ProjectNumber"];

            var businessDevelopmentProj = ServiceCallers.Custom.Project(p => p.GetBusinessDevelopmentProject());
            string totalHours = string.Empty;

            if (businessDevelopmentProj.ProjectNumber.ToUpper() == SelectedProjectNumber.ToUpper())
            {
                HostingPage.AccountId = Convert.ToInt32(lnkProject.Attributes["AccountId"]);
                HostingPage.BusinessUnitIds = lnkProject.Attributes["GroupId"] + ",";
                ucGroupByBusinessDevelopment.Visible = true;
                ucProjectDetailReport.Visible = false;
                ucGroupByBusinessDevelopment.PopulateByBusinessDevelopment();
                totalHours = GetDoubleFormat(HostingPage.Total);
            }
            else
            {

                var list = ServiceCallers.Custom.Report(r => r.ProjectDetailReportByResource(SelectedProjectNumber, null,
                     HostingPage.StartDate, HostingPage.EndDate,
                    null)).ToList();

                totalHours = GetDoubleFormat(list.Sum(l => l.TotalHours));
                ucGroupByBusinessDevelopment.Visible = false;
                ucProjectDetailReport.Visible = true;
                ucProjectDetailReport.DataBindByResourceDetail(list);
            }

            ltrlProject.Text = "<b class=\"colorGray\">" +
                lnkProject.Attributes["ClientName"] + " > " + lnkProject.Attributes["GroupName"] + " > </b><b>" + lnkProject.Text + "</b>";
            ltrlProjectDetailTotalhours.Text = totalHours;

            mpeProjectDetailReport.Show();
        }

        protected string GetProjectName(string projectNumber, string name)
        {
            return projectNumber + " - " + name;
        }

        public void DataBindProject(ProjectLevelGroupedHours[] reportData, bool isPopulateFilters)
        {
            if (isPopulateFilters)
            {
                PopulateFilterPanels(reportData);
            }
            if (reportData.Length > 0 || cblClients.Items.Count > 1 || cblProjectStatus.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                repProject.Visible = true;
                repProject.DataSource = reportData;
                repProject.DataBind();
                cblClients.SaveSelectedIndexesInViewState();
                cblProjectStatus.SaveSelectedIndexesInViewState();
                ImgClientFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblClients.FilterPopupClientID,
                  cblClients.SelectedIndexes, cblClients.CheckBoxListObject.ClientID, cblClients.WaterMarkTextBoxBehaviorID);
                ImgProjectStatusFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblProjectStatus.FilterPopupClientID,
                  cblProjectStatus.SelectedIndexes, cblProjectStatus.CheckBoxListObject.ClientID, cblProjectStatus.WaterMarkTextBoxBehaviorID);

                //Populate header hover               
                LblProjectedHours.Attributes[OnMouseOver] = string.Format(ShowPanel, LblProjectedHours.ClientID, pnlTotalProjectedHours.ClientID, 0);
                LblProjectedHours.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalProjectedHours.ClientID);

                LblBillable.Attributes[OnMouseOver] = string.Format(ShowPanel, LblBillable.ClientID, pnlTotalBillableHours.ClientID, 0);
                LblBillable.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalBillableHours.ClientID);

                LblNonBillable.Attributes[OnMouseOver] = string.Format(ShowPanel, LblNonBillable.ClientID, pnlTotalNonBillableHours.ClientID, 0);
                LblNonBillable.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalNonBillableHours.ClientID);

                LblActualHours.Attributes[OnMouseOver] = string.Format(ShowPanel, LblActualHours.ClientID, pnlTotalActualHours.ClientID, 0);
                LblActualHours.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalActualHours.ClientID);

                LblBillableHoursVariance.Attributes[OnMouseOver] = string.Format(ShowPanel, LblBillableHoursVariance.ClientID, pnlBillableHoursVariance.ClientID, 0);
                LblBillableHoursVariance.Attributes[OnMouseOut] = string.Format(HidePanel, pnlBillableHoursVariance.ClientID);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repProject.Visible = false;
            }
            PopulateHeaderSection(reportData);
        }

        private void PopulateFilterPanels(ProjectLevelGroupedHours[] reportData)
        {
            PopulateClientFilter(reportData);
            PopulateProjectStatusFilter(reportData);
        }

        private void PopulateClientFilter(ProjectLevelGroupedHours[] reportData)
        {
            var clients = reportData.Select(r => new { Id = r.Project.Client.Id, Name = r.Project.Client.HtmlEncodedName }).Distinct().ToList().OrderBy(s => s.Name).ToArray();
            int height = 17 * clients.Length;
            Unit unitHeight = new Unit((height + 17) > 50 ? 50 : height + 17);
            DataHelper.FillListDefault(cblClients.CheckBoxListObject, "All Clients", clients, false, "Id", "Name");
            cblClients.Height = unitHeight;
            cblClients.SelectAllItems(true);
        }

        private void PopulateProjectStatusFilter(ProjectLevelGroupedHours[] reportData)
        {
            var projectStatusIds = reportData.Select(r => new { Id = r.Project.Status.Id, Name = r.Project.Status.Name }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblProjectStatus.CheckBoxListObject, "All Status", projectStatusIds.ToArray(), false, "Id", "Name");
            cblProjectStatus.SelectAllItems(true);
        }

        private void PopulateHeaderSection(ProjectLevelGroupedHours[] reportData)
        {
            double billableHours = reportData.Sum(p => p.BillableHours);
            double nonBillableHours = reportData.Sum(p => p.NonBillableHours);
            double totalProjectedHours = reportData.Sum(p => p.ForecastedHours);
            double totalActualHours = reportData.Sum(p => p.TotalHours);
            double totalBillableHoursVariance = reportData.Sum(p => p.BillableHoursVariance);

            int noOfEmployees = reportData.Length;
            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (billableHours != 0 || nonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(billableHours, nonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }
            ltProjectCount.Text = noOfEmployees + " Projects";
            lbRange.Text = HostingPage.Range;
            ltrlTotalHours.Text = lblTotalActualHours.Text = totalActualHours.ToString(Constants.Formatting.DoubleValue);
            ltrlAvgHours.Text = noOfEmployees > 0 ? ((billableHours + nonBillableHours) / noOfEmployees).ToString(Constants.Formatting.DoubleValue) : "0.00";
            ltrlBillableHours.Text = lblTotalBillableHours.Text = lblTotalBillablePanlActual.Text = billableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlNonBillableHours.Text = lblTotalNonBillableHours.Text = lblTotalNonBillablePanlActual.Text = nonBillableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlBillablePercent.Text = billablePercent.ToString();
            ltrlNonBillablePercent.Text = nonBillablePercent.ToString();
            lblTotalProjectedHours.Text = totalProjectedHours.ToString(Constants.Formatting.DoubleValue);
            lblTotalBillableHoursVariance.Text = totalBillableHoursVariance.ToString(Constants.Formatting.DoubleValue);
            if (totalBillableHoursVariance < 0)
            {
                lblExclamationMarkPanl.Visible = true;
            }

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


        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            PopulateByProjectData(false);
        }


    }
}

