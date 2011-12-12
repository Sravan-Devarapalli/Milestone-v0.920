﻿using System;
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
                DataHelper.FillPersonListWithPersonFirstLastName(ddlEmployees, null, null, (int)PersonStatusType.Active);
            }
        }

        protected override void Display()
        {

        }
        public string GetPersonFirstLastName(Person person)
        {
            return person.FirstName + ", " + person.LastName;
        }
        protected void btnSearch_OnClick(object sender, EventArgs e)
        {
            if (!pnlSearchResults.Visible)
            {
                pnlSearchResults.Visible = true;
                var companyTitle = HttpUtility.HtmlDecode(BrandingConfigurationManager.GetCompanyTitle());
                lblSearchResultsTitle.Text = string.Format("{0} Employee Skills Search Results", companyTitle);
            }


            Person[] persons;

            using (var service = new PersonSkillService.PersonSkillServiceClient())
            {
                persons = service.PersonsSearchBySkillsText(txtSearch.Text);
            }

            dlPerson.DataSource = persons.OrderBy(p => p.FirstName).ThenBy(p => p.LastName);
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

        protected String GetSkillProfileUrl(string personId)
        {
            return "~/SkillsProfile.aspx?Id=" + personId; 
        }

        protected void btnEmployeeOK_OnClick(object sender, EventArgs e)
        {
            String personId = ddlEmployees.SelectedValue; 
            Response.Redirect("~/SkillsProfile.aspx?Id=" + personId);
        }
    }
}

