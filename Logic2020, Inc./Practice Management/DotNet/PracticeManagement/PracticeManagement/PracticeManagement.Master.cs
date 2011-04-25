using System;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ActivityLogService;
using PraticeManagement.Configuration;
using PraticeManagement.Controls;
using PraticeManagement.PersonService;
using System.Text;

namespace PraticeManagement
{    
    public partial class PracticeManagement : System.Web.UI.MasterPage, IPostBackEventHandler
	{
		#region Properties

		/// <summary>
		/// Gets or sets whether data on the page are dirty (not saved).
		/// </summary>
		public bool IsDirty
		{
			get
			{
				bool result;
				return bool.TryParse(hidDirtyData.Value, out result) && result;
			}
			set
			{
				hidDirtyData.Value = value.ToString();
			}
		}

		/// <summary>
		/// Gets or sets whether the user selected save dirty data.
		/// </summary>
		public bool SaveDirty
		{
			get
			{
				bool result;
				return bool.TryParse(hidDoSaveDirty.Value, out result) && result;
			}
			set
			{
				hidDoSaveDirty.Value = value.ToString();
			}
		}

		/// <summary>
		/// Gets or sets if continue without saving is allowed.
		/// </summary>
		public bool AllowContinueWithoutSave
		{
			get
			{
				bool result;
				return bool.TryParse(hidAllowContinueWithoutSave.Value, out result) && result;
			}
			set
			{
				hidAllowContinueWithoutSave.Value = value.ToString();
			}
		}

		public event NavigatingEventHandler Navigating;

		#endregion

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			AllowContinueWithoutSave = true;
		}

		protected void Page_Load(object sender, EventArgs e)
        {
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
                UrlRoleMappingElementSection mapping = UrlRoleMappingElementSection.Current;
			    if (mapping != null)
				    hlHome.NavigateUrl = mapping.Mapping.FindFirstUrl(
                        Roles.GetRolesForUser(Page.User.Identity.Name));
            }
        }

		protected void Page_PreRender(object sender, EventArgs e)
		{
			siteMenu.Visible = Request.IsAuthenticated;

            if (Request.IsAuthenticated &&
                Thread.CurrentPrincipal != null &&
                Thread.CurrentPrincipal.Identity != null)
            {
                Person person = DataHelper.CurrentPerson;
                if (person != null)
                {
                    (loginView.FindControl("lblUserFirstName") as Label).Text = person.FirstName;
                    (loginView.FindControl("lblUserLastName") as Label).Text = person.LastName;
                    if (Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName))
                    {
                        dvBecomeUser.Visible = true;
                    }
                }
            }

			// Confirt save dirty data on logout
			foreach (Control ctrl in loginStatus.Controls)
			{
				if (ctrl is LinkButton)
				{
					((LinkButton)ctrl).OnClientClick = "confirmSaveDirty();";
				}
			}

            // Set logo image.
            imgLogo.ImageUrl = BrandingConfigurationManager.GetLogoImageUrl();

            if (title.Controls.Count > 0)
            {
                LiteralControl lc = title.Controls[0] as LiteralControl;
                if (lc != null)
                {
                    string text = lc.Text;
                    if (text != null)
                    {
                        int titleCloseIndex = text.IndexOf("</title>", StringComparison.OrdinalIgnoreCase);
                        if (titleCloseIndex > 0)
                        {
                            StringBuilder sb = new StringBuilder(text);
                            sb.Insert(titleCloseIndex, " - ");
                            titleCloseIndex += 3;
                            sb.Insert(titleCloseIndex, BrandingConfigurationManager.GetCompanyTitle());
                            lc.Text = sb.ToString();
                        }
                    }
                }
            }
		}

        protected void lbBecomeUserOk_Click(Object sender, EventArgs e)
        {
            string userName = ddlBecomeUserList.SelectedValue;
            logImpersonateLogin(Thread.CurrentPrincipal.Identity.Name, userName);
            GenericIdentity identity = new GenericIdentity(userName);
            string[] roles = Roles.GetRolesForUser(userName); //{DataTransferObjects.Constants.RoleNames.AdministratorRoleName, DataTransferObjects.Constants.RoleNames.SalespersonRoleName};
            GenericPrincipal principal = new GenericPrincipal(identity, roles);
            Thread.CurrentPrincipal = principal;
            FormsAuthentication.SetAuthCookie(userName, true);
            //Response.Redirect(FormsAuthentication.GetRedirectUrl(userName, false));
            Response.Redirect("~/CompanyCalendar.aspx");

        }

        protected void lbBecomeUser_Click(Object sender, EventArgs e)
        {
            using (PersonServiceClient serviceClient = new PersonServiceClient())
            {
                try
                {
                    DataHelper.FillPersonListForImpersonate(ddlBecomeUserList);
                    ddlBecomeUserList.SelectedIndex = ddlBecomeUserList.Items.IndexOf(
                        ddlBecomeUserList.Items.FindByValue(Thread.CurrentPrincipal.Identity.Name));
                    ddlBecomeUserList.Visible = true;
                    lbBecomeUserOk.Visible = true;
                    lbBecomeUser.Enabled = false;
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void logImpersonateLogin(string oldUserName, string newUserName)
        {
            string ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            string logText = string.Format(@"<Login><NEW_VALUES user = ""{0}"" become = ""{1}"" IPAddress = ""{2}""><OLD_VALUES /></NEW_VALUES></Login>",
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

		protected bool CheckMenuSplitter(object dataItem)
		{
			bool result = false;
			MenuItem item = (MenuItem)dataItem;

			int itemsCount = siteMenu.Items.Count;
			int itemPosition = siteMenu.Items.IndexOf(item);

			result = (itemsCount > 6)
                        && (itemPosition + 1 == (itemsCount / 2)) 
                        && (!Request.UserAgent.Contains("Chrome"));
			
			//if (item.Parent != null)
			//{
			//    int itemsCount = item.Parent.ChildItems.Count;
			//    int itemPosition = item.Parent.ChildItems.IndexOf(item);

			//    result = itemPosition == (itemsCount / 2);
			//}

			return result;
		}

        public void RaisePostBackEvent(string eventArgument)
        {
            FireNavigating();
        }
	}
}

