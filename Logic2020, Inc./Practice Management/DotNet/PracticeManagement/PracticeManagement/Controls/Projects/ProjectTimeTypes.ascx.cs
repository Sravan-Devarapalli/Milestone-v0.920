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
            var timeTypesAssignedToProject = "filterTableRows('" + txtTimeTypesAssignedToProject.ClientID + "','" + cblTimeTypesAssignedToProject.ClientID + "', true);";
            var timeTypesNotAssignedToProject = "filterTableRows('" + txtTimeTypesNotAssignedToProject.ClientID + "','" + cblTimeTypesNotAssignedToProject.ClientID + "', true);";

            txtTimeTypesAssignedToProject.Attributes["onkeyup"] = timeTypesAssignedToProject;
            txtTimeTypesNotAssignedToProject.Attributes["onkeyup"] = timeTypesNotAssignedToProject;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "filterTableRows", timeTypesAssignedToProject + timeTypesNotAssignedToProject, true);
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
                TimeTypeRecord[] NotIncludedTimeTypes = AllTimeTypes.AsQueryable().Where(T => T.IsAdministrative).ToArray();
                AllTimeTypes = AllTimeTypes.AsQueryable().Except(NotIncludedTimeTypes).ToArray();

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
                    string ProjectTimetypesIds = "";
                    foreach (TimeTypeRecord ptt in ProjectTimetypes)
                    {
                        ProjectTimetypesIds = ProjectTimetypesIds + ptt.Id + ',';
                        if (ptt.InUse)
                        {
                            projectTimeTypesInUseIds = projectTimeTypesInUseIds + ptt.Id + ',';
                        }
                    }
                    ProjectTimetypes = GetSelectedList(AllTimeTypes.ToList(), ProjectTimetypesIds).ToArray();
                    hdnProjectTimeTypesInUse.Value = projectTimeTypesInUseIds;
                }
                else
                {
                    ProjectTimetypes = AllTimeTypes.Where(t => t.IsDefault).ToArray();
                    AllTimeTypes = new List<TimeTypeRecord>().ToArray();//remove all
                }
                AllTimeTypes = AllTimeTypes.AsQueryable().Except(ProjectTimetypes).ToArray();

                /* initially hdnTimeTypesAssignedToProject value will be empty if there is is 
                any change in the work types hidden field will be assigned the worktypeid's in javascript */
                hdnTimeTypesAssignedToProject.Value = "";
            }


            if (!String.IsNullOrEmpty(hdnTimeTypesAssignedToProject.Value))
            {
                string TimeTypesAssignedToProject = hdnTimeTypesAssignedToProject.Value;
                List<TimeTypeRecord> CombinedList = new List<TimeTypeRecord>();
                CombinedList.AddRange(ProjectTimetypes);
                CombinedList.AddRange(AllTimeTypes);
                CombinedList = CombinedList.Distinct().ToList();
                ProjectTimetypes = GetSelectedList(CombinedList, TimeTypesAssignedToProject).ToArray();
                AllTimeTypes = CombinedList.Except(ProjectTimetypes).ToArray();
            }
            cblTimeTypesAssignedToProject.DataSource = ProjectTimetypes.OrderBy(t => t.Name);
            cblTimeTypesAssignedToProject.DataBind();
            cblTimeTypesNotAssignedToProject.DataSource = AllTimeTypes.OrderBy(t => t.Name);
            cblTimeTypesNotAssignedToProject.DataBind();
            AddAttributescblTimeTypesNotAssignedToProject();
            AddAttributesTocblTimeTypesAssignedToProject();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SetTimeTypesAssignedToProject", "SetTimeTypesAssignedToProject();changeAlternateitemscolrsForCBL();", true);
        }

        private List<TimeTypeRecord> GetSelectedList(List<TimeTypeRecord> CombinedList, string Ids)
        {
            List<string> IdsList = Ids.Split(',').ToList();
            List<TimeTypeRecord> SelectedList = new List<TimeTypeRecord>();
            foreach (string Id in IdsList)
            {
                int id = 0;
                int.TryParse(Id, out id);
                if (CombinedList.AsQueryable().Any(t => t.Id == id))
                {
                    TimeTypeRecord tt = CombinedList.AsQueryable().First(t => t.Id == id);
                    SelectedList.Add(tt);
                }
            }
            return SelectedList;
        }

        private void AddAttributescblTimeTypesNotAssignedToProject()
        {
            foreach (ListItem item in cblTimeTypesNotAssignedToProject.Items)
            {
                item.Attributes["timeTypeid"] = item.Value;
                item.Attributes["timeTypename"] = item.Text;
            }
        }

        private void AddAttributesTocblTimeTypesAssignedToProject()
        {
            foreach (ListItem item in cblTimeTypesAssignedToProject.Items)
            {
                item.Attributes["timeTypeid"] = item.Value;
                item.Attributes["timeTypename"] = item.Text;
            }
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

                    if (String.IsNullOrEmpty(hdnTimeTypesAssignedToProject.Value))
                    {
                        string TimeTypesAssignedToProject = ",";
                        foreach (ListItem item in cblTimeTypesAssignedToProject.Items)
                        {
                            TimeTypesAssignedToProject = TimeTypesAssignedToProject + item.Value + ",";
                        }
                        hdnTimeTypesAssignedToProject.Value = TimeTypesAssignedToProject;
                    }

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



        public string HdnTimeTypesAssignedToProjectValue
        {
            get
            {
                return hdnTimeTypesAssignedToProject.Value;
            }
        }
    }
}

