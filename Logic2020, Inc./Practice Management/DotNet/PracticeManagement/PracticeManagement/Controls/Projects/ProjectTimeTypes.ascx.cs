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
                AllTimeTypes = ServiceCallers.Invoke<TimeTypeServiceClient, TimeTypeRecord[]>(TimeType => TimeType.GetAllTimeTypes());
                TimeTypeRecord[] notIncludedTimeTypes = AllTimeTypes.AsQueryable().Where(T => T.IsAdministrative).ToArray();
                AllTimeTypes = AllTimeTypes.AsQueryable().Except(notIncludedTimeTypes).ToArray();

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
                    ProjectTimetypes = ServiceCallers.Invoke<ProjectServiceClient, TimeTypeRecord[]>(Project => Project.GetTimeTypesByProjectId(projectId, false, null, null));

                    string projectTimeTypesInUseIds = "";
                    string projectTimetypesIds = "";

                    foreach (TimeTypeRecord ptt in ProjectTimetypes)
                    {
                        projectTimetypesIds = projectTimetypesIds + ptt.Id + ',';
                        if (ptt.InUse)
                        {
                            projectTimeTypesInUseIds = projectTimeTypesInUseIds + ptt.Id + ',';
                        }
                    }

                    ProjectTimetypes = GetSelectedList(AllTimeTypes.ToList(), projectTimetypesIds).ToArray();
                    hdnProjectTimeTypesInUse.Value = projectTimeTypesInUseIds;
                }
                else
                {
                    ProjectTimetypes = AllTimeTypes.Where(t => t.IsDefault).ToArray();
                    AllTimeTypes = new List<TimeTypeRecord>().ToArray();//remove all
                }

                AllTimeTypes = AllTimeTypes.AsQueryable().Except(ProjectTimetypes).ToArray();

                /* initially hdnTimeTypesAssignedToProject value will be empty if there is is 
                any change in the work types hidden field will be assigned the work type id's in javascript */
                hdnTimeTypesAssignedToProject.Value = "";
            }


            if (!String.IsNullOrEmpty(hdnTimeTypesAssignedToProject.Value))
            {
                string timeTypesAssignedToProject = hdnTimeTypesAssignedToProject.Value;
                List<TimeTypeRecord> combinedList = new List<TimeTypeRecord>();
                combinedList.AddRange(ProjectTimetypes);
                combinedList.AddRange(AllTimeTypes);
                combinedList = combinedList.Distinct().ToList();

                ProjectTimetypes = GetSelectedList(combinedList, timeTypesAssignedToProject).ToArray();
                AllTimeTypes = combinedList.Except(ProjectTimetypes).ToArray();
            }
            repTimeTypesAssignedToProject.DataSource = ProjectTimetypes.OrderBy(t => t.Name);
            repTimeTypesAssignedToProject.DataBind();
            repTimeTypesNotAssignedToProject.DataSource = AllTimeTypes.OrderBy(t => t.Name);
            repTimeTypesNotAssignedToProject.DataBind();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SetTimeTypesAssignedToProject", "SetTimeTypesAssignedToProject();changeAlternateitemscolrsForCBL();", true);
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
            e.IsValid = true;
            if (hdnTimeTypesAssignedToProject.Value == ",")
            {
                e.IsValid = false;
            }
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

                    hdnTimeTypesAssignedToProject.Value += customTimeType.Id;

                    var att = AllTimeTypes.ToList();
                    att.Add(customTimeType);
                    AllTimeTypes = att.ToArray();

                    txtNewTimeType.Text = "";
                }
                catch (Exception e1)
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
                return hdnTimeTypesAssignedToProject.Value;
            }

            set
            {
                hdnTimeTypesAssignedToProject.Value = value;
            }
        }

        internal void ShowAlert(string message)
        {
            lbAlertMessage.Text = message;
            mpeTimetypeAlertMessage.Show();
        }
    }
}

