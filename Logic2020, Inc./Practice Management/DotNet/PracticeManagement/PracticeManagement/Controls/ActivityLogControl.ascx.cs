using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Xsl;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using System.Xml;
using PraticeManagement.Configuration;
using PraticeManagement.Utils;
using PraticeManagement.FilterObjects;

namespace PraticeManagement.Controls
{
    public partial class ActivityLogControl : UserControl
    {
        #region Constants

        private const string ViewstateDisplayVisible = "DISPLAY_VISIBLE";
        private const string ProjectDisplayViewstate = "DISPLAY_PROJECT";
        private const string PersonDisplayViewstate = "DISPLAY_PERSON";
        private const string ViewstateEventSource = "EVENT_SOURCE";
        private const string OpportunityIdViewstate = "OPPORTUNITY_ID";
        private const string MilestoneIdViewstate = "MILESTONE_ID";
        private const string ModifiedByNameAttribute = "ModifiedByName";
        private const string UserNameAttribute = "UserName";
        private const string UserAttribute = "User";

        private const string LoggedInActivity = "logged in";
        private const string NewValuesTag = "new_values";
        private const string LoginTag = "login";

        #endregion

        #region Titles

        //  Provides mapping between event sources and drop-down item titles
        private static readonly Dictionary<ActivityEventSource, string> EventSourceTitles =
            new Dictionary<ActivityEventSource, string>();

        //  Provides mapping between even sources and target drop-downs to set value at
        private static readonly Dictionary<ActivityEventSource, DropDownList> ControlMapping =
            new Dictionary<ActivityEventSource, DropDownList>();

        private string _currentUrl;
        private XsltArgumentList _argumentList;

        static ActivityLogControl()
        {
            InitDropDownTitles();
        }

        /// <summary>
        /// 	Default constructor of ActivityLogControl.
        /// </summary>
        public ActivityLogControl()
        {
            ControlMapping.Clear();
            ControlMapping.Add(ActivityEventSource.Project, ddlProjects);
            ControlMapping.Add(ActivityEventSource.Person, ddlPersonName);
        }

        protected void InitXsltParams()
        {
            _currentUrl = HttpUtility.UrlEncode(Request.Url.AbsoluteUri) + (IsActivityLogPage ? (Request.Url.Query.Length > 0 ? string.Empty : Constants.FilterKeys.QueryStringOfApplyFilterFromCookie) : string.Empty);

            _argumentList = new XsltArgumentList();
            _argumentList.AddParam("currentUrl", string.Empty, _currentUrl);
        }

