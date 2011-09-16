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
using System.Linq;
using System.Collections;
using PraticeManagement.Utils;
using DataTransferObjects;
using System.Configuration;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace PraticeManagement
{
    public partial class PracticeManagementMain : MasterPage, IPostBackEventHandler
    {
        public const string Level1MenuItemTemplate = " <li class='l1' id='{2}'><a title='{0}' style='' {3}>{0}</a>{1}</li>";
        public const string Level2MenuItemTemplate = " <dd id='{2}'  class='l3'> <a title='{0}' {3}>{0}</a>{1}</dd>";
        public const string Level3MenuItemTemplate = "<a  {1} >{0}</a>";
        public const string AnchorTagpropertiestemplate = " href = '{0}' onclick='return checkDirtyWithRedirect(this.href);'";
        public const string PopupTimebeforeFormsAuthTimeOutSecKey = "PopupTimebeforeFormsAuthTimeOutSec";
        public const string MailToSubjectFormat = "mailto:{0}?subject={1} Issue";


        #region Properties

        public bool SkipTicketRenewal
        {
            set;
            get;
        }

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

        public string _PageTitle { get; set; }

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
            SetPageTitle();

            SetMailToEmailSupport();

            if (Request.Url.AbsoluteUri.Contains("LoggedOut.aspx") && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
                SkipTicketRenewal = true;
            }

            MembershipUser user = Membership.GetUser(HttpContext.Current.User.Identity.Name);
            TimeSpan ts = new TimeSpan(00, 00, 20);

            if (!string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
            {
                hlnkChangePassword.Visible = true;

                if (user != null && user.CreationDate.Subtract(user.LastPasswordChangedDate).Duration() < ts)
                {
                    if (!(Request.AppRelativeCurrentExecutionFilePath == Constants.ApplicationPages.ChangePasswordPage))
                    {
                        if (Session["IsLoggedInthroughLoginPage"] != null && Convert.ToBoolean(Session["IsLoggedInthroughLoginPage"]))
                            Response.Redirect(Constants.ApplicationPages.ChangePasswordPage);
                    }
                }
            }
            else
            {
                hlnkChangePassword.Visible = false;
            }

            Page.Header.DataBind();


            var formsAuthenticationTimeOutStr = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.FormsAuthenticationTimeOutKey);
            int formsAuthenticationTimeOutMin;
            if (!string.IsNullOrEmpty(formsAuthenticationTimeOutStr))
            {
                formsAuthenticationTimeOutMin = int.Parse(formsAuthenticationTimeOutStr);
            }
            else
            {
                formsAuthenticationTimeOutMin = 60;
            }

            hdnRunTimeOutPopuUpScript.Value = HttpContext.Current.User.Identity.IsAuthenticated.ToString().ToLower();

            var popupTimebeforeFormsAutTimeOutSec = GetConfigValueByKey(PopupTimebeforeFormsAuthTimeOutSecKey);
            if (!string.IsNullOrEmpty(popupTimebeforeFormsAutTimeOutSec))
            {
                popupTimebeforeFormsAutTimeOutSec = (int.Parse(popupTimebeforeFormsAutTimeOutSec) * 1000).ToString();
            }
            else
            {
                popupTimebeforeFormsAutTimeOutSec = "60000"; // 1 min before time out
            }

            hdnPopupTimebeforeFormsAutTimeOut.Value = popupTimebeforeFormsAutTimeOutSec;

            ltrScript.Text =
                string.Format(ltrScript.Text,
                              hidDirtyData.ClientID,
                              hidDoSaveDirty.ClientID,
                              bool.TrueString,
                              bool.FalseString,
                              hidAllowContinueWithoutSave.ClientID
                              );
            string htmltext = GetMenuHtml();

            if (user != null
                && user.CreationDate.Subtract(user.LastPasswordChangedDate).Duration() < ts)
            {
                if (Session["IsLoggedInthroughLoginPage"] != null && Convert.ToBoolean(Session["IsLoggedInthroughLoginPage"]))
                    htmltext = string.Empty;
            }

            ltrlMenu.Text = htmltext;

            if (!Page.IsPostBack)
            {
                var mapping = UrlRoleMappingElementSection.Current;
                if (mapping != null)
                    hlHome.NavigateUrl = mapping.Mapping.FindFirstUrl(
                        Roles.GetRolesForUser(Page.User.Identity.Name));
            }
        }

        private void SetMailToEmailSupport()
        {
            var _emailSupport = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.PMSupportEmailAddressKey);
            var _pageTitle = _PageTitle;

            if (string.IsNullOrEmpty(_pageTitle))
            {
                _pageTitle = lblCurrentPage.Text;
                _pageTitle = _pageTitle.Replace(" / ", " > ");
                if (string.IsNullOrEmpty(_pageTitle))
                {
                    _pageTitle = Page.Title;
                }
            }
            emailSupportMailToLink.HRef = string.Format(MailToSubjectFormat, _emailSupport, _pageTitle);
        }

        private void SetPageTitle()
        {
            var url = Request.RawUrl;
            int returnToIndex = url.IndexOf("returnTo");
            if (returnToIndex > -1)
            {
                url = url.Substring(0, returnToIndex);
            }

            string pagetitle = GetPageTitle(smdsMain.Provider.RootNode, string.Empty, url);
            if (!string.IsNullOrEmpty(pagetitle))
            {
                lblCurrentPage.Text = pagetitle;
            }
            else
            {
                if (!string.IsNullOrEmpty(Page.Header.Title))
                {
                    string pagetTitle = Page.Header.Title.Replace("Practice Management - ", string.Empty);
                    pagetTitle = pagetTitle.Replace(" | Practice Management", string.Empty);
                    if (pagetTitle != "Welcome to Practice Management")
                    {
                        lblCurrentPage.Text = pagetTitle;
                        return;
                    }
                }
                lblCurrentPage.Text = string.Empty;
            }
        }

        private string GetPageTitle(SiteMapNode siteMapNode, string pageNavPath, string url)
        {
            if (url.Contains(siteMapNode.Url) && !string.IsNullOrEmpty(siteMapNode.Url))
            {
                return pageNavPath + (string.IsNullOrEmpty(pageNavPath) ? siteMapNode.Title : " / " + siteMapNode.Title);
            }
            else if (siteMapNode.ChildNodes.Count > 0)
            {
                foreach (SiteMapNode node in siteMapNode.ChildNodes)
                {
                    var tempstring = pageNavPath + (string.IsNullOrEmpty(pageNavPath) ? string.Empty : " / ") + siteMapNode.Title;
                    var pageNavPathLocal = GetPageTitle(node, tempstring, url);
                    if (!string.IsNullOrEmpty(pageNavPathLocal))
                    {
                        return pageNavPathLocal;
                    }
                }
                return string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetMenuHtml()
        {
            string htmltext = string.Empty;

            foreach (SiteMapNode item in ((System.Web.SiteMapNodeCollection)(smdsMain.Provider.RootNode.ChildNodes)))
            {
                htmltext += GetLevel1MenuItemHtml(item);
            }

            return htmltext;
        }

        private string GetLevel1MenuItemHtml(SiteMapNode item)
        {
            if (CheckChildsExists(item, 1))
            {
                return string.Format(Level1MenuItemTemplate, item.Title, "<dl class='l2'>" + GetLevel2MenuItemHtml(item) + "</dl>", item.Title.Replace(' ', '_'), string.Empty);
            }
            else if (item.Description == "Show url")
            {
                return string.Format(Level1MenuItemTemplate, item.Title, string.Empty, item.Title.Replace(' ', '_'),
                    string.Format(AnchorTagpropertiestemplate, item.Url)
                    );
            }
            else
            {
                return string.Empty;
            }
        }

        private bool CheckChildsExists(SiteMapNode item, int level)
        {
            bool result = false;
            if (level > 1)
            {
                result = (item.ChildNodes.Count > 0);
            }
            else
            {
                foreach (SiteMapNode childItem in item.ChildNodes)
                {
                    if (childItem.ChildNodes.Count > 0 || childItem.Description == "Show url")
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private string GetLevel2MenuItemHtml(SiteMapNode item)
        {
            string htmltext = string.Empty;
            foreach (SiteMapNode level2Item in item.ChildNodes)
            {
                if (level2Item.Title == "Group By..." || level2Item.Title == "Revenue Goals")
                {
                    var person = DataHelper.CurrentPerson;
                    //Only persons with the Director seniority and up should be able to see/access Group by Director view.
                    if (person == null || Seniority.GetSeniorityValueById(person.Seniority.Id) > 35)
                    {
                        continue;
                    }
                }
                if (CheckChildsExists(level2Item, 2))
                {
                    htmltext += string.Format(Level2MenuItemTemplate, level2Item.Title, GetLevel3MenuItemHtml(level2Item), level2Item.Title.Replace(' ', '_') + level2Item.ParentNode.Title.Replace(' ', '_'), string.Empty);
                }
                else if (level2Item.Description == "Show url")
                {

                    htmltext += string.Format(Level2MenuItemTemplate, level2Item.Title, string.Empty, level2Item.Title.Replace(' ', '_'),
                        string.Format(AnchorTagpropertiestemplate, level2Item.Url)
                        );
                }
            }
            return htmltext;
        }

        private string GetLevel3MenuItemHtml(SiteMapNode level2Item)
        {
            string htmltext = string.Empty;
            foreach (SiteMapNode level3Item in level2Item.ChildNodes)
            {

                htmltext += string.Format(Level3MenuItemTemplate, level3Item.Title,
                    (!level3Item.Url.Contains("Temp.aspx")) ? (string.Format(AnchorTagpropertiestemplate, level3Item.Url)) : string.Empty);
            }
            if (level2Item.ChildNodes.Count > 0)
            {
                htmltext = "<div class='flyout'>" + htmltext + "</div>";
            }
            return htmltext;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //siteMenu.Visible = Request.IsAuthenticated;

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

           
            try
            {
               
                UpdateLastServerVisitInfo();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateLastServerVisitInfo()
        {
            if (Response.ContentType.ToLowerInvariant() == "text/html" && HttpContext.Current.User.Identity.IsAuthenticated && !SkipTicketRenewal)
            {

                //as part of 2800
                var ticket = ((System.Web.Security.FormsIdentity)(HttpContext.Current.User.Identity)).Ticket;
                ticket = Generic.SetCustomFormsAuthenticationTicket(HttpContext.Current.User.Identity.Name, ticket.IsPersistent, this.Page);
                var formsAuthTicketExpiry = ticket.Expiration.ToString();
                Response.Cookies.Set(new HttpCookie("FormsAuthTicketExpiry", formsAuthTicketExpiry));
                hdnFormsAuthTicketExpiry.Value = formsAuthTicketExpiry;
                var now = DateTime.Now.ToString();
                hdnLastServerVisit.Value = now;
                Response.Cookies.Set(new HttpCookie("LastServerVisit", now));
                Response.Cookies.Set(new HttpCookie("IsLoggedIn", (DataHelper.CurrentPerson != null).ToString().ToLower()));
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
            SkipTicketRenewal = !e.Cancel;

        }

        protected void smdsSubMenu_OnMenuItemDataBound(object sender, MenuEventArgs e)
        {
            var item = e.Item;
            if (item.Text == "Group By..." || item.Text == "Revenue Goals")
            {
                var person = DataHelper.CurrentPerson;
                //Only persons with the Director seniority and up should be able to see/access Group by Director view.
                if (person == null || Seniority.GetSeniorityValueById(person.Seniority.Id) > 35)
                {
                    item.Text = string.Empty;
                }
            }
        }

        protected void siteMenu_OnMenuItemDataBound(object sender, MenuEventArgs e)
        {
            var item = e.Item;
        }

        public Unit GetItemWidth(MenuItemTemplateContainer container)
        {
            int width = 0;
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

        protected string GetTodayWithTimeZone()
        {
            var timezone = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.TimeZoneKey);
            var isDayLightSavingsTimeEffect = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.IsDayLightSavingsTimeEffectKey);

            if (timezone == "-08:00" && isDayLightSavingsTimeEffect.ToLower() == "true")
            {
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")).ToLongDateString();
            }
            else
            {
                var timezoneWithoutSign = timezone.Replace("+", string.Empty);
                TimeZoneInfo ctz = TimeZoneInfo.CreateCustomTimeZone("cid", TimeSpan.Parse(timezoneWithoutSign), "customzone", "customzone");
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, ctz).ToLongDateString();
            }
        }

        protected void btnLogOutOnSessionTimeOut_OnClick(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            if (hdnRedirectToLogedOutPage.Value == "true")
            {
                hdnRedirectToLogedOutPage.Value = "";
                Response.Redirect("~/LoggedOut.aspx");
            }
            else
            {
                Response.Redirect(FormsAuthentication.LoginUrl);
            }
        }

        private string GetConfigValueByKey(string key)
        {
            var val = string.Empty;
            try
            {
                if (WCFClientUtility.IsWebAzureRole())
                {
                    val = RoleEnvironment.GetConfigurationSettingValue(key);
                }
                else
                {
                    val = ConfigurationManager.AppSettings[key];
                }
            }
            catch
            {
                val = string.Empty;
            }
            return val;
        }
    }
}

