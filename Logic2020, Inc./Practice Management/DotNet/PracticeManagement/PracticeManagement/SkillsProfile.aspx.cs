using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using DataTransferObjects.Skills;
using System.Web.Security;

namespace PraticeManagement
{
    public partial class SkillsProfile : System.Web.UI.Page
    {
        #region Constants

        private const string ViewStatePersonWithSkills = "PersonWithSkills";

        #endregion

        #region Datamembers

        private List<Industry> industries;

        private List<SkillType> skillTypes;

        private List<SkillLevel> skillLevels;

        #endregion

        #region Properties

        public List<SkillCategory> SkillCategories
        {
            get;
            set;
        }

        public List<Skill> Skills
        {
            get;
            set;
        }

        public List<SkillLevel> SkillLevels
        {
            get
            {
                if (skillLevels == null)
                {
                    skillLevels = Utils.SettingsHelper.GetSkillLevels();
                }
                return skillLevels;
            }
        }

        public List<SkillType> SkillTypes
        {
            get
            {
                if (skillTypes == null)
                {
                    skillTypes = Utils.SettingsHelper.GetSkillTypes();
                }
                return skillTypes;
            }
        }

        public List<Industry> Industries
        {
            get
            {
                if (industries == null)
                {
                    industries = Utils.SettingsHelper.GetIndustrySkillsAll();
                }
                return industries;
            }
        }

        public int? PersonId
        {
            get
            {
                int personId;
                if (!string.IsNullOrEmpty(Page.Request.QueryString["id"]) && int.TryParse(Page.Request.QueryString["id"], out personId))
                {
                    return personId;
                }
                else
                {
                    return null;
                }
            }
        }

        public Person Person
        {
            get
            {
                int personId;
                if (PersonId.HasValue)
                {
                    personId = PersonId.Value;
                }
                else if (DataHelper.CurrentPerson != null)
                {
                    personId = DataHelper.CurrentPerson.Id.Value;
                }
                else
                {
                    return null;
                }
                if (ViewState[ViewStatePersonWithSkills] == null)
                {
                    using (var serviceClient = new PersonSkillService.PersonSkillServiceClient())
                    {
                        ViewState[ViewStatePersonWithSkills] = serviceClient.GetPersonProfilesWithSkills(personId);
                    }
                }
                return (Person)ViewState[ViewStatePersonWithSkills];
            }
            set
            {
                ViewState[ViewStatePersonWithSkills] = value;
            }
        }

        #endregion

        #region Events

