using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Web.Security;
using PraticeManagement.Controls.Reports.ByPerson;
using AjaxControlToolkit;

namespace PraticeManagement.Controls.Reports
{
    public partial class TimePeriodSummaryByResource : System.Web.UI.UserControl
    {
        private string TimePeriodSummaryReportExport = "TimePeriod Summary Report By Resource";

        private string TimePeriodSummaryReportPayCheckExport = "TimePeriod Summary Report By Resource(Pay Chex)";

        private string ShowPanel = "ShowPanel('{0}', '{1}');";
        private string HidePanel = "HidePanel('{0}');";
        private string OnMouseOver = "onmouseover";
        private string OnMouseOut = "onmouseout";      

        public ModalPopupExtender PersonDetailPopup
        {
            get
            {
                return mpePersonDetailReport;

            }
        }

        public int SelectedPersonForDetails
        {
            get
            {
                return (int)ViewState["SelectedPersonForDetail"];
            }
            set
            {
                ViewState["SelectedPersonForDetail"] = value;
            }
        }

        private HtmlImage ImgSeniorityFilter { get; set; }

        private HtmlImage ImgPayTypeFilter { get; set; }

        private HtmlImage ImgOffshoreFilter { get; set; }

        private HtmlImage ImgPersonStatusTypeFilter { get; set; }

        private HtmlImage ImgDivisionFilter { get; set; }

