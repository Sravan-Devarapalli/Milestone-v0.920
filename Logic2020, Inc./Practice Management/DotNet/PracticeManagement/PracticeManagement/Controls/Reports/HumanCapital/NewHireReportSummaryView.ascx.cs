using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DataTransferObjects;
using System.Text;

namespace PraticeManagement.Controls.Reports.HumanCapital
{
    public partial class NewHireReportSummaryView : System.Web.UI.UserControl
    {

        #region Properties

        private HtmlImage ImgSeniorityFilter { get; set; }

        private HtmlImage ImgPayTypeFilter { get; set; }

        private HtmlImage ImgHiredateFilter { get; set; }

        private HtmlImage ImgPersonStatusTypeFilter { get; set; }

        private HtmlImage ImgDivisionFilter { get; set; }

        private HtmlImage ImgRecruiterFilter { get; set; }

        private Label ImgRecruiterFilterHidden { get; set; }

        public Button BtnExportToExcelButton { get { return btnExportToExcel; } }

        private string PayTypeIds
        {
            get
            {
                return cblPayTypes.SelectedItemsXmlFormat != null ? cblPayTypes.SelectedItemsXmlFormat : HostingPage.PayTypes;
            }
        }

        private string PersonStatus
        {
            get
            {
                return cblPersonStatusType.SelectedItemsXmlFormat != null ? cblPersonStatusType.SelectedItemsXmlFormat : HostingPage.PersonStatus;
            }
        }



