using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DataTransferObjects;

namespace PraticeManagement.Controls.Reports.HumanCapital
{
    public partial class NewHireReportSummaryView : System.Web.UI.UserControl
    {

        #region Properties

        private HtmlImage ImgTitleFilter { get; set; }

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
            cblRecruiter.OKButtonId = cblHireDate.OKButtonId = cblTitles.OKButtonId = cblPayTypes.OKButtonId = cblPersonStatusType.OKButtonId = cblDivision.OKButtonId = btnFilterOK.ClientID;
        }

        #endregion

        #region ControlEvents

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            var btn = sender as Button;
            bool isTitle;
            Boolean.TryParse(btn.Attributes["IsTitle"], out isTitle);
            string filterValue = btn.Attributes["FilterValue"];
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var data = ServiceCallers.Custom.Report(r => r.NewHireReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.PersonStatus, HostingPage.PayTypes, HostingPage.Practices, HostingPage.ExcludeInternalProjects, null, null, null, null)).ToList();
                if (!string.IsNullOrEmpty(filterValue))
                {
                    filterValue = filterValue.Trim();
                    bool isUnassigned = filterValue.Equals(Constants.FilterKeys.Unassigned);
                    if (isTitle)
                    {
                        data = data.Where(p => p.Title != null ? p.Title.TitleName == filterValue : isUnassigned).ToList();
                    }
                    else
                    {
                        data = data.Where(p => p.RecruiterId.HasValue ? p.RecruiterLastFirstName == filterValue : isUnassigned).ToList();
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
                ImgTitleFilter = e.Item.FindControl("imgTitleFilter") as HtmlImage;
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
                    data = ServiceCallers.Custom.Report(r => r.NewHireReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, PersonStatus, PayTypeIds, HostingPage.Practices, HostingPage.ExcludeInternalProjects, cblDivision.SelectedItemsXmlFormat, cblTitles.SelectedItemsXmlFormat, cblHireDate.SelectedItemsXmlFormat, cblRecruiter.SelectedItemsXmlFormat)).ToList();
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
            ImgTitleFilter.Visible =
            ImgPayTypeFilter.Visible =
            ImgHiredateFilter.Visible =
            ImgDivisionFilter.Visible =
            ImgPersonStatusTypeFilter.Visible =
            ImgRecruiterFilter.Visible = false;
        }

        public void DataBindResource(List<Person> reportData, bool isPopUp)
        {
            var reportDataList = reportData.ToList();
            if (reportDataList.Count > 0 || cblTitles.Items.Count > 1 || cblPayTypes.Items.Count > 1 || cblHireDate.Items.Count > 1 || cblDivision.Items.Count > 1 || cblPersonStatusType.Items.Count > 1 || cblRecruiter.Items.Count > 1)
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
            cblTitles.SaveSelectedIndexesInViewState();
            cblPayTypes.SaveSelectedIndexesInViewState();
            cblRecruiter.SaveSelectedIndexesInViewState();
            cblDivision.SaveSelectedIndexesInViewState();
            cblHireDate.SaveSelectedIndexesInViewState();
            cblPersonStatusType.SaveSelectedIndexesInViewState();

            ImgTitleFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblTitles.FilterPopupClientID,
              cblTitles.SelectedIndexes, cblTitles.CheckBoxListObject.ClientID, cblTitles.WaterMarkTextBoxBehaviorID);

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
            PopulateTitleFilter(reportData);
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

        private void PopulateTitleFilter(List<Person> reportData)
        {
            var titles = reportData.Select(r => new { TitleId = r.Title != null ? r.Title.TitleId : 0, TitleName = r.Title != null ? r.Title.HtmlEncodedTitleName : Constants.FilterKeys.Unassigned }).Distinct().ToList().OrderBy(s => s.TitleName);
            DataHelper.FillListDefault(cblTitles.CheckBoxListObject, "All Titles", titles.ToArray(), false, "TitleId", "TitleName");
            cblTitles.SelectAllItems(true);
        }

        private void PopulatePayTypeFilter(List<Person> reportData)
        {
            var payTypes = reportData.Select(r => new { Text = r.CurrentPay == null || string.IsNullOrEmpty(r.CurrentPay.TimescaleName) ? Constants.FilterKeys.Unassigned : r.CurrentPay.TimescaleName, Value = r.CurrentPay == null || string.IsNullOrEmpty(r.CurrentPay.TimescaleName) ? 0 : (int)r.CurrentPay.Timescale }).Distinct().ToList().OrderBy(t => t.Text);
            DataHelper.FillListDefault(cblPayTypes.CheckBoxListObject, "All Pay Types", payTypes.ToArray(), false, "Value", "Text");
            cblPayTypes.SelectAllItems(true);
        }

        private void PopulateRecruiterFilter(List<Person> reportData)
        {
            var recruiters = reportData.Select(r => new { Text = r.RecruiterId.HasValue ? r.RecruiterLastFirstName : Constants.FilterKeys.Unassigned, Value = r.RecruiterId.HasValue ? r.RecruiterId.Value : 0 }).Distinct().ToList().OrderBy(t => t.Text);
            DataHelper.FillListDefault(cblRecruiter.CheckBoxListObject, "All Recruiter(s)", recruiters.ToArray(), false, "Value", "Text");
            cblRecruiter.SelectAllItems(true);
        }

        #endregion

    }
}

