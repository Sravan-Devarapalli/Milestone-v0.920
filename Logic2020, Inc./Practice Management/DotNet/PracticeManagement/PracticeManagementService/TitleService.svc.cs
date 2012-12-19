using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TitleService" in code, svc and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TitleService : ITitleService
    {
        /// <summary>
        /// Gets all titles.
        /// </summary>
        /// <returns></returns>
        public List<Title> GetAllTitles()
        {
            return TitleDAL.GetAllTitles();
        }

        /// <summary>
        /// Gets all Title Types
        /// </summary>
        /// <returns></returns>
        public List<TitleType> GetTitleTypes()
        {
            return TitleDAL.GetTitleTypes();
        }

        /// <summary>
        /// Gets a Title for given titleid.
        /// </summary>
        /// <param name="titleId"></param>
        /// <returns></returns>
        public Title GetTitleById(int titleId)
        {
            return TitleDAL.GetTitleById(titleId);
        }

        /// <summary>
        /// Inserts new Title.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="titleTypeId"></param>
        /// <param name="sortOrder"></param>
        /// <param name="pTOAccural"></param>
        /// <param name="minimumSalary"></param>
        /// <param name="maximumSalary"></param>
        /// <param name="userLogin"></param>
        public void TitleInset(string title, int titleTypeId, int sortOrder, int pTOAccural, int? minimumSalary, int? maximumSalary, string userLogin)
        {
            TitleDAL.TitleInset(title, titleTypeId, sortOrder, pTOAccural, minimumSalary, maximumSalary, userLogin);
        }

        /// <summary>
        /// Updates the Given Title With the Given details.
        /// </summary>
        /// <param name="titleId"></param>
        /// <param name="title"></param>
        /// <param name="titleTypeId"></param>
        /// <param name="sortOrder"></param>
        /// <param name="pTOAccural"></param>
        /// <param name="minimumSalary"></param>
        /// <param name="maximumSalary"></param>
        /// <param name="userLogin"></param>
        public void TitleUpdate(int titleId, string title, int titleTypeId, int sortOrder, int pTOAccural, int? minimumSalary, int? maximumSalary, string userLogin)
        {
            TitleDAL.TitleUpdate(titleId, title, titleTypeId, sortOrder, pTOAccural, minimumSalary, maximumSalary, userLogin);
        }

        /// <summary>
        /// Deletes title for the given titleid.
        /// </summary>
        /// <param name="titleId"></param>
        /// <param name="userLogin"></param>
        public void TitleDelete(int titleId, string userLogin)
        {
            TitleDAL.TitleDelete(titleId, userLogin);
        }
    }
}

