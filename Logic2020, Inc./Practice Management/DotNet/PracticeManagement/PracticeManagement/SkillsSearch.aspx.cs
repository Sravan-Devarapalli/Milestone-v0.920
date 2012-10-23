using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Configuration;
using PraticeManagement.Controls;
using DataTransferObjects;
using System.Collections;
using DataTransferObjects.Skills;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.ComponentModel;
using PraticeManagement.Utils;
using System.Drawing;

namespace PraticeManagement
{
    public partial class SkillsSearch : PracticeManagementPageBase
    {
        #region Constants

        private const string ddlLevelId = "ddlLevel";
        private const string ddlCategoryId = "ddlCategory";
        private const string ddlSkillId = "ddlSkill";
        private const string hdRowNoId = "hdRowNo";
        private const string lnkbtnClearId = "lnkbtnClear";
        private const string lnkbtnDeleteId = "lnkbtnDelete";
        private const string ddlIndustryId = "ddlIndustry";
        private const string ViewStatePreviousActiveTabIndex = "PreviousActiveTabIndex";
        private const string SkillXml = "<Skill CatagoryId='{0}' Id='{1}' LevelId='{2}' RowId ='{3}'> </Skill>";
        private const string IndustryXml = "<Industry  Id='{0}' RowId ='{1}'> </Industry>";
        private const string CatagoryIdXmlAttribute = "CatagoryId";
        private const string SkillIdXmlAttribute = "Id";
        private const string LevelIdXmlAttribute = "LevelId";
        private const string RowIdXmlAttribute = "RowId";
        private const string SkillXmlElement = "Skill";
        private const string IndustryXmlElement = "Industry";
        private const string SearchResultCountMessage = "There were {0} matches found based on the criteria entered. ";
        private const string HighlightMessage = "Highlighted names reflect profiles updated in the last 4 months.";
        #endregion

        private bool IsHighlightedMessage = false;

        #region Properties

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

        /*
            <Skill CatagoryId='' Id='' LevelId='' > </Skill>
            <Skill CatagoryId='' Id='' LevelId='' > </Skill>
             ...
         */
        public string BusinessSkillsXml
        {
            get
            {
                if (ViewState["BusinessSkillsXml"] == null)
                {
                    ViewState["BusinessSkillsXml"] = "<Skill CatagoryId='0' Id='0' LevelId='0' RowId ='1' > </Skill>";
                }
                return ViewState["BusinessSkillsXml"] as string;
            }
            set
            {
                ViewState["BusinessSkillsXml"] = value;
            }
        }

        /*
            <Skill CatagoryId='' Id='' LevelId='' > </Skill>
            <Skill CatagoryId='' Id='' LevelId='' > </Skill>
             ...
         */
        public string TechnicalSkillsXml
        {
            get
            {
                if (ViewState["TechnicalSkillsXml"] == null)
                {
                    ViewState["TechnicalSkillsXml"] = "<Skill CatagoryId='0' Id='0' LevelId='0' RowId ='1' > </Skill>";
                }
                return ViewState["TechnicalSkillsXml"] as string;
            }
            set
            {
                ViewState["TechnicalSkillsXml"] = value;
            }
        }

        /*
           <Industry  Id=''> </Industry>
            <Industry  Id=''> </Industry>
            ...
        */
        public string IndustriesXml
        {
            get
            {
                if (ViewState["IndustriesXml"] == null)
                {
                    ViewState["IndustriesXml"] = "<Industry  Id='0' RowId ='1'> </Industry>";
                }
                return ViewState["IndustriesXml"] as string;
            }
            set
            {
                ViewState["IndustriesXml"] = value;
            }
        }

        public List<SkillCategory> BusinessCategories
        {
            get
            {
                List<SkillCategory> _businessCategory = new List<SkillCategory>();
                _businessCategory.Add(new SkillCategory() { Id = 0, Description = string.Empty });
                _businessCategory.AddRange(Utils.SettingsHelper.GetSkillCategoriesByType(1));
                return _businessCategory;
            }
        }