        private PraticeManagement.Reporting.NewHireReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.NewHireReport)Page); }
        }

        public List<Person> PopUpFilteredPerson { get; set; }

        #endregion

        #region PageEvents

        protected void Page_Load(object sender, EventArgs e)
        {
            cblRecruiter.OKButtonId = cblHireDate.OKButtonId = cblSeniorities.OKButtonId = cblPayTypes.OKButtonId = cblPersonStatusType.OKButtonId = cblDivision.OKButtonId = btnFilterOK.ClientID;
        }

        #endregion

        #region ControlEvents

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            var btn = sender as Button;
            bool isSeniority;
            Boolean.TryParse(btn.Attributes["IsSeniority"], out isSeniority);
            string filterValue = btn.Attributes["FilterValue"];
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var data = ServiceCallers.Custom.Report(r => r.NewHireReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.PersonStatus, HostingPage.PayTypes, HostingPage.Practices, HostingPage.ExcludeInternalProjects, null, null, null, null)).ToList();
                if (!string.IsNullOrEmpty(filterValue))
                {
                    filterValue = filterValue.Trim();
                    bool isUnassigned = filterValue.Equals(Constants.FilterKeys.Unassigned);
                    if (isSeniority)
                    {
                        data = data.Where(p => p.Seniority != null ? p.Seniority.SeniorityCategory.Name == filterValue : isUnassigned).ToList();
                    }
                    else
                    {
                        data = data.Where(p => p.RecruiterCommission.Any() ? p.RecruiterCommission.First().Recruiter.PersonLastFirstName == filterValue : isUnassigned).ToList();
                    }
                }
                HostingPage.ExportToExcel(data);
            }
        }

        protected void repResource_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgDivisionFilter = e.Item.FindControl("imgDivisionFilter") as HtmlImage;
                ImgSeniorityFilter = e.Item.FindControl("imgSeniorityFilter") as HtmlImage;
                ImgPayTypeFilter = e.Item.FindControl("imgPayTypeFilter") as HtmlImage;
                ImgHiredateFilter = e.Item.FindControl("imgHiredateFilter") as HtmlImage;
                ImgPersonStatusTypeFilter = e.Item.FindControl("imgPersonStatusTypeFilter") as HtmlImage;
                ImgRecruiterFilter = e.Item.FindControl("imgRecruiterFilter") as HtmlImage;
                ImgRecruiterFilterHidden = e.Item.FindControl("imgRecruiterFilterHidden") as Label;
            }
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            PopulateData();
        }

        #endregion

        #region Methods

        protected string GetDateFormat(DateTime date)
        {
            return date.ToString(Constants.Formatting.EntryDateFormat);
        }

        protected string GetRecruiter(List<RecruiterCommission> recruiterCommission)
        {
            return recruiterCommission.Count > 0 ? recruiterCommission.First().Recruiter.PersonLastFirstName : string.Empty;
        }

        public void PopulateData(bool isPopUp = false)
        {
            List<Person> data;
            if (!isPopUp)
            {
                if (HostingPage.SetSelectedFilters)
                {
                    data = ServiceCallers.Custom.Report(r => r.NewHireReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.PersonStatus, HostingPage.PayTypes, HostingPage.Practices, HostingPage.ExcludeInternalProjects, null, null, null, null)).ToList();
                    PopulateFilterPanels(data);
                }
                else
                {
                    data = ServiceCallers.Custom.Report(r => r.NewHireReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, PersonStatus, PayTypeIds, HostingPage.Practices, HostingPage.ExcludeInternalProjects, cblDivision.SelectedItemsXmlFormat, cblSeniorities.SelectedItemsXmlFormat, cblHireDate.SelectedItemsXmlFormat, cblRecruiter.SelectedItemsXmlFormat)).ToList();
                }
            }
            else
            {
                data = PopUpFilteredPerson;
            }
            DataBindResource(data, isPopUp);
        }

        private void RemoveFilters()
        {
            ImgSeniorityFilter.Visible =
            ImgPayTypeFilter.Visible =
            ImgHiredateFilter.Visible =
            ImgDivisionFilter.Visible =
            ImgPersonStatusTypeFilter.Visible =
            ImgRecruiterFilter.Visible = false;
        }

        public void DataBindResource(List<Person> reportData, bool isPopUp)
        {
            var reportDataList = reportData.ToList();
            if (reportDataList.Count > 0 || cblSeniorities.Items.Count > 1 || cblPayTypes.Items.Count > 1 || cblHireDate.Items.Count > 1 || cblDivision.Items.Count > 1 || cblPersonStatusType.Items.Count > 1 || cblRecruiter.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                btnExportToExcel.Enabled =
                repResource.Visible = true;
                repResource.DataSource = reportDataList;
                repResource.DataBind();
                if (!isPopUp)
                {
                    SetAttribitesForFiltersImages();
                }
                else
                {
                    RemoveFilters();
                }
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                btnExportToExcel.Enabled =
                repResource.Visible = false;
            }
            if (!isPopUp)
            {
                HostingPage.PopulateHeaderSection(reportDataList);
            }
        }

        private void SetAttribitesForFiltersImages()
        {
            cblSeniorities.SaveSelectedIndexesInViewState();
            cblPayTypes.SaveSelectedIndexesInViewState();
            cblRecruiter.SaveSelectedIndexesInViewState();
            cblDivision.SaveSelectedIndexesInViewState();
            cblHireDate.SaveSelectedIndexesInViewState();
            cblPersonStatusType.SaveSelectedIndexesInViewState();

            ImgSeniorityFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblSeniorities.FilterPopupClientID,
              cblSeniorities.SelectedIndexes, cblSeniorities.CheckBoxListObject.ClientID, cblSeniorities.WaterMarkTextBoxBehaviorID);

            ImgPayTypeFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblPayTypes.FilterPopupClientID,
               cblPayTypes.SelectedIndexes, cblPayTypes.CheckBoxListObject.ClientID, cblPayTypes.WaterMarkTextBoxBehaviorID);

            ImgHiredateFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblHireDate.FilterPopupClientID,
               cblHireDate.SelectedIndexes, cblHireDate.CheckBoxListObject.ClientID, cblHireDate.WaterMarkTextBoxBehaviorID);

            ImgDivisionFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblDivision.FilterPopupClientID,
               cblDivision.SelectedIndexes, cblDivision.CheckBoxListObject.ClientID, cblDivision.WaterMarkTextBoxBehaviorID);

            ImgPersonStatusTypeFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblPersonStatusType.FilterPopupClientID,
               cblPersonStatusType.SelectedIndexes, cblPersonStatusType.CheckBoxListObject.ClientID, cblPersonStatusType.WaterMarkTextBoxBehaviorID);

            ImgRecruiterFilter.Attributes["onclick"] = string.Format("ClickHiddenImg(\'{4}\');Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblRecruiter.FilterPopupClientID,
              cblRecruiter.SelectedIndexes, cblRecruiter.CheckBoxListObject.ClientID, cblRecruiter.WaterMarkTextBoxBehaviorID, ImgRecruiterFilterHidden.ClientID);
        }

        private void PopulateFilterPanels(List<Person> reportData)
        {
            PopulateDivisionFilter(reportData);
            PopulateSeniorityFilter(reportData);
            PopulateHireDateFilter(reportData);
            PopulatePayTypeFilter(reportData);
            PopulatPersonStatusTypeFilter(reportData);
            PopulateRecruiterFilter(reportData);
        }

        private void PopulateDivisionFilter(List<Person> reportData)
        {
            var divisionList = reportData.Select(r => new { Value = ((int)r.DivisionType).ToString(), Name = (int)r.DivisionType == 0 ? Constants.FilterKeys.Unassigned : r.DivisionType.ToString() }).Distinct().ToList().OrderBy(d => d.Value).ToArray();
            DataHelper.FillListDefault(cblDivision.CheckBoxListObject, "All Division Types", divisionList, false, "Value", "Name");
            cblDivision.SelectAllItems(true);
        }

        private void PopulateHireDateFilter(List<Person> reportData)
        {
            var hireDateList = reportData.Select(r => new { Text = r.HireDate.ToString("MMM yyyy"), Value = r.HireDate.ToString("MM/01/yyyy"), orderby = r.HireDate.ToString("yyyy/MM") }).Distinct().ToList().OrderBy(s => s.orderby);
            DataHelper.FillListDefault(cblHireDate.CheckBoxListObject, "All Months ", hireDateList.ToArray(), false, "Value", "Text");
            cblHireDate.SelectAllItems(true);
        }

        private void PopulatPersonStatusTypeFilter(List<Person> reportData)
        {
            var personStatusTypeList = reportData.Select(r => new { Name = r.Status.Name, Id = r.Status.Id }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblPersonStatusType.CheckBoxListObject, "All Person Status Types", personStatusTypeList.ToArray(), false, "Id", "Name");
            cblPersonStatusType.SelectAllItems(true);
        }

        private void PopulateSeniorityFilter(List<Person> reportData)
        {
            var seniorities = reportData.Select(r => new { Id = r.Seniority != null ? r.Seniority.Id : 0, Name = r.Seniority != null ? r.Seniority.Name : Constants.FilterKeys.Unassigned }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblSeniorities.CheckBoxListObject, "All Seniorities", seniorities.ToArray(), false, "Id", "Name");
            cblSeniorities.SelectAllItems(true);
        }

        private void PopulatePayTypeFilter(List<Person> reportData)
        {
            var payTypes = reportData.Select(r => new { Text = r.CurrentPay == null || string.IsNullOrEmpty(r.CurrentPay.TimescaleName) ? Constants.FilterKeys.Unassigned : r.CurrentPay.TimescaleName, Value = r.CurrentPay == null || string.IsNullOrEmpty(r.CurrentPay.TimescaleName) ? 0 : (int)r.CurrentPay.Timescale }).Distinct().ToList().OrderBy(t => t.Text);
            DataHelper.FillListDefault(cblPayTypes.CheckBoxListObject, "All Pay Types", payTypes.ToArray(), false, "Value", "Text");
            cblPayTypes.SelectAllItems(true);
        }

        private void PopulateRecruiterFilter(List<Person> reportData)
        {
            var recruiters = reportData.Select(r => new { Text = r.RecruiterCommission.Count > 0 ? r.RecruiterCommission.First().Recruiter.PersonLastFirstName : Constants.FilterKeys.Unassigned, Value = r.RecruiterCommission.Count > 0 ? r.RecruiterCommission.First().Recruiter.Id : 0 }).Distinct().ToList().OrderBy(t => t.Text);
            DataHelper.FillListDefault(cblRecruiter.CheckBoxListObject, "All Recruiter(s)", recruiters.ToArray(), false, "Value", "Text");
            cblRecruiter.SelectAllItems(true);
        }

        #endregion

    }
}

