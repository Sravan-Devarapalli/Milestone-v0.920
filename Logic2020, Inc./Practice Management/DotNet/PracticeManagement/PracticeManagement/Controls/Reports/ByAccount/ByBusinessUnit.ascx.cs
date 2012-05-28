using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DataTransferObjects.Reports.ByAccount;

namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class ByBusinessUnit : System.Web.UI.UserControl
    {
        #region Properties

        private HtmlImage ImgBusinessUnitFilter { get; set; }

        private PraticeManagement.Reporting.AccountSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.AccountSummaryReport)Page); }
        }

        private String BusinessUnitIds
        {
            get
            {
                if (cblBusinessUnits.SelectedItems == null)
                {
                    return HostingPage.BusinessUnitIds;
                }

                return cblBusinessUnits.SelectedItems;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            cblBusinessUnits.OKButtonId = btnFilterOK.ClientID;
        }

        protected void repBusinessUnit_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgBusinessUnitFilter = e.Item.FindControl("imgBusinessUnitFilter") as HtmlImage;
            }
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            PopulateByBusinessUnitReport(true);
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        public void PopulateByBusinessUnitReport(bool isPopulateFilters = true)
        {
            BusinessUnitLevelGroupedHours[] data = null;
            if (isPopulateFilters)
            {
                data = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.AccountId, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value));
            }
            else
            {
                data = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.AccountId, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value));
            }

            DataBindBusinesUnit(data, isPopulateFilters);
        }

        public void DataBindBusinesUnit(BusinessUnitLevelGroupedHours[] reportData, bool isPopulateFilters)
        {
            var reportDataList = reportData.ToList();
            if (isPopulateFilters)
            {
                PopulateFilterPanels(reportDataList);
            }
            if (reportDataList.Count > 0 || cblBusinessUnits.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                repBusinessUnit.Visible = true;
                repBusinessUnit.DataSource = reportDataList;
                repBusinessUnit.DataBind();
                cblBusinessUnits.SaveSelectedIndexesInViewState();
                ImgBusinessUnitFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblBusinessUnits.FilterPopupClientID,
                  cblBusinessUnits.SelectedIndexes, cblBusinessUnits.CheckBoxListObject.ClientID, cblBusinessUnits.WaterMarkTextBoxBehaviorID);

            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repBusinessUnit.Visible = false;
            }

            //PopulateHeaderSection(reportDataList);
        }

        private void PopulateFilterPanels(List<BusinessUnitLevelGroupedHours> reportData)
        {
            PopulateBusinessUnitFilter(reportData);
        }

        private void PopulateBusinessUnitFilter(List<BusinessUnitLevelGroupedHours> reportData)
        {
            var businessUnitList = reportData.Select(r => new { Name = r.BusinessUnit.Name }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblBusinessUnits.CheckBoxListObject, "All Business Units", businessUnitList.ToArray(), false, "Name", "Name");
            cblBusinessUnits.SelectAllItems(true);
        }
    }
}
