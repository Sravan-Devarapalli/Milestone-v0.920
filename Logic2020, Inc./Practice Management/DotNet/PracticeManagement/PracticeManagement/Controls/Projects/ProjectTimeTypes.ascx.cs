using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.TimeEntryService;
using PraticeManagement.Config;
using DataTransferObjects.TimeEntry;
using PraticeManagement.ProjectService;
using DataTransferObjects.Utils;
using System.Web.Security;
using DataTransferObjects;
using PraticeManagement.TimeTypeService;
using System.Web.UI.HtmlControls;
using System.Text;

namespace PraticeManagement.Controls.Projects
{
    public partial class ProjectTimeTypes : System.Web.UI.UserControl
    {

        public TimeTypeRecord[] AllTimeTypes
        {
            get
            {
                return (TimeTypeRecord[])ViewState[ProjectDetail.AllTimeTypesKey];
            }
            set
            {
                ViewState[ProjectDetail.AllTimeTypesKey] = value;
            }
        }

        public TimeTypeRecord[] ProjectTimetypes
        {
            get
            {
                return (TimeTypeRecord[])ViewState[ProjectDetail.ProjectTimeTypesKey];
            }
            set
            {
                ViewState[ProjectDetail.ProjectTimeTypesKey] = value;
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var timeTypesAssignedToProject = "filterTableRows('" + txtTimeTypesAssignedToProject.ClientID + "','tblTimeTypesAssignedToProject',false, true,'timetypename');";
            var timeTypesNotAssignedToProject = "filterTableRows('" + txtTimeTypesNotAssignedToProject.ClientID + "', 'tblTimeTypesNotAssignedToProject',false, true,'timetypename');";

            txtTimeTypesAssignedToProject.Attributes["onkeyup"] = timeTypesAssignedToProject;
            txtTimeTypesNotAssignedToProject.Attributes["onkeyup"] = timeTypesNotAssignedToProject;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "filterTableRowsKey", timeTypesAssignedToProject + timeTypesNotAssignedToProject, true);
        }

        public void ResetSearchTextFilters()
        {
            txtTimeTypesAssignedToProject.Text = string.Empty;
            txtTimeTypesNotAssignedToProject.Text = string.Empty;
        }

        public void PopulateControls()
        {
            if (AllTimeTypes == null && ProjectTimetypes == null)
            {
                var allTimeTypes = ServiceCallers.Invoke<TimeTypeServiceClient, TimeTypeRecord[]>(tt => tt.GetAllTimeTypes());

                AllTimeTypes = allTimeTypes.Where(t => t.IsAdministrative == false).ToArray();

                if (Page is ProjectDetail && ((ProjectDetail)Page).ProjectId != null)
                {
                    Project project = ((ProjectDetail)Page).Project;

                    if (project.IsInternal) //default and internal time types for all internal projects
                    {
                        AllTimeTypes = AllTimeTypes.AsQueryable().Where(T => T.IsDefault || T.IsInternal).ToArray();
                    }
                    else //default and external time types for all external projects
                    {
                        AllTimeTypes = AllTimeTypes.AsQueryable().Where(T => T.IsDefault || !T.IsInternal).ToArray();
                    }

                    int projectId = ((ProjectDetail)Page).ProjectId.Value;
                    ProjectTimetypes = ServiceCallers.Invoke<ProjectServiceClient, TimeTypeRecord[]>(proj => proj.GetTimeTypesByProjectId(projectId, false, null, null));

                }

                AllTimeTypes = AllTimeTypes.Where(tt => ProjectTimetypes.Any(t => tt.Id != t.Id)).ToArray();

            }

            DataBindAllRepeaters();
        }

        public void DataBindAllRepeaters()
        {

            repDefaultTimeTypesAssignedToProject.DataSource = ProjectTimetypes.OrderBy(t => t.Name).Where(t => t.IsDefault == true);
            repDefaultTimeTypesAssignedToProject.DataBind();

            repCustomTimeTypesAssignedToProject.DataSource = ProjectTimetypes.OrderBy(t => t.Name).Where(t => t.IsDefault == false);
            repCustomTimeTypesAssignedToProject.DataBind();

            repDefaultTimeTypesNotAssignedToProject.DataSource = AllTimeTypes.OrderBy(t => t.Name).Where(t => t.IsDefault == true);
            repDefaultTimeTypesNotAssignedToProject.DataBind();

            repCustomTimeTypesNotAssignedToProject.DataSource = AllTimeTypes.OrderBy(t => t.Name).Where(t => t.IsDefault == false);
            repCustomTimeTypesNotAssignedToProject.DataBind();
        }