        public List<SkillCategory> TechnicalCategories
        {
            get
            {
                List<SkillCategory> _technicalCategory = new List<SkillCategory>();
                _technicalCategory.Add(new SkillCategory() { Id = 0, Description = string.Empty });
                _technicalCategory.AddRange(Utils.SettingsHelper.GetSkillCategoriesByType(2));
                return _technicalCategory;
            }
        }

        public List<Industry> Industries
        {
            get
            {
                List<Industry> _industries = new List<Industry>();
                _industries.Add(new Industry() { Id = 0, Description = string.Empty });
                _industries.AddRange(Utils.SettingsHelper.GetIndustrySkillsAll().ToList());
                return _industries;
            }
        }

        public string SkillsXml
        {
            get
            {
                StringBuilder xml = new StringBuilder();
                xml.Append("<Skills>");
                xml.Append(BusinessSkillsXml);
                xml.Append(TechnicalSkillsXml);
                xml.Append(IndustriesXml);
                xml.Append("</Skills>");
                return xml.ToString();
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var companyTitle = HttpUtility.HtmlDecode(BrandingConfigurationManager.GetCompanyTitle());
                lblSearchTitle.Text = string.Format("{0} Employee Skills Search", companyTitle);
                string statusIds = (int)PersonStatusType.Active + " , " + (int)PersonStatusType.TerminationPending;
                DataHelper.FillPersonList(ddlEmployees, "Select an Employee", statusIds);
                BindSkills(tcSkillsEntry.ActiveTabIndex);
            }


        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            EnableSearchAndClearButtons();
            btnEmployeeOK.Enabled = ddlEmployees.SelectedIndex != 0;
            if (IsHighlightedMessage)
            {
                lblSearchcriteria.Text = lblSearchcriteria.Text + HighlightMessage;
            }
        }

        protected override void Display()
        {

        }

        #endregion

        #region Control Events

        protected void btnSearch_OnClick(object sender, EventArgs e)
        {
            SaveSkillsToXml(tcSkillsEntry.ActiveTabIndex);
            if (!pnlSearchResults.Visible)
            {
                pnlSearchResults.Visible = true;
                var companyTitle = HttpUtility.HtmlDecode(BrandingConfigurationManager.GetCompanyTitle());
                lblSearchResultsTitle.Text = string.Format("{0} Employee Skills Search Results", companyTitle);
            }


            Person[] persons = null;

            using (var service = new PersonSkillService.PersonSkillServiceClient())
            {
                persons = service.PersonsSearchBySkills(SkillsXml);
            }

            dlPerson.DataSource = persons;
            dlPerson.DataBind();
            lblSearchcriteria.Text = string.Format(SearchResultCountMessage, persons.Count());
            dlPerson.Visible = persons.Any();
            UpdateClearButtons(gvIndustrySkills);
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

        protected void dlPerson_OnItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var row = e.Item as DataListItem;
                var person = e.Item.DataItem as DataTransferObjects.Person;

                var hlPersonSkillProfile = row.FindControl("hlPersonSkillProfile") as HyperLink;
                hlPersonSkillProfile.NavigateUrl = "~/SkillsProfile.aspx?Id=" + person.Id;
                hlPersonSkillProfile.Text = person.LastName + ", " + person.FirstName;
                var lbPersonProfile = row.FindControl("lbPersonProfile") as Label;
                var hlPersonProfile = row.FindControl("hlPersonProfile") as HyperLink;
                if (person.Profiles.Any())
                {
                    hlPersonProfile.NavigateUrl = person.Profiles.First().ProfileUrl;
                    hlPersonProfile.Text = person.Profiles.First().ProfileName;
                    lbPersonProfile.Visible = false;
                    hlPersonProfile.Visible = true;
                }
                else
                {
                    lbPersonProfile.Visible = true;
                    hlPersonProfile.Visible = false;
                }
                if (person.IsHighlighted)
                {
                    IsHighlightedMessage = true;
                    hlPersonSkillProfile.BackColor = Color.Yellow;
                }
            }
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddlCategory = sender as DropDownList;
            int categoryId = 0;
            List<Skill> skills = new List<Skill>();
            var row = ddlCategory.NamingContainer as GridViewRow;
            var gridView = row.NamingContainer as GridView;
            var ddlSkill = row.FindControl(ddlSkillId) as DropDownList;
            var ddlLevel = row.FindControl(ddlLevelId) as DropDownList;

