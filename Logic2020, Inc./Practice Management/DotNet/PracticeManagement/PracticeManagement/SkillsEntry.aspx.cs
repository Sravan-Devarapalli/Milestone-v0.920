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
    public partial class SkillsEntry : PracticeManagementPageBase
    {
        #region Constants

        private const string SessionPersonWithSkills = "PersonWithSkills";

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

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblUserName.Text = DataHelper.CurrentPerson.PersonLastFirstName;
                RenderSkills(tcSkillsEntry.ActiveTabIndex);
            }
        }

        protected override void Display()
        {

        }

        protected void tcSkillsEntry_ActiveTabChanged(object sender, EventArgs e)
        {
            RenderSkills(tcSkillsEntry.ActiveTabIndex);
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int activeTabIndex = tcSkillsEntry.ActiveTabIndex;
            if (IsDirty)
            {
                SaveSkills(activeTabIndex);
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int c = tcSkillsEntry.ActiveTabIndex;
            SaveSkills(c);
        }

        private void SaveSkills(int activeTabIndex)
        {
            switch (activeTabIndex)
            {
                case 0:
                    SaveBusinessSkills();
                    break;
                case 1:
                    SaveTechnicalSkills();
                    break;
                case 2:
                    SaveIndustrySkills();
                    break;
            }
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
                    break;
                case 1:
                    gvTechnicalSkills.DataSource = Utils.SettingsHelper.GetSkillsByCategory(SelectedTechnicalCategory);
                    gvTechnicalSkills.DataBind();
                    break;
                case 2:
                    gvIndustrySkills.DataSource = Utils.SettingsHelper.GetIndustrySkillsAll();
                    gvIndustrySkills.DataBind();
                    break;
            }
        }

        private void SaveBusinessSkills()
        {
            var rows = gvBusinessSkills.Rows;
            var newSkills = new List<PersonSkill>();
            var previousSkills = new List<PersonSkill>();
            previousSkills.AddRange(Person.Skills);

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

                    if (ddlLevel.SelectedIndex == 0 && ddlExperience.SelectedIndex == 0 && ddlLastUsed.SelectedIndex == 0)
                    {
                        //delete records.
                        if (previousSkills.Count > 0 && previousSkills.Where(s => s.Skill.Id == skillId).Count() != 0)
                        {
                            int index = previousSkills.FindIndex(s => s.Skill.Id == skillId);
                            previousSkills.RemoveAt(index);
                        }
                    }
                    else
                    {
                        int levelSelected = Convert.ToInt32(ddlLevel.SelectedValue);
                        int experienceSelected = Convert.ToInt32(ddlExperience.SelectedValue);
                        int lastUsedSelected = Convert.ToInt32(ddlLastUsed.SelectedValue);


                        //insert or update.
                        if (previousSkills.Count == 0 || previousSkills.Where(s => s.Skill.Id == skillId).Count() == 0)
                        {
                            //insert.
                            PersonSkill personSkill = new PersonSkill
                            {
                                Skill = new Skill { Id = skillId },
                                SkillLevel = new SkillLevel { Id = levelSelected },
                                YearsExperience = experienceSelected,
                                LastUsed = lastUsedSelected
                            };

                            newSkills.Add(personSkill);
                        }
                        else
                        {
                            //update.
                            var skill = previousSkills.Where(s => s.Skill.Id == skillId).First();
                            skill.SkillLevel.Id = levelSelected;
                            skill.YearsExperience = experienceSelected;
                            skill.LastUsed = lastUsedSelected;
                        }
                    }

                    XmlElement skillTag = doc.CreateElement("Skill");

                    skillTag.SetAttribute("Id", hdnId.Value);
                    skillTag.SetAttribute("Level", ddlLevel.SelectedValue);
                    skillTag.SetAttribute("Experience", ddlExperience.SelectedValue);
                    skillTag.SetAttribute("LastUsed", ddlLastUsed.SelectedValue);

                    root.AppendChild(skillTag);

                }
            }
            previousSkills.AddRange(newSkills);

            doc.AppendChild(root);
            string skillsXml = doc.InnerXml;

            using (var serviceClient = new PersonSkillService.PersonSkillServiceClient())
            {
                try
                {
                    serviceClient.SavePersonSkills(Person.Id.Value, skillsXml);
                    Person.Skills.Clear();
                    Person.Skills.AddRange(previousSkills);

                    ClearDirty();
                }
                catch
                {
                    serviceClient.Abort();
                }
            }
        }

        private void SaveTechnicalSkills()
        {
        }

        private void SaveIndustrySkills()
        {
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
    }
}
