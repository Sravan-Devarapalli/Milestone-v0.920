using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using System.Web.Security;
using DataTransferObjects;


namespace PraticeManagement
{
    public partial class DashBoard : PracticeManagementSearchPageBase
    {

        #region Fields

        private bool _userIsAdministrator;
        private bool _userIsClientDirector;
        private bool _userIsConsultant;
        private bool _userIsPracticeAreaManger;
        private bool _userIsProjectLead;
        private bool _userIsRecruiter;
        private bool _userIsHR;
        private bool _userIsSalesperson;
        private bool _userIsSeniorLeadership;

        #endregion


        /// <summary>
        /// Gets a text to be searched for.
        /// </summary>
        public override string SearchText
        {
            get
            {
                return txtSearchText.Text;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            // Security
            InitSecurity();

            if (!IsPostBack)
            {


                PopulateDashBoardTypeDropDown();

                //search section
                PopulateSearchSection();


                //All QuickLinks List
                PopulateAddQuickLinks();

                //Quick links
                PopulateQuickLinksSection();

                PopulateAnnouncement();
            }

        }

        private void PopulateAnnouncement(string text = null)
        {
            string announcement;

            announcement = text ?? GetLatestAnnouncement();

            pnlHtmlAnnounceMent.GroupingText = announcement;

            ckeAnnouncementEditor.Text = announcement;
        }

        private void PopulateQuickLinksSection()
        {
            string dashBoardValue = _userIsAdministrator ? DashBoardType.Admin.ToString() :
                                    (_userIsRecruiter || _userIsHR) ? DashBoardType.Recruiter.ToString() :
                                    _userIsSeniorLeadership ? DashBoardType.SeniorLeadership.ToString() :
                                    _userIsClientDirector ? DashBoardType.ClientDirector.ToString() :
                                    _userIsSalesperson ? DashBoardType.BusinessDevelopment.ToString() :
                                    (_userIsPracticeAreaManger || _userIsProjectLead) ? DashBoardType.Manager.ToString() :
                                    _userIsConsultant ? DashBoardType.Consulant.ToString() :
                                    string.Empty;
            DashBoardType dashBoardtype = (DashBoardType)Enum.Parse(typeof(DashBoardType), _userIsAdministrator ? ddlDashBoardType.SelectedValue : dashBoardValue);
            List<QuickLinks> qlinks = DataHelper.GetQuickLinksByDashBoardType(dashBoardtype);

            repQuickLinks.DataSource = qlinks;
            repQuickLinks.DataBind();

            hdnSelectedQuckLinks.Value = string.Empty;

            var indexes = string.Empty;

            for (int i = 0; i < cblQuickLinks.Items.Count; i++)
            {
                cblQuickLinks.Items[i].Selected = false;
                if (qlinks.Any(v => cblQuickLinks.Items[i].Value == v.VirtualPath))
                {
                    cblQuickLinks.Items[i].Selected = true;
                    indexes += i.ToString() + ",";
                }
            }

            hdnSelectedQuckLinks.Value = indexes;
            txtSearchBox.Text = string.Empty;

        }

        private void PopulateDashBoardTypeDropDown()
        {

            if (!_userIsAdministrator)
            {
                ddlDashBoardType.Visible = false;
                pnlDashBoard.Visible = false;
            }
            else
            {
                ddlDashBoardType.Items.Clear();
                ddlDashBoardType.DataSource = Enum.GetNames(typeof(DashBoardType));
                ddlDashBoardType.DataBind();

                ddlDashBoardType.SelectedValue = DashBoardType.Admin.ToString();
            }
        }

        private void PopulateAddQuickLinks()
        {
            btnAddQuicklink.Visible = _userIsAdministrator;
            if (_userIsAdministrator)
            {
                SiteMapDataSource smdsMain = new SiteMapDataSource();

                Dictionary<string, string> dicQuickLinks = new Dictionary<string, string>();

                dicQuickLinks.Add("Request Time Off", "mailto:{0}?subject=Time Off Request");

                foreach (SiteMapNode childNode in ((System.Web.SiteMapNodeCollection)(smdsMain.Provider.RootNode.ChildNodes)))
                {
                    if (childNode.HasChildNodes)
                    {
                        AddQuckLinks(childNode.ChildNodes, dicQuickLinks);
                    }
                    else
                    {
                        if (childNode["VirtualPath"] != null && childNode["QuickLinkTitle"] != null)
                        {
                            if (!dicQuickLinks.Any(k => k.Key == childNode["QuickLinkTitle"]))
                                dicQuickLinks.Add(childNode["QuickLinkTitle"], childNode["VirtualPath"]);
                        }

                    }
                }

                cblQuickLinks.DataSource = dicQuickLinks;
                cblQuickLinks.DataBind();
            }
        }

        private void AddQuckLinks(SiteMapNodeCollection smncollection, Dictionary<string, string> dicQuickLinks)
        {
            foreach (SiteMapNode childNode in smncollection)
            {
                if (childNode.HasChildNodes)
                {
                    AddQuckLinks(childNode.ChildNodes, dicQuickLinks);
                }
                else
                {
                    if (childNode["VirtualPath"] != null && childNode["QuickLinkTitle"] != null)
                    {
                        if (!dicQuickLinks.Any(k => k.Key == childNode["QuickLinkTitle"]))
                            dicQuickLinks.Add(childNode["QuickLinkTitle"], childNode["VirtualPath"]);
                    }
                }
            }
        }

        private void PopulateSearchSection()
        {
            Dictionary<string, string> listOfItems = new Dictionary<string, string>();

            if (_userIsAdministrator)
            {
                if (ddlDashBoardType.SelectedValue == DashBoardType.Admin.ToString())
                {
                    listOfItems.Add("Project", "Project");
                    listOfItems.Add("Opportunity", "Opportunity");
                    listOfItems.Add("Person", "Person");
                }
                else if (ddlDashBoardType.SelectedValue == DashBoardType.Recruiter.ToString())
                {
                    if (!listOfItems.Any(k => k.Key == "Person"))
                    {
                        listOfItems.Add("Person", "Person");
                    }
                }
                else if (ddlDashBoardType.SelectedValue == DashBoardType.ClientDirector.ToString() ||
                    ddlDashBoardType.SelectedValue == DashBoardType.SeniorLeadership.ToString() ||
                    ddlDashBoardType.SelectedValue == DashBoardType.BusinessDevelopment.ToString())
                {
                    if (!listOfItems.Any(k => k.Key == "Project"))
                    {
                        listOfItems.Add("Project", "Project");
                    }

                    if (!listOfItems.Any(k => k.Key == "Opportunity"))
                    {
                        listOfItems.Add("Opportunity", "Opportunity");
                    }
                }
                else if (ddlDashBoardType.SelectedValue == DashBoardType.Manager.ToString())
                {
                    if (!listOfItems.Any(k => k.Key == "Project"))
                    {
                        listOfItems.Add("Project", "Project");
                    }
                }

            }
            else if (_userIsRecruiter || _userIsHR)
            {
                if (!listOfItems.Any(k => k.Key == "Person"))
                {
                    listOfItems.Add("Person", "Person");
                }
            }
            else if (_userIsClientDirector || _userIsSeniorLeadership || _userIsSalesperson)//new role
            {
                if (!listOfItems.Any(k => k.Key == "Project"))
                {
                    listOfItems.Add("Project", "Project");
                }

                if (!listOfItems.Any(k => k.Key == "Opportunity"))
                {
                    listOfItems.Add("Opportunity", "Opportunity");
                }
            }
            else if (_userIsPracticeAreaManger || _userIsProjectLead)
            {
                if (!listOfItems.Any(k => k.Key == "Project"))
                {
                    listOfItems.Add("Project", "Project");
                }
            }

            ddlSearchType.Items.Clear();

            foreach (var item in listOfItems)
            {
                ddlSearchType.Items.Add(new ListItem(item.Key, item.Value));
            }

            if (!_userIsAdministrator)
            {
                pnlSearchSection.Visible = IsShowSearchSection();
            }
            else
            {
                pnlSearchSection.Visible = IsShowSearchSectionForAdmin();
            }
        }

        private void InitSecurity()
        {
            var roles = new List<string>(Roles.GetRolesForUser());

            _userIsAdministrator =
                roles.Contains(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            _userIsClientDirector =
            roles.Contains(DataTransferObjects.Constants.RoleNames.DirectorRoleName);
            _userIsSeniorLeadership =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName); // #2913: userIsSeniorLeadership is added as per the requirement.
            _userIsProjectLead =
                 roles.Contains(DataTransferObjects.Constants.RoleNames.ProjectLead);
            _userIsSalesperson =
                roles.Contains(DataTransferObjects.Constants.RoleNames.SalespersonRoleName);
            _userIsRecruiter =
                roles.Contains(DataTransferObjects.Constants.RoleNames.RecruiterRoleName);
            _userIsHR = roles.Contains(DataTransferObjects.Constants.RoleNames.HRRoleName);
            _userIsPracticeAreaManger =
                roles.Contains(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName);
            _userIsConsultant =
                roles.Contains(DataTransferObjects.Constants.RoleNames.ConsultantRoleName);
        }

        protected string GetVirtualPath(string virtualPath)
        {
            if (virtualPath == "mailto:{0}?subject=Time Off Request")
            {
                var personAlias = DataHelper.CurrentPerson.Manager.Alias;
                var modified = string.Format(virtualPath, personAlias);

                return modified;
            }


            return virtualPath;
        }

        protected void ddlDashBoardType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateQuickLinksSection();
            PopulateSearchSection();
            txtSearchText.Text = string.Empty;
        }