        private static void InitDropDownTitles()
        {
            EventSourceTitles.Add(
                ActivityEventSource.All, Resources.Controls.EventSourceAll);
            EventSourceTitles.Add(
                ActivityEventSource.Error, Resources.Controls.EventSourceError);
            EventSourceTitles.Add(
                ActivityEventSource.Person, Resources.Controls.EventSourcePerson);
            EventSourceTitles.Add(
                ActivityEventSource.TargetPerson,
                Resources.Controls.EventSourceTargetPerson);
            EventSourceTitles.Add(
                ActivityEventSource.Project, Resources.Controls.EventSourceProject);
            EventSourceTitles.Add(
                ActivityEventSource.ProjectAndMilestones,
                Resources.Controls.EventSourceProjectAndMilestones);
            EventSourceTitles.Add(
                ActivityEventSource.TimeEntry,
                Resources.Controls.EvenSourceTimeEntries);
            EventSourceTitles.Add(
                ActivityEventSource.Opportunity,
                Resources.Controls.EventSourceOpportunities);
            EventSourceTitles.Add(
               ActivityEventSource.Milestone,
               Resources.Controls.EventSourceMilestones);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 	Defines whether to show Display drop-down or not
        /// </summary>
        public bool ShowDisplayDropDown
        {
            get
            {
                var obj = ViewState[ViewstateDisplayVisible];

                if (obj != null)
                    return (bool)obj;

                return true;
            }

            set { ViewState[ViewstateDisplayVisible] = value; }
        }

        /// <summary>
        /// 	Defines whether to show Project drop-down or not
        /// </summary>
        public bool ShowProjectDropDown
        {
            get
            {
                var obj = ViewState[ProjectDisplayViewstate];

                if (obj != null)
                    return (bool)obj;

                return true;
            }

            set { ViewState[ProjectDisplayViewstate] = value; }
        }

        /// <summary>
        /// 	Defines whether to show Project drop-down or not
        /// </summary>
        public bool ShowPersonDropDown
        {
            get
            {
                var obj = ViewState[PersonDisplayViewstate];

                if (obj != null)
                    return (bool)obj;

                return true;
            }

            set { ViewState[PersonDisplayViewstate] = value; }
        }

        /// <summary>
        /// 	Defines whether to show Display drop-down or not
        /// </summary>
        public int? OpportunityId
        {
            get
            {
                var obj = ViewState[OpportunityIdViewstate];

                if (obj != null)
                    return (int?)obj;

                return null;
            }

            set { ViewState[OpportunityIdViewstate] = value; }
        }

        public int? MilestoneId
        {
            get
            {
                var obj = ViewState[MilestoneIdViewstate];

                if (obj != null)
                    return (int?)obj;

                return null;
            }

            set { ViewState[MilestoneIdViewstate] = value; }
        }
        /// <summary>
        /// 	Sets Display drop-down value
        /// </summary>
        public ActivityEventSource DisplayDropDownValue
        {
            get
            {
                var obj = ViewState[ViewstateEventSource];

                if (obj != null)
                    return (ActivityEventSource)obj;

                return ActivityEventSource.All;
            }
            set
            {
                ViewState[ViewstateEventSource] = value;
            }
        }

        public bool IsFreshRequest { get; set; }

        public bool IsActivityLogPage { get; set; }

        #endregion

        public DateTime? FromDateFilterValue
        {
            get { return diYear.FromDate; }
            set { diYear.FromDate = value; }
        }

        public DateTime? ToDateFilterValue
        {
            get { return diYear.ToDate; }
            set { diYear.ToDate = value; }
        }

        public bool IsFiltersReadingFromCookie { get; set; }


        private static string GetStringByValue(ActivityEventSource value)
        {
            return Enum.GetName(typeof(ActivityEventSource), value);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack || IsFreshRequest)
            {
                if (IsActivityLogPage)
                {
                    var cookie = SerializationHelper.DeserializeCookie(Constants.FilterKeys.ActivityLogFilterCookie) as ActivityLogFilter;
                    if (Request.QueryString[Constants.FilterKeys.ApplyFilterFromCookieKey] == "true" && cookie != null)
                    {
                        if (cookie.FiltersChanged)
                        {
                            IsFiltersReadingFromCookie = true;
                            DisplayDropDownValue = (ActivityEventSource)Enum.Parse(typeof(ActivityEventSource), cookie.EventSourceSelected);
                            FromDateFilterValue = cookie.FromDateFilterValue;
                            ToDateFilterValue = cookie.ToDateFilterValue;
                            if (!string.IsNullOrEmpty(cookie.ProjectSelected))
                                ddlProjects.SelectedValue = cookie.ProjectSelected.Trim();
                            if (!string.IsNullOrEmpty(cookie.PersonSelected))
                                ddlPersonName.SelectedValue = cookie.PersonSelected.Trim();

                            hdnResetFilter.Value = cookie.FiltersChanged.ToString();
                            btnResetFilter.Enabled = cookie.FiltersChanged;
                        }
                        gvActivities.PageIndex = cookie.CurrentIndex;
                    }
                    spnProjects.Disabled = false;
                }

                FillEventList();

                ddlProjects.DataBind();
                ddlPersonName.DataBind();

                if (!IsActivityLogPage)
                {
                    var display = ShowDisplayDropDown;
                    ddlEventSource.Visible = display;
                    lblDisplay.Visible = display;
                    spnProjects.Visible = ShowProjectDropDown;
                    spnPersons.Visible = ShowPersonDropDown;
                }
            }

            InitXsltParams();

            if (!IsPostBack)
            {
                ResetFilters();
            }


            if (IsActivityLogPage)
            {
                lblDisplay.Width = Unit.Percentage(20);
                ddlEventSource.Width = Unit.Percentage(80);
                Label1.Width = Unit.Percentage(18);
                ddlPersonName.Width = Unit.Percentage(82);
                Label2.Width = Unit.Percentage(10);
                ddlProjects.Width = Unit.Percentage(90);
                tblActivitylog.Attributes["class"] = "CompPerfTable WholeWidth";
                tdEventSource.Style.Add("width", "19%");
                tdYear.Style.Add("width", "27%");
                spnPersons.Style.Add("width", "19%");
                spnProjects.Style.Add("width", "20%");
                tdBtnList.Style.Add("width", "15%");
                tdBtnList.Align = "right";
                divActivitylog.Style.Add("background-color", "#d4dff8");
                btnResetFilter.Visible = true;
            }
            else
            {
                ddlEventSource.Width = Unit.Pixel(100);
                ddlPersonName.Width = Unit.Pixel(150);
                ddlProjects.Width = Unit.Pixel(150);
            }
        }

