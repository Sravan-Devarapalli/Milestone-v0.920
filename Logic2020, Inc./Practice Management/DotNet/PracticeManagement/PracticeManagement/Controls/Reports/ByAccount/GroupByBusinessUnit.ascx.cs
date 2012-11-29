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
    public partial class GroupByBusinessUnit : System.Web.UI.UserControl
    {

        private List<string> CollapsiblePanelExtenderClientIds
        {
            get;
            set;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public double PopulateData(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate)
        {
            List<BusinessUnitLevelGroupedHours> data = ServiceCallers.Custom.Report(r => r.AccountReportGroupByBusinessUnit(accountId, businessUnitIds, startDate, endDate)).ToList();
            DatabindbyBusinessUnitDetails(data);


            SetHeaderSectionValues(data);

            return data.Sum(d => d.TotalHours);
        }

        private void SetHeaderSectionValues(List<BusinessUnitLevelGroupedHours> reportData)
        {
            if (Page is AccountSummaryReport)
            {
                var hostingPage = (AccountSummaryReport)Page;
                hostingPage.UpdateHeaderSection = true;

                hostingPage.BusinessUnitsCount = reportData.Select(r => r.BusinessUnit.Id.Value).Distinct().Count();
                hostingPage.ProjectsCount = reportData.Count > 0 ? 1 : 0;
                hostingPage.PersonsCount = reportData.SelectMany(g => g.PersonLevelGroupedHoursList.Select(p => p.Person.Id.Value)).Distinct().Count();

                hostingPage.TotalProjectHours = reportData.Sum(g => g.TotalHours);
                hostingPage.BDHours = hostingPage.TotalProjectHours;
                hostingPage.BillableHours = 0d;
                hostingPage.NonBillableHours = hostingPage.TotalProjectHours;
            }
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

        protected void repBusinessUnits_ItemDataBound(object sender, RepeaterItemEventArgs e)
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

        protected void repPersons_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var cpePerson = e.Item.FindControl("cpePerson") as CollapsiblePanelExtender;
                CollapsiblePanelExtenderClientIds.Add(cpePerson.BehaviorID);
            }
        }

        protected void repDate_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
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

