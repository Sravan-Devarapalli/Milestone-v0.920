using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls.Persons;
using PraticeManagement.Controls.TimeEntry;
using PraticeManagement.Controls;
using DataTransferObjects;
using PraticeManagement.Utils;
using System.Web.Security;
using DataTransferObjects.TimeEntry;
using PraticeManagement.Controls.Generic.TotalCalculator;
using System.Text;
using System.Data;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using PraticeManagement.Controls.Generic.DuplicateOptionsRemove;
using PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox;
using System.Drawing;
using AjaxControlToolkit;

namespace PraticeManagement
{
    public partial class TimeEntry_New : PracticeManagementPageBase, IPostBackEventHandler
    {

        #region Constants

        #region XMLConstants

        public const string AccountIdXname = "AccountId";
        public const string AccountNameXname = "AccountName";
        public const string ProjectIdXname = "ProjectId";
        public const string ProjectNumberXname = "ProjectNumber";
        public const string ProjectNameXname = "ProjectName";
        public const string BusinessUnitNameXname = "BusinessUnitName";
        public const string TimeEntrySectionIdXname = "TimeEntrySectionId";
        public const string WorkTypeXname = "WorkType";
        public const string BusinessUnitIdXname = "BusinessUnitId";
        public const string IsPTOXname = "IsPTO";
        public const string IsHolidayXname = "IsHoliday";
        public const string IdXname = "Id";
        public const string OldIdXname = "OldId";
        public const string CssClassXname = "CssClass";
        public const string IsNoteRequiredXname = "IsNoteRequired";
        public const string IsChargeCodeOffXname = "IsChargeCodeOff";
        public const string IsPTODisableXname = "IsPTODisable";
        public const string TimeEntryRecordXname = "TimeEntryRecord";
        public const string DateXname = "Date";
        public const string ActualHoursXname = "ActualHours";
        public const string NoteXname = "Note";
        public const string IsChargeableXname = "IsChargeable";
        public const string IsReviewedXname = "IsReviewed";
        public const string IsDirtyXname = "IsDirty";
        public const string EntryDateXname = "EntryDate";
        public const string IsHourlyRevenueXname = "IsHourlyRevenue";
        public const string IsRecursiveXname = "IsRecursive";
        public const string IsRecursiveAllowedXname = "IsRecursiveAllowed";
        public const string workTypeOldId = "workTypeOldId";
        public const string CalendarItemXname = "CalendarItem";
        public const string AccountAndProjectSelectionXname = "AccountAndProjectSelection";
        private const string SectionXname = "Section";
        private const string sectionsXmlOpen = "<Sections>";
        private const string sectionsXmlClose = "</Sections>";
        private const string projectSectionXmlOpen = "<Section Id=\"1\">";
        private const string projectSectionXmlClose = "</Section>";
        private const string busineeDevelopmentSectionXmlOpen = "<Section Id=\"2\">";
        private const string busineeDevelopmentSectionXmlClose = "</Section>";
        private const string internalSectionXmlOpen = "<Section Id=\"3\">";
        private const string internalSectionXmlClose = "</Section>";
        private const string administrativeSectionXmlOpen = "<Section Id=\"4\">";
        private const string administrativeSectionXmlClose = "</Section>";
        private const string accountAndProjectSelectionXmlOpen = "<AccountAndProjectSelection AccountId=\"{0}\" AccountName=\"{1}\" ProjectId=\"{2}\" ProjectName=\"{3}\" ProjectNumber=\"{4}\" BusinessUnitId=\"{5}\" BusinessUnitName=\"{6}\" IsRecursive=\"{7}\"  IsPTO=\"{8}\"  IsHoliday=\"{9}\" IsRecursiveAllowed=\"{10}\" >";
        private const string accountAndProjectSelectionXmlClose = "</AccountAndProjectSelection>";
        private const string workTypeXmlOpen = "<WorkType Id=\"{0}\" >";
        private const string workTypeXmlOpenWithOldId = "<WorkType Id=\"{0}\"  OldId=\"{1}\" >";
        private const string workTypeXmlClose = "</WorkType>";
        private const string calendarItemXmlOpen = "<CalendarItem Date=\"{0}\" CssClass=\"{1}\" IsNoteRequired=\"{2}\"  IsHourlyRevenue=\"{3}\" IsChargeCodeOff=\"{4}\" >";
        private const string calendarItemXmlClose = "</CalendarItem>";
        private const string billableXmlOpen = "<TimeEntryRecord  ActualHours=\"{0}\" Note=\"{1}\" IsChargeable=\"{2}\" EntryDate=\"{3}\" IsReviewed=\"{4}\" IsDirty=\"{5}\" >";
        private const string billableXmlClose = "</TimeEntryRecord>";
        private const string nonBillableXmlOpen = "<TimeEntryRecord  ActualHours=\"{0}\" Note=\"{1}\" IsChargeable=\"{2}\" EntryDate=\"{3}\"  IsReviewed=\"{4}\" IsDirty=\"{5}\" >";
        private const string NonBillableXmlClose = "</TimeEntryRecord>";

        #endregion

        #region ControlIds

        private const string teBarId = "bar";
        private const string extDayTotalId = "extDayTotal";
        private const string repProjectTesRepeater = "repProjectTes";
        private const string repProjectTesHeaderRepeater = "repProjectTesHeader";
        private const string repBusinessDevelopmentTesRepeater = "repBusinessDevelopmentTes";
        private const string repBusinessDevelopmentHeaderRepeater = "repBusinessDevelopmentHeader";
        private const string repInternalTesRepeater = "repInternalTes";
        private const string repInternalTesHeaderRepeater = "repInternalTesHeader";
        private const string repAdministrativeTesHeaderRepeater = "repAdministrativeTesHeader";
        private const string imgPlusProjectSectionImage = "imgPlusProjectSection";
        private const string imgPlusBusinessDevelopmentSectionImage = "imgPlusBusinessDevelopmentSection";
        private const string imgPlusAdministrativeSectionImage = "imgPlusAdministrativeSection";
        private const string imgPlusInternalSectionImage = "imgPlusInternalSection";
        private const string extDupilcateOptionsRemoveExtender = "extDupilcateOptionsRemoveExtender";
        private const string imgDropTesImage = "imgDropTes";
        private const string imgBtnRecursiveProjectSectionImage = "imgBtnRecursiveProjectSection";
        private const string imgBtnRecurrenceBusinessDevelopmentSectionImage = "imgBtnRecurrenceBusinessDevelopmentSection";
        private const string imgBtnRecurrenceInternalSectionImage = "imgBtnRecurrenceInternalSection";
        private const string imgBtnDeleteProjectSectionImage = "imgBtnDeleteProjectSection";
        private const string imgBtnDeleteBusinessDevelopmentSectionImage = "imgBtnDeleteBusinessDevelopmentSection";
        private const string imgBtnDeleteInternalSectionImage = "imgBtnDeleteInternalSection";
        private const string cbeImgBtnRecursiveProjectSectionExtender = "cbeImgBtnRecursiveProjectSection";
        private const string cbeImgBtnRecurrenceBusinessDevelopmentSectionExtender = "cbeImgBtnRecurrenceBusinessDevelopmentSection";
        private const string cbeImgBtnRecurrenceInternalSectionExtender = "cbeImgBtnRecurrenceInternalSection";
        private const string lblDupilcateOptionsRemoveExtender = "lblDupilcateOptionsRemoveExtender";
        public const string lblDayTotalId = "lblDayTotal";
        public const string hdnDayTotalId = "hdnDayTotal";
        public const string mpeTimetypeAlertMessageBehaviourId = "mpeTimetypeAlertMessage";
        public const string imgPlusBusinessDevelopmentSectionId = "imgPlusBusinessDevelopmentSection";
        public const string imgPlusInternalSectionId = "imgPlusInternalSection";
        public const string extMaxValueAllowedForTextBoxExtenderId = "extMaxValueAllowedForTextBoxExtender";

        #endregion

        #region  Attributes

        private const string addAttribute = "add";
        private const string recursiveSectionImageUrl = "~/Images/Recursive.png";
        private const string nonRecursiveSectionImageUrl = "~/Images/Not_Recursive.png";
        private const string recursiveSectionConfirmTextFormat = "Continuing will disable the recurring functionality for this {0}. This {0} will cease to automatically appear in future time periods.  Are you sure you want to disable this functionality?";
        private const string nonRecursiveSectionConfirmTextFormat = "Continuing will enable the recurring functionality for this {0}.  This {0} will continue to automatically appear in future time periods until either you disable the functionality, or until the {1}.  Are you sure you want to enable this functionality?";
        private const string recursiveToolTip = "Disable Recurring Behavior";
        private const string nonRecursiveToolTip = "Enable Recurring Behavior";
        private const string deleteSectionToolTipFormat = "Remove {0}";
        private const string parentDropDownClientIdAttribute = "parentDropDownClientId";
        private const string childDropDownClientIdAttribute = "childDropDownClientId";
        private const string btnAddAttribute = "btnAdd";
        public const string selectedInActiveWorktypeName = "selectedInActiveWorktypeName";
        public const string selectedInActiveWorktypeid = "selectedInActiveWorktypeid";
        public const string personIdAttribute = "personId";
        public const string startDateAttribute = "startDate";
        public const string endDateAttribute = "endDate";
        public const string DayTotalAttribute = "DayTotal";
        public const string RowsCountAttribute = "rowsCount";
        public const string PlusToolTipFormat = "Add additional Work Type to {0}";

        #endregion

        #region text

        private const string AlertTextFormat = " :  Notes are {0} for time entered.";
        private const string SavedAllConfirmation = "Time Entries saved sucessfully.";

        #endregion

        #region ViewStateKey

        private const string projectSectionXmlKey = "ProjectSection_Key";
        private const string businessDevelopmentSectionXmlKey = "BusinessDevelopmentSection_Key";
        private const string internalSectionXmlKey = "InternalSection_Key";
        private const string administrativeSectionXmlKey = "AdminiStrativeSection_Key";
        private const string billableControlIdsKey = "billableControlIds";
        private const string nonBillableControlIdsKey = "nonBillableControlIds";

        #endregion

        #endregion

        #region variables

        private string AccountId = "";
        private string ProjectId = "";
        private string BusinessUnitId = "";
        private string ddlWorkTypeIdsList = "";

        #endregion

        #region properties

        public bool IsSaving { get; set; }

        public bool IsValidNote { get; set; }

        public bool IsValidHours { get; set; }

        public bool IsValidPTOHours { get; set; }

        public bool IsValidWorkType { get; set; }

        public bool IsValidDayTotal { get; set; }

        public bool IsWeekOrPersonChanged
        {
            set
            {
                hdIsWeekOrPersonChanged.Value = value.ToString();
            }
        }

        public Dictionary<DateTime, bool> IsNoteRequiredList { get; set; }

        public Dictionary<DateTime, bool> IsHourlyRevenueList { get; set; }

        public TimeTypeRecord[] AdministrativeTimeTypes { get; set; }

        public Person SelectedPerson
        {
            get
            {
                return pcPersons.SelectedPerson;
            }
        }

        public DateTime[] SelectedDates
        {
            get
            {
                return wsChoose.SelectedDates;
            }
        }

        public CalendarItem[] CalendarItems
        {
            get;
            set;
        }

        public string ProjectSectionXml
        {
            get { return (string)ViewState[projectSectionXmlKey]; }
            set { ViewState[projectSectionXmlKey] = value; }
        }

        public string BusinessDevelopmentSectionXml
        {
            get { return (string)ViewState[businessDevelopmentSectionXmlKey]; }
            set { ViewState[businessDevelopmentSectionXmlKey] = value; }
        }

        public string InternalSectionXml
        {
            get { return (string)ViewState[internalSectionXmlKey]; }
            set { ViewState[internalSectionXmlKey] = value; }
        }

        public string AdministrativeSectionXml
        {
            get { return (string)ViewState[administrativeSectionXmlKey]; }
            set { ViewState[administrativeSectionXmlKey] = value; }
        }

