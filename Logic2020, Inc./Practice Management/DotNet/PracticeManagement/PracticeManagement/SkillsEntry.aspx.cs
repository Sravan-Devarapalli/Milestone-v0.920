using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.Utils;
using System.ComponentModel;
using DataTransferObjects;
using DataTransferObjects.Skills;
using System.Xml;


namespace PraticeManagement
{
    public partial class SkillsEntry : PracticeManagementPageBase,IPostBackEventHandler
    {
        #region Constants

        private const string SessionPersonWithSkills = "PersonWithSkills";
        private const string ViewStatePreviousActiveTabIndex = "PreviousActiveTabIndex";
        private const string ViewStatePreviousCategoryIndex = "PreviousCategoryIndex";
        private const string ValidationPopUpMessage = "Please select a value for ‘Level’, ‘Experience’, ‘Last Used’, for these skills ";

        #endregion

        #region fields

        public bool IsFirst = true;
        private bool IsPreviousTabValid = true;

        #endregion

        #region Properties

        public Person Person
        {
            get
            {
                if (Session[SessionPersonWithSkills] == null)
                {
                    using(var serviceClient = new PersonSkillService.PersonSkillServiceClient())
                    {
                        Session[SessionPersonWithSkills] = serviceClient.GetPersonWithSkills(DataHelper.CurrentPerson.Id.Value);
                    } 
                }
                return (Person)Session[SessionPersonWithSkills];
            }
            set
            {
                Session[SessionPersonWithSkills] = value;
            }
        }

        public int SelectedBusinessCategory
        {
            get
            {
                return Convert.ToInt32(ddlBusinessCategory.SelectedValue);
            }
        }

        public int SelectedTechnicalCategory
        {
            get
            {
                return Convert.ToInt32(ddlTechnicalCategory.SelectedValue);
            }
        }

        public int PreviousActiveTabIndex
        {
            get
            {
                if (ViewState[ViewStatePreviousActiveTabIndex] == null)
                {
                    ViewState[ViewStatePreviousActiveTabIndex] = 0;
                }
                return (int)ViewState[ViewStatePreviousActiveTabIndex];
            }
            set
            {
                ViewState[ViewStatePreviousActiveTabIndex] = value;
            }
        }

        public int PreviousCategoryIndex
        {
            get
            {
                if (ViewState[ViewStatePreviousCategoryIndex] == null)
                {
                    ViewState[ViewStatePreviousCategoryIndex] = 0;
                }
                return (int)ViewState[ViewStatePreviousCategoryIndex];
            }
            set
            {
                ViewState[ViewStatePreviousCategoryIndex] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session[SessionPersonWithSkills] = null;
                lblUserName.Text = DataHelper.CurrentPerson.PersonLastFirstName;
                RenderSkills(tcSkillsEntry.ActiveTabIndex); 
            }
            hdnValidationMessage.Value = ValidationPopUpMessage;
        }

        protected override void Display()
        {

        }

        protected void tcSkillsEntry_ActiveTabChanged(object sender, EventArgs e)
        {
            if (IsFirst)
            {
                if (IsDirty)
                {
                    if (!ValidateAndSave(PreviousActiveTabIndex))
                    {
                        tcSkillsEntry.ActiveTabIndex = PreviousActiveTabIndex;
                        IsFirst = false;
                        IsPreviousTabValid = false;
                        return;
                    }
                }

                RenderSkills(tcSkillsEntry.ActiveTabIndex);
                IsFirst = false;
            }

            if (!IsFirst && !IsPreviousTabValid)
            {
                tcSkillsEntry.ActiveTabIndex = PreviousActiveTabIndex;
            }
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int activeTabIndex = tcSkillsEntry.ActiveTabIndex;
            if (IsDirty)
            {
                if (!ValidateAndSave(activeTabIndex))
                {
                    var ddlCategory = sender as DropDownList;
                    ddlCategory.SelectedIndex = PreviousCategoryIndex;
                    return;
                }
            }

            BindSkills(activeTabIndex);
        }

        protected void gvSkills_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var ddlLevel = e.Row.FindControl("ddlLevel") as DropDownList;
                var ddlExperience = e.Row.FindControl("ddlExperience") as DropDownList;
                var ddlLastUsed = e.Row.FindControl("ddlLastUsed") as DropDownList;
                var hdnId = e.Row.FindControl("hdnId") as HiddenField;