            if (int.TryParse(ddlCategory.SelectedValue, out categoryId))
            {
                skills = Utils.SettingsHelper.GetSkillsByCategory(categoryId);
            }
            skills.Add(new Skill() { Id = 0, Description = "" });
            skills = skills.OrderBy(s => s.Id).ToList();
            ddlSkill.DataSource = skills;
            ddlSkill.DataBind();
            ddlLevel.DataSource = GetSkillLevels(ddlCategory.SelectedIndex == 0);
            ddlLevel.DataBind();
            UpdateClearButtons(gridView);
            SaveSkillsToXml(tcSkillsEntry.ActiveTabIndex);

        }

        protected void gvSkills_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var skillElement = e.Row.DataItem as XElement;
                var ddlCategory = e.Row.FindControl(ddlCategoryId) as DropDownList;
                var ddlSkill = e.Row.FindControl(ddlSkillId) as DropDownList;
                var ddlLevel = e.Row.FindControl(ddlLevelId) as DropDownList;
                var hdRowNo = e.Row.FindControl(hdRowNoId) as HiddenField;
                var clearLink = e.Row.FindControl(lnkbtnClearId) as LinkButton;
                var deleterLink = e.Row.FindControl(lnkbtnDeleteId) as LinkButton;
                List<SkillCategory> catagories = ddlCategory.Attributes["isBusiness"] == "true" ? BusinessCategories : TechnicalCategories;
                ddlCategory.DataSource = catagories;
                ddlCategory.DataBind();
                int categoryId;
                int skillId = int.Parse(skillElement.Attribute(SkillIdXmlAttribute).Value);
                int levelId = int.Parse(skillElement.Attribute(LevelIdXmlAttribute).Value);
                int rowId = int.Parse(skillElement.Attribute(RowIdXmlAttribute).Value);

                List<Skill> skills = new List<Skill>();
                if (int.TryParse(skillElement.Attribute(CatagoryIdXmlAttribute).Value, out categoryId))
                {
                    skills = Utils.SettingsHelper.GetSkillsByCategory(categoryId);
                }
                skills.Add(new Skill() { Id = 0, Description = "" });
                skills = skills.OrderBy(s => s.Id).ToList();
                ddlSkill.DataSource = skills;
                ddlSkill.DataBind();


                ddlCategory.SelectedValue = categoryId.ToString();
                ddlSkill.SelectedValue = skillId.ToString();
                ddlLevel.SelectedValue = levelId.ToString();
                hdRowNo.Value = rowId.ToString();
                deleterLink.Visible = rowId != 1;

                ddlLevel.DataSource = GetSkillLevels(ddlCategory.SelectedIndex == 0);
                ddlLevel.DataBind();

