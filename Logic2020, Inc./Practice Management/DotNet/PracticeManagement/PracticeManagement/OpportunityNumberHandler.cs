using System;
using System.Web;
using PraticeManagement.OpportunityService;
using System.Threading;
using PraticeManagement.Utils;

namespace PraticeManagement
{
    public class OpportunityNumberHandler : IHttpHandler
    {        
        #region IHttpHandler Members

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
                    string opportunityNumber = context.Request.Params[0];
                    int? opportunityId = null;

                    try
                    {
                        using (var serviceClient = new OpportunityServiceClient())
                        {
                            opportunityId = serviceClient.GetOpportunityId(opportunityNumber);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                        Generic.RedirectWithReturnTo(Constants.ApplicationPages.OpportunityList, context.Request.Url.AbsoluteUri, context.Response);
                    }

                    if (opportunityId.HasValue)
                    {
                        string redirectUrl = string.Concat(Constants.ApplicationPages.OpportunityDetail, "?id=", opportunityId);

                        // Redirecting user to project detail.
                        Generic.RedirectWithReturnTo(redirectUrl, Constants.ApplicationPages.OpportunityList, context.Response);
                        return;
                    }
                }

                // Project was not found, or project Id was not found.
                Generic.RedirectWithReturnTo(Constants.ApplicationPages.OpportunityList, context.Request.Url.AbsoluteUri, context.Response);
            }
            else
            {
                // In case if authentication was disabled by some reason.
                Generic.RedirectWithReturnTo("~/Login.aspx", context.Request.Url.AbsoluteUri, context.Response);
            }
        }

        #endregion

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