        public string DdlWorkTypeIdsList
        {
            get
            {
                return ddlWorkTypeIdsList;
            }
            set
            {
                ddlWorkTypeIdsList = value;
            }
        }

        public string SpreadSheetTotalCalculatorExtenderId { get; set; }

        public List<TimeEntrySection> ProjectSection
        {
            get;
            set;
        }

        public List<TimeEntrySection> BusinessDevelopmentSection
        {
            get;
            set;
        }

        public List<TimeEntrySection> InternalSection
        {
            get;
            set;
        }

        public List<TimeEntrySection> AdminiStrativeSection
        {
            get;
            set;
        }

        private string BillableControlIds
        {
            get
            {
                return ViewState[billableControlIdsKey] as string;
            }
            set
            {
                ViewState[billableControlIdsKey] = value;
            }
        }

        private string NonBillableControlIds
        {
            get
            {
                return ViewState[nonBillableControlIdsKey] as string;
            }
            set
            {
                ViewState[nonBillableControlIdsKey] = value;
            }
        }

        #endregion

        #region Control events

        #region Page lifecyle methods

        protected void Page_Load(object sender, EventArgs e)
        {

            btnAddProjectSection.Attributes["onclick"] = "ExpandPanel('" + cpeProjectSection.BehaviorID + "');";
            btnAddInternalProjectSection.Attributes["onclick"] = "ExpandPanel('" + cpeInternalSection.BehaviorID + "');";
            btnAddBusinessDevelopmentSection.Attributes["onclick"] = "ExpandPanel('" + cpeBusinessDevelopmentSection.BehaviorID + "');";

            if (!IsPostBack)
            {
                var clients = ServiceCallers.Custom.Client(c => c.ClientListAllWithoutPermissions());
                DataHelper.FillListDefault(ddlAccountProjectSection, "- - Select Account - -", clients, false);
                DataHelper.FillListDefault(ddlAccountBusinessDevlopmentSection, "- - Select Account - -", clients, false);
                var groups = ServiceCallers.Custom.Group(client => client.GetInternalBusinessUnits());
                DataHelper.FillListDefault(ddlBusinessUnitInternal, "- - Select Business Unit - -", groups, false);

                AddAttributes();
            }
            mlConfirmation.ClearMessage();
            SpreadSheetTotalCalculatorExtenderId = extTotalHours.ClientID + ";" + extNonBillableGrandTotal.ClientID + ";" + extBillableGrandTotal.ClientID;
        }

        protected void Page_PreRender(object sender, EventArgs eventArgs)
        {
            extTotalHours.ControlsToCheck = BillableControlIds + ";" + NonBillableControlIds;
            extBillableGrandTotal.ControlsToCheck = BillableControlIds;
            extNonBillableGrandTotal.ControlsToCheck = NonBillableControlIds;

            if (!IsPostBack)
            {
                cpeProjectSection.Collapsed = !(repProjectSections.Items.Count > 0);
                cpeBusinessDevelopmentSection.Collapsed = !(repBusinessDevelopmentSections.Items.Count > 0);
                cpeInternalSection.Collapsed = !(repInternalSections.Items.Count > 0);
                var xdoc = XDocument.Parse(AdministrativeSectionXml);
                cpeAdministrative.Collapsed = !(xdoc.Descendants(XName.Get(TimeEntryRecordXname)).ToList().Count > 0);
                repTotalHoursHeader.DataSource = SelectedDates;
                repTotalHoursHeader.DataBind();
            }

            ddlAccountProjectSection.Attributes[personIdAttribute] =
            ddlAccountBusinessDevlopmentSection.Attributes[personIdAttribute] =
            ddlBusinessUnitInternal.Attributes[personIdAttribute] = SelectedPerson.Id.ToString();

            ddlAccountProjectSection.Attributes[startDateAttribute] =
            ddlAccountBusinessDevlopmentSection.Attributes[startDateAttribute] =
            ddlBusinessUnitInternal.Attributes[startDateAttribute] = SelectedDates[0].ToString();

            ddlAccountProjectSection.Attributes[endDateAttribute] =
            ddlAccountBusinessDevlopmentSection.Attributes[endDateAttribute] =
            ddlBusinessUnitInternal.Attributes[endDateAttribute] = SelectedDates[SelectedDates.Length - 1].ToString();

            lbProjectSection.Attributes[RowsCountAttribute] = repProjectSections.Items.Count.ToString();
            lbBusinessDevelopmentSection.Attributes[RowsCountAttribute] = repBusinessDevelopmentSections.Items.Count.ToString();
            lbInternalSection.Attributes[RowsCountAttribute] = repInternalSections.Items.Count.ToString();
            var administrativeXdoc = XDocument.Parse(AdministrativeSectionXml);
            int administrativeTECount = administrativeXdoc.Descendants(XName.Get(TimeEntryRecordXname)).ToList().Count ;
            lbAdministrativeSection.Attributes[RowsCountAttribute] = administrativeTECount.ToString();
        }

        #endregion

        #region  Validations

        protected void custWorkType_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = IsValidWorkType;
        }

        protected void cvDayTotal_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = IsValidDayTotal;
        }

        protected void custActualHours_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = IsValidHours;
        }

        protected void custPTOHours_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = IsValidPTOHours;
        }

        protected void custNote_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = IsValidNote;
        }

        #endregion

        protected void repProjectSections_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repProjectTes = e.Item.FindControl(repProjectTesRepeater) as Repeater;
                var teSectionDataItem = e.Item.DataItem as XElement;
                var isRecursive = teSectionDataItem.Attribute(XName.Get(IsRecursiveXname)).Value;
                var isRecursiveAllowed = teSectionDataItem.Attribute(XName.Get(IsRecursiveAllowedXname)).Value;

                AccountId = teSectionDataItem.Attribute(XName.Get(AccountIdXname)).Value;
                ProjectId = teSectionDataItem.Attribute(XName.Get(ProjectIdXname)).Value;
                BusinessUnitId = teSectionDataItem.Attribute(XName.Get(BusinessUnitIdXname)).Value;

                DdlWorkTypeIdsList = string.Empty;
                repProjectTes.DataSource = teSectionDataItem.Descendants(XName.Get(WorkTypeXname)).ToList();
                repProjectTes.DataBind();

                var extOptionRemove = e.Item.FindControl(extDupilcateOptionsRemoveExtender) as DupilcateOptionsRemoveExtender;

                extOptionRemove.ControlsToCheck = DdlWorkTypeIdsList;
                var imgPlusProjectSection = FindControlInFooter(repProjectTes, "imgPlusProjectSection");
                extOptionRemove.PlusButtonClientID = imgPlusProjectSection != null ? imgPlusProjectSection.ClientID : String.Empty;

                var imgBtnRecursiveProjectSection = e.Item.FindControl(imgBtnRecursiveProjectSectionImage) as ImageButton;
                var cbeImgBtnRecursiveProjectSection = e.Item.FindControl(cbeImgBtnRecursiveProjectSectionExtender) as ConfirmButtonExtender;
                imgBtnRecursiveProjectSection.ImageUrl = Convert.ToBoolean(isRecursive) ? recursiveSectionImageUrl : nonRecursiveSectionImageUrl;
                imgBtnRecursiveProjectSection.ToolTip = Convert.ToBoolean(isRecursive) ? recursiveToolTip : nonRecursiveToolTip;
                cbeImgBtnRecursiveProjectSection.ConfirmText = string.Format(Convert.ToBoolean(isRecursive) ? recursiveSectionConfirmTextFormat : nonRecursiveSectionConfirmTextFormat, "project", "project end");
                if (isRecursiveAllowed == false.ToString())
                {
                    cbeImgBtnRecursiveProjectSection.Enabled = false;
                    imgBtnRecursiveProjectSection.Attributes["onclick"] = "return IsrecusiveAllowed(this);";
                    imgBtnRecursiveProjectSection.Attributes[IsRecursiveAllowedXname] = isRecursiveAllowed;
                }
                imgBtnRecursiveProjectSection.Attributes[AccountIdXname] = AccountId;
                imgBtnRecursiveProjectSection.Attributes[ProjectIdXname] = ProjectId;
                imgBtnRecursiveProjectSection.Attributes[BusinessUnitIdXname] = BusinessUnitId;
                imgBtnRecursiveProjectSection.Attributes[IsRecursiveXname] = isRecursive;
                imgBtnRecursiveProjectSection.Attributes[TimeEntrySectionIdXname] = ((int)TimeEntrySectionType.Project).ToString();

                var imgBtnDeleteProjectSection = e.Item.FindControl(imgBtnDeleteProjectSectionImage) as ImageButton;
                imgBtnDeleteProjectSection.Attributes[TimeEntrySectionIdXname] = ((int)TimeEntrySectionType.Project).ToString();
                imgBtnDeleteProjectSection.ToolTip = String.Format(deleteSectionToolTipFormat, "Project");
