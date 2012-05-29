using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports.ByAccount;
using DataTransferObjects.Reports;
using AjaxControlToolkit;
using System.Web.Script.Serialization;


namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class GroupByBusinessUnit : System.Web.UI.UserControl
    {

        private PraticeManagement.Reporting.AccountSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.AccountSummaryReport)Page); }
        }


        private List<string> CollapsiblePanelExtenderClientIds
        {
            get;
            set;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void PopulateData(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate)
        {
            List<BusinessUnitLevelGroupedHours> data = ServiceCallers.Custom.Report(r => r.AccountReportGroupByBusinessUnit(accountId, businessUnitIds, startDate, endDate)).ToList();
            DatabindbyBusinessUnitDetails(data);

            SetHeaderSectionValues(data);
        }

        private void SetHeaderSectionValues(List<BusinessUnitLevelGroupedHours> reportData)
        {
            HostingPage.UpdateHeaderSection = true;
            HostingPage.BusinessUnitsCount = reportData.Select(r => r.BusinessUnit.Id.Value).Distinct().Count();
            HostingPage.ProjectsCount = 1;

            HostingPage.PersonsCount = reportData.SelectMany(g => g.PersonLevelGroupedHoursList.Select(p =>  p.Person.Id.Value )).Distinct().Count();

            HostingPage.TotalProjectHours = reportData.Sum(g => g.TotalHours);
            HostingPage.BDHours = HostingPage.TotalProjectHours;
            HostingPage.BillableHours = 0d;
            HostingPage.NonBillableHours = HostingPage.TotalProjectHours;
        }

        private void DatabindbyBusinessUnitDetails(List<BusinessUnitLevelGroupedHours> reportdata)
        {
            if (reportdata.Count > 0)
            {
                reportdata = reportdata.OrderBy(bu => bu.BusinessUnit.Name).ToList();
                divEmptyMessage.Style["display"] = "none";
                repBusinessUnits.Visible = true;
                repBusinessUnits.DataSource = reportdata;
                repBusinessUnits.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repBusinessUnits.Visible = false;
            }

            HostingPage.ByBusinessDevelopmentControl.ApplyAttributes(reportdata.Count);

        }

        protected void repBusinessUnits_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelExtenderClientIds = new List<string>();

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var cpeBusinessUnit = e.Item.FindControl("cpeBusinessUnit") as CollapsiblePanelExtender;
                CollapsiblePanelExtenderClientIds.Add(cpeBusinessUnit.BehaviorID);

            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var output = jss.Serialize(CollapsiblePanelExtenderClientIds);
                HostingPage.ByBusinessDevelopmentControl.SetExpandCollapseIdsTohiddenField(output);
            }
        }

        protected void repPersons_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
            }
        }

        protected void repDate_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
            }
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetBusinessUnitStatus(bool isActive)
        {
            return isActive ? "(Active)" : "(InActive)";
        }

        protected bool GetNoteVisibility(String note)
        {
            if (!String.IsNullOrEmpty(note))
            {
                return true;
            }
            return false;

        }

        protected string GetDateFormat(DateTime date)
        {
            return date.ToString(Constants.Formatting.ReportDateFormat);
        }
    }
}