        private PraticeManagement.Reporting.TimePeriodSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.TimePeriodSummaryReport)Page); }
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetPayTypeSortValue(string payType, string name)
        {
            if (string.IsNullOrEmpty(payType))
            {
                return "-1" + name;
            }
            return payType + name;
        }

        protected bool IsPersonTerminated(int statusId)
        {
            return statusId == 2;//Terminated
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(TimePeriodSummaryReportExport);

            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {

                var data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByResource(HostingPage.StartDate.Value,
                    HostingPage.EndDate.Value, HostingPage.IncludePersonWithNoTimeEntries, cblOffShore.SelectedItemsXmlFormat,
                    cblSeniorities.SelectedItems,
                    cblPayTypes.SelectedItemsXmlFormat, cblPersonStatusType.SelectedItems, cblDivision.SelectedItemsXmlFormat)).ToList();
                
                string filterApplied = "Filters applied to columns: ";
                List<string> filteredColoums = new List<string>();
                if (!cblOffShore.AllItemsSelected || !cblPersonStatusType.AllItemsSelected || !cblDivision.AllItemsSelected)
                {
                    filteredColoums.Add("Resource");
                }
                if (!cblSeniorities.AllItemsSelected)
                {
                    filteredColoums.Add("Seniority");
                }
                if (!cblPayTypes.AllItemsSelected)
                {
                    filteredColoums.Add("Pay Type");
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("TimePeriod_ByResource Report");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(data.Count + " Employees");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(HostingPage.Range);
                sb.Append("\t");
                if (filteredColoums.Count > 0)
                {
                    sb.AppendLine();
                    for (int i = 0; i < filteredColoums.Count; i++)
                    {
                        if (i == filteredColoums.Count-1)
                            filterApplied = filterApplied + filteredColoums[i] + ".";
                        else
                            filterApplied = filterApplied + filteredColoums[i] + ",";
                    }
                    sb.Append(filterApplied);
                    sb.Append("\t");
                }
                sb.AppendLine();
                sb.AppendLine();

                if (data.Count > 0)
                {
                    //Header
                    sb.Append("Resource");
                    sb.Append("\t");
                    sb.Append("Seniority");
                    sb.Append("\t");
                    sb.Append("Pay Types");
                    sb.Append("\t");
                    sb.Append("IsOffshore");
                    sb.Append("\t");
                    sb.Append("Billable");
                    sb.Append("\t");
                    sb.Append("Non-Billable");
                    sb.Append("\t");
                    sb.Append("BD");
                    sb.Append("\t");
                    sb.Append("Internal");
                    sb.Append("\t");
                    sb.Append("Time-Off");
                    sb.Append("\t");
                    sb.Append("Total");
                    sb.Append("\t");
                    sb.Append("Utilization Percent this Period");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var personLevelGroupedHours in data)
                    {
                        sb.Append(personLevelGroupedHours.Person.HtmlEncodedName);
                        sb.Append("\t");
                        sb.Append(personLevelGroupedHours.Person.Seniority.Name);
                        sb.Append("\t");
                        sb.Append(personLevelGroupedHours.Person.CurrentPay.TimescaleName);
                        sb.Append("\t");
                        sb.Append(personLevelGroupedHours.Person.IsOffshore ? "Yes" : "No");
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.BillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.ProjectNonBillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.BusinessDevelopmentHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.InternalHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.AdminstrativeHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.TotalHours));
                        sb.Append("\t");
                        sb.Append(personLevelGroupedHours.Person.UtlizationPercent + "%");
                        sb.Append("\t");
                        sb.AppendLine();
                    }

                }
                else
                {
                    sb.Append("There are no Time Entries by any Employee  for the selected range.");
                }
                //“TimePeriod_ByResource_[StartOfRange]_[EndOfRange].xls”.  
                var filename = string.Format("{0}_{1}-{2}.xls", "TimePeriod_ByResource", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        protected void btnPayCheckExport_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(TimePeriodSummaryReportPayCheckExport);

            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                List<PersonLevelPayCheck> personLevelPayCheckList = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryByResourcePayCheck(HostingPage.StartDate.Value, HostingPage.EndDate.Value,
                    HostingPage.IncludePersonWithNoTimeEntries, cblOffShore.SelectedItemsXmlFormat, cblSeniorities.SelectedItems, cblPayTypes.SelectedItemsXmlFormat, cblPersonStatusType.SelectedItems, cblDivision.SelectedItemsXmlFormat)).ToList();

                string filterApplied = "Filters applied to columns: ";
                List<string> filteredColoums = new List<string>();
                if (!cblOffShore.AllItemsSelected || !cblPersonStatusType.AllItemsSelected || !cblDivision.AllItemsSelected)
                {
                    filteredColoums.Add("Resource");
                }
                if (!cblSeniorities.AllItemsSelected)
                {
                    filteredColoums.Add("Seniority");
                }
                if (!cblPayTypes.AllItemsSelected)
                {
                    filteredColoums.Add("Pay Type");
                }

                StringBuilder sb = new StringBuilder();
                sb.Append(" Paychex ");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(personLevelPayCheckList.Count.ToString() + " Employees");
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
                bool isUserAdminstrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);

                if (personLevelPayCheckList.Count > 0)
                {
                    Dictionary<string, double> workTypeLevelTimeOffHours = personLevelPayCheckList[0].WorkTypeLevelTimeOffHours;

                    //Header
                    sb.Append("BranchID");
                    sb.Append("\t");
                    sb.Append("DeptID");
                    sb.Append("\t");
                    sb.Append("EmployeeID");
                    sb.Append("\t");
                    if (isUserAdminstrator)
                    {
                        sb.Append("PaychexID");
                        sb.Append("\t");
                    }
                    sb.Append("Last Name");
                    sb.Append("\t");
                    sb.Append("First Name");
                    sb.Append("\t");
                    sb.Append("Pay Types");
                    sb.Append("\t");
                    sb.Append("Hours");
                    sb.Append("\t");
                    foreach (string worktype in workTypeLevelTimeOffHours.Keys)
                    {
                        sb.Append(worktype);
                        sb.Append("\t");
                    }
                    sb.Append("Total");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var personLevelPayCheck in personLevelPayCheckList)
                    {
                        sb.Append(personLevelPayCheck.BranchID);
                        sb.Append("\t");
                        sb.Append(personLevelPayCheck.DeptID);
                        sb.Append("\t");
                        sb.Append(personLevelPayCheck.Person.EmployeeNumber);
                        sb.Append("\t");
                        if (isUserAdminstrator)
                        {
                            sb.Append(personLevelPayCheck.Person.PaychexID);
                            sb.Append("\t");
                        }
                        sb.Append(personLevelPayCheck.Person.LastName);
                        sb.Append("\t");
                        sb.Append(personLevelPayCheck.Person.FirstName);
                        sb.Append("\t");
                        sb.Append(personLevelPayCheck.Person.CurrentPay.TimescaleName);
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelPayCheck.TotalHoursExcludingTimeOff));
                        sb.Append("\t");
                        foreach (string worktype in workTypeLevelTimeOffHours.Keys)
                        {
                            sb.Append(personLevelPayCheck.WorkTypeLevelTimeOffHours[worktype]);
                            sb.Append("\t");
                        }
                        sb.Append(GetDoubleFormat(personLevelPayCheck.TotalHoursIncludingTimeOff));
                        sb.Append("\t");
                        sb.AppendLine();
                    }

                }
                else
                {
                    sb.Append("There are no Time Entries by any Employee  for the selected range.");
                }
                //“Paychex_[StartOfRange]_[EndOfRange].xls”.  
                var filename = string.Format("{0}_{1}-{2}.xls", "Paychex", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            PopulateByResourceData(false);
        }

        protected void lnkPerson_OnClick(object sender, EventArgs e) 
        {
            var lnkPerson = sender as LinkButton;
            var personId = Convert.ToInt32(lnkPerson.Attributes["PersonId"]);
            SelectedPersonForDetails = personId;
            var list = ServiceCallers.Custom.Report(r => r.PersonTimeEntriesDetails(personId, HostingPage.StartDate.Value, HostingPage.EndDate.Value)).ToList();
            ucPersonDetailReport.DatabindRepepeaterPersonDetails(list,lnkPerson.Text);

            mpePersonDetailReport.Show();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            cblOffShore.OKButtonId = cblSeniorities.OKButtonId = cblPayTypes.OKButtonId = cblPersonStatusType.OKButtonId = cblDivision.OKButtonId = btnFilterOK.ClientID;
        }

        protected void repResource_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgSeniorityFilter = e.Item.FindControl("imgSeniorityFilter") as HtmlImage;
                ImgPayTypeFilter = e.Item.FindControl("imgPayTypeFilter") as HtmlImage;
                ImgOffshoreFilter = e.Item.FindControl("imgOffShoreFilter") as HtmlImage;
                ImgPersonStatusTypeFilter = e.Item.FindControl("imgPersonStatusTypeFilter") as HtmlImage;
                ImgDivisionFilter = e.Item.FindControl("imgDivisionFilter") as HtmlImage;

               
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lnkPerson = e.Item.FindControl("lnkPerson") as LinkButton;
                var imgZoomIn = e.Item.FindControl("imgZoomIn") as HtmlImage;

                lnkPerson.Attributes["onmouseover"] = string.Format("document.getElementById(\'{0}\').style.visibility='visible';", imgZoomIn.ClientID);
                lnkPerson.Attributes["onmouseout"] = string.Format("document.getElementById(\'{0}\').style.visibility='hidden';", imgZoomIn.ClientID);
            }
        }

        public void PopulateByResourceData(bool isPopulateFilters = true)
        {
            PersonLevelGroupedHours[] data;
            if (isPopulateFilters)
            {
                data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByResource(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.IncludePersonWithNoTimeEntries, null, null, null, null, null));
            }
            else
            {
                data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByResource(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.IncludePersonWithNoTimeEntries, cblOffShore.SelectedItemsXmlFormat, cblSeniorities.SelectedItems, cblPayTypes.SelectedItemsXmlFormat, cblPersonStatusType.SelectedItems, cblDivision.SelectedItemsXmlFormat));
            }

            DataBindResource(data, isPopulateFilters);
        }

        public void DataBindResource(PersonLevelGroupedHours[] reportData, bool isPopulateFilters)
        {
            var reportDataList = reportData.ToList();
            if (isPopulateFilters)
            {
                PopulateFilterPanels(reportDataList);
            }
            if (reportDataList.Count > 0 || cblSeniorities.Items.Count > 1 || cblPayTypes.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                repResource.Visible = true;
                repResource.DataSource = reportDataList;
                repResource.DataBind();
                cblSeniorities.SaveSelectedIndexesInViewState();
                cblPayTypes.SaveSelectedIndexesInViewState();
                cblOffShore.SaveSelectedIndexesInViewState();
                cblDivision.SaveSelectedIndexesInViewState();
                cblPersonStatusType.SaveSelectedIndexesInViewState();
                ImgSeniorityFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblSeniorities.FilterPopupClientID,
                  cblSeniorities.SelectedIndexes, cblSeniorities.CheckBoxListObject.ClientID, cblSeniorities.WaterMarkTextBoxBehaviorID);

                ImgPayTypeFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblPayTypes.FilterPopupClientID,
                   cblPayTypes.SelectedIndexes, cblPayTypes.CheckBoxListObject.ClientID, cblPayTypes.WaterMarkTextBoxBehaviorID);

                ImgOffshoreFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblOffShore.FilterPopupClientID,
                   cblOffShore.SelectedIndexes, cblOffShore.CheckBoxListObject.ClientID, cblOffShore.WaterMarkTextBoxBehaviorID);

                ImgDivisionFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblDivision.FilterPopupClientID,
                   cblDivision.SelectedIndexes, cblDivision.CheckBoxListObject.ClientID, cblDivision.WaterMarkTextBoxBehaviorID);

                ImgPersonStatusTypeFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblPersonStatusType.FilterPopupClientID,
                   cblPersonStatusType.SelectedIndexes, cblPersonStatusType.CheckBoxListObject.ClientID, cblPersonStatusType.WaterMarkTextBoxBehaviorID);

                PopulateSumLabels(reportDataList);

            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repResource.Visible = false;
            }

            PopulateHeaderSection(reportDataList);
        }

        private void PopulateSumLabels(List<PersonLevelGroupedHours> reportData)
        {            
            lblBillable.Text =
            pthLblBillable.Text = reportData.Sum(p => p.BillableHours).ToString(Constants.Formatting.DoubleValue); 
          
            lblNonBillable.Text =
            pthLblNonBillable.Text = reportData.Sum(p => p.ProjectNonBillableHours).ToString(Constants.Formatting.DoubleValue);

            lblBD.Text = 
            pthLblBD.Text = reportData.Sum(p => p.BusinessDevelopmentHours).ToString(Constants.Formatting.DoubleValue);

            lblInternal.Text = 
            pthLblInternal.Text = reportData.Sum(p => p.InternalHours).ToString(Constants.Formatting.DoubleValue);

            lblTimeOff.Text = 
            pthLblTimeOff.Text = reportData.Sum(p => p.AdminstrativeHours).ToString(Constants.Formatting.DoubleValue);

            pthLblGrandTotal.Text = reportData.Sum(p => p.TotalHours).ToString(Constants.Formatting.DoubleValue);

        }

        private void PopulateFilterPanels(List<PersonLevelGroupedHours> reportData)
        {
            PopulateOffshoreFilter(reportData);
            PopulateSeniorityFilter(reportData);
            PopulatePayTypeFilter(reportData);
            PopulatPersonStatusTypeFilter(reportData);
            PopulateDivisionFilter(reportData);
        }

        private void PopulateOffshoreFilter(List<PersonLevelGroupedHours> reportData)
        {
            var offshoreList = reportData.Select(r => new { Name = r.Person.OffshoreText }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblOffShore.CheckBoxListObject, "All Resource Types", offshoreList.ToArray(), false, "Name", "Name");
            cblOffShore.SelectAllItems(true);
        }

        private void PopulateDivisionFilter(List<PersonLevelGroupedHours> reportData)
        {
            var divisionList = reportData.Select(r => new { Value = (int)r.Person.DivisionType == 0 ? string.Empty : ((int)r.Person.DivisionType).ToString(), Name = (int)r.Person.DivisionType == 0 ? "Unassigned" : r.Person.DivisionType.ToString() }).Distinct().ToList().OrderBy(d => d.Value).ToArray();
            DataHelper.FillListDefault(cblDivision.CheckBoxListObject, "All Division Types", divisionList, false, "Value", "Name");
            cblDivision.SelectAllItems(true);
        }

        private void PopulatPersonStatusTypeFilter(List<PersonLevelGroupedHours> reportData)
        {
            var personStatusTypeList = reportData.Select(r => new { Name = r.Person.Status.Name, Id = r.Person.Status.Id }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblPersonStatusType.CheckBoxListObject, "All Person Status Types", personStatusTypeList.ToArray(), false, "Id", "Name");
            cblPersonStatusType.SelectAllItems(true);
        }

        private void PopulateSeniorityFilter(List<PersonLevelGroupedHours> reportData)
        {
            var seniorities = reportData.Select(r => new { Id = r.Person.Seniority.Id, Name = r.Person.Seniority.Name }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblSeniorities.CheckBoxListObject, "All Seniorities", seniorities.ToArray(), false, "Id", "Name");
            cblSeniorities.SelectAllItems(true);
        }

        private void PopulatePayTypeFilter(List<PersonLevelGroupedHours> reportData)
        {
            var payTypes = reportData.Select(r => new { Text = string.IsNullOrEmpty(r.Person.CurrentPay.TimescaleName) ? "Unassigned" : r.Person.CurrentPay.TimescaleName, Value = r.Person.CurrentPay.TimescaleName }).Distinct().ToList().OrderBy(t => t.Value);
            DataHelper.FillListDefault(cblPayTypes.CheckBoxListObject, "All Pay Types", payTypes.ToArray(), false, "Value", "Text");
            cblPayTypes.SelectAllItems(true);
        }

        private void PopulateHeaderSection(List<PersonLevelGroupedHours> reportData)
        {
            double billableHours = reportData.Sum(p => p.BillableHours);
            double nonBillableHours = reportData.Sum(p => p.NonBillableHours);
            double totalTimeOffHours = reportData.Sum(p => p.AdminstrativeHours);
            double pTOHours = reportData.Sum(p => p.PTOHours);
            double bereavementHours = reportData.Sum(p => p.BereavementHours);
            double juryDutyHours = reportData.Sum(p => p.JuryDutyHours);
            double oRTHours = reportData.Sum(p => p.ORTHours);
            double unpaidHours = reportData.Sum(p => p.UnpaidHours);
            double holidayHours = reportData.Sum(p => p.HolidayHours);

            int noOfEmployees = reportData.Count;
            double totalUtlization = reportData.Sum(p => p.Person.UtlizationPercent);
            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (billableHours != 0 || nonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(billableHours, nonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }
            ltPersonCount.Text = noOfEmployees + " Employees";
            lbRange.Text = HostingPage.Range;
            ltrlTotalHours.Text = (billableHours + nonBillableHours).ToString(Constants.Formatting.DoubleValue);
            ltrlTotalTimeoffHours.Text = totalTimeOffHours.ToString(Constants.Formatting.DoubleValue);
            ltrlAvgHours.Text = noOfEmployees > 0 ? ((billableHours + nonBillableHours) / noOfEmployees).ToString(Constants.Formatting.DoubleValue) : "0.00";
            ltrlAvgUtilization.Text = noOfEmployees > 0 ? Math.Round((totalUtlization / noOfEmployees), 0).ToString() + "%" : "0%";
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

            lblPtoHours.Text = pTOHours.ToString(Constants.Formatting.DoubleValue);
            lblBereavementHours.Text = bereavementHours.ToString(Constants.Formatting.DoubleValue);
            lblJuryDutyHours.Text = juryDutyHours.ToString(Constants.Formatting.DoubleValue);
            lblOrtHours.Text = oRTHours.ToString(Constants.Formatting.DoubleValue);
            lblUnpaidHours.Text = unpaidHours.ToString(Constants.Formatting.DoubleValue);
            lblHolidayHours.Text = holidayHours.ToString(Constants.Formatting.DoubleValue);

            ltrlTotalTimeoffHours.Attributes[OnMouseOver] = string.Format(ShowPanel, ltrlTotalTimeoffHours.ClientID, pnlTotalTimeOff.ClientID);
            ltrlTotalTimeoffHours.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalTimeOff.ClientID);
        }

    }
}