        protected void repIndustries_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = e.Item.DataItem as PersonIndustry;
                var lblIndustryDesc = e.Item.FindControl("lblIndustryDesc") as Label;
                var lblExp = e.Item.FindControl("lblExp") as Label;
                lblIndustryDesc.Text = Industries.First(i => i.Id == dataItem.Industry.Id).Description;
                lblExp.Text = dataItem.YearsExperience.ToString().Length == 1 ? "0" + dataItem.YearsExperience.ToString() : dataItem.YearsExperience.ToString();
            }

        }

        protected void repIndustries_OnLoad(object sender, EventArgs e)
        {
            if (repIndustries.Items.Count == 0)
            {
                var lblInduatriesMsg = repIndustries.Controls[0].Controls[0].FindControl("lblInduatriesMsg") as Label;
                lblInduatriesMsg.Text = "No industry experience entered.";
                lblInduatriesMsg.Visible = true;
            }
            else
            {
                var lblInduatriesMsg = repIndustries.Controls[0].Controls[0].FindControl("lblInduatriesMsg") as Label;
                lblInduatriesMsg.Visible = false;
            }
        }

        protected void repTypes_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = e.Item.DataItem as SkillType;
                var repCategories = e.Item.FindControl("repCategories") as Repeater;
                var lblTypes = e.Item.FindControl("lblTypes") as Label;
                var lblTypesMsg = e.Item.FindControl("lblTypesMsg") as Label;
                //if (dataItem.DisplayOrder == 1)
                //{
                var lblLevel = e.Item.FindControl("lblLevel") as Label;
                var lblLastUsed = e.Item.FindControl("lblLastUsed") as Label;
                var lblYearsUsed = e.Item.FindControl("lblYearsUsed") as Label;
                lblLevel.Text = "Level";
                lblLastUsed.Text = "Last Used";
                lblYearsUsed.Text = "Years Used";
                //}
                lblTypes.Text = dataItem.Description;
                if (Person.Skills.Any(s => s.Skill.Category.SkillType.Id == dataItem.Id))
                {
                    lblTypesMsg.Visible = false;
                    SkillCategories = (Utils.SettingsHelper.GetSkillCategoriesByType(dataItem.Id)).OrderBy(sc => sc.Description).ToList();
                    var categories = Person.Skills.FindAll(s => s.Skill.Category.SkillType.Id == dataItem.Id).Select(s => s.Skill.Category.Id).Distinct();
                    repCategories.DataSource = categories;
                    repCategories.DataBind();
                }
                else
                {
                    lblTypesMsg.Text = "No " + dataItem.Description.ToLower() + " skills entered.";
                    lblTypesMsg.Visible = true;
                }
            }
        }

        protected void repCategories_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = Convert.ToInt32(e.Item.DataItem);
                var repSkills = e.Item.FindControl("repSkills") as Repeater;
                var lblCategories = e.Item.FindControl("lblCategories") as Label;
                Skills = Utils.SettingsHelper.GetSkillsByCategory(dataItem).OrderBy(s => s.Description).ToList();
                lblCategories.Text = SkillCategories.First(s => s.Id == dataItem).Description;
                repSkills.DataSource = Person.Skills.FindAll(s => s.Skill.Category.Id == dataItem);
                repSkills.DataBind();
            }
        }

        protected void repSkills_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = e.Item.DataItem as PersonSkill;
                var lblSkill = e.Item.FindControl("lblSkill") as Label;
                var lblSkillLevel = e.Item.FindControl("lblSkillLevel") as Label;
                var lblLastUsed = e.Item.FindControl("lblLastUsed") as Label;
                var lblYearsUsed = e.Item.FindControl("lblYearsUsed") as Label;

                lblSkillLevel.Text = SkillLevels.First(s => s.Id == dataItem.SkillLevel.Id).Description;
                lblSkill.Text = Skills.First(s => s.Id == dataItem.Skill.Id).Description;
                lblLastUsed.Text = dataItem.LastUsed.ToString();
                lblYearsUsed.Text = dataItem.YearsExperience.ToString().Length == 1 ? "0" + dataItem.YearsExperience.ToString() : dataItem.YearsExperience.ToString();
            }
        }

        protected void repProfiles_OnLoad(object sender, EventArgs e)
        {
            var lblProfileMsg = repProfiles.Controls[0].Controls[0].FindControl("lblProfileMsg") as Label;
            if (repProfiles.Items.Count == 0)
            {
                lblProfileMsg.Text = "Consultant profile not entered.";
                lblProfileMsg.Visible = true;
            }
            else
            {
                lblProfileMsg.Visible = false;
            }
        }

        #endregion

        #region page_events

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (!PersonId.HasValue && DataHelper.CurrentPerson == null)
                {
                    Response.Redirect(Constants.ApplicationPages.AccessDeniedPage);
                }
                lblPersonName.Text = Person.LastName + " " + Person.FirstName;

                if (Person.HasPicture)
                {
                    imgPersonPicture.ImageUrl = string.Format(Constants.ApplicationPages.PersonPictureHandlerFormat, Person.Id.Value);
                }
                
                imgPersonPicture.AlternateText = lblPersonName.Text;
                repProfiles.DataSource = Person.Profiles;
                repProfiles.DataBind();
                repIndustries.DataSource = Person.Industries;
                repIndustries.DataBind();
                repTypes.DataSource = SkillTypes;
                repTypes.DataBind();
                if (PersonId.HasValue && (DataHelper.CurrentPerson == null || (PersonId != DataHelper.CurrentPerson.Id && !(Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName)))))
                {
                    btnUpdate.Visible = false;
                }
                else
                {
                    btnUpdate.PostBackUrl = PersonId.HasValue ? string.Format(Constants.ApplicationPages.SkillsEntryPageFormat, PersonId) : Constants.ApplicationPages.SkillsEntryPage;
                }
            }
        }

        #endregion

    }
}

