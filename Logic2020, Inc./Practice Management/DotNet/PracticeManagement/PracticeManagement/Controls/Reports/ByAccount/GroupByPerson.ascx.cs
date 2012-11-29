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
using PraticeManagement.Reporting;


namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class GroupByPerson : System.Web.UI.UserControl
    {
        private List<string> CollapsiblePanelExtenderClientIds
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void DatabindbyBusinessUnitDetails(List<DataTransferObjects.Reports.ByAccount.GroupByPerson> reportdata)
        {
            if (reportdata.Count > 0)
            {
                reportdata = reportdata.OrderBy(bu => bu.Person.Name).ToList();
                divEmptyMessage.Style["display"] = "none";
                repPersonsList.Visible = true;
                repPersonsList.DataSource = reportdata;
                repPersonsList.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repPersonsList.Visible = false;
            }

            if (Page is AccountSummaryReport)
            {
                var hostingPage = (AccountSummaryReport)Page;
                hostingPage.ByBusinessDevelopmentControl.ApplyAttributes(reportdata.Count);
            }
            else
            {
                var hostingPage = (TimePeriodSummaryReport)Page;
                hostingPage.ByProjectControl.ByBusinessDevelopmentControl.ApplyAttributes(reportdata.Count);
            }

        }

        protected void repPersonsList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelExtenderClientIds = new List<string>();
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var output = jss.Serialize(CollapsiblePanelExtenderClientIds);
                if (Page is AccountSummaryReport)
                {
                    var hostingPage = (AccountSummaryReport)Page;
                    hostingPage.ByBusinessDevelopmentControl.SetExpandCollapseIdsTohiddenField(output);
                }
                else
                {
                    var hostingPage = (TimePeriodSummaryReport)Page;
                    hostingPage.ByProjectControl.ByBusinessDevelopmentControl.SetExpandCollapseIdsTohiddenField(output);
                }
            }
        }

        protected void repBusinessUnit_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {            
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var cpeBusinessUnit = e.Item.FindControl("cpeBusinessUnit") as CollapsiblePanelExtender;
                CollapsiblePanelExtenderClientIds.Add(cpeBusinessUnit.BehaviorID);
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

        public double PopulateData(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate)
        {
            var data = ServiceCallers.Custom.Report(r => r.AccountReportGroupByPerson(accountId, businessUnitIds, startDate, endDate)).ToList();
            DatabindbyBusinessUnitDetails(data);

            SetHeaderSectionValues(data);

            return data.Sum(d => d.TotalHours);
        }

        private void SetHeaderSectionValues(List<DataTransferObjects.Reports.ByAccount.GroupByPerson> reportData)
        {
            if (Page is AccountSummaryReport)
            {
                var hostingPage = (AccountSummaryReport)Page;
                hostingPage.UpdateHeaderSection = true;

                hostingPage.BusinessUnitsCount = reportData.SelectMany(p => p.BusinessUnitLevelGroupedHoursList.Select(g => g.BusinessUnit.Id.Value)).Distinct().Count();
                hostingPage.ProjectsCount = reportData.Count > 0 ? 1 : 0;
                hostingPage.PersonsCount = reportData.Select(p => p.Person.Id.Value).Distinct().Count();

                hostingPage.TotalProjectHours = reportData.Sum(p => p.TotalHours);
                hostingPage.BDHours = reportData.Sum(p => p.TotalHours);
                hostingPage.BillableHours = 0d;
                hostingPage.NonBillableHours = reportData.Sum(g => g.TotalHours);
            }
        }

    }
}