        protected void imgDeleteQuickLink_OnClick(object sender, EventArgs e)
        {
            var imgDelete = sender as ImageButton;

            var quickLinkId = imgDelete.Attributes["QuickLinkId"];

            if (!string.IsNullOrEmpty(quickLinkId))
            {
                int id = Convert.ToInt32(quickLinkId);
                DataHelper.DeleteQuickLinkById(id);

                PopulateQuickLinksSection();
            }

        }

        protected void btnSaveAnnouncement_OnClick(object sender, EventArgs e)
        {
            string text = string.Empty;
            string richText = ckeAnnouncementEditor.Text;

            SaveAnnouncement(text, richText);
        }

        private string GetLatestAnnouncement()
        {
            using (var serviceClient = new ConfigurationService.ConfigurationServiceClient())
            {
                return serviceClient.GetLatestAnnouncement();
            }
        }

        private void SaveAnnouncement(string text, string richText)
        {
            using (var serviceClient = new ConfigurationService.ConfigurationServiceClient())
            {
                try
                {
                    serviceClient.SaveAnnouncement(text, richText);

                    PopulateAnnouncement(richText);
                }
                catch (Exception ex)
                {
                    serviceClient.Abort();
                }
            }
        }

        protected void btnSaveQuickLinks_OnClick(object sender, EventArgs e)
        {
            var names = "";
            var virtualPaths = "";

            foreach (ListItem listItem in cblQuickLinks.Items)
            {
                if (listItem.Selected)
                {
                    names += listItem.Text + ",";
                    virtualPaths += listItem.Value + ",";
                }
            }

            DataHelper.SaveQuickLinksForDashBoard(names, virtualPaths, (DashBoardType)Enum.Parse(typeof(DashBoardType), ddlDashBoardType.SelectedValue));
            PopulateQuickLinksSection();

        }

        protected void repQuickLinks_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!_userIsAdministrator)
            {
                var imgDelete = e.Item.FindControl("imgDeleteQuickLink") as ImageButton;
                imgDelete.Visible = false;
            }
        }

        protected bool IsShowSearchSectionForAdmin()
        {
            var result = ddlDashBoardType.SelectedValue != DashBoardType.Consulant.ToString();

            return result;
        }

        protected bool IsShowSearchSection()
        {
            var result = _userIsAdministrator || _userIsClientDirector || _userIsSeniorLeadership || _userIsPracticeAreaManger || _userIsProjectLead || _userIsRecruiter || _userIsHR || _userIsSalesperson;

            return result;
        }

    }
}