        private void ResetFilters()
        {
            if (!IsFiltersReadingFromCookie)
            {
                diYear.FromDate = SettingsHelper.GetCurrentPMTime().AddYears(-1);
                diYear.ToDate = SettingsHelper.GetCurrentPMTime();
            }

            if (IsActivityLogPage)
            {
                if (!IsFiltersReadingFromCookie)
                {
                    ddlEventSource.SelectedIndex = ddlPersonName.SelectedIndex = ddlProjects.SelectedIndex = 0;
                    hdnResetFilter.Value = "false";
                    btnResetFilter.Enabled = false;
                }
                ddlEventSource.Attributes["onchange"] = ddlPersonName.Attributes["onchange"] = ddlProjects.Attributes["onchange"] = "EnableResetButton();";
                diYear.OnClientChange = "EnableResetButtonForDateIntervalChange";
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SetTooltipsForallDropDowns", "SetTooltipsForallDropDowns();", true);
            if (IsFreshRequest)
            {
                Update();
            }
        }

        private void SaveFilterSettings()
        {
            ActivityLogFilter filter = GetFilterSettings();
            SerializationHelper.SerializeCookie(filter, Constants.FilterKeys.ActivityLogFilterCookie);
        }

        private ActivityLogFilter GetFilterSettings()
        {
            var filter = new ActivityLogFilter
            {
                EventSourceSelected = ddlEventSource.SelectedValue,
                FromDateFilterValue = FromDateFilterValue,
                ToDateFilterValue = ToDateFilterValue,
                PersonSelected = ddlPersonName.SelectedValue.Trim(),
                ProjectSelected = ddlProjects.SelectedValue.Trim(),
                CurrentIndex = gvActivities.PageIndex,
                FiltersChanged = Convert.ToBoolean(hdnResetFilter.Value)
            };
            return filter;
        }

        private void FillEventList()
        {
            ddlEventSource.DataSource = EventSourceTitles;
            ddlEventSource.DataTextField = "Value";
            ddlEventSource.DataValueField = "Key";
            ddlEventSource.DataBind();

            ddlEventSource.SelectedValue = GetStringByValue(DisplayDropDownValue);
        }


        /// <summary>
        /// If we're on the page with ID parameter, select corresponding entity
        /// </summary>
        private void SelectFilterType()
        {
            var entityId = Page.Request[Constants.QueryStringParameterNames.Id];
            if (!string.IsNullOrEmpty(entityId))
            {
                entityId = entityId.Trim();
                var selectedValue = ddlEventSource.SelectedValue;

                if (selectedValue == GetStringByValue(ActivityEventSource.Project))
                    PrepareDropDown(entityId, ddlProjects);
                else if (selectedValue == GetStringByValue(ActivityEventSource.TargetPerson))
                    PrepareDropDown(entityId, ddlPersonName);
            }
        }

        private static void PrepareDropDown(string entityId, DropDownList listControl)
        {
            listControl.SelectedValue = entityId;
            listControl.Enabled = false;
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            UpdateGrid();

            if (hdnResetFilter.Value == "true")
                btnResetFilter.Enabled = true;
            else
                btnResetFilter.Enabled = false;

        }

        protected void btnResetFilter_Click(object sender, EventArgs e)
        {
            ResetFilters();
            Update();
        }

        private void UpdateGrid()
        {
            gvActivities.PageIndex = 0;
            gvActivities.DataBind();
        }

        public void Update()
        {
            UpdateGrid();
            updActivityLog.Update();
        }

        protected void gvActivities_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                InitTransformation(e, "xmlChanges");
                InitTransformation(e, "xmlActivityItem");
            }
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                TableCell tblcell0 = e.Row.Cells[0];
                tblcell0.Attributes.Add("colSpan", "1");