        private List<TimeTypeRecord> GetSelectedList(List<TimeTypeRecord> combinedList, string ids)
        {
            List<string> idsList = ids.Split(',').ToList();
            List<TimeTypeRecord> selectedList = new List<TimeTypeRecord>();

            foreach (string id in idsList)
            {
                int timeTypeId = 0;

                int.TryParse(id, out timeTypeId);

                if (combinedList.AsQueryable().Any(t => t.Id == timeTypeId))
                {
                    TimeTypeRecord tt = combinedList.AsQueryable().First(t => t.Id == timeTypeId);
                    selectedList.Add(tt);
                }
            }

            return selectedList;
        }

        protected void cvNewTimeTypeName_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = true;
            if (IsTimeTypeAlreadyExisting(txtNewTimeType.Text))
            {
                e.IsValid = false;
            }
        }

        protected void cvTimetype_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = ProjectTimetypes.Count() > 0;

        }


        protected void btnCloseWorkType_OnClick(object sender, EventArgs e)
        {
            mpeAddTimeType.Hide();
            txtNewTimeType.Text = string.Empty;

        }

        protected void btnInsertTimeType_OnClick(object sender, EventArgs e)
        {
            Page.Validate("NewTimeType");
            if (!Page.IsValid)
            {
                mpeAddTimeType.Show();
            }
            else
            {
                try
                {
                    TimeTypeRecord customTimeType = new TimeTypeRecord();
                    customTimeType.IsDefault = false;

                    if (Page is ProjectDetail && ((ProjectDetail)Page).ProjectId != null)
                    {
                        Project project = ((ProjectDetail)Page).Project;
                        customTimeType.IsInternal = project.IsInternal;
                    }

                    customTimeType.Name = txtNewTimeType.Text;
                    customTimeType.IsActive = true;
                    customTimeType.IsAllowedToEdit = true;
                    int customTimeTypeId = ServiceCallers.Invoke<TimeTypeServiceClient, int>(TimeType => TimeType.AddTimeType(customTimeType));
                    customTimeType.Id = customTimeTypeId;

                    var att = ProjectTimetypes.ToList();
                    att.Add(customTimeType);
                    ProjectTimetypes = att.ToArray();

                    repCustomTimeTypesAssignedToProject.DataSource = ProjectTimetypes.OrderBy(t => t.Name).Where(t => t.IsDefault == false);
                    repCustomTimeTypesAssignedToProject.DataBind();

                    txtNewTimeType.Text = "";
                }
                catch
                {
                    cvNewTimeTypeName.ToolTip = "Error Occurred.";
                    cvNewTimeTypeName.IsValid = false;
                    mpeAddTimeType.Show();
                }
            }
        }

        private bool IsTimeTypeAlreadyExisting(string newTimeType)
        {
            using (var serviceClient = new TimeTypeServiceClient())
            {
                TimeTypeRecord[] timeTypesArray = serviceClient.GetAllTimeTypes();

                foreach (TimeTypeRecord timeType in timeTypesArray)
                {
                    if (timeType.Name.ToLower() == newTimeType.ToLower())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected void rep_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var row = e.Item;
                var tt = e.Item.DataItem as TimeTypeRecord;

                var inputBtn = row.FindControl("imgDeleteWorkType") as HtmlInputImage;
                inputBtn.Style["visibility"] = (tt.InUse || tt.IsDefault || tt.IsInternal || tt.IsAdministrative) ? "hidden" : "";
            }
        }

        public string HdnTimeTypesAssignedToProjectValue
        {
            get
            {
                return ProjectTimetypes.Select(tt => tt.Id.ToString()).Aggregate(((name, next) => next + "," + name));
            }


        }

        internal void ShowAlert(string message)
        {
            lbAlertMessage.Text = message;
            mpeTimetypeAlertMessage.Show();
        }


        protected void btnAssignAll_OnClick(object sender, EventArgs e)
        {
            var timeTypesList = AllTimeTypes.ToList();
            timeTypesList.AddRange(ProjectTimetypes);
            ProjectTimetypes = timeTypesList.ToArray();
            AllTimeTypes = AllTimeTypes.Where(t => 1 == 0).ToArray();

        }

        protected void btnUnAssignAll_OnClick(object sender, EventArgs e)
        {
            var timeTypesList = AllTimeTypes.ToList();
            timeTypesList.AddRange(ProjectTimetypes);
            AllTimeTypes = timeTypesList.ToArray();
            ProjectTimetypes = ProjectTimetypes.Where(t => 1 == 0).ToArray();
        }

        protected void btnUnAssign_OnClick(object sender, EventArgs e)
        {
            StringBuilder timeTypeIds = new StringBuilder();
            List<int> ttIds = new List<int>();

            foreach (RepeaterItem row in repDefaultTimeTypesAssignedToProject.Items)
            {
                var cb = row.FindControl("cbTimeTypesNotAssignedToProject") as HtmlInputCheckBox;
                var imgDeleteWorkType = row.FindControl("imgDeleteWorkType") as HtmlImage;

                if (cb.Checked)
                {
                    var timeTypeId = Convert.ToInt32(imgDeleteWorkType.Attributes["timetypeid"]);
                    timeTypeIds.Append(timeTypeId);
                    timeTypeIds.Append(",");

                    ttIds.Add(timeTypeId);
                }
            }

            foreach (RepeaterItem row in repCustomTimeTypesAssignedToProject.Items)
            {
                var cb = row.FindControl("cbTimeTypesNotAssignedToProject") as HtmlInputCheckBox;
                var imgDeleteWorkType = row.FindControl("imgDeleteWorkType") as HtmlImage;

                if (cb.Checked)
                {
                    var timeTypeId = Convert.ToInt32(imgDeleteWorkType.Attributes["timetypeid"]);
                    timeTypeIds.Append(timeTypeId);
                    timeTypeIds.Append(",");

                    ttIds.Add(timeTypeId);
                }
            }

            var timeTypesList = ProjectTimetypes.Where(ptt => ttIds.Any(tt => tt == ptt.Id)).ToList();

            ProjectTimetypes = ProjectTimetypes.Where(ptt => ttIds.Any(tt => tt == ptt.Id) == false).ToArray();

            AllTimeTypes.ToList().AddRange(timeTypesList);

            AllTimeTypes = AllTimeTypes;

            DataBindAllRepeaters();

        }

        protected void btnAssign_OnClick(object sender, EventArgs e)
        {
            StringBuilder timeTypeIds = new StringBuilder();
            List<int> ttIds = new List<int>();

            foreach (RepeaterItem row in repDefaultTimeTypesNotAssignedToProject.Items)
            {
                var cb = row.FindControl("cbTimeTypesAssignedToProject") as HtmlInputCheckBox;
                var imgDeleteWorkType = row.FindControl("imgDeleteWorkType") as HtmlImage;

                if (cb.Checked)
                {
                    var timeTypeId = Convert.ToInt32(imgDeleteWorkType.Attributes["timetypeid"]);
                    timeTypeIds.Append(timeTypeId);
                    timeTypeIds.Append(",");

                    ttIds.Add(timeTypeId);
                }
            }

            foreach (RepeaterItem row in repCustomTimeTypesNotAssignedToProject.Items)
            {
                var cb = row.FindControl("cbTimeTypesAssignedToProject") as HtmlInputCheckBox;
                var imgDeleteWorkType = row.FindControl("imgDeleteWorkType") as HtmlImage;

                if (cb.Checked)
                {
                    var timeTypeId = Convert.ToInt32(imgDeleteWorkType.Attributes["timetypeid"]);
                    timeTypeIds.Append(timeTypeId);
                    timeTypeIds.Append(",");

                    ttIds.Add(timeTypeId);
                }
            }


            var timeTypesList = AllTimeTypes.Where(ptt => ttIds.Any(tt => tt == ptt.Id)).ToList();

            AllTimeTypes = AllTimeTypes.Where(ptt => ttIds.Any(tt => tt == ptt.Id) == false).ToArray();

            ProjectTimetypes.ToList().AddRange(timeTypesList);

            ProjectTimetypes = ProjectTimetypes;

            DataBindAllRepeaters();
        }
    }
}

