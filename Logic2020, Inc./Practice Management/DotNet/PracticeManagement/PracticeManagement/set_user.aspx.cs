using System;
using System.Configuration;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web.Security;
using System.Web.UI.WebControls;
using PraticeManagement.PersonService;

namespace PraticeManagement
{
    public partial class set_user : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (PersonServiceClient serviceClient = new PersonServiceClient())
            {
                try
                {
                    //var persons = serviceClient.PersonListAllShort(null, DateTime.MinValue, DateTime.MaxValue);
                    string userName = ConfigurationManager.AppSettings["AdminsEmail"];
                    var persons = serviceClient.GetPersonListWithCurrentPay(null, true, Int16.MaxValue, 0,
                        string.Empty, null, userName, null, null, false, false, false, null);
                    foreach (var person in persons)
                    {
                        var userRoles = string.Empty;

                        if (person.Alias != null)
                        {
                            var rb = new StringBuilder();

                            foreach (var role in Roles.GetRolesForUser(person.Alias))
                                rb.AppendFormat("{0}, ", role);

                            userRoles = rb.ToString();

                            if (string.IsNullOrEmpty(userRoles))
                                userRoles = "No roles";
                        }

                        var li = new ListItem(
                            string.Format("{1}, {0} ({2}, {3}) {4}",
                                person.FirstName, person.LastName,
                                person.Seniority.Name, person.Seniority.Id,
                                userRoles),
                            person.Alias);

                        ddlUsers.Items.Add(li);
                    }

                    lblCurrentVersion.Text = Utils.Generic.SystemVersion;
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void btnSetUser_Click(Object sender, EventArgs e)
        {
            var userName = ddlUsers.SelectedValue;
            var identity = new GenericIdentity(userName);
            var roles = Roles.GetRolesForUser(userName);
            var principal = new GenericPrincipal(identity, roles);
            Thread.CurrentPrincipal = principal;
            FormsAuthentication.SetAuthCookie(userName, true);
            var message = string.Format(
                            "User was set to {0} [{1}]",
                            userName,
                            roles != null && roles.Length > 0 ? string.Join(", ", roles) : "no roles");
            lblBecameUser.Text = message;

            Response.Redirect(ResolveClientUrl("~/set_user.aspx"));
        }

        protected void menu_OnMenuItemDataBound(object sender, MenuEventArgs e)
        {
            var item = e.Item;
            if (item.NavigateUrl.Contains("Temp.aspx"))
            {
                item.NavigateUrl = string.Empty;
                //if ((((System.Web.SiteMapNode)(item.DataItem)).ChildNodes).Count == 0)
                //{
                //    item.DataItem = false;
                //}
            }

            if (item.ToolTip.ToLower() == "show url")
            {
                item.ToolTip = item.Text;
            }
        }
    }
}