                TableCell tblcell = new TableCell();
                tblcell.Text = "&nbsp;&nbsp; of " + gvActivities.PageCount + " Pages";
                tblcell.Attributes.Add("colSpan", "2");
                e.Row.Cells.AddAt(1, tblcell);
            }
        }

        protected void gvActivities_OnDataBound(object sender, EventArgs e)
        {
            if (IsActivityLogPage)
            {
                SaveFilterSettings();
            }
        }

        protected void ddlProjects_OnDataBound(object sender, EventArgs e)
        {
            AddEmptyRow(ddlProjects, Resources.Controls.AllProjects);
            SelectFilterType();
        }

        private static void AddEmptyRow(DropDownList dropDown, string text)
        {
            if (dropDown.Items.Count > 0)
                dropDown.Items.Insert(
                    0, new ListItem(text, string.Empty));
        }

        protected void ddlPersonName_OnDataBound(object sender, EventArgs e)
        {
            AddEmptyRow(ddlPersonName, Resources.Controls.AnyUserText);
            SelectFilterType();
        }

        private void InitTransformation(GridViewRowEventArgs e, string xmlchangesControl)
        {
            var entityChanges = e.Row.FindControl(xmlchangesControl) as Xml;

            if (entityChanges != null)
                entityChanges.TransformArgumentList = _argumentList;
        }

        protected void odsActivities_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["startDateFilter"] = diYear.FromDate.HasValue ? diYear.FromDate : SettingsHelper.GetCurrentPMTime().AddYears(-1);
            e.InputParameters["endDateFilter"] = diYear.ToDate.HasValue ? diYear.ToDate : SettingsHelper.GetCurrentPMTime();
            e.InputParameters["sourceFilter"] = ddlEventSource.SelectedValue;
            e.InputParameters["opportunityId"] = (this.OpportunityId == null ? null : this.OpportunityId.ToString());
            e.InputParameters["milestoneId"] = (this.MilestoneId == null ? null : this.MilestoneId.ToString());
            e.InputParameters["personId"] = string.IsNullOrEmpty(ddlPersonName.SelectedValue) ?
                                                null : ddlPersonName.SelectedValue;
            e.InputParameters["projectId"] = string.IsNullOrEmpty(ddlProjects.SelectedValue) ?
                                                null : ddlProjects.SelectedValue;

        }

        public object AddDefaultProjectAndMileStoneInfo(object logDataObject)
        {
            if (logDataObject != null)
            {
                var logDataStr = logDataObject.ToString();
                var XmlDoc = new XmlDocument();
                XmlDoc.LoadXml(logDataStr);
                var Root = XmlDoc.FirstChild;
                var defaultProjectId = MileStoneConfigurationManager.GetProjectId();
                var defaultMileStoneId = MileStoneConfigurationManager.GetMileStoneId();
                if (defaultProjectId.HasValue)
                {
                    var defaultProjectIdElement = XmlDoc.CreateElement("DefaultProjectId");
                    defaultProjectIdElement.InnerText = defaultProjectId.Value.ToString();
                    Root.InsertAfter(defaultProjectIdElement, Root.LastChild);
                }
                if (defaultMileStoneId.HasValue)
                {
                    var defaultMileStoneIdElement = XmlDoc.CreateElement("DefaultMileStoneId");
                    defaultMileStoneIdElement.InnerText = defaultMileStoneId.Value.ToString();
                    Root.InsertAfter(defaultMileStoneIdElement, Root.LastChild);
                }

                return Root.OuterXml;
            }
            else
            {
                return logDataObject;
            }
        }

        public string GetModifiedByDetails(object personId, object personLastFirstName, string SystemUser, object logData)
        {
            if (personId != null)
            {
                return personLastFirstName.ToString();
            }

            string modifiedOrUser = string.Empty;

            if (logData != null)
            {
                var logDataStr = logData.ToString();
                var XmlDoc = new XmlDocument();
                XmlDoc.LoadXml(logDataStr);
                var Root = XmlDoc.FirstChild;
                if (Root.HasChildNodes)
                {
                    var newValues = Root.FirstChild;
                    if (Root.Name.ToLower() == LoginTag)
                    {
                        modifiedOrUser = GetAttribute(newValues, UserNameAttribute);
                    }
                    else if (Root.Name.ToLower() == "becomeuser" || Root.Name.ToLower() == "export")
                    {
                        modifiedOrUser = GetAttribute(newValues, UserAttribute);
                    }
                    else if (Root.Name.ToLower() == "note")
                    {
                        modifiedOrUser = GetAttribute(newValues, "By");
                    }
                    //else if (Root.Name.ToLower() == "error")
                    //{
                    //    modifiedOrUser = GetAttribute(newValues, "Login");
                    //}
                    else if (logData.ToString().Contains(ModifiedByNameAttribute))
                    {
                        modifiedOrUser = GetAttribute(newValues, ModifiedByNameAttribute);
                    }
                    else if (logData.ToString().Contains(UserNameAttribute))
                    {
                        modifiedOrUser = GetAttribute(newValues, UserNameAttribute);
                    }
                }
            }

            return string.IsNullOrEmpty(modifiedOrUser) ? SystemUser : modifiedOrUser;
        }

        private string GetAttribute(XmlNode newValues, string attribute)
        {
            if (newValues.Name.ToLower() == NewValuesTag && newValues.OuterXml.Contains(attribute))
            {
                var modifiedBy = newValues.Attributes.GetNamedItem(attribute);
                if (modifiedBy != null)
                    return modifiedBy.Value;
            }
            return null;
        }

        public string NoNeedToShowActivityType(object activityName)
        {
            return activityName.ToString().ToLower() == LoggedInActivity ? string.Empty : activityName.ToString();
        }
    }
}
