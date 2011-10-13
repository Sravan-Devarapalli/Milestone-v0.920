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
            int c = tcSkillsEntry.ActiveTabIndex;
            BindSkills(c);
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int c = tcSkillsEntry.ActiveTabIndex;
            switch (c)
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
                    break;

                case 1:
                    if (ddlTechnicalCategory.DataSource == null && ddlTechnicalCategory.Items.Count == 0)
                    {
                        ddlTechnicalCategory.DataSource = categories;
                        ddlTechnicalCategory.DataBind();
                        ddlTechnicalCategory.SelectedIndex = 0;
                    }
                    break;

                case 2:
                    break;
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
            }
        }

        private void SaveBusinessSkills()
        {
            var rows = gvBusinessSkills.Rows;

            foreach (GridViewRow row in rows)
            {
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
            emptyItem.Id = null;
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
