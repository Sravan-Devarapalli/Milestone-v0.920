using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.Utils;
using System.ComponentModel;


namespace PraticeManagement
{
    public partial class SkillsEntry : PracticeManagementPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblUserName.Text = DataHelper.CurrentPerson.PersonLastFirstName;
                ddlCategory.DataBind();
                ddlLevel.DataBind();
                ddlExperience.DataBind();
                ddlLastUsed.DataBind();
            }
        }

        protected override void Display()
        {

        }

        protected void odsSkillCategories_OnSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["skillTypeId"] = (tcSkillsEntry.ActiveTabIndex + 1).ToString();
        }

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

    }
}
