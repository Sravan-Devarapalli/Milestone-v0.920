using System;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.ActivityLogService;
using PraticeManagement.Configuration;
using PraticeManagement.Controls;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Drawing;

namespace PraticeManagement
{
    public partial class PracticeManagementMain : MasterPage, IPostBackEventHandler
    {
        #region Properties

        /// <summary>
        /// 	Gets or sets whether data on the page are dirty (not saved).
        /// </summary>
        public bool IsDirty
        {
            get
            {
                bool result;
                return bool.TryParse(hidDirtyData.Value, out result) && result;
            }
            set { hidDirtyData.Value = value.ToString(); }
        }

        /// <summary>
        /// 	Gets or sets whether the user selected save dirty data.
        /// </summary>
        public bool SaveDirty
        {
            get
            {
                bool result;
                return bool.TryParse(hidDoSaveDirty.Value, out result) && result;
            }
            set { hidDoSaveDirty.Value = value.ToString(); }
        }

        /// <summary>
        /// 	Gets or sets if continue without saving is allowed.
        /// </summary>
        public bool AllowContinueWithoutSave
        {
            get
            {
                bool result;
                return bool.TryParse(hidAllowContinueWithoutSave.Value, out result) && result;
            }
            set { hidAllowContinueWithoutSave.Value = value.ToString(); }
        }

        public event NavigatingEventHandler Navigating;

        #endregion

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            FireNavigating();

            if (eventArgument != string.Empty)
                Response.Redirect(eventArgument);
        }

        #endregion

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            AllowContinueWithoutSave = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
            {
                hlnkChangePassword.Visible = true;
            }
            else
            {
                hlnkChangePassword.Visible = false;
            }

            Page.Header.DataBind();
            ltrScript.Text =
                string.Format(ltrScript.Text,
                              hidDirtyData.ClientID,
                              hidDoSaveDirty.ClientID,
                              bool.TrueString,
                              bool.FalseString,
                              hidAllowContinueWithoutSave.ClientID);
            if (!Page.IsPostBack)
            {
                var mapping = UrlRoleMappingElementSection.Current;
                if (mapping != null)
                    hlHome.NavigateUrl = mapping.Mapping.FindFirstUrl(
                        Roles.GetRolesForUser(Page.User.Identity.Name));
            }
           
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            siteMenu.Visible = Request.IsAuthenticated;

            var person = DataHelper.CurrentPerson;
            if (person != null)
                ((Label)loginView.FindControl("lblUserName")).Text
                    = string.Format(
                        Constants.Formatting.GreetingUserName,
                        person.FirstName,
                        person.LastName);

            // Confirt save dirty data on logout
            foreach (Control ctrl in loginStatus.Controls)
                if (ctrl is LinkButton)
                    ((LinkButton)ctrl).OnClientClick = "confirmSaveDirty();";

            // Set logo image.
            imgLogo.ImageUrl = BrandingConfigurationManager.GetLogoImageUrl();

            if (title.Controls.Count > 0)
            {
                var lc = title.Controls[0] as LiteralControl;
                if (lc != null)
                {
                    var text = lc.Text;
                    if (text != null)
                    {
                        var titleCloseIndex = text.IndexOf("</title>", StringComparison.OrdinalIgnoreCase);
                        if (titleCloseIndex > 0)
                        {
                            var sb = new StringBuilder(text);
                            sb.Insert(titleCloseIndex, " - ");
                            titleCloseIndex += 3;
                            sb.Insert(titleCloseIndex, BrandingConfigurationManager.GetCompanyTitle());
                            lc.Text = sb.ToString();
                        }
                    }
                }
            }
            setCssForMenu();
        }

        private void setCssForMenu()
        {
            string nodeTitle = null;
            SiteMapNode selectedNode = null;

            if (smdsSubMenu != null && smdsSubMenu.Controls != null && smdsSubMenu.Controls.Count > 0)
            {
                nodeTitle = this.smdsSubMenu.SelectedValue;

                if (nodeTitle != string.Empty)
                {
                    selectedNode = smdsMain.Provider.CurrentNode;
                    var rootNode = smdsMain.Provider.GetParentNode(selectedNode);

                    if (rootNode.Title == "Reports")
                    {
                        topMenuBreak.Visible = true;
                    }
                    if (siteMenu != null && siteMenu.Controls != null && siteMenu.Controls.Count > 0)
                    {
                        MenuItem item2 = (siteMenu as Menu).FindItem(rootNode.Title);
                        if (item2 != null)
                            item2.Selected = true;
                    }
                }
                else if (siteMenu != null && siteMenu.Controls != null && siteMenu.Controls.Count > 0)
                {
                    nodeTitle = this.siteMenu.SelectedValue;

                    if (nodeTitle == "Configuration" || nodeTitle == "Projects" || nodeTitle == "Opportunities")
                    {
                        MenuItem item2 = (this.smdsSubMenu as Menu).Items[0];
                        if (item2 != null)
                            item2.Selected = true;
                    }
                    else if (nodeTitle == "Reports")
                    {
                        topMenuBreak.Visible = true;
                    }
                }
                
                
            }
        }

        protected void logImpersonateLogin(string oldUserName, string newUserName)
        {
            var ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            var logText =
                string.Format(
                    @"<Login><NEW_VALUES user = ""{0}"" become = ""{1}"" IPAddress = ""{2}""><OLD_VALUES /></NEW_VALUES></Login>",
                    oldUserName,
                    newUserName,
                    ipAddress);
            using (var serviceClient = new ActivityLogServiceClient())
            {
                try
                {
                    serviceClient.ActivityLogInsert(1, logText);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void btnMenuItem_Command(object sender, CommandEventArgs e)
        {
            var ne = FireNavigating();

            if (!ne.Cancel)
            {
                Response.Redirect((string)e.CommandArgument);
            }
        }

        private NavigatingEventArgs FireNavigating()
        {
            var ne = new NavigatingEventArgs();
            if (Navigating != null)
            {
                Navigating(this, ne);
            }
            return ne;
        }

        protected void loginStatus_LoggingOut(object sender, LoginCancelEventArgs e)
        {
            var ne = FireNavigating();

            e.Cancel = ne.Cancel;
        }

        protected void smdsSubMenu_OnMenuItemDataBound(object sender, MenuEventArgs e)
        {
            var item = e.Item;
            if (item.Text == "Group By..." || item.Text == "Revenue Goals")
            {
                var person = DataHelper.CurrentPerson;
                //Only persons with the Director seniority and up should be able to see/access Group by Director view.
                if (person == null || person.Seniority.Id > 35)
                {
                    item.Text = string.Empty;
                }
            }
        }


        public Unit GetItemWidth(MenuItemTemplateContainer container)
        {
            int  width = 0;
            MenuItem item = (MenuItem)container.DataItem;                       
            if (!string.IsNullOrEmpty(item.Text))
            {
                SizeF size = Graphics.FromImage(new Bitmap(1, 1)).MeasureString(item.Text, new Font("Arial", 11));
                width = Convert.ToInt32(size.Width);

                // Maintaining minimum width for submenu items 
                if (width < 80)
                {
                    width = 80;
                } 
            }
            return Unit.Pixel(width);
        }
    }
}

