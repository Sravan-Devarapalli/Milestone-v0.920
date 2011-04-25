using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using PraticeManagement.Configuration;
using PraticeManagement.Utils;
using System.Configuration;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace PraticeManagement
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!IsAzureWebRole())
            {
                return;
            }

            if (!Request.IsSecureConnection)
            {
                var url = Request.Url.AbsoluteUri.Replace("http:", "https:");

                Response.Redirect(url, true); //Redirecting (http 302) to the same URL but with https

                Response.StatusCode = 301;

                Response.End();
            }
        }

        private Boolean IsAzureWebRole()
        {
            try
            {
                bool hasHttpsEndPoint = true;
                if (RoleEnvironment.IsAvailable && Boolean.TryParse(RoleEnvironment.GetConfigurationSettingValue("HasHttpsEndPoint"), out hasHttpsEndPoint))
                {
                    return hasHttpsEndPoint;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // We need a customize default page depending on the user's role in the system
            if (Request.IsAuthenticated &&
                (Request.AppRelativeCurrentExecutionFilePath == Constants.ApplicationPages.AppRootUrl ||
                Request.AppRelativeCurrentExecutionFilePath == VirtualPathUtility.ToAppRelative(FormsAuthentication.DefaultUrl)))
            {
                UrlRoleMappingElementSection mapping = UrlRoleMappingElementSection.Current;
                string defaultUrl =
                    mapping != null ?
                    mapping.Mapping.FindFirstUrl(Roles.GetRolesForUser(HttpContext.Current.User.Identity.Name)) : string.Empty;
                if (defaultUrl != string.Empty)
                {
                    Response.Redirect(defaultUrl);
                }
            }

            // The default behavior of the ASP .NET Security system is to redirect the user to the Login page
            // whenever he/she is not authorized for the requested resource.
            // We want to display an especial page to notify the user about an authorized access.
            if (Request.IsAuthenticated &&
                !UrlAuthorizationModule.CheckUrlAccessForPrincipal(Request.AppRelativeCurrentExecutionFilePath,
                new RolePrincipal(HttpContext.Current.User.Identity),
                Request.RequestType))
            {
                // It's not enough to set the Response's status code to some value to see a custom error page.
                // We need to redirect the user manually.
                var config =
                    WebConfigurationManager.GetWebApplicationSection("system.web/customErrors") as CustomErrorsSection;
                if (config != null)
                {
                    CustomError error = config.Errors[((int)HttpStatusCode.Forbidden).ToString()];
                    if (error != null)
                    {
                        Response.Redirect(error.Redirect);
                    }
                }

                // Finally if we didn't found an appropriate page we set a Respones status to display the default
                // error page to the user.
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Response.End();
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            string excMsg = string.Empty;       //  Exception message
            string excSrc = string.Empty;       //  Exception source
            string innerExcMsg = string.Empty;  //  Exception inner message
            string innerExcSrc = string.Empty;  //  Exception inner source

            Exception exc = Server.GetLastError();
            if (exc != null)
            {
                excSrc = exc.Source;
                excMsg = HttpUtility.HtmlEncode(exc.Message);

                if (exc.InnerException != null)
                {
                    innerExcMsg = HttpUtility.HtmlEncode(exc.InnerException.Message);
                    innerExcSrc = exc.InnerException.Source;
                }
            }

            //  Path of the page who threw an exception
            string srcUrl = HttpContext.Current.Request.Url.GetComponents(
                                UriComponents.Path, UriFormat.SafeUnescaped);

            //  Query data of the string who threw an exception
            string srcQuery = GetXmlParsableQueryData();

            //  User currently logged in
            string userName = Thread.CurrentPrincipal.Identity.Name;

            //  Wrap all this data with XML string
            try
            {
                Logging.LogErrorMessage(excMsg, excSrc, innerExcMsg, innerExcSrc, srcUrl, srcQuery, userName);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Removes returnTo parameter and replaces '&' separator with space
        /// </summary>
        /// <returns>List of parameters separated with space</returns>
        private static string GetXmlParsableQueryData()
        {
            // Get query string
            string queryString = HttpContext.Current.Request.Url.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped);
            // Split it into chunks
            string[] chunks = queryString.Split(new[] { '&' });

            var sb = new StringBuilder();
            foreach (string chunk in chunks)
            {
                // Get rid of returnUrl parameter
                if (!chunk.StartsWith(Constants.QueryStringParameterNames.ReturnUrl))
                    sb.Append(chunk + " ");
            }

            return sb.ToString();
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}