                if (Person.Skills.Count > 0)
                {
                    if (Person.Skills.Where(s => s.Skill.Id == Convert.ToInt32(hdnId.Value)).Count() > 0)
                    {
                        var skill = Person.Skills.Where(s => s.Skill.Id == Convert.ToInt32(hdnId.Value)).First();

                        if (skill != null)
                        {
                            ddlLevel.SelectedValue = skill.SkillLevel.Id.ToString();
                            ddlExperience.SelectedValue = skill.YearsExperience.Value.ToString();
                            ddlLastUsed.SelectedValue = skill.LastUsed.ToString();
                        }
                    }
                }
            }
        }

        protected void gvIndustrySkills_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var ddlExperience = e.Row.FindControl("ddlExperience") as DropDownList;
                var hdnId = e.Row.FindControl("hdnId") as HiddenField;
                if (Person.Industries.Count > 0)
                {
                    if (Person.Industries.Where(i => i.Industry.Id == Convert.ToInt32(hdnId.Value)).Count() > 0)
                    {
                        var industry = Person.Industries.Where(i => i.Industry.Id == Convert.ToInt32(hdnId.Value)).First();

                        if (industry != null)
                        {
                            ddlExperience.SelectedValue = industry.YearsExperience.ToString();
                        }
                    }
                }
            }
        }

        protected void cvSkills_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            var validator = sender as CustomValidator;
            var row = validator.Parent.Parent as GridViewRow;            
            
            var hdnChanged = row.FindControl("hdnChanged") as HiddenField;
            if (hdnChanged != null && hdnChanged.Value == "1")
            {
                var ddlLevel = row.FindControl("ddlLevel") as DropDownList;
                var ddlExperience = row.FindControl("ddlExperience") as DropDownList;
                var ddlLastUsed = row.FindControl("ddlLastUsed") as DropDownList;
                var hdnDescription = row.FindControl("hdnDescription") as HiddenField;

                if (!(ddlLevel.SelectedIndex == 0 && ddlExperience.SelectedIndex == 0 && ddlLastUsed.SelectedIndex == 0))
                {
                    if (ddlLevel.SelectedIndex == 0 || ddlExperience.SelectedIndex == 0 || ddlLastUsed.SelectedIndex == 0)
                    {
                        hdnValidationMessage.Value = (ValidationPopUpMessage == hdnValidationMessage.Value) 
                                                    ? hdnValidationMessage.Value + "\n\r \t" + hdnDescription.Value
                                                    : hdnValidationMessage.Value + ",\n\r \t" + hdnDescription.Value;
                        e.IsValid = false;
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int activeTabIndex = tcSkillsEntry.ActiveTabIndex;
            ValidateAndSave(activeTabIndex);
        }

        protected new void btnCancel_Click(object sender, EventArgs e)
        {
            BindSkills(tcSkillsEntry.ActiveTabIndex);
        }

        private bool ValidateAndSave(int activeTabIndex)
        {
            bool result = false;
            switch (activeTabIndex)
            {
                case 0:
                    Page.Validate(valSummaryBusiness.ValidationGroup);
                    if (Page.IsValid)
                    {
                        result = Page.IsValid;
                        SaveBusinessSkills();
                    }
                    break;
                case 1:
                    Page.Validate(valSummaryTechnical.ValidationGroup);
                    if (Page.IsValid)
                    {
                        result = Page.IsValid;
                        SaveTechnicalSkills();
                    }
                    break;
                case 2:
                    SaveIndustrySkills();
                    result = true;
                    break;
            }
            EnableSaveAndCancelButtons(!result);
            hdnIsValid.Value = result.ToString().ToLower();
            return result;
        }

        private void EnableSaveAndCancelButtons(bool enable)
        {
            btnSave.Enabled = enable;
            btnCancel.Enabled = enable;
        }

        private void RenderSkills(int activeTabIndex)
        {
            if (activeTabIndex == 0 || activeTabIndex == 1)
            {
                var categories = Utils.SettingsHelper.GetSkillCategoriesByType(activeTabIndex + 1);

                switch (activeTabIndex)
                {
                    case 0:
                        if (ddlBusinessCategory.DataSource == null && ddlBusinessCategory.Items.Count == 0)
                        {
                            ddlBusinessCategory.DataSource = categories;
                            ddlBusinessCategory.DataBind();
                            ddlBusinessCategory.SelectedIndex = 0;
                        }
                        ddlBusinessCategory.Attributes.Add("PreviousSelected", ddlBusinessCategory.SelectedIndex.ToString());
                        break;

                    case 1:
                        if (ddlTechnicalCategory.DataSource == null && ddlTechnicalCategory.Items.Count == 0)
                        {
                            ddlTechnicalCategory.DataSource = categories;
                            ddlTechnicalCategory.DataBind();
                            ddlTechnicalCategory.SelectedIndex = 0;
                        }
                        ddlTechnicalCategory.Attributes.Add("PreviousSelected", ddlTechnicalCategory.SelectedIndex.ToString());
                        break;
                }
            }

            BindSkills(activeTabIndex);
        }

        private void BindSkills(int activeTabIndex)
        {
            switch (activeTabIndex)
            {
                case 0:
                    gvBusinessSkills.DataSource = Utils.SettingsHelper.GetSkillsByCategory(SelectedBusinessCategory);
                    gvBusinessSkills.DataBind();
                    PreviousCategoryIndex = ddlBusinessCategory.SelectedIndex;
                    break;
                case 1:
                    gvTechnicalSkills.DataSource = Utils.SettingsHelper.GetSkillsByCategory(SelectedTechnicalCategory);
                    gvTechnicalSkills.DataBind();
                    PreviousCategoryIndex = ddlTechnicalCategory.SelectedIndex;
                    break;
                case 2:
                    gvIndustrySkills.DataSource = Utils.SettingsHelper.GetIndustrySkillsAll();
                    gvIndustrySkills.DataBind();
                    break;
            }
            PreviousActiveTabIndex = activeTabIndex;
        }

        private void SaveTechnicalSkills()
        {
            var rows = gvTechnicalSkills.Rows;
            SaveBusinessORTechnicalSkills(rows);
        }

        private void SaveBusinessSkills()
        {
            var rows = gvBusinessSkills.Rows;
            SaveBusinessORTechnicalSkills(rows);
        }

        private void SaveBusinessORTechnicalSkills(GridViewRowCollection rows)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Skills");

            foreach (GridViewRow row in rows)
            {
                var hdnChanged = row.FindControl("hdnChanged") as HiddenField;
                if (hdnChanged != null && hdnChanged.Value == "1")
                {
                    var ddlLevel = row.FindControl("ddlLevel") as DropDownList;
                    var ddlExperience = row.FindControl("ddlExperience") as DropDownList;
                    var ddlLastUsed = row.FindControl("ddlLastUsed") as DropDownList;
                    var hdnId = row.FindControl("hdnId") as HiddenField;
                    int skillId = Convert.ToInt32(hdnId.Value);

                    XmlElement skillTag = doc.CreateElement("Skill");

                    skillTag.SetAttribute("Id", hdnId.Value);
                    skillTag.SetAttribute("Level", ddlLevel.SelectedValue);
                    skillTag.SetAttribute("Experience", ddlExperience.SelectedValue);
                    skillTag.SetAttribute("LastUsed", ddlLastUsed.SelectedValue);

                    root.AppendChild(skillTag);

                }
            }

            doc.AppendChild(root);
            string skillsXml = doc.InnerXml;

            using (var serviceClient = new PersonSkillService.PersonSkillServiceClient())
            {
                try
                {
                    serviceClient.SavePersonSkills(Person.Id.Value, skillsXml, User.Identity.Name);
                    Session[SessionPersonWithSkills] = null;

                    EnableSaveAndCancelButtons(false);
                    ClearDirty();
                }
                catch
                {
                    serviceClient.Abort();
                }
            }
        }

        private void SaveIndustrySkills()
        {
            var rows = gvIndustrySkills.Rows;

            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Skills");

            foreach (GridViewRow row in rows)
            {
                var hdnChanged = row.FindControl("hdnChanged") as HiddenField;
                if (hdnChanged != null && hdnChanged.Value == "1")
                {
                    var ddlExperience = row.FindControl("ddlExperience") as DropDownList;
                    var hdnId = row.FindControl("hdnId") as HiddenField;
                    int industryId = Convert.ToInt32(hdnId.Value);

                    XmlElement skillTag = doc.CreateElement("IndustrySkill");

                    skillTag.SetAttribute("Id", hdnId.Value);
                    skillTag.SetAttribute("Experience", ddlExperience.SelectedValue);

                    root.AppendChild(skillTag);

                }
            }

            doc.AppendChild(root);
            string skillsXml = doc.InnerXml;

            using (var serviceClient = new PersonSkillService.PersonSkillServiceClient())
            {
                try
                {
                    serviceClient.SavePersonIndustrySkills(Person.Id.Value, skillsXml, User.Identity.Name);
                    Session[SessionPersonWithSkills] = null;

                    EnableSaveAndCancelButtons(false);
                    ClearDirty();
                }
                catch
                {
                    serviceClient.Abort();
                }
            }
        }
        
        #region ObjectDataSource Select Methods

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static List<NameValuePair> GetExperiences()
        {
            var experience = new List<NameValuePair>();
            for (var index = 0; index <= 30; index++)
            {
                var item = new NameValuePair();
                item.Id = index;
                if (index == 1)
                {
                    item.Name = "1 Year";
                }
                else if (index > 1)
                {
                    item.Name = index.ToString() + " Years";
                }

                experience.Add(item);
            }
            return experience;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static List<NameValuePair> GetLastUsedYears()
        {
            var years = new List<NameValuePair>();
            var currentYear = SettingsHelper.GetCurrentPMTime().Year;
            var emptyItem = new NameValuePair();
            emptyItem.Id = 0;
            years.Add(emptyItem);
            for (var index = 1990; index <= currentYear; index++)
            {
                var item = new NameValuePair();
                item.Id = index;
                item.Name = index.ToString();
                years.Add(item);
            }
            return years;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static List<SkillLevel> GetSkillLevels()
        {
            var Skills = new List<SkillLevel>();
            Skills.Add(new SkillLevel { Id = 0 });
            Skills.AddRange(SettingsHelper.GetSkillLevels());
            return Skills;
        }

        #endregion

        public void RaisePostBackEvent(string eventArgument)
        {
            if (IsDirty)
            {
                if (SaveDirty && !ValidateAndSave(PreviousActiveTabIndex))
                {
                    return;
                }
            }
            Redirect(eventArgument == string.Empty ? Constants.ApplicationPages.OpportunityList : eventArgument);
        }
    }
}
