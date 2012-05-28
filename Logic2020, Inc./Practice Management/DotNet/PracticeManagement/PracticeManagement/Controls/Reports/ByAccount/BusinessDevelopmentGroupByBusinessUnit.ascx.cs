using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports.ByAccount;

namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class BusinessDevelopmentGroupByBusinessUnit : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void PopulateData(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate)
        {
            var data = ServiceCallers.Custom.Report(r => r.AccountReportGroupByBusinessUnit(accountId, businessUnitIds, startDate, endDate));
            repBusinessUnits.DataSource = data;
            repBusinessUnits.DataBind();
        }

        //private void DataBindByBusinessUnitDetail(BusinessUnitLevelGroupedHours[] reportData)
        //{
        //    if (reportData.Length > 0)
        //    {
        //        reportData = reportData.OrderBy(bu => bu.b p.Person.PersonLastFirstName).ToArray();
        //        divEmptyMessage.Style["display"] = "none";
        //        repBusinessUnits.Visible = btnExpandOrCollapseAll.Visible = true;
        //        repBusinessUnits.DataSource = reportData;
        //        repBusinessUnits.DataBind();
        //    }
        //    else
        //    {
        //        divEmptyMessage.Style["display"] = "";
        //        repBusinessUnits.Visible = btnExpandOrCollapseAll.Visible = false;
        //    }
        //    btnExportToPDF.Enabled =
        //    btnExportToExcel.Enabled = reportData.Count() > 0;
        //}

        //protected void repBusinessUnits_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //{

        //}

        //public void DataBindByResourceDetail(BusinessUnitLevelGroupedHours[] reportData)
        //{
        //    if (reportData.Length > 0)
        //    {
        //        reportData = reportData.OrderBy(p => p.Person.PersonLastFirstName).ToArray();
        //        divEmptyMessage.Style["display"] = "none";
        //        repPersons.Visible = btnExpandOrCollapseAll.Visible = true;
        //        repPersons.DataSource = reportData;
        //        repPersons.DataBind();
        //    }
        //    else
        //    {
        //        divEmptyMessage.Style["display"] = "";
        //        repPersons.Visible = btnExpandOrCollapseAll.Visible = false;
        //    }
        //    btnExportToPDF.Enabled =
        //    btnExportToExcel.Enabled = reportData.Count() > 0;
        //}

        //protected void repPersons_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //{
        //    if (e.Item.ItemType == ListItemType.Header)
        //    {
        //        CollapsiblePanelExtenderClientIds = new List<string>();

        //    }
        //    else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //    {
        //        var repDate = e.Item.FindControl("repDate") as Repeater;
        //        PersonLevelGroupedHours dataitem = (PersonLevelGroupedHours)e.Item.DataItem;

        //        var cpePerson = e.Item.FindControl("cpePerson") as CollapsiblePanelExtender;
        //        cpePerson.BehaviorID = cpePerson.ClientID + e.Item.ItemIndex.ToString();

        //        sectionId = dataitem.TimeEntrySectionId;
        //        repDate.DataSource = dataitem.DayTotalHours != null ? dataitem.DayTotalHours.OrderBy(p => p.Date).ToList() : dataitem.DayTotalHours;
        //        repDate.DataBind();
        //        CollapsiblePanelExtenderClientIds.Add(cpePerson.BehaviorID);

        //    }
        //    else if (e.Item.ItemType == ListItemType.Footer)
        //    {
        //        JavaScriptSerializer jss = new JavaScriptSerializer();
        //        var output = jss.Serialize(CollapsiblePanelExtenderClientIds);
        //        hdncpeExtendersIds.Value = output;
        //        btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = "Expand All";
        //        hdnCollapsed.Value = "true";
        //    }
        //}

        //protected void repDate_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //{
        //    if (e.Item.ItemType == ListItemType.Header)
        //    {

        //    }
        //    else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //    {
        //        var repWorktype = e.Item.FindControl("repWorktype") as Repeater;
        //        TimeEntriesGroupByDate dataitem = (TimeEntriesGroupByDate)e.Item.DataItem;
        //        var rep = sender as Repeater;

        //        //var cpeDate = e.Item.FindControl("cpeDate") as CollapsiblePanelExtender;
        //        //cpeDate.BehaviorID = cpeDate.ClientID + e.Item.ItemIndex.ToString();
        //        // CollapsiblePanelDateExtenderClientIds.Add(cpeDate.BehaviorID);


        //        repWorktype.DataSource = dataitem.DayTotalHoursList;
        //        repWorktype.DataBind();
        //    }
        //}

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetPersonRole(string role)
        {
            return string.IsNullOrEmpty(role) ? "" : "(" + role + ")";
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