                if (ddlCategory.SelectedIndex != 0)
                {
                    clearLink.Enabled = true;
                    clearLink.Attributes["disable"] = false.ToString();
                    System.Drawing.Color blue = System.Drawing.ColorTranslator.FromHtml("#0898E6");
                    clearLink.ForeColor = blue;
                }
                else
                {
                    clearLink.Enabled = false;
                    clearLink.Attributes["disable"] = true.ToString();
                    System.Drawing.Color gray = System.Drawing.ColorTranslator.FromHtml("#8F8F8F");
                    clearLink.ForeColor = gray;
                }
            }
        }

        protected void gvIndustrySkills_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var skillElement = e.Row.DataItem as XElement;
                var ddlIndustry = e.Row.FindControl(ddlIndustryId) as DropDownList;
                var hdRowNo = e.Row.FindControl(hdRowNoId) as HiddenField;
                var clearLink = e.Row.FindControl(lnkbtnClearId) as LinkButton;
                var deleterLink = e.Row.FindControl(lnkbtnDeleteId) as LinkButton;
                ddlIndustry.DataSource = Industries;
                ddlIndustry.DataBind();
                int industryId = int.Parse(skillElement.Attribute(SkillIdXmlAttribute).Value);
                int rowId = int.Parse(skillElement.Attribute(RowIdXmlAttribute).Value);
                hdRowNo.Value = rowId.ToString();
                deleterLink.Visible = rowId != 1;
                ddlIndustry.SelectedValue = industryId.ToString();

                if (ddlIndustry.SelectedIndex != 0)
                {
                    clearLink.Enabled = true;
                    clearLink.Attributes["disable"] = false.ToString();
                    System.Drawing.Color blue = System.Drawing.ColorTranslator.FromHtml("#0898E6");
                    clearLink.ForeColor = blue;
                }
                else
                {
                    clearLink.Enabled = false;
                    clearLink.Attributes["disable"] = true.ToString();
                    System.Drawing.Color gray = System.Drawing.ColorTranslator.FromHtml("#8F8F8F");
                    clearLink.ForeColor = gray;
                }
            }
        }

        protected void tcSkillsEntry_ActiveTabChanged(object sender, EventArgs e)
        {
            SaveSkillsToXml(PreviousActiveTabIndex);
            BindSkills(tcSkillsEntry.ActiveTabIndex);
        }

        protected void lnkbtnDelete_Click(object sender, EventArgs e)
        {
            SaveSkillsToXml(tcSkillsEntry.ActiveTabIndex);
            var deleteLink = sender as LinkButton;
            var row = deleteLink.NamingContainer as GridViewRow;
            var hdRowNo = row.FindControl(hdRowNoId) as HiddenField;
            int rowId = int.Parse(hdRowNo.Value);
            string xml = tcSkillsEntry.ActiveTabIndex == 0 ? BusinessSkillsXml : tcSkillsEntry.ActiveTabIndex == 1 ? TechnicalSkillsXml : IndustriesXml;
            XDocument xdoc = XDocument.Parse("<Skills>" + xml + "</Skills>");
            var skillElements = tcSkillsEntry.ActiveTabIndex == 2 ? xdoc.Descendants(XName.Get(IndustryXmlElement)).ToList() : xdoc.Descendants(XName.Get(SkillXmlElement)).ToList();
            if (skillElements.Any(p => p.Attribute(RowIdXmlAttribute).Value == rowId.ToString()))
            {
                var element = skillElements.First(p => p.Attribute(RowIdXmlAttribute).Value == rowId.ToString());
                skillElements.Remove(element);
                if (tcSkillsEntry.ActiveTabIndex == 0)
                {
                    BusinessSkillsXml = GetXmlFromXelementList(skillElements, tcSkillsEntry.ActiveTabIndex);
                }
                else if (tcSkillsEntry.ActiveTabIndex == 1)
                {
                    TechnicalSkillsXml = GetXmlFromXelementList(skillElements, tcSkillsEntry.ActiveTabIndex);
                }
                else
                {
                    IndustriesXml = GetXmlFromXelementList(skillElements, tcSkillsEntry.ActiveTabIndex);
                }

            }
            BindSkills(tcSkillsEntry.ActiveTabIndex);
        }

        protected void lnkbtnClear_Click(object sender, EventArgs e)
        {
            SaveSkillsToXml(tcSkillsEntry.ActiveTabIndex);
            var clearLink = sender as LinkButton;
            var row = clearLink.NamingContainer as GridViewRow;
            var hdRowNo = row.FindControl(hdRowNoId) as HiddenField;
            int rowId = int.Parse(hdRowNo.Value);
            string xml = tcSkillsEntry.ActiveTabIndex == 0 ? BusinessSkillsXml : tcSkillsEntry.ActiveTabIndex == 1 ? TechnicalSkillsXml : IndustriesXml;
            XDocument xdoc = XDocument.Parse("<Skills>" + xml + "</Skills>");
            var skillElements = tcSkillsEntry.ActiveTabIndex == 2 ? xdoc.Descendants(XName.Get(IndustryXmlElement)).ToList() : xdoc.Descendants(XName.Get(SkillXmlElement)).ToList();
            if (skillElements.Any(p => p.Attribute(RowIdXmlAttribute).Value == rowId.ToString()))
            {
                var element = skillElements.First(p => p.Attribute(RowIdXmlAttribute).Value == rowId.ToString());
                if (tcSkillsEntry.ActiveTabIndex == 2)
                {
                    element.Attribute(XName.Get(SkillIdXmlAttribute)).Value = "0";
                }
                else
                {
                    element.Attribute(XName.Get(CatagoryIdXmlAttribute)).Value =
                    element.Attribute(XName.Get(SkillIdXmlAttribute)).Value =
                    element.Attribute(XName.Get(LevelIdXmlAttribute)).Value = "0";
                }
                if (tcSkillsEntry.ActiveTabIndex == 0)
                {
                    BusinessSkillsXml = GetXmlFromXelementList(skillElements, tcSkillsEntry.ActiveTabIndex);
                }
                else if (tcSkillsEntry.ActiveTabIndex == 1)
                {
                    TechnicalSkillsXml = GetXmlFromXelementList(skillElements, tcSkillsEntry.ActiveTabIndex);
                }
                else
                {
                    IndustriesXml = GetXmlFromXelementList(skillElements, tcSkillsEntry.ActiveTabIndex);
                }
            }
            BindSkills(tcSkillsEntry.ActiveTabIndex);
        }

        protected void lnkbtnAddSkill_Click(object sender, EventArgs e)
        {
            SaveSkillsToXml(tcSkillsEntry.ActiveTabIndex);
            string xml = tcSkillsEntry.ActiveTabIndex == 0 ? BusinessSkillsXml : tcSkillsEntry.ActiveTabIndex == 1 ? TechnicalSkillsXml : IndustriesXml;
            XDocument xdoc = XDocument.Parse("<Skills>" + xml + "</Skills>");
            var skillElements = tcSkillsEntry.ActiveTabIndex == 2 ? xdoc.Descendants(XName.Get(IndustryXmlElement)).ToList() : xdoc.Descendants(XName.Get(SkillXmlElement)).ToList();
            int maxRowno = skillElements.Max(l => int.Parse(l.Attribute(RowIdXmlAttribute).Value));
            string newRow = tcSkillsEntry.ActiveTabIndex == 2 ? string.Format(IndustryXml, 0, maxRowno + 1) : string.Format(SkillXml, 0, 0, 0, maxRowno + 1);

            if (tcSkillsEntry.ActiveTabIndex == 0)
            {
                BusinessSkillsXml = xml + newRow;
            }
            else if (tcSkillsEntry.ActiveTabIndex == 1)
            {
                TechnicalSkillsXml = xml + newRow;
            }
            else
            {
                IndustriesXml = xml + newRow;
            }
            BindSkills(tcSkillsEntry.ActiveTabIndex);
        }

        protected void btnClearAll_Click(object sender, EventArgs e)
        {
            BusinessSkillsXml = TechnicalSkillsXml = IndustriesXml = null;
            BindSkills(tcSkillsEntry.ActiveTabIndex);
            pnlSearchResults.Visible = false;
        }

        #endregion

        #region Methods

        private void BindSkills(int activeTabIndex)
        {
            switch (activeTabIndex)
            {
                case 0:
                    XDocument xdoc = XDocument.Parse("<Skills>" + BusinessSkillsXml + "</Skills>");
                    var skillElements = xdoc.Descendants(XName.Get(SkillXmlElement)).ToList();
                    gvBusinessSkills.DataSource = skillElements;
                    gvBusinessSkills.DataBind();
                    break;

                case 1:
                    xdoc = XDocument.Parse(("<Skills>" + TechnicalSkillsXml + "</Skills>")); ;
                    skillElements = xdoc.Descendants(XName.Get(SkillXmlElement)).ToList();
                    gvTechnicalSkills.DataSource = skillElements;
                    gvTechnicalSkills.DataBind();
                    break;
                case 2:
                    xdoc = XDocument.Parse("<Skills>" + IndustriesXml + "</Skills>");
                    skillElements = xdoc.Descendants(XName.Get(IndustryXmlElement)).ToList();
                    gvIndustrySkills.DataSource = skillElements;
                    gvIndustrySkills.DataBind();
                    break;
            }
            PreviousActiveTabIndex = activeTabIndex;
        }

        private void SaveSkillsToXml(int tabIndex)
        {
            switch (tabIndex)
            {
                case 0:
                    BusinessSkillsXml = GetXmlFromGridView(gvBusinessSkills, tabIndex);
                    break;
                case 1:
                    TechnicalSkillsXml = GetXmlFromGridView(gvTechnicalSkills, tabIndex);
                    break;
                case 2:
                    IndustriesXml = GetXmlFromGridView(gvIndustrySkills, tabIndex);
                    break;
            }

        }

        private string GetXmlFromGridView(GridView gridView, int tabIndex)
        {
            StringBuilder xml = new StringBuilder();
            int rowId = 1;
            foreach (GridViewRow row in gridView.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    if (tabIndex == 2)
                    {
                        var ddlIndustry = row.FindControl(ddlIndustryId) as DropDownList;
                        xml.Append(string.Format(IndustryXml, ddlIndustry.SelectedValue, rowId));
                    }
                    else
                    {
                        var ddlCategory = row.FindControl(ddlCategoryId) as DropDownList;
                        var ddlSkill = row.FindControl(ddlSkillId) as DropDownList;
                        var ddlLevel = row.FindControl(ddlLevelId) as DropDownList;
                        xml.Append(string.Format(SkillXml, ddlCategory.SelectedValue, ddlSkill.SelectedValue, ddlLevel.SelectedValue, rowId));
                    }
                    rowId++;
                }
            }
            return xml.ToString();
        }

        private string GetXmlFromXelementList(List<XElement> xList, int tabIndex)
        {
            StringBuilder xml = new StringBuilder();
            foreach (var ele in xList)
            {
                if (tabIndex == 2)
                {
                    xml.Append(string.Format(IndustryXml, ele.Attribute(SkillIdXmlAttribute).Value, ele.Attribute(RowIdXmlAttribute).Value));
                }
                else
                {
                    xml.Append(string.Format(SkillXml, ele.Attribute(CatagoryIdXmlAttribute).Value, ele.Attribute(SkillIdXmlAttribute).Value, ele.Attribute(LevelIdXmlAttribute).Value, ele.Attribute(RowIdXmlAttribute).Value));
                }
            }
            return xml.ToString();
        }

        private void UpdateClearButtons(GridView gridView)
        {
            foreach (GridViewRow row in gridView.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    var ddl = row.FindControl(ddlCategoryId) as DropDownList;
                    if (ddl == null)
                    {
                        ddl = row.FindControl(ddlIndustryId) as DropDownList;
                    }
                    var clearLink = row.FindControl(lnkbtnClearId) as LinkButton;
                    if (ddl.SelectedIndex != 0)
                    {
                        clearLink.Enabled = true;
                        clearLink.Attributes["disable"] = false.ToString();
                        System.Drawing.Color blue = System.Drawing.ColorTranslator.FromHtml("#0898E6");
                        clearLink.ForeColor = blue;
                    }
                    else
                    {
                        clearLink.Enabled = false;
                        clearLink.Attributes["disable"] = true.ToString();
                        System.Drawing.Color gray = System.Drawing.ColorTranslator.FromHtml("#8F8F8F");
                        clearLink.ForeColor = gray;
                    }
                }
            }

        }

        private List<SkillLevel> GetSkillLevels(bool onlyFirstItem)
        {
            var Skills = new List<SkillLevel>();
            Skills.Add(new SkillLevel { Id = 0 });
            if (!onlyFirstItem)
                Skills.AddRange(SettingsHelper.GetSkillLevels());
            return Skills;
        }

        private void EnableSearchAndClearButtons()
        {
            XDocument xdoc = XDocument.Parse(SkillsXml);
            var skillElements = xdoc.Descendants(XName.Get(SkillXmlElement)).ToList();
            var industryElements = xdoc.Descendants(XName.Get(IndustryXmlElement)).ToList();
            btnClearAll.Enabled =
            btnSearch.Enabled = skillElements.Any(p => p.Attribute(CatagoryIdXmlAttribute).Value != 0.ToString()) || industryElements.Any(p => p.Attribute(SkillIdXmlAttribute).Value != 0.ToString());
        }


        #endregion
    }
}

