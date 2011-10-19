using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Configuration;
using PraticeManagement.Controls;
using DataTransferObjects;

namespace PraticeManagement
{
    public partial class SkillsSearch : PracticeManagementPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var companyTitle = HttpUtility.HtmlDecode(BrandingConfigurationManager.GetCompanyTitle());
                lblSearchTitle.Text = string.Format("{0} Employee Skills Search", companyTitle);
                DataHelper.FillPersonList(ddlEmployees, null, (int)PersonStatusType.Active);
            }
        }

        protected override void Display()
        {

        }

        protected void btnSearch_OnClick(object sender, EventArgs e)
        {
            if (!pnlSearchResults.Visible)
            {
                pnlSearchResults.Visible = true;
                var companyTitle = HttpUtility.HtmlDecode(BrandingConfigurationManager.GetCompanyTitle());
                lblSearchResultsTitle.Text = string.Format("{0} Employee Skills Search Results", companyTitle);
            }
            
            List<Person> persons = new List<Person>();

            for (int i = 1; i <= 10; i++)
            {
                var person = new Person()
                {
                    Id = i,
                    LastName = "Last Name" + i.ToString(),
                    FirstName = "First Name" + i.ToString()
                };
                persons.Add(person);
            }

            dlPerson.DataSource = persons;
            dlPerson.DataBind();

            if (persons.Any())
            {
                lblSearchcriteria.Text = string.Format("Employees with at least one of the following skills: {0}",
                        txtSearch.Text);
            }
            else
            {
                lblSearchcriteria.Text = string.Format("There are no Employees with following skills: {0}",
                        txtSearch.Text);
            }
        }

        protected void btnEmployeeOK_OnClick(object sender, EventArgs e)
        {

        }
    }
}
