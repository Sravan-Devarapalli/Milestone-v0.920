using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DataTransferObjects.Reports;
using DataTransferObjects.TimeEntry;

namespace PraticeManagement.Controls.Reports
{
    public partial class ProjectSummaryByMatrix : System.Web.UI.UserControl
    {
        //#region Constants

        //private const string Repeater_MatrixHeaders = "repMatrixHeaders";
        //private const string Repeater_MatrixHoursPerWorkType = "repMatrixHoursPerWorkType";

        //#endregion

        //#region Properties

        //public List<TimeTypeRecord> WorkTypes { get; set; }


        //#endregion

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    extBillableNonBillableAndTotalExtender.ControlsToCheck = rbBillable.ClientID + ";" + rbCombined.ClientID + ";" + rbNonBillable.ClientID;
        //}

        //protected void repMatrix_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //{
        //    if (e.Item.ItemType == ListItemType.Header)
        //    {
        //        var repResourceHeaders = e.Item.FindControl(Repeater_MatrixHeaders) as Repeater;
        //        repResourceHeaders.DataSource = WorkTypes;
        //        repResourceHeaders.DataBind();
        //        extBillableNonBillableAndTotalExtender.TargetControlsToCheck = string.Empty;
        //    }
        //    else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //    {
        //        //var repMatrixHoursPerWorkType = e.Item.FindControl(Repeater_MatrixHoursPerWorkType) as Repeater;
        //        //var tdPersonTotalHours = e.Item.FindControl("tdPersonTotalHours") as HtmlTableCell;
        //        //extBillableNonBillableAndTotalExtender.TargetControlsToCheck += tdPersonTotalHours.ClientID + ";";
                
        //        //var resourceHoursPerWorkType = new Dictionary<TimeTypeRecord, WorkTypeLevelGroupedHours>();
        //        //var groupedHours = ((PersonLevelGroupedHours)e.Item.DataItem).GroupedWorkTypeHoursList;

        //        //foreach (var workType in WorkTypes)
        //        //{
        //        //    resourceHoursPerWorkType.Add(workType, GetWorkTypeLevelGroupedHours(groupedHours, workType));
        //        //}

        //        //repMatrixHoursPerWorkType.DataSource = resourceHoursPerWorkType;
        //        //repMatrixHoursPerWorkType.DataBind();
        //    }
        //}

        //protected void repMatrixHoursPerWorkType_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        //{

        //    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //    {
        //        var tdWorkTypeTotalHours = e.Item.FindControl("tdWorkTypeTotalHours") as HtmlTableCell;
        //        extBillableNonBillableAndTotalExtender.TargetControlsToCheck += tdWorkTypeTotalHours.ClientID + ";";
        //    }
        //}


        //private WorkTypeLevelGroupedHours GetWorkTypeLevelGroupedHours(List<WorkTypeLevelGroupedHours> groupedHours, TimeTypeRecord workType)
        //{
        //    if (groupedHours.Any(d => d.WorkType.Id == workType.Id))
        //    {
        //        return groupedHours.First(gh => gh.WorkType.Id == workType.Id);
        //    }

        //    return new WorkTypeLevelGroupedHours();
        //}

        //public void DataBindResource(PersonLevelGroupedHours[] reportData)
        //{
        //    var workTypes = new List<TimeTypeRecord>();

        //    foreach (var personLevel in reportData)
        //    {
        //        foreach (var item in personLevel.GroupedWorkTypeHoursList)
        //        {
        //            if (!(workTypes.Any(w => w.Id == item.WorkType.Id)))
        //            {
        //                workTypes.Add(item.WorkType);
        //            }
        //        }
        //    }

        //    WorkTypes = workTypes;
        //    repMatrix.DataSource = reportData;
        //    repMatrix.DataBind();
        //}
    }
}
