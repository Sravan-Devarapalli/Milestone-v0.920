namespace PraticeManagement
{
    using System;
    using System.Threading;
    using System.Web;
    using ProjectService;
    using Utils;

    /// <summary>
    /// Handles request to open project by its number.
    /// Gets ProjectId by ProjectNumber and redirets to ProjectDetail page if success.
    /// Logs error and redirects to default application page in case of failure.
    /// </summary>
    public class ProjectNumberHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                if (context.Request.Params.Count != 0)
                {
                    string projectNumber = context.Request.Params[0];
                    int? projectId = null;

                    try
                    {
                        using (var serviceClient = new ProjectServiceClient())
                        {
                            projectId = serviceClient.GetProjectId(projectNumber);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                        Generic.RedirectWithReturnTo(Constants.ApplicationPages.Projects, context.Request.Url.AbsoluteUri, context.Response);
                    }                    

                    if (projectId.HasValue)
                    {
                        string redirectUrl = string.Concat(Constants.ApplicationPages.ProjectDetail, "?id=", projectId);

                        // Redirecting user to project detail.
                        Generic.RedirectWithReturnTo(redirectUrl, Constants.ApplicationPages.Projects, context.Response);
                        return;
                    }
                }

                // Project was not found, or project Id was not found.
                Generic.RedirectWithReturnTo(Constants.ApplicationPages.Projects, context.Request.Url.AbsoluteUri, context.Response);
            }
            else
            {
                // In case if authentication was disabled by some reason.
                Generic.RedirectWithReturnTo("~/Login.aspx", context.Request.Url.AbsoluteUri, context.Response);
            }
        }

        private static void LogException(Exception exc)
        {
            string excMsg = string.Empty; //  Exception message
            string excSrc = string.Empty; //  Exception source
            string innerExcMsg = string.Empty; //  Exception inner message
            string innerExcSrc = string.Empty; //  Exception inner source

            if (exc != null)
            {
                excSrc = exc.Source;
                excMsg = exc.Message;

                if (exc.InnerException != null)
                {
                    innerExcMsg = exc.InnerException.Message;
                    innerExcSrc = exc.InnerException.Source;
                }
            }

            //  Path of the page who threw an exception
            string srcUrl = HttpContext.Current.Request.Url.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);

            //  Query data of the string who threw an exception
            string srcQuery = HttpUtility.HtmlEncode(HttpContext.Current.Request.Url.Query.Replace('&', ' '));

            //  User currently logged in
            string userName = Thread.CurrentPrincipal.Identity.Name;

            //  Wrap all this data with XML string
            Logging.LogErrorMessage(excMsg, excSrc, innerExcMsg, innerExcSrc, srcUrl, srcQuery, userName);
        }
    }
}
