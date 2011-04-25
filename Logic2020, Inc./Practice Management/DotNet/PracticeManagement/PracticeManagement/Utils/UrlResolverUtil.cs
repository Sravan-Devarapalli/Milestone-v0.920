using System;
using DataTransferObjects;

namespace PraticeManagement.Utils
{
    public class Urls
    {
        #region Constants

        private const int MaxAllowedUrlSizeInIe = 2083;

        #endregion

        #region Page addresses

        public static string GetPersonDetailsUrl(Person p, string returnTo)
        {
            var personDetailsUrl = String.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                                 Constants.ApplicationPages.PersonDetail,
                                                 p.Id);
            return Generic.GetTargetUrlWithReturn(personDetailsUrl, returnTo);
        }

        public static string GetProjectDetailsUrl(object projectId, string returnTo)
        {
            var projectDetailsUrl = String.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                                  Constants.ApplicationPages.ProjectDetail,
                                                  projectId);
            return Generic.GetTargetUrlWithReturn(projectDetailsUrl, returnTo);
        }

        public static string GerMilestonePersonDetailsUrlWithReturn(object milestoneId, object milestonePersonId, string originalString)
        {
            var personPageUrl =
                String.Format(Constants.ApplicationPages.RedirectMilestonePersonIdFormat,
                              Constants.ApplicationPages.MilestonePersonDetail,
                              milestoneId,
                              milestonePersonId);

            return Generic.GetTargetUrlWithReturn(personPageUrl, originalString);
        }

        public static string GetMilestoneRedirectUrl(object milestoneId, string originalString, int projectValue)
        {
            var milestonePageUrl =
                String.Format(
                    Constants.ApplicationPages.MilestonePrevNextRedirectFormat,
                    Constants.ApplicationPages.MilestoneDetail,
                    milestoneId, projectValue);

            return Generic.GetTargetUrlWithReturn(milestonePageUrl, originalString);
        }

        #endregion

        #region Url utils

        /*
         * Example of the query string:
         * 
         *  ~/MilestoneDetail.aspx?id=143&projectId=33&
         *      returnTo=http%3a%2f%2fws63.ua.akvelon.com%3a80%2fPracticeManagement%2fProjectDetail.aspx%3fid%3d33%26
         *      returnTo%3dhttp%253a%252f%252fws63.ua.akvelon.com%253a80%252fPracticeManagement%252fClientDetails.aspx%253fid%253d541%2526
         *      returnTo%253dhttp%25253a%25252f%25252fws63.ua.akvelon.com%25253a80%25252fPracticeManagement%25252fClientList.aspx
         *      
         * It becomes too long, so according to the maximum URL length in Internet Explorer (which is 2,083 characters) 
         *      we remove the last address in the chain
         */

        /// <summary>
        /// Removes several returnTo parameters when GET query length becomes too long.
        /// </summary>
        /// <param name="url">URL to analyse</param>
        /// <returns>URL with removed returnTo-s</returns>
        public static string GetUrlWithoutReturnTo(string url)
        {
            if (url.Length >= MaxAllowedUrlSizeInIe)
            {
                var withoutLastReturnTo = 
                    url.Substring(0, url.LastIndexOf(Constants.QueryStringParameterNames.ReturnUrl));
                return GetUrlWithoutReturnTo(withoutLastReturnTo);
            }

            return url;
        }

        public static string GetUrlWithoutReturnTo(string url, int maxDepth)
        {
            var index = 0;
            int lastIndex;
            do
            {
                lastIndex = url.IndexOf(Constants.QueryStringParameterNames.ReturnUrl, index + 1);

                if (lastIndex > 0)
                    index = lastIndex;

                maxDepth--;
            } while (maxDepth >= 0 && lastIndex >= 0);

            return maxDepth < 0 ? url.Substring(0, index - 1) : url;
        }

        #endregion

        public static string OpportunityDetailsLink(int opportunityId)
        {
            return String.Format(
                Constants.ApplicationPages.DetailRedirectFormat,
                Constants.ApplicationPages.OpportunityDetail,
                opportunityId);
        }
    }
}

