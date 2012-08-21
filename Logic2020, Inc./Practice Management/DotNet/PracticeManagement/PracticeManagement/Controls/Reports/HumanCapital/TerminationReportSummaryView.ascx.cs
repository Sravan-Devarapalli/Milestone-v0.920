using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DataTransferObjects;
using System.Text;
using DataTransferObjects.Reports.HumanCapital;

namespace PraticeManagement.Controls.Reports.HumanCapital
{
    public partial class TerminationReportSummaryView : System.Web.UI.UserControl
    {

        #region Properties

        private HtmlImage ImgSeniorityFilter { get; set; }

        private HtmlImage ImgPayTypeFilter { get; set; }

        private HtmlImage ImgHiredateFilter { get; set; }

        private HtmlImage ImgPersonStatusTypeFilter { get; set; }

        private HtmlImage ImgDivisionFilter { get; set; }

        private HtmlImage ImgRecruiterFilter { get; set; }

        private HtmlImage ImgTerminationDateFilter { get; set; }

        private HtmlImage ImgTerminationReasonFilter { get; set; }

        private PraticeManagement.Reporting.TerminationReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.TerminationReport)Page); }
        }

        private string PayTypes
        {
            get
            {
                return cblPayTypes.SelectedItemsXmlFormat != null ? cblPayTypes.SelectedItemsXmlFormat : HostingPage.PayTypes;
            }
        }

        private string Seniorities
        {
            get
            {
                return cblSeniorities.SelectedItemsXmlFormat != null ? cblSeniorities.SelectedItemsXmlFormat : HostingPage.Seniorities;
            }
        }

        private string TerminationReasons
        {
            get
            {
                return cblTerminationReason.SelectedItemsXmlFormat != null ? cblTerminationReason.SelectedItemsXmlFormat : HostingPage.TerminationReasons;
            }
        }



        #endregion

        #region PageEvents

        protected void Page_Load(object sender, EventArgs e)
        {
            cblRecruiter.OKButtonId = cblHireDate.OKButtonId = cblSeniorities.OKButtonId = cblPayTypes.OKButtonId = cblPersonStatusType.OKButtonId = cblDivision.OKButtonId = cblTerminationReason.OKButtonId = cblTerminationDate.OKButtonId = btnFilterOK.ClientID;
        }

        #endregion

        #region ControlEvents

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            HostingPage.ExportToExcel();
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
                ImgTerminationDateFilter = e.Item.FindControl("imgTerminationdateFilter") as HtmlImage;
                ImgTerminationReasonFilter = e.Item.FindControl("imgTerminationReasonFilter") as HtmlImage;
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
        public void PopulateData()
        {
            TerminationPersonsInRange data;
            if (HostingPage.SetSelectedFilters)
            {
                data = ServiceCallers.Custom.Report(r => r.TerminationReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.PayTypes, null, HostingPage.Seniorities, HostingPage.TerminationReasons, HostingPage.Practices, HostingPage.ExcludeInternalProjects, null, null, null, null));
                PopulateFilterPanels(data.PersonList);
            }
            else
            {
                data = ServiceCallers.Custom.Report(r => r.TerminationReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, PayTypes, cblPersonStatusType.SelectedItemsXmlFormat, Seniorities, TerminationReasons, HostingPage.Practices, HostingPage.ExcludeInternalProjects, cblDivision.SelectedItemsXmlFormat, cblRecruiter.SelectedItemsXmlFormat, cblHireDate.SelectedItemsXmlFormat, cblTerminationDate.SelectedItemsXmlFormat));
            }
            DataBindResource(data);
        }

        public void DataBindResource(TerminationPersonsInRange reportData)
        {
            var reportDataList = reportData.PersonList.ToList();
            if (reportDataList.Count > 0 || cblSeniorities.Items.Count > 1 || cblPayTypes.Items.Count > 1 || cblHireDate.Items.Count > 1 || cblDivision.Items.Count > 1 || cblPersonStatusType.Items.Count > 1 || cblRecruiter.Items.Count > 1 || cblTerminationDate.Items.Count > 1 || cblTerminationReason.Items.Count > 1)
            {
                divEmptyMessage.Attributes["class"] = "displayNone";
                btnExportToExcel.Enabled = 
                repResource.Visible = true;
                repResource.DataSource = reportDataList;
                repResource.DataBind();
                SetAttribitesForFiltersImages();
            }
            else
            {
                divEmptyMessage.Attributes["class"] = "EmptyMessagediv";
                btnExportToExcel.Enabled = 
                repResource.Visible = false;
            }
            HostingPage.PopulateHeaderSection(reportData);
        }

        private void SetAttribitesForFiltersImages()
        {
            cblSeniorities.SaveSelectedIndexesInViewState();
            cblPayTypes.SaveSelectedIndexesInViewState();
            cblRecruiter.SaveSelectedIndexesInViewState();
            cblDivision.SaveSelectedIndexesInViewState();
            cblHireDate.SaveSelectedIndexesInViewState();
            cblPersonStatusType.SaveSelectedIndexesInViewState();
            cblTerminationDate.SaveSelectedIndexesInViewState();
            cblTerminationReason.SaveSelectedIndexesInViewState();

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

            ImgRecruiterFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblRecruiter.FilterPopupClientID,
              cblRecruiter.SelectedIndexes, cblRecruiter.CheckBoxListObject.ClientID, cblRecruiter.WaterMarkTextBoxBehaviorID);

            ImgRecruiterFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblTerminationDate.FilterPopupClientID,
             cblTerminationDate.SelectedIndexes, cblTerminationDate.CheckBoxListObject.ClientID, cblTerminationDate.WaterMarkTextBoxBehaviorID);

            ImgRecruiterFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblTerminationReason.FilterPopupClientID,
             cblTerminationReason.SelectedIndexes, cblTerminationReason.CheckBoxListObject.ClientID, cblTerminationReason.WaterMarkTextBoxBehaviorID);
        }

        private void PopulateFilterPanels(List<Person> reportData)
        {
            PopulateDivisionFilter(reportData);
            PopulateSeniorityFilter(reportData);
            PopulateHireDateFilter(reportData);
            PopulatePayTypeFilter(reportData);
            PopulatPersonStatusTypeFilter(reportData);
            PopulateRecruiterFilter(reportData);
            PopulateTerminationDateFilter(reportData);
            PopulateTerminationReasonFilter(reportData);
        }

        private void PopulateTerminationDateFilter(List<Person> reportData)
        {
            var terminationDateList = reportData.Select(r => new { Text = r.TerminationDate.Value.ToString("MMM yyyy"), Value = r.TerminationDate.Value.ToString("MM/01/yyyy"), orderby = r.TerminationDate.Value.ToString("yyyy/MM") }).Distinct().ToList().OrderBy(s => s.orderby);
            DataHelper.FillListDefault(cblTerminationDate.CheckBoxListObject, "All Months ", terminationDateList.ToArray(), false, "Value", "Text");
            cblTerminationDate.SelectAllItems(true);
        }

        private void PopulateTerminationReasonFilter(List<Person> reportData)
        {
            var terminationReasons = reportData.Select(r => new { Id = r.TerminationReasonid, Name = r.TerminationReason }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblTerminationReason.CheckBoxListObject, "All Reasons", terminationReasons.ToArray(), false, "Id", "Name");
            cblTerminationReason.SelectAllItems(true);
        }

        private void PopulateDivisionFilter(List<Person> reportData)
        {
            var divisionList = reportData.Select(r => new { Value = (int)r.DivisionType == 0 ? string.Empty : ((int)r.DivisionType).ToString(), Name = (int)r.DivisionType == 0 ? Constants.FilterKeys.Unassigned : r.DivisionType.ToString() }).Distinct().ToList().OrderBy(d => d.Value).ToArray();
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
            var payTypes = reportData.Select(r => new { Text = r.RecruiterCommission.Count > 0 ? r.RecruiterCommission.First().Recruiter.PersonFirstLastName : Constants.FilterKeys.Unassigned, Value = r.RecruiterCommission.Count > 0 ? r.RecruiterCommission.First().Recruiter.Id : 0 }).Distinct().ToList().OrderBy(t => t.Text);
            DataHelper.FillListDefault(cblRecruiter.CheckBoxListObject, "All Recruiter(s)", payTypes.ToArray(), false, "Value", "Text");
            cblRecruiter.SelectAllItems(true);
        }

        protected string GetRecruiter(List<RecruiterCommission> recruiterCommission)
        {
            return recruiterCommission.Count > 0 ? recruiterCommission.First().Recruiter.PersonFirstLastName : string.Empty;
        }

        #endregion
    }
}