;
            }
        }

        protected void repBusinessDevelopmentSections_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repBusinessDevelopmentTes = e.Item.FindControl(repBusinessDevelopmentTesRepeater) as Repeater;
                var teSectionDataItem = e.Item.DataItem as XElement;

                AccountId = teSectionDataItem.Attribute(XName.Get(AccountIdXname)).Value;
                ProjectId = teSectionDataItem.Attribute(XName.Get(ProjectIdXname)).Value;
                BusinessUnitId = teSectionDataItem.Attribute(XName.Get(BusinessUnitIdXname)).Value;
                var isRecursive = teSectionDataItem.Attribute(XName.Get(IsRecursiveXname)).Value;

                DdlWorkTypeIdsList = string.Empty;
                repBusinessDevelopmentTes.DataSource = teSectionDataItem.Descendants(XName.Get(WorkTypeXname)).ToList();
                repBusinessDevelopmentTes.DataBind();

                var extOptionRemove = e.Item.FindControl(extDupilcateOptionsRemoveExtender) as DupilcateOptionsRemoveExtender;

                extOptionRemove.ControlsToCheck = DdlWorkTypeIdsList;
                var imgPlusBusinessDevelopmentSection = FindControlInFooter(repBusinessDevelopmentTes, imgPlusBusinessDevelopmentSectionId);
                extOptionRemove.PlusButtonClientID = imgPlusBusinessDevelopmentSection != null ? imgPlusBusinessDevelopmentSection.ClientID : String.Empty;

                var imgBtnRecurrenceBusinessDevelopmentSection = e.Item.FindControl(imgBtnRecurrenceBusinessDevelopmentSectionImage) as ImageButton;
                var cbeImgBtnRecurrenceBusinessDevelopmentSection = e.Item.FindControl(cbeImgBtnRecurrenceBusinessDevelopmentSectionExtender) as ConfirmButtonExtender;
                imgBtnRecurrenceBusinessDevelopmentSection.ToolTip = Convert.ToBoolean(isRecursive) ? recursiveToolTip : nonRecursiveToolTip;
                imgBtnRecurrenceBusinessDevelopmentSection.ImageUrl = Convert.ToBoolean(isRecursive) ? recursiveSectionImageUrl : nonRecursiveSectionImageUrl;
                cbeImgBtnRecurrenceBusinessDevelopmentSection.ConfirmText = string.Format(Convert.ToBoolean(isRecursive) ? recursiveSectionConfirmTextFormat : nonRecursiveSectionConfirmTextFormat, "account","account is disabled");
                imgBtnRecurrenceBusinessDevelopmentSection.Attributes[AccountIdXname] = AccountId;
                imgBtnRecurrenceBusinessDevelopmentSection.Attributes[ProjectIdXname] = ProjectId;
                imgBtnRecurrenceBusinessDevelopmentSection.Attributes[BusinessUnitIdXname] = BusinessUnitId;
                imgBtnRecurrenceBusinessDevelopmentSection.Attributes[IsRecursiveXname] = isRecursive;
                imgBtnRecurrenceBusinessDevelopmentSection.Attributes[TimeEntrySectionIdXname] = ((int)TimeEntrySectionType.BusinessDevelopment).ToString();

                var imgBtnDeleteBusinessDevelopmentSection = e.Item.FindControl(imgBtnDeleteBusinessDevelopmentSectionImage) as ImageButton;
                imgBtnDeleteBusinessDevelopmentSection.Attributes[TimeEntrySectionIdXname] = ((int)TimeEntrySectionType.BusinessDevelopment).ToString();
                imgBtnDeleteBusinessDevelopmentSection.ToolTip = String.Format(deleteSectionToolTipFormat,"Account");
                
            }
        }

        protected void repInternalSections_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repInternalTes = e.Item.FindControl(repInternalTesRepeater) as Repeater;
                var teSectionDataItem = e.Item.DataItem as XElement;
                var isRecursive = teSectionDataItem.Attribute(XName.Get(IsRecursiveXname)).Value;

                AccountId = teSectionDataItem.Attribute(XName.Get(AccountIdXname)).Value;
                ProjectId = teSectionDataItem.Attribute(XName.Get(ProjectIdXname)).Value;
                BusinessUnitId = teSectionDataItem.Attribute(XName.Get(BusinessUnitIdXname)).Value;


                DdlWorkTypeIdsList = string.Empty;
                repInternalTes.DataSource = teSectionDataItem.Descendants(XName.Get(WorkTypeXname)).ToList();
                repInternalTes.DataBind();

                var extOptionRemove = e.Item.FindControl(extDupilcateOptionsRemoveExtender) as DupilcateOptionsRemoveExtender;

                extOptionRemove.ControlsToCheck = DdlWorkTypeIdsList;
                var imgPlusInternalSection = FindControlInFooter(repInternalTes, imgPlusInternalSectionId);
                extOptionRemove.PlusButtonClientID = imgPlusInternalSection != null ? imgPlusInternalSection.ClientID : String.Empty;

                var imgBtnRecurrenceInternalSection = e.Item.FindControl(imgBtnRecurrenceInternalSectionImage) as ImageButton;
                var cbeImgBtnRecurrenceInternalSection = e.Item.FindControl(cbeImgBtnRecurrenceInternalSectionExtender) as ConfirmButtonExtender;
                imgBtnRecurrenceInternalSection.ImageUrl = Convert.ToBoolean(isRecursive) ? recursiveSectionImageUrl : nonRecursiveSectionImageUrl;
                imgBtnRecurrenceInternalSection.ToolTip = Convert.ToBoolean(isRecursive) ? recursiveToolTip : nonRecursiveToolTip;
                cbeImgBtnRecurrenceInternalSection.ConfirmText = string.Format(Convert.ToBoolean(isRecursive) ? recursiveSectionConfirmTextFormat : nonRecursiveSectionConfirmTextFormat, "project", "project end");

                imgBtnRecurrenceInternalSection.Attributes[AccountIdXname] = AccountId;
                imgBtnRecurrenceInternalSection.Attributes[ProjectIdXname] = ProjectId;
                imgBtnRecurrenceInternalSection.Attributes[BusinessUnitIdXname] = BusinessUnitId;
                imgBtnRecurrenceInternalSection.Attributes[IsRecursiveXname] = isRecursive;
                imgBtnRecurrenceInternalSection.Attributes[TimeEntrySectionIdXname] = ((int)TimeEntrySectionType.Internal).ToString();

                var imgBtnDeleteInternalSection = e.Item.FindControl(imgBtnDeleteInternalSectionImage) as ImageButton;
                imgBtnDeleteInternalSection.Attributes[TimeEntrySectionIdXname] = ((int)TimeEntrySectionType.Internal).ToString();
                imgBtnDeleteInternalSection.ToolTip = String.Format(deleteSectionToolTipFormat, "Project");
            }
        }

        protected void repProjectTes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Header)
            {
                var repProjectTesHeader = e.Item.FindControl(repProjectTesHeaderRepeater) as Repeater;
                repProjectTesHeader.DataSource = SelectedDates;
                repProjectTesHeader.DataBind();
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var workTypeElement = ((XElement)e.Item.DataItem);

                var bar = e.Item.FindControl(teBarId) as BillableAndNonBillableTimeEntryBar;
                bar.SelectedWorkType = new TimeTypeRecord()
                {
                    Id = Convert.ToInt32(workTypeElement.Attribute(XName.Get(IdXname)).Value)
                };

                bar.WorkTypes = ServiceCallers.Custom.Project(p => p.GetTimeTypesByProjectId(Convert.ToInt32(ProjectId), true, SelectedDates[0], SelectedDates[SelectedDates.Length - 1]));
                bar.TeBarDataSource = workTypeElement.Descendants(XName.Get(CalendarItemXname)).ToList();
                bar.AccountId = AccountId;
                bar.BusinessUnitId = BusinessUnitId;
                bar.ProjectId = ProjectId;

                var imgDropTes = bar.FindControl(imgDropTesImage) as ImageButton;
                imgDropTes.Attributes[AccountIdXname] = AccountId;
                imgDropTes.Attributes[ProjectIdXname] = ProjectId;
                string OldId = workTypeElement.Attribute(XName.Get(OldIdXname)) != null ? workTypeElement.Attribute(XName.Get(OldIdXname)).Value : null;
                if (!String.IsNullOrEmpty(OldId))
                    imgDropTes.Attributes[workTypeOldId] = OldId;
                imgDropTes.Attributes[WorkTypeXname] = workTypeElement.Attribute(XName.Get(IdXname)).Value;

                bar.UpdateTimeEntries();
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                var imgPlusProjectSection = e.Item.FindControl(imgPlusProjectSectionImage) as ImageButton;
                imgPlusProjectSection.Attributes[AccountIdXname] = AccountId;
                imgPlusProjectSection.Attributes[ProjectIdXname] = ProjectId;
                imgPlusProjectSection.ToolTip = string.Format(PlusToolTipFormat, "Project");
            }
        }

        protected void repBusinessDevelopmentTes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var repProjectTesHeader = e.Item.FindControl(repBusinessDevelopmentHeaderRepeater) as Repeater;
                repProjectTesHeader.DataSource = SelectedDates;
                repProjectTesHeader.DataBind();
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                var workTypeElement = ((XElement)e.Item.DataItem);

                var bar = e.Item.FindControl(teBarId) as NonBillableTimeEntryBar;
                bar.SelectedTimeType = new TimeTypeRecord()
                {
                    Id = Convert.ToInt32(workTypeElement.Attribute(XName.Get(IdXname)).Value)
                };

                bar.TimeTypes = ServiceCallers.Custom.Project(p => p.GetTimeTypesByProjectId(Convert.ToInt32(ProjectId), true, SelectedDates[0], SelectedDates[SelectedDates.Length - 1]));
                bar.TeBarDataSource = workTypeElement.Descendants(XName.Get(CalendarItemXname)).ToList();
                bar.AccountId = AccountId;
                bar.BusinessUnitId = BusinessUnitId;
                bar.ProjectId = ProjectId;

                var imgDropTes = bar.FindControl(imgDropTesImage) as ImageButton;
                imgDropTes.Attributes[AccountIdXname] = AccountId;
                imgDropTes.Attributes[BusinessUnitIdXname] = BusinessUnitId;
                imgDropTes.Attributes[ProjectIdXname] = ProjectId;
                string OldId = workTypeElement.Attribute(XName.Get(OldIdXname)) != null ? workTypeElement.Attribute(XName.Get(OldIdXname)).Value : null;
                if (!String.IsNullOrEmpty(OldId))
                    imgDropTes.Attributes[workTypeOldId] = OldId;
                imgDropTes.Attributes[WorkTypeXname] = workTypeElement.Attribute(XName.Get(IdXname)).Value;

                bar.UpdateTimeEntries();

            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                var imgPlusBusinessDevelopmentSection = e.Item.FindControl(imgPlusBusinessDevelopmentSectionImage) as ImageButton;
                imgPlusBusinessDevelopmentSection.Attributes[AccountIdXname] = AccountId;
                imgPlusBusinessDevelopmentSection.Attributes[ProjectIdXname] = ProjectId;
                imgPlusBusinessDevelopmentSection.Attributes[BusinessUnitIdXname] = BusinessUnitId;
                imgPlusBusinessDevelopmentSection.ToolTip = string.Format(PlusToolTipFormat, "Account");
            }

        }

        protected void repInternalTes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var repProjectTesHeader = e.Item.FindControl(repInternalTesHeaderRepeater) as Repeater;
                repProjectTesHeader.DataSource = SelectedDates;
                repProjectTesHeader.DataBind();
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var workTypeElement = ((XElement)e.Item.DataItem);

                var bar = e.Item.FindControl(teBarId) as NonBillableTimeEntryBar;
                bar.SelectedTimeType = new TimeTypeRecord()
                {
                    Id = Convert.ToInt32(workTypeElement.Attribute(XName.Get(IdXname)).Value)
                };

                bar.TimeTypes = ServiceCallers.Custom.Project(p => p.GetTimeTypesByProjectId(Convert.ToInt32(ProjectId), true, SelectedDates[0], SelectedDates[SelectedDates.Length - 1]));
                bar.TeBarDataSource = workTypeElement.Descendants(XName.Get(CalendarItemXname)).ToList();
                bar.AccountId = AccountId;
                bar.BusinessUnitId = BusinessUnitId;
                bar.ProjectId = ProjectId;

                var imgDropTes = bar.FindControl(imgDropTesImage) as ImageButton;
                imgDropTes.Attributes[AccountIdXname] = AccountId;
                imgDropTes.Attributes[BusinessUnitIdXname] = BusinessUnitId;
                imgDropTes.Attributes[ProjectIdXname] = ProjectId;
                string OldId = workTypeElement.Attribute(XName.Get(OldIdXname)) != null ? workTypeElement.Attribute(XName.Get(OldIdXname)).Value : null;
                if (!String.IsNullOrEmpty(OldId))
                    imgDropTes.Attributes[workTypeOldId] = OldId;
                imgDropTes.Attributes[WorkTypeXname] = workTypeElement.Attribute(XName.Get(IdXname)).Value;

                bar.UpdateTimeEntries();

            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                var imgPlusInternalSection = e.Item.FindControl(imgPlusInternalSectionImage) as ImageButton;
                imgPlusInternalSection.Attributes[AccountIdXname] = AccountId;
                imgPlusInternalSection.Attributes[ProjectIdXname] = ProjectId;
                imgPlusInternalSection.Attributes[BusinessUnitIdXname] = BusinessUnitId;
                imgPlusInternalSection.ToolTip = string.Format(PlusToolTipFormat, "Project");
            }
        }

        protected void repAdministrativeTes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                DdlWorkTypeIdsList = string.Empty;
                var repProjectTesHeader = e.Item.FindControl(repAdministrativeTesHeaderRepeater) as Repeater;
                repProjectTesHeader.DataSource = SelectedDates;
                repProjectTesHeader.DataBind();
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                var teSectionDataItem = e.Item.DataItem as XElement;
                var bar = e.Item.FindControl(teBarId) as AdministrativeTimeEntryBar;
                var projectId = Convert.ToInt32(teSectionDataItem.Attribute(XName.Get(ProjectIdXname)).Value);
                bar.AccountId = teSectionDataItem.Attribute(XName.Get(AccountIdXname)).Value;
                bar.BusinessUnitId = teSectionDataItem.Attribute(XName.Get(BusinessUnitIdXname)).Value;
                bar.ProjectId = projectId.ToString();

                bar.IsPTO = Convert.ToBoolean(teSectionDataItem.Attribute(XName.Get(IsPTOXname)).Value);
                bar.IsHoliday = Convert.ToBoolean(teSectionDataItem.Attribute(XName.Get(IsHolidayXname)).Value);

                var workTypeElement = teSectionDataItem.Descendants(XName.Get(WorkTypeXname)).ToList()[0];

                AdministrativeTimeTypes = AdministrativeTimeTypes ?? ServiceCallers.Custom.TimeType(p => p.GetAllAdministrativeTimeTypes());

                if (bar.IsPTO || bar.IsHoliday)
                {
                    bar.SelectedTimeType = ServiceCallers.Custom.Project(p => p.GetTimeTypesByProjectId(Convert.ToInt32(projectId), true, SelectedDates[0], SelectedDates[SelectedDates.Length - 1]))[0];
                }
                else
                {
                    bar.SelectedTimeType = new TimeTypeRecord()
                    {
                        Id = Convert.ToInt32(workTypeElement.Attribute(XName.Get(IdXname)).Value)
                    };

                    var imgDropTes = bar.FindControl(imgDropTesImage) as ImageButton;
                    imgDropTes.Attributes[AccountIdXname] = teSectionDataItem.Attribute(XName.Get(AccountIdXname)).Value;
                    imgDropTes.Attributes[BusinessUnitIdXname] = teSectionDataItem.Attribute(XName.Get(BusinessUnitIdXname)).Value;
                    imgDropTes.Attributes[ProjectIdXname] = teSectionDataItem.Attribute(XName.Get(ProjectIdXname)).Value;
                    string OldId = workTypeElement.Attribute(XName.Get(OldIdXname)) != null ? workTypeElement.Attribute(XName.Get(OldIdXname)).Value : null;
                    if (!String.IsNullOrEmpty(OldId))
                        imgDropTes.Attributes[workTypeOldId] = OldId;
                    imgDropTes.Attributes[WorkTypeXname] = workTypeElement.Attribute(XName.Get(IdXname)).Value;
                }

                bar.TimeTypes = AdministrativeTimeTypes;
                bar.TeBarDataSource = teSectionDataItem.Descendants(XName.Get(CalendarItemXname)).ToList();
                bar.UpdateTimeEntries();
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                var imgPlus = e.Item.FindControl(imgPlusAdministrativeSectionImage) as ImageButton;
                if (!IsPostBack)
                {
                    AdministrativeTimeTypes = AdministrativeTimeTypes ?? ServiceCallers.Custom.TimeType(p => p.GetAllAdministrativeTimeTypes());

                    if (AdministrativeTimeTypes.Count() < 1)
                        imgPlus.Style["display"] = "none";
                }

                extDupilcateOptionsRemoveExtenderAdministrative.ControlsToCheck = DdlWorkTypeIdsList;
                extDupilcateOptionsRemoveExtenderAdministrative.PlusButtonClientID = imgPlus.ClientID;
            }
        }

        protected void repDayTotalHours_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                var index = e.Item.ItemIndex;
                var controlIDList = "";
                var extColumnDayTotalHours = (TotalCalculatorExtender)e.Item.FindControl(extDayTotalId);
                var extMaxValueAllowedForTextBoxExtender = (MaxValueAllowedForTextBoxExtender)e.Item.FindControl(extMaxValueAllowedForTextBoxExtenderId);

                var lblDayTotal = (Label)e.Item.FindControl(lblDayTotalId);
                var hdnDayTotal = (HiddenField)e.Item.FindControl(hdnDayTotalId);
                lblDayTotal.Attributes[DayTotalAttribute] = hdnDayTotal.ClientID;

                foreach (RepeaterItem barItem in repAdministrativeTes.Items)
                {
                    var bar = barItem.FindControl(teBarId) as AdministrativeTimeEntryBar;
                    controlIDList += bar.TbAcutualHoursClientIds[index] + ";";
                    NonBillableControlIds += bar.TbAcutualHoursClientIds[index] + ";";

                    bar.UpdateVerticalTotalCalculatorExtenderId(index, extColumnDayTotalHours.ClientID);
                    bar.AddAttributeToPTOTextBox(index);
                }


                foreach (RepeaterItem barItem in repProjectSections.Items)
                {
                    var repProjectTes = barItem.FindControl(repProjectTesRepeater) as Repeater;

                    foreach (RepeaterItem item in repProjectTes.Items)
                    {
                        var bar = item.FindControl(teBarId) as BillableAndNonBillableTimeEntryBar;
                        controlIDList += bar.BillableTbAcutualHoursClientIds[index] + ";" + bar.NonBillableTbAcutualHoursClientIds[index] + ";";
                        BillableControlIds += bar.BillableTbAcutualHoursClientIds[index] + ";";
                        NonBillableControlIds += bar.NonBillableTbAcutualHoursClientIds[index] + ";";
                        bar.UpdateVerticalTotalCalculatorExtenderId(index, extColumnDayTotalHours.ClientID);
                    }

                    var imgBtnDeleteProjectSection = barItem.FindControl(imgBtnDeleteProjectSectionImage) as ImageButton;
                    imgBtnDeleteProjectSection.Attributes["onclick"] = "return DeleteSection('" + cpeProjectSection.BehaviorID + "','" + repProjectSections.Items.Count.ToString() + "','" + imgBtnDeleteProjectSection.ClientID + "','1');";

                }

                foreach (RepeaterItem barItem in repBusinessDevelopmentSections.Items)
                {
                    var repBusinessDevelopmentTes = barItem.FindControl(repBusinessDevelopmentTesRepeater) as Repeater;

                    foreach (RepeaterItem item in repBusinessDevelopmentTes.Items)
                    {
                        var bar = item.FindControl(teBarId) as NonBillableTimeEntryBar;
                        controlIDList += bar.NonBillableTbAcutualHoursClientIds[index] + ";";
                        NonBillableControlIds += bar.NonBillableTbAcutualHoursClientIds[index] + ";";
                        bar.UpdateVerticalTotalCalculatorExtenderId(index, extColumnDayTotalHours.ClientID);
                    }

                    var imgBtnDeleteBusinessDevelopmentSection = barItem.FindControl(imgBtnDeleteBusinessDevelopmentSectionImage) as ImageButton;
                    imgBtnDeleteBusinessDevelopmentSection.Attributes["onclick"] = "return DeleteSection('" + cpeBusinessDevelopmentSection.BehaviorID + "','" + repBusinessDevelopmentSections.Items.Count.ToString() + "','" + imgBtnDeleteBusinessDevelopmentSection.ClientID + "','2');";

                }

                foreach (RepeaterItem barItem in repInternalSections.Items)
                {
                    var repInternalTes = barItem.FindControl(repInternalTesRepeater) as Repeater;

                    foreach (RepeaterItem item in repInternalTes.Items)
                    {
                        var bar = item.FindControl(teBarId) as NonBillableTimeEntryBar;
                        controlIDList += bar.NonBillableTbAcutualHoursClientIds[index] + ";";
                        NonBillableControlIds += bar.NonBillableTbAcutualHoursClientIds[index] + ";";
                        bar.UpdateVerticalTotalCalculatorExtenderId(index, extColumnDayTotalHours.ClientID);

                    }

                    var imgBtnDeleteInternalSection = barItem.FindControl(imgBtnDeleteInternalSectionImage) as ImageButton;
                    imgBtnDeleteInternalSection.Attributes["onclick"] = "return DeleteSection('" + cpeInternalSection.BehaviorID + "','" + repInternalSections.Items.Count.ToString() + "','" + imgBtnDeleteInternalSection.ClientID + "','3');";

                }

                extMaxValueAllowedForTextBoxExtender.ControlsToCheck = extColumnDayTotalHours.ControlsToCheck = controlIDList;
                extMaxValueAllowedForTextBoxExtender.MaximumValue = 24;
            }
        }

        protected void imgPlusInternalSection_OnClick(object sender, EventArgs e)
        {
            var imgPlusInternalSection = ((ImageButton)(sender));
            var projectId = imgPlusInternalSection.Attributes[ProjectIdXname];
            var accountId = imgPlusInternalSection.Attributes[AccountIdXname];
            var businessUnitId = imgPlusInternalSection.Attributes[BusinessUnitIdXname];

            var xdoc = PrePareXmlForInternalSectionFromRepeater();

            List<XElement> xlist = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();

            var xelem = xlist.First(element => element.Attribute(XName.Get(BusinessUnitIdXname)).Value == businessUnitId && element.Attribute(XName.Get(AccountIdXname)).Value == accountId && element.Attribute(XName.Get(ProjectIdXname)).Value == projectId);

            int ttypeId = xelem.Descendants(XName.Get(WorkTypeXname)).ToList().Min(k => Convert.ToInt32(k.Attribute(XName.Get(IdXname)).Value));

            StringBuilder xml = new StringBuilder();
            xml.Append(string.Format(workTypeXmlOpen, ttypeId < 1 ? ttypeId - 1 : -1));

            CalendarItems = CalendarItems ??
                               ServiceCallers.Custom.Calendar(
                                                                c => c.GetCalendar(wsChoose.SelectedDates[0], wsChoose.SelectedDates[wsChoose.SelectedDates.Length - 1], pcPersons.SelectedPerson.Id.Value, null)
                                                              );


            IsNoteRequiredList = IsNoteRequiredList ??
                              DataHelper.GetIsNoteRequiredDetailsForSelectedDateRange(wsChoose.SelectedStartDate, wsChoose.SelectedEndDate, pcPersons.SelectedPersonId);
            ;

            int _projectId = Convert.ToInt32(projectId);
            int personId = SelectedPerson.Id.Value;
            DateTime startDate = SelectedDates[0];
            DateTime endDate = SelectedDates[SelectedDates.Length - 1];
            IsHourlyRevenueList = ServiceCallers.Custom.Project(p => p.GetIsHourlyRevenueByPeriod(_projectId, personId, startDate, endDate));


            foreach (CalendarItem day in CalendarItems)
            {
                var cssClass = Utils.Calendar.GetCssClassByCalendarItem(day);
                xml.Append(string.Format(calendarItemXmlOpen, day.Date, cssClass, IsNoteRequiredList[day.Date], IsHourlyRevenueList[day.Date], false));
                xml.Append(calendarItemXmlClose);
            }
            xml.Append(workTypeXmlClose);

            var xlement = XElement.Parse(xml.ToString());

            xelem.Add(xlement);

            InternalSectionXml = xdoc.ToString();

            DatabindRepeater(repInternalSections, xlist);
            IsDirty = true;
        }

        protected void imgPlusProjectSection_OnClick(object sender, EventArgs e)
        {
            var imgPlusProjectSection = ((ImageButton)(sender));
            var projectId = imgPlusProjectSection.Attributes[ProjectIdXname];
            var accountId = imgPlusProjectSection.Attributes[AccountIdXname];


            var xdoc = PrePareXmlForProjectSectionFromRepeater();


            List<XElement> xlist = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();


            var xelem = xlist.First(element => element.Attribute(XName.Get(AccountIdXname)).Value == accountId && element.Attribute(XName.Get(ProjectIdXname)).Value == projectId);

            int ttypeId = xelem.Descendants(XName.Get(WorkTypeXname)).ToList().Min(k => Convert.ToInt32(k.Attribute(XName.Get(IdXname)).Value));

            StringBuilder xml = new StringBuilder();
            xml.Append(string.Format(workTypeXmlOpen, ttypeId < 1 ? ttypeId - 1 : -1));

            CalendarItems = CalendarItems ??
                               ServiceCallers.Custom.Calendar(
                                                                c => c.GetCalendar(wsChoose.SelectedDates[0], wsChoose.SelectedDates[wsChoose.SelectedDates.Length - 1], pcPersons.SelectedPerson.Id.Value, null)
                                                              );

            IsNoteRequiredList = IsNoteRequiredList ??
                                 DataHelper.GetIsNoteRequiredDetailsForSelectedDateRange(wsChoose.SelectedStartDate, wsChoose.SelectedEndDate, pcPersons.SelectedPersonId);
            ;

            int _projectId = Convert.ToInt32(projectId);
            int personId = SelectedPerson.Id.Value;
            DateTime startDate = SelectedDates[0];
            DateTime endDate = SelectedDates[SelectedDates.Length - 1];
            IsHourlyRevenueList = ServiceCallers.Custom.Project(p => p.GetIsHourlyRevenueByPeriod(_projectId, personId, startDate, endDate));


            foreach (CalendarItem day in CalendarItems)
            {
                var cssClass = Utils.Calendar.GetCssClassByCalendarItem(day);
                xml.Append(string.Format(calendarItemXmlOpen, day.Date, cssClass, IsNoteRequiredList[day.Date], IsHourlyRevenueList[day.Date], false));
                xml.Append(calendarItemXmlClose);
            }
            xml.Append(workTypeXmlClose);

            var xlement = XElement.Parse(xml.ToString());

            xelem.Add(xlement);

            ProjectSectionXml = xdoc.ToString();

            DatabindRepeater(repProjectSections, xlist);
            IsDirty = true;
        }

        protected void imgPlusBusinessDevelopmentSection_OnClick(object sender, EventArgs e)
        {
            var imgPlusBusinessDevelopmentSection = ((ImageButton)(sender));
            var projectId = imgPlusBusinessDevelopmentSection.Attributes[ProjectIdXname] == String.Empty ? ((int)486).ToString() : imgPlusBusinessDevelopmentSection.Attributes[ProjectIdXname];
            var accountId = imgPlusBusinessDevelopmentSection.Attributes[AccountIdXname];
            var businessUnitId = imgPlusBusinessDevelopmentSection.Attributes[BusinessUnitIdXname];


            var xdoc = PrePareXmlForBusinessDevelopmentSectionFromRepeater();

            List<XElement> xlist = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();

            var xelem = xlist.First(element => element.Attribute(XName.Get(BusinessUnitIdXname)).Value == businessUnitId && element.Attribute(XName.Get(AccountIdXname)).Value == accountId && element.Attribute(XName.Get(ProjectIdXname)).Value == projectId);

            int ttypeId = xelem.Descendants(XName.Get(WorkTypeXname)).ToList().Min(k => Convert.ToInt32(k.Attribute(XName.Get(IdXname)).Value));

            StringBuilder xml = new StringBuilder();
            xml.Append(string.Format(workTypeXmlOpen, ttypeId < 1 ? ttypeId - 1 : -1));

            CalendarItems = CalendarItems ??
                               ServiceCallers.Custom.Calendar(
                                                                c => c.GetCalendar(wsChoose.SelectedDates[0], wsChoose.SelectedDates[wsChoose.SelectedDates.Length - 1], pcPersons.SelectedPerson.Id.Value, null)
                                                              );
            IsNoteRequiredList = IsNoteRequiredList ??
                                DataHelper.GetIsNoteRequiredDetailsForSelectedDateRange(wsChoose.SelectedStartDate, wsChoose.SelectedEndDate, pcPersons.SelectedPersonId);
            ;

            int _projectId = Convert.ToInt32(projectId);
            int personId = SelectedPerson.Id.Value;
            DateTime startDate = SelectedDates[0];
            DateTime endDate = SelectedDates[SelectedDates.Length - 1];
            IsHourlyRevenueList = ServiceCallers.Custom.Project(p => p.GetIsHourlyRevenueByPeriod(_projectId, personId, startDate, endDate));


            foreach (CalendarItem day in CalendarItems)
            {
                var cssClass = Utils.Calendar.GetCssClassByCalendarItem(day);
                xml.Append(string.Format(calendarItemXmlOpen, day.Date, cssClass, IsNoteRequiredList[day.Date], IsHourlyRevenueList[day.Date], false));
                xml.Append(calendarItemXmlClose);
            }
            xml.Append(workTypeXmlClose);

            var xlement = XElement.Parse(xml.ToString());

            xelem.Add(xlement);

            BusinessDevelopmentSectionXml = xdoc.ToString();

            DatabindRepeater(repBusinessDevelopmentSections, xlist);

            IsDirty = true;
        }

        protected void imgPlusAdministrativeSection_OnClick(object sender, EventArgs e)
        {
            XDocument xdoc = PrePareXmlForAdminstrativeSectionFromRepeater();

            List<XElement> xelementsList = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();


            var teSection = new TimeEntrySection()
            {
                Project = new Project() { Id = -1 },
                Account = new Client() { Id = -1 },
                BusinessUnit = new ProjectGroup() { Id = -1 },
                SectionId = TimeEntrySectionType.Administrative,
                IsRecursive = false
            };

            StringBuilder xml = new StringBuilder();

            PrePareXmlForAccountProjectSelection(xml, teSection);

            xdoc.Descendants(XName.Get(SectionXname)).First().Add(XElement.Parse(xml.ToString()));

            DatabindRepeater(repAdministrativeTes, xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList());

            AdministrativeSectionXml = xdoc.ToString();

        }

        protected void imgBtnRecursiveSection_OnClick(object sender, EventArgs args)
        {
            var imgBtnRecursiveSection = ((ImageButton)(sender));
            var repeaterItem = imgBtnRecursiveSection.NamingContainer as RepeaterItem;
            ConfirmButtonExtender cbeimgBtnRecursiveSection = null;

            int projectId = Convert.ToInt32(imgBtnRecursiveSection.Attributes[ProjectIdXname]);
            int accountId = Convert.ToInt32(imgBtnRecursiveSection.Attributes[AccountIdXname]);
            int businessUnitId = Convert.ToInt32(imgBtnRecursiveSection.Attributes[BusinessUnitIdXname]);
            int timeEntrySectionId = Convert.ToInt32(imgBtnRecursiveSection.Attributes[TimeEntrySectionIdXname]);
            int personId = SelectedPerson.Id.Value;
            DateTime[] dates = SelectedDates;
            bool isRecursive = !Convert.ToBoolean(imgBtnRecursiveSection.Attributes[IsRecursiveXname]);
            imgBtnRecursiveSection.Attributes[IsRecursiveXname] = (isRecursive).ToString();
            imgBtnRecursiveSection.ImageUrl = isRecursive ? recursiveSectionImageUrl : nonRecursiveSectionImageUrl;
            imgBtnRecursiveSection.ToolTip = isRecursive ? recursiveToolTip : nonRecursiveToolTip;

            XDocument xdoc = null;
            if ((int)TimeEntrySectionType.Project == timeEntrySectionId)
            {
                xdoc = PrePareXmlForProjectSectionFromRepeater();
                cbeimgBtnRecursiveSection = repeaterItem.FindControl(cbeImgBtnRecursiveProjectSectionExtender) as ConfirmButtonExtender;
                cbeimgBtnRecursiveSection.ConfirmText = string.Format(Convert.ToBoolean(isRecursive) ? recursiveSectionConfirmTextFormat : nonRecursiveSectionConfirmTextFormat, "project", "project end");
            }
            else if ((int)TimeEntrySectionType.BusinessDevelopment == timeEntrySectionId)
            {
                xdoc = PrePareXmlForBusinessDevelopmentSectionFromRepeater();
                cbeimgBtnRecursiveSection = repeaterItem.FindControl(cbeImgBtnRecurrenceBusinessDevelopmentSectionExtender) as ConfirmButtonExtender;
                cbeimgBtnRecursiveSection.ConfirmText = string.Format(Convert.ToBoolean(isRecursive) ? recursiveSectionConfirmTextFormat : nonRecursiveSectionConfirmTextFormat, "account", "account is disabled");
            }
            else if ((int)TimeEntrySectionType.Internal == timeEntrySectionId)
            {
                xdoc = PrePareXmlForInternalSectionFromRepeater();
                cbeimgBtnRecursiveSection = repeaterItem.FindControl(cbeImgBtnRecurrenceInternalSectionExtender) as ConfirmButtonExtender;
                cbeimgBtnRecursiveSection.ConfirmText = string.Format(Convert.ToBoolean(isRecursive) ? recursiveSectionConfirmTextFormat : nonRecursiveSectionConfirmTextFormat, "project", "project end");
            }

            List<XElement> xlist = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();
            XElement accountAndProjectSelectionElement = xlist.First(element => element.Attribute(XName.Get(AccountIdXname)).Value == accountId.ToString() && element.Attribute(XName.Get(ProjectIdXname)).Value == projectId.ToString() && element.Attribute(XName.Get(BusinessUnitIdXname)).Value == businessUnitId.ToString());
            accountAndProjectSelectionElement.SetAttributeValue(IsRecursiveXname, (isRecursive).ToString());


            ServiceCallers.Custom.TimeEntry(t => t.SetPersonTimeEntryRecursiveSelection(personId, accountId, businessUnitId, projectId, timeEntrySectionId, isRecursive, dates[0]));

            if ((int)TimeEntrySectionType.Project == timeEntrySectionId)
            {
                ProjectSectionXml = xdoc.ToString();
            }
            else if ((int)TimeEntrySectionType.BusinessDevelopment == timeEntrySectionId)
            {
                BusinessDevelopmentSectionXml = xdoc.ToString();
            }
            else if ((int)TimeEntrySectionType.Internal == timeEntrySectionId)
            {
                InternalSectionXml = xdoc.ToString();
            }
        }

        protected void imgBtnDeleteSection_OnClick(object sender, EventArgs args)
        {
            var imgBtnDeleteSection = ((ImageButton)(sender));
            int timeEntrySectionId = Convert.ToInt32(imgBtnDeleteSection.Attributes[TimeEntrySectionIdXname]);
            ImageButton imgBtnRecursiveSection = null;

            XDocument xdoc = null;
            if ((int)TimeEntrySectionType.Project == timeEntrySectionId)
            {
                imgBtnRecursiveSection = imgBtnDeleteSection.NamingContainer.FindControl(imgBtnRecursiveProjectSectionImage) as ImageButton;
                xdoc = PrePareXmlForProjectSectionFromRepeater();
            }
            else if ((int)TimeEntrySectionType.BusinessDevelopment == timeEntrySectionId)
            {
                imgBtnRecursiveSection = imgBtnDeleteSection.NamingContainer.FindControl(imgBtnRecurrenceBusinessDevelopmentSectionImage) as ImageButton;
                xdoc = PrePareXmlForBusinessDevelopmentSectionFromRepeater();
            }
            else if ((int)TimeEntrySectionType.Internal == timeEntrySectionId)
            {
                imgBtnRecursiveSection = imgBtnDeleteSection.NamingContainer.FindControl(imgBtnRecurrenceInternalSectionImage) as ImageButton;
                xdoc = PrePareXmlForInternalSectionFromRepeater();
            }
            int projectId = Convert.ToInt32(imgBtnRecursiveSection.Attributes[ProjectIdXname]);
            int accountId = Convert.ToInt32(imgBtnRecursiveSection.Attributes[AccountIdXname]);
            int businessUnitId = Convert.ToInt32(imgBtnRecursiveSection.Attributes[BusinessUnitIdXname]);
            int personId = SelectedPerson.Id.Value;
            DateTime[] dates = SelectedDates;

            List<XElement> xlist = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();
            XElement accountAndProjectSelectionElement = xlist.First(element => element.Attribute(XName.Get(AccountIdXname)).Value == accountId.ToString() && element.Attribute(XName.Get(ProjectIdXname)).Value == projectId.ToString() && element.Attribute(XName.Get(BusinessUnitIdXname)).Value == businessUnitId.ToString());
            accountAndProjectSelectionElement.Remove();
            xlist.Remove(accountAndProjectSelectionElement);
            if ((int)TimeEntrySectionType.Project == timeEntrySectionId)
            {
                ProjectSectionXml = xdoc.ToString();
                DatabindRepeater(repProjectSections, xlist);
            }
            else if ((int)TimeEntrySectionType.BusinessDevelopment == timeEntrySectionId)
            {
                BusinessDevelopmentSectionXml = xdoc.ToString();
                DatabindRepeater(repBusinessDevelopmentSections, xlist);
            }
            else if ((int)TimeEntrySectionType.Internal == timeEntrySectionId)
            {
                InternalSectionXml = xdoc.ToString();
                DatabindRepeater(repInternalSections, xlist);
            }

            ServiceCallers.Custom.TimeEntry(t => t.SetPersonTimeEntrySelection(personId, accountId, businessUnitId, projectId, timeEntrySectionId, true, dates[0], dates[dates.Length - 1], Context.User.Identity.Name));

        }

        protected void btnSave_OnClick(object sender, EventArgs e)
        {
            SaveData(true);
        }

        protected void btnAddProjectSection_OnClick(object sender, EventArgs e)
        {
            var accountId = Convert.ToInt32(ddlAccountProjectSection.SelectedValue);
            var projectId = Convert.ToInt32(ddlProjectProjectSection.SelectedValue);

            var project = ServiceCallers.Custom.Project(pro => pro.ProjectGetById(projectId));

            XDocument xdoc = PrePareXmlForProjectSectionFromRepeater();

            List<XElement> xelementsList = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();

            if (!xelementsList.Any(element => Convert.ToInt32(element.Attribute(XName.Get(AccountIdXname)).Value) == accountId && Convert.ToInt32(element.Attribute(XName.Get(ProjectIdXname)).Value) == projectId))
            {
                var teSection = new TimeEntrySection()
                {
                    Project = new Project() { Id = projectId, Name = project.Name, ProjectNumber = project.ProjectNumber },
                    Account = new Client() { Id = accountId, Name = ddlAccountProjectSection.SelectedItem.Text },
                    BusinessUnit = new ProjectGroup() { Id = project.Group.Id, Name = project.Group.Name },
                    SectionId = TimeEntrySectionType.Project,
                    IsRecursive = false

                };

                StringBuilder xml = new StringBuilder();

                PrePareXmlForAccountProjectSelection(xml, teSection);

                xdoc.Descendants(XName.Get(SectionXname)).First().Add(XElement.Parse(xml.ToString()));

                DatabindRepeater(repProjectSections, xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList());

                ProjectSectionXml = xdoc.ToString();

                int personId = SelectedPerson.Id.Value;
                DateTime[] dates = SelectedDates;
                int timeEntrySectionId = (int)TimeEntrySectionType.Project;
                int projectGroupId = project.Group.Id.Value;
                ServiceCallers.Custom.TimeEntry(t => t.SetPersonTimeEntrySelection(personId, accountId, projectGroupId, projectId, timeEntrySectionId, false, dates[0], dates[dates.Length - 1], Context.User.Identity.Name));

            }

            ddlProjectProjectSection.SelectedIndex = 0;
            ddlAccountProjectSection.SelectedIndex = 0;

        }

        protected void btnAddBusinessDevelopmentSection_OnClick(object sender, EventArgs e)
        {
            var project = ServiceCallers.Custom.Project(pro => pro.GetBusinessDevelopmentProject());
            var businessUnitId = Convert.ToInt32(ddlBusinessUnitBusinessDevlopmentSection.SelectedValue);
            var accountId = Convert.ToInt32(ddlAccountBusinessDevlopmentSection.SelectedValue);
            var selectedBusinessunitText = ddlBusinessUnitBusinessDevlopmentSection.SelectedItem.Text;
            var businessunitName = selectedBusinessunitText.Substring(0, selectedBusinessunitText.IndexOf(":::"));

            XDocument xdoc = PrePareXmlForBusinessDevelopmentSectionFromRepeater();

            List<XElement> xelementsList = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();

            if (!xelementsList.Any(element => Convert.ToInt32(element.Attribute(XName.Get(AccountIdXname)).Value) == accountId && Convert.ToInt32(element.Attribute(XName.Get(BusinessUnitIdXname)).Value) == businessUnitId && Convert.ToInt32(element.Attribute(XName.Get(ProjectIdXname)).Value) == project.Id))
            {
                var teSection = new TimeEntrySection()
                {
                    Project = new Project() { Id = project.Id, Name = project.Name, ProjectNumber = project.ProjectNumber },
                    Account = new Client() { Id = accountId, Name = ddlAccountBusinessDevlopmentSection.SelectedItem.Text },
                    BusinessUnit = new ProjectGroup() { Id = businessUnitId, Name = businessunitName },
                    SectionId = TimeEntrySectionType.BusinessDevelopment,
                    IsRecursive = false
                };

                StringBuilder xml = new StringBuilder();

                PrePareXmlForAccountProjectSelection(xml, teSection);

                xdoc.Descendants(XName.Get(SectionXname)).First().Add(XElement.Parse(xml.ToString()));

                BusinessDevelopmentSectionXml = xdoc.ToString();


                DatabindRepeater(repBusinessDevelopmentSections, xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList());

            }

            ddlBusinessUnitBusinessDevlopmentSection.SelectedIndex = 0;
            ddlAccountBusinessDevlopmentSection.SelectedIndex = 0;

            int personId = SelectedPerson.Id.Value;
            DateTime[] dates = SelectedDates;
            int timeEntrySectionId = (int)TimeEntrySectionType.BusinessDevelopment;
            ServiceCallers.Custom.TimeEntry(t => t.SetPersonTimeEntrySelection(personId, accountId, businessUnitId, project.Id.Value, timeEntrySectionId, false, dates[0], dates[dates.Length - 1], Context.User.Identity.Name));

        }

        protected void btnAddInternalProjectSection_OnClick(object sender, EventArgs e)
        {
            var businessUnitId = Convert.ToInt32(ddlBusinessUnitInternal.SelectedValue);
            var projectId = Convert.ToInt32(ddlProjectInternal.SelectedValue);
            var project = ServiceCallers.Custom.Project(pro => pro.ProjectGetById(projectId));
            var businessunitName = ddlBusinessUnitInternal.SelectedItem.Text;
            var account = ServiceCallers.Custom.Client(c => c.GetInternalAccount());

            XDocument xdoc = PrePareXmlForInternalSectionFromRepeater();

            List<XElement> xelementsList = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();


            if (!xelementsList.Any(element => Convert.ToInt32(element.Attribute(XName.Get(AccountIdXname)).Value) == account.Id && Convert.ToInt32(element.Attribute(XName.Get(BusinessUnitIdXname)).Value) == businessUnitId && Convert.ToInt32(element.Attribute(XName.Get(ProjectIdXname)).Value) == projectId))
            {
                var teSection = new TimeEntrySection()
                {
                    Project = project,
                    Account = new Client() { Id = account.Id, Name = account.Name },
                    BusinessUnit = new ProjectGroup() { Id = businessUnitId, Name = businessunitName },
                    SectionId = TimeEntrySectionType.Internal,
                    IsRecursive = false
                };

                StringBuilder xml = new StringBuilder();

                PrePareXmlForAccountProjectSelection(xml, teSection);

                xdoc.Descendants(XName.Get(SectionXname)).First().Add(XElement.Parse(xml.ToString()));

                DatabindRepeater(repInternalSections, xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList());

                InternalSectionXml = xdoc.ToString();
            }


            ddlProjectInternal.SelectedIndex = 0;
            ddlBusinessUnitInternal.SelectedIndex = 0;

            int personId = SelectedPerson.Id.Value;
            DateTime[] dates = SelectedDates;
            int timeEntrySectionId = (int)TimeEntrySectionType.Internal;
            ServiceCallers.Custom.TimeEntry(t => t.SetPersonTimeEntrySelection(personId, account.Id.Value, businessUnitId, projectId, timeEntrySectionId, false, dates[0], dates[dates.Length - 1], Context.User.Identity.Name));

        }

        protected void pcPersons_PersonChanged(object sender, PersonChangedEventArguments args)
        {
            IsWeekOrPersonChanged = true;
            UpdateTimeEntries();
            wsChoose.UpdateWeekLabel();
        }

        protected void wsChoose_WeekChanged(object sender, WeekChangedEventArgs args)
        {
            IsWeekOrPersonChanged = true;
            UpdateTimeEntries();
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateTimeEntries();
            }
        }

        protected void dpChoose_OnSelectionChanged(object sender, EventArgs args)
        {
            var dp = (TextBox)sender;

            var pageBase = this.Page as PracticeManagementPageBase;
            if (pageBase.IsDirty && !SaveData())
            {
                dp.Text = wsChoose.SelectedStartDate.ToShortDateString();
                //   RaiseError();
            }
            else
            {
                ClearDirtyState();
                wsChoose.SetDate(Convert.ToDateTime(dp.Text));
            }
        }

        protected string GetDayOffCssCalss(CalendarItem calendarItem)
        {
            return Utils.Calendar.GetCssClassByCalendarItem(calendarItem); 
        }

        #endregion

        #region Methods

        private void ValidateAll()
        {
            IsValidDayTotal = IsValidHours = IsValidNote = IsValidWorkType = IsValidPTOHours = true;

            foreach (RepeaterItem barItem in repProjectSections.Items)
            {
                var repProjectTes = barItem.FindControl(repProjectTesRepeater) as Repeater;

                foreach (RepeaterItem item in repProjectTes.Items)
                {
                    var bar = item.FindControl(teBarId) as BillableAndNonBillableTimeEntryBar;
                    bar.ValidateAll();
                }

            }

            foreach (RepeaterItem barItem in repBusinessDevelopmentSections.Items)
            {
                var repBusinessDevelopmentTes = barItem.FindControl(repBusinessDevelopmentTesRepeater) as Repeater;

                foreach (RepeaterItem item in repBusinessDevelopmentTes.Items)
                {
                    var bar = item.FindControl(teBarId) as NonBillableTimeEntryBar;
                    bar.ValidateAll();
                }
            }

            foreach (RepeaterItem barItem in repInternalSections.Items)
            {
                var repInternalTes = barItem.FindControl(repInternalTesRepeater) as Repeater;

                foreach (RepeaterItem item in repInternalTes.Items)
                {
                    var bar = item.FindControl(teBarId) as NonBillableTimeEntryBar;
                    bar.ValidateAll();
                }
            }


            foreach (RepeaterItem barItem in repAdministrativeTes.Items)
            {
                var bar = barItem.FindControl(teBarId) as AdministrativeTimeEntryBar;
                
                bar.ValidateAll();
            }

            foreach (RepeaterItem barItem in repDayTotalHours.Items)
            {
                var lblDayTotal = barItem.FindControl(lblDayTotalId) as Label;
                var hdnDayTotal = (HiddenField)barItem.FindControl(hdnDayTotalId);

                double val = 0;
                double.TryParse(hdnDayTotal.Value, out val);

                if (val > 24)
                {
                    IsValidDayTotal = false;
                    lblDayTotal.BackColor = Color.Red;
                }
                else
                {
                    lblDayTotal.BackColor = Color.White;
                }
            }
        }

        private void AddAttributes()
        {
            ddlAccountProjectSection.Attributes[addAttribute] = ddlProjectProjectSection.Attributes[addAttribute] = btnAddProjectSection.ClientID;
            ddlBusinessUnitBusinessDevlopmentSection.Attributes[addAttribute] = ddlAccountBusinessDevlopmentSection.Attributes[addAttribute] = btnAddBusinessDevelopmentSection.ClientID;
            ddlProjectInternal.Attributes[addAttribute] = ddlBusinessUnitInternal.Attributes[addAttribute] = btnAddInternalProjectSection.ClientID;

            ddlAccountProjectSection.Attributes[childDropDownClientIdAttribute] = ddlProjectProjectSection.ClientID;
            ddlAccountBusinessDevlopmentSection.Attributes[childDropDownClientIdAttribute] = ddlBusinessUnitBusinessDevlopmentSection.ClientID;
            ddlBusinessUnitInternal.Attributes[childDropDownClientIdAttribute] = ddlProjectInternal.ClientID;

            btnAddProject.Attributes[parentDropDownClientIdAttribute] = ddlAccountProjectSection.ClientID;
            btnAddProject.Attributes[childDropDownClientIdAttribute] = ddlProjectProjectSection.ClientID;
            btnAddProject.Attributes[btnAddAttribute] = btnAddProjectSection.ClientID;

            btnAddAccount.Attributes[parentDropDownClientIdAttribute] = ddlAccountBusinessDevlopmentSection.ClientID;
            btnAddAccount.Attributes[childDropDownClientIdAttribute] = ddlBusinessUnitBusinessDevlopmentSection.ClientID;
            btnAddAccount.Attributes[btnAddAttribute] = btnAddBusinessDevelopmentSection.ClientID;

            btnAddInternalProject.Attributes[parentDropDownClientIdAttribute] = ddlBusinessUnitInternal.ClientID;
            btnAddInternalProject.Attributes[childDropDownClientIdAttribute] = ddlProjectInternal.ClientID;
            btnAddInternalProject.Attributes[btnAddAttribute] = btnAddInternalProjectSection.ClientID;
        }

        protected override void Display()
        {
        }

        private Control FindControlInFooter(Repeater repeater, string controlName)
        {
            return repeater.Controls[repeater.Controls.Count - 1].Controls[0].FindControl(controlName);
        }

        public bool SaveData(bool isUpdateTimeEntries = false)
        {
            ValidateAll();
            Page.Validate(valSumSaveTimeEntries.ValidationGroup);
            if (Page.IsValid)
            {
                SaveAll();

                if (isUpdateTimeEntries)
                    UpdateTimeEntries();
            }

            return Page.IsValid;
        }

        private void SaveAll()
        {
            IsSaving = true;
            ProjectSectionXml = PrePareXmlForProjectSectionFromRepeater().ToString();
            BusinessDevelopmentSectionXml = PrePareXmlForBusinessDevelopmentSectionFromRepeater().ToString();
            InternalSectionXml = PrePareXmlForInternalSectionFromRepeater().ToString();
            AdministrativeSectionXml = PrePareXmlForAdminstrativeSectionFromRepeater().ToString();

            StringBuilder resultXml = new StringBuilder();
            resultXml.Append(sectionsXmlOpen);
            resultXml.Append(ProjectSectionXml);
            resultXml.Append(BusinessDevelopmentSectionXml);
            resultXml.Append(InternalSectionXml);
            resultXml.Append(AdministrativeSectionXml);
            resultXml.Append(sectionsXmlClose);
            ServiceCallers.Custom.TimeEntry(te => te.SaveTimeTrack(resultXml.ToString(), SelectedPerson.Id.Value, SelectedDates[0], SelectedDates[SelectedDates.Length - 1], User.Identity.Name));
            ClearDirty();

            IsSaving = false;
            mlConfirmation.ShowInfoMessage(SavedAllConfirmation);
        }

        private XDocument PrePareXmlForProjectSectionFromData()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append(projectSectionXmlOpen);


            foreach (TimeEntrySection teSection in ProjectSection)
            {
                PrePareXmlForAccountProjectSelection(xml, teSection, true);
            }

            xml.Append(projectSectionXmlClose);

            var xmlStr = xml.ToString();

            ProjectSectionXml = xmlStr;

            return XDocument.Parse(xmlStr);
        }

        private XDocument PrePareXmlForBusinessDevelopmentSectionFromData()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append(busineeDevelopmentSectionXmlOpen);


            foreach (TimeEntrySection teSection in BusinessDevelopmentSection)
            {
                PrePareXmlForAccountProjectSelection(xml, teSection, true);
            }

            xml.Append(busineeDevelopmentSectionXmlClose);

            var xmlStr = xml.ToString();

            BusinessDevelopmentSectionXml = xmlStr;

            return XDocument.Parse(xmlStr);
        }

        private XDocument PrePareXmlForInternalSectionFromData()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append(internalSectionXmlOpen);


            foreach (TimeEntrySection teSection in InternalSection)
            {
                PrePareXmlForAccountProjectSelection(xml, teSection, true);
            }

            xml.Append(internalSectionXmlClose);

            var xmlStr = xml.ToString();

            InternalSectionXml = xmlStr;

            return XDocument.Parse(xmlStr);
        }

        private XDocument PrePareXmlForAdminiStrativeSectionFromData()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append(administrativeSectionXmlOpen);


            foreach (TimeEntrySection teSection in AdminiStrativeSection)
            {
                PrePareXmlForAccountProjectSelection(xml, teSection, true);
            }

            xml.Append(administrativeSectionXmlClose);

            var xmlStr = xml.ToString();

            AdministrativeSectionXml = xmlStr;

            return XDocument.Parse(xmlStr);
        }

        private void PrePareXmlForAccountProjectSelection(StringBuilder xml, TimeEntrySection teSection, bool intialPrepare = false)
        {
            int accountId = teSection.Account.Id.Value;
            int projectId = teSection.Project.Id.Value;
            int businessUnitId = teSection.BusinessUnit.Id.Value;
            int personId = SelectedPerson.Id.Value;
            DateTime startDate = SelectedDates[0];
            DateTime endDate = SelectedDates[SelectedDates.Length - 1];
            bool isRecursiveAllowed = teSection.Project.EndDate.HasValue && teSection.Project.EndDate.Value > endDate ;

            xml.Append(string.Format(accountAndProjectSelectionXmlOpen, accountId, teSection.Account.HtmlEncodedName, projectId, teSection.Project.HtmlEncodedName, teSection.Project.ProjectNumber, businessUnitId, teSection.BusinessUnit.HtmlEncodedName, teSection.IsRecursive, teSection.Project.IsPTOProject.ToString(), teSection.Project.IsHolidayProject.ToString(), isRecursiveAllowed.ToString()));


            foreach (KeyValuePair<TimeTypeRecord, List<TimeEntryRecord>> keyVal in teSection.TimeEntriesByTimeType)
            {
                int timeTypeId = keyVal.Key.Id;
                if (intialPrepare)
                {
                    xml.Append(string.Format(workTypeXmlOpenWithOldId, timeTypeId, timeTypeId));
                }
                else
                {
                    xml.Append(string.Format(workTypeXmlOpen, timeTypeId));
                }

                CalendarItems = CalendarItems ??
                                   ServiceCallers.Custom.Calendar(
                                                                    c => c.GetCalendar(wsChoose.SelectedDates[0], wsChoose.SelectedDates[wsChoose.SelectedDates.Length - 1], pcPersons.SelectedPerson.Id.Value, null)
                                                                  );
                IsNoteRequiredList = IsNoteRequiredList ??
                                  DataHelper.GetIsNoteRequiredDetailsForSelectedDateRange(wsChoose.SelectedStartDate, wsChoose.SelectedEndDate, pcPersons.SelectedPersonId);
                ;

                IsHourlyRevenueList = ServiceCallers.Custom.Project(p => p.GetIsHourlyRevenueByPeriod(projectId, personId, startDate, endDate));
                Dictionary<DateTime, bool> isChargeCodeTurnOffList = ServiceCallers.Custom.TimeEntry(p => p.GetIsChargeCodeTurnOffByPeriod(personId, accountId, businessUnitId, projectId, timeTypeId, startDate, endDate));

                var teRecords = keyVal.Value;
                foreach (CalendarItem day in CalendarItems)
                {
                    var filterByDateTeRecords = teRecords != null ? teRecords.Where(ter => ter.MilestoneDate.Date == day.Date).ToList() : null;

                    var cssClass = Utils.Calendar.GetCssClassByCalendarItem(day);

                    xml.Append(string.Format(calendarItemXmlOpen, day.Date, cssClass, IsNoteRequiredList[day.Date], IsHourlyRevenueList[day.Date], isChargeCodeTurnOffList[day.Date]));

                    if (filterByDateTeRecords != null)
                    {
                        var bterecord = filterByDateTeRecords.Any(v => v.IsChargeable == true) ? filterByDateTeRecords.First(k => k.IsChargeable == true) : null;
                        var nbterecord = filterByDateTeRecords.Any(v => v.IsChargeable == false) ? filterByDateTeRecords.First(k => k.IsChargeable == false) : null;

                        if (bterecord != null)
                        {
                            xml.Append(string.Format(billableXmlOpen, bterecord.ActualHours.ToString(Constants.Formatting.DoubleFormat), bterecord.HtmlEncodedNote, bterecord.IsChargeable, bterecord.EntryDate.ToString(Constants.Formatting.EntryDateFormat), bterecord.IsReviewed.ToString(), "none"));
                            xml.Append(billableXmlClose);
                        }

                        if (nbterecord != null)
                        {
                            xml.Append(string.Format(nonBillableXmlOpen, nbterecord.ActualHours.ToString(Constants.Formatting.DoubleFormat), nbterecord.HtmlEncodedNote, nbterecord.IsChargeable, nbterecord.EntryDate.ToString(Constants.Formatting.EntryDateFormat), nbterecord.IsReviewed.ToString(), "none"));
                            xml.Append(NonBillableXmlClose);
                        }
                    }

                    xml.Append(calendarItemXmlClose);
                }

                xml.Append(workTypeXmlClose);
            }

            xml.Append(accountAndProjectSelectionXmlClose);
        }

        private XDocument PrePareXmlForProjectSectionFromRepeater()
        {
            var xdoc = XDocument.Parse(ProjectSectionXml);
            var accountAndProjectSelectionElements = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();
            for (int i = 0; i < accountAndProjectSelectionElements.Count; i++)
            {
                var accountAndProjectSelectionElement = accountAndProjectSelectionElements[i];
                var workTypeElements = accountAndProjectSelectionElement.Descendants(XName.Get(WorkTypeXname)).ToList();

                RepeaterItem barItem = repProjectSections.Items[i];
                var repProjectTes = barItem.FindControl(repProjectTesRepeater) as Repeater;

                for (int j = 0; j < workTypeElements.Count; j++)
                {
                    var workTypeElement = workTypeElements[j];
                    RepeaterItem repProjectTesItem = repProjectTes.Items[j];
                    var bar = repProjectTesItem.FindControl(teBarId) as BillableAndNonBillableTimeEntryBar;

                    bar.UpdateWorkType(workTypeElement, accountAndProjectSelectionElement);

                    var calendarItemElements = workTypeElement.Descendants(XName.Get(CalendarItemXname)).ToList();

                    bar.UpdateNoteAndActualHours(calendarItemElements);

                    workTypeElements[j] = workTypeElement;
                }
            }

            return xdoc;
        }

        private XDocument PrePareXmlForBusinessDevelopmentSectionFromRepeater()
        {
            var xdoc = XDocument.Parse(BusinessDevelopmentSectionXml);
            var accountAndProjectSelectionElements = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();
            for (int i = 0; i < accountAndProjectSelectionElements.Count; i++)
            {
                var accountAndProjectSelectionElement = accountAndProjectSelectionElements[i];
                var workTypeElements = accountAndProjectSelectionElement.Descendants(XName.Get(WorkTypeXname)).ToList();

                RepeaterItem barItem = repBusinessDevelopmentSections.Items[i];
                var repBusinessDevelopmentTes = barItem.FindControl(repBusinessDevelopmentTesRepeater) as Repeater;

                for (int j = 0; j < workTypeElements.Count; j++)
                {
                    var workTypeElement = workTypeElements[j];
                    var calendarItemElements = workTypeElement.Descendants(XName.Get(CalendarItemXname)).ToList();
                    RepeaterItem repProjectTesItem = repBusinessDevelopmentTes.Items[j];
                    var bar = repProjectTesItem.FindControl(teBarId) as NonBillableTimeEntryBar;

                    bar.UpdateWorkType(workTypeElement, accountAndProjectSelectionElement);
                    bar.UpdateNoteAndActualHours(calendarItemElements);
                }
            }

            return xdoc;
        }

        private XDocument PrePareXmlForInternalSectionFromRepeater()
        {
            var xdoc = XDocument.Parse(InternalSectionXml);
            var accountAndProjectSelectionElements = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();
            for (int i = 0; i < accountAndProjectSelectionElements.Count; i++)
            {
                var accountAndProjectSelectionElement = accountAndProjectSelectionElements[i];
                var workTypeElements = accountAndProjectSelectionElement.Descendants(XName.Get(WorkTypeXname)).ToList();

                RepeaterItem barItem = repInternalSections.Items[i];
                var repInternalTes = barItem.FindControl(repInternalTesRepeater) as Repeater;

                for (int j = 0; j < workTypeElements.Count; j++)
                {
                    var workTypeElement = workTypeElements[j];
                    var calendarItemElements = workTypeElement.Descendants(XName.Get(CalendarItemXname)).ToList();
                    RepeaterItem repInternalTesItem = repInternalTes.Items[j];
                    var bar = repInternalTesItem.FindControl(teBarId) as NonBillableTimeEntryBar;

                    bar.UpdateWorkType(workTypeElement, accountAndProjectSelectionElement);
                    bar.UpdateNoteAndActualHours(calendarItemElements);
                }
            }

            return xdoc;
        }

        private XDocument PrePareXmlForAdminstrativeSectionFromRepeater()
        {
            var xdoc = XDocument.Parse(AdministrativeSectionXml);
            var accountAndProjectSelectionElements = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();
            for (int i = 0; i < accountAndProjectSelectionElements.Count; i++)
            {
                var accountAndProjectSelectionElement = accountAndProjectSelectionElements[i];
                var workTypeElement = accountAndProjectSelectionElement.Descendants(XName.Get(WorkTypeXname)).ToList()[0];
                var calendarItemElements = workTypeElement.Descendants(XName.Get(CalendarItemXname)).ToList();

                RepeaterItem repAdministrativeTesItem = repAdministrativeTes.Items[i];


                var bar = repAdministrativeTesItem.FindControl(teBarId) as AdministrativeTimeEntryBar;

                bar.UpdateAccountAndProjectWorkType(accountAndProjectSelectionElement, workTypeElement);


                bar.UpdateNoteAndActualHours(calendarItemElements);


            }

            return xdoc;
        }

        private void UpdateTimeEntries()
        {

            var teSctions = ServiceCallers.Custom.TimeEntry(te => te.PersonTimeEntriesByPeriod(pcPersons.SelectedPerson.Id.Value, SelectedDates[0], SelectedDates[SelectedDates.Length - 1]));

            AdminiStrativeSection = teSctions.Where(ts => ts.SectionId == TimeEntrySectionType.Administrative).OrderByDescending(tes => tes.Project.IsHolidayProject).ThenByDescending(tes => tes.Project.IsPTOProject).ToList();
            InternalSection = teSctions.Where(ts => ts.SectionId == TimeEntrySectionType.Internal).OrderByDescending(tes => tes.IsRecursive).ToList();
            BusinessDevelopmentSection = teSctions.Where(ts => ts.SectionId == TimeEntrySectionType.BusinessDevelopment).OrderByDescending(tes => tes.IsRecursive).ToList();
            ProjectSection = teSctions.Where(ts => ts.SectionId == TimeEntrySectionType.Project).OrderByDescending(tes => tes.IsRecursive).ToList();

            var isSelectedActive =
                 SelectedPerson.Status != null &&
                 SelectedPerson.Status.ToStatusType() == PersonStatusType.Active;
            var currentIsAdmin =
                Roles.IsUserInRole(
                    DataTransferObjects.Constants.RoleNames.AdministratorRoleName);


            var isNoteRequiredList = DataHelper.GetIsNoteRequiredDetailsForSelectedDateRange(wsChoose.SelectedStartDate, wsChoose.SelectedEndDate, pcPersons.SelectedPersonId);

            IsNoteRequiredList = isNoteRequiredList;

            var result = true;

            if (SelectedPerson.TerminationDate.HasValue)
                result = isNoteRequiredList.Any(p => p.Value == true && SelectedPerson.TerminationDate >= p.Key);

            lblAlertNote.Text = string.Format(AlertTextFormat, isNoteRequiredList.Any(p => p.Value == true
                                                                    && SelectedPerson.HireDate <= p.Key
                                                                    && result
                                                                    ) ? "REQUIRED" : "OPTIONAL");

            var showGrid = isSelectedActive || currentIsAdmin;
            pnlShowTimeEntries.Visible = showGrid;
            if (showGrid)
            {
                if (SelectedDates.Length > 0)
                {
                    if (SelectedPerson != null)
                    {

                        var projectSection = PrePareXmlForProjectSectionFromData();
                        List<XElement> xProjectSelectionlist = projectSection.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();

                        var businessDevelopmentSection = PrePareXmlForBusinessDevelopmentSectionFromData();
                        List<XElement> xbusinessDevelopmentlist = businessDevelopmentSection.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();

                        var internalSection = PrePareXmlForInternalSectionFromData();
                        List<XElement> xinternalSectionlist = internalSection.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();

                        var adminiStrativeSection = PrePareXmlForAdminiStrativeSectionFromData();
                        List<XElement> xadminiStrativeSectionlist = adminiStrativeSection.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();

                        DatabindRepeater(repProjectSections, xProjectSelectionlist, false);
                        DatabindRepeater(repBusinessDevelopmentSections, xbusinessDevelopmentlist, false);
                        DatabindRepeater(repInternalSections, xinternalSectionlist, false);
                        DatabindRepeater(repAdministrativeTes, xadminiStrativeSectionlist);
                    }
                    else
                    {
                        //mlMessage.ShowInfoMessage(Resources.Messages.PersonIsNotAssgned);
                    }
                }
            }
            else
            {
                mlErrors.ShowErrorMessage(Resources.Messages.NotAllowedToRecordTE);
            }
        }

        private void DatabindRepeater(Repeater rep, List<XElement> dataSource, bool isDayTotalDatabind = true)
        {
            rep.DataSource = dataSource;
            rep.DataBind();

            if (isDayTotalDatabind)
            {
                CalendarItems = CalendarItems ??
                                   ServiceCallers.Custom.Calendar(
                                                                    c => c.GetCalendar(wsChoose.SelectedDates[0], wsChoose.SelectedDates[wsChoose.SelectedDates.Length - 1], pcPersons.SelectedPerson.Id.Value, null)
                                                                  );
                BillableControlIds = string.Empty;
                NonBillableControlIds = string.Empty;
                repDayTotalHours.DataSource = CalendarItems;
                repDayTotalHours.DataBind();
            }
        }

        public void ClearDirtyState()
        {
            var pageBase = this.Page as PracticeManagementPageBase;
            if (pageBase != null)
            {
                pageBase.IsDirty = false;
            }
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "clearDirty();", true);
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            ValidateAll();
            Page.Validate(valSumSaveTimeEntries.ValidationGroup);
            if (Page.IsValid)
            {
                SaveAll();
                int i = 0;
                int.TryParse(eventArgument, out i);

                if (i > 0)
                {
                    var urlTemplate = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) ?
                                        Constants.ApplicationPages.TimeEntry_NewForAdmin : Constants.ApplicationPages.TimeEntry_New;
                    eventArgument = string.Format(urlTemplate, SelectedDates[0].ToString("yyyy-MM-dd"), i);

                }

                Redirect(eventArgument);
            }
        }

        public void RemoveWorktypeFromXMLForProjectSection(int accountId, int projectId, int workTypeindex)
        {
            var xdoc = PrePareXmlForProjectSectionFromRepeater();
            List<XElement> xlist = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();
            var accountAndProjectSelectionElement = xlist.First(element => element.Attribute(XName.Get(AccountIdXname)).Value == accountId.ToString() && element.Attribute(XName.Get(ProjectIdXname)).Value == projectId.ToString());
            var workTypeElements = accountAndProjectSelectionElement.Descendants(XName.Get(WorkTypeXname)).ToList();
            var workTypeElement = workTypeElements.ElementAt(workTypeindex);
            workTypeElement.Remove();
            if (workTypeElements.Count == 1)
            {
                StringBuilder xml = new StringBuilder();
                xml.Append(string.Format(workTypeXmlOpen, -1));

                CalendarItems = CalendarItems ??
                                   ServiceCallers.Custom.Calendar(
                                                                    c => c.GetCalendar(wsChoose.SelectedDates[0], wsChoose.SelectedDates[wsChoose.SelectedDates.Length - 1], pcPersons.SelectedPerson.Id.Value, null)
                                                                  );
                IsNoteRequiredList = IsNoteRequiredList ??
                            DataHelper.GetIsNoteRequiredDetailsForSelectedDateRange(wsChoose.SelectedStartDate, wsChoose.SelectedEndDate, pcPersons.SelectedPersonId);
                ;

                int personId = SelectedPerson.Id.Value;
                DateTime startDate = SelectedDates[0];
                DateTime endDate = SelectedDates[SelectedDates.Length - 1];
                IsHourlyRevenueList = ServiceCallers.Custom.Project(p => p.GetIsHourlyRevenueByPeriod(projectId, personId, startDate, endDate));

                foreach (CalendarItem day in CalendarItems)
                {
                    var cssClass = Utils.Calendar.GetCssClassByCalendarItem(day);
                    xml.Append(string.Format(calendarItemXmlOpen, day.Date, cssClass, IsNoteRequiredList[day.Date], IsHourlyRevenueList[day.Date], false));
                    xml.Append(calendarItemXmlClose);
                }
                xml.Append(workTypeXmlClose);

                var xlement = XElement.Parse(xml.ToString());

                accountAndProjectSelectionElement.Add(xlement);
            }

            ProjectSectionXml = xdoc.ToString();
            DatabindRepeater(repProjectSections, xlist);

        }

        public void RemoveWorktypeFromXMLForBusinessDevelopmentAndInternalSection(int accountId, int businessUnitId, int projectId, int workTypeindex, bool isBusinessDevelopment)
        {

            var xdoc = isBusinessDevelopment ? PrePareXmlForBusinessDevelopmentSectionFromRepeater() : PrePareXmlForInternalSectionFromRepeater();

            List<XElement> xlist = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();
            var accountAndProjectSelectionElement = xlist.First(element => element.Attribute(XName.Get(AccountIdXname)).Value == accountId.ToString() && element.Attribute(XName.Get(BusinessUnitIdXname)).Value == businessUnitId.ToString() && element.Attribute(XName.Get(ProjectIdXname)).Value == projectId.ToString());
            var workTypeElements = accountAndProjectSelectionElement.Descendants(XName.Get(WorkTypeXname)).ToList();
            var workTypeElement = workTypeElements.ElementAt(workTypeindex);
            workTypeElement.Remove();
            if (workTypeElements.Count == 1)
            {
                StringBuilder xml = new StringBuilder();
                xml.Append(string.Format(workTypeXmlOpen, -1));

                CalendarItems = CalendarItems ??
                                   ServiceCallers.Custom.Calendar(
                                                                    c => c.GetCalendar(wsChoose.SelectedDates[0], wsChoose.SelectedDates[wsChoose.SelectedDates.Length - 1], pcPersons.SelectedPerson.Id.Value, null)
                                                                  );
                IsNoteRequiredList = IsNoteRequiredList ??
                            DataHelper.GetIsNoteRequiredDetailsForSelectedDateRange(wsChoose.SelectedStartDate, wsChoose.SelectedEndDate, pcPersons.SelectedPersonId);
                ;

                int personId = SelectedPerson.Id.Value;
                DateTime startDate = SelectedDates[0];
                DateTime endDate = SelectedDates[SelectedDates.Length - 1];
                IsHourlyRevenueList = ServiceCallers.Custom.Project(p => p.GetIsHourlyRevenueByPeriod(projectId, personId, startDate, endDate));

                foreach (CalendarItem day in CalendarItems)
                {
                    var cssClass = Utils.Calendar.GetCssClassByCalendarItem(day);
                    xml.Append(string.Format(calendarItemXmlOpen, day.Date, cssClass, IsNoteRequiredList[day.Date], IsHourlyRevenueList[day.Date], false));
                    xml.Append(calendarItemXmlClose);
                }
                xml.Append(workTypeXmlClose);

                var xlement = XElement.Parse(xml.ToString());

                accountAndProjectSelectionElement.Add(xlement);
            }

            if (isBusinessDevelopment)
            {
                BusinessDevelopmentSectionXml = xdoc.ToString();
                DatabindRepeater(repBusinessDevelopmentSections, xlist);
            }
            else
            {
                InternalSectionXml = xdoc.ToString();
                DatabindRepeater(repInternalSections, xlist);
            }

        }

        public void RemoveWorktypeFromXMLForAdminstrativeSection(int repindex)
        {
            var xdoc = PrePareXmlForAdminstrativeSectionFromRepeater();
            List<XElement> accountAndProjectSelectionElements = xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList();
            var accountAndProjectSelectionElement = accountAndProjectSelectionElements.ElementAt(repindex);
            accountAndProjectSelectionElement.Remove();
            AdministrativeSectionXml = xdoc.ToString();
            DatabindRepeater(repAdministrativeTes, xdoc.Descendants(XName.Get(AccountAndProjectSelectionXname)).ToList());

        }

        #endregion

    }
}

