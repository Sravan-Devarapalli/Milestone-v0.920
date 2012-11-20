using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataTransferObjects;
using DataAccess;
using System.ServiceModel.Activation;

namespace PracticeManagementService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TitleService" in code, svc and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TitleService : ITitleService
    {

        public List<Title> GetAllTitles()
        {
            try
            {
                return TitleDal.GetAllTitles();
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        public Title GetTitleById(int titleId)
        {
            try
            {
                return TitleDal.GetTitleById(titleId);
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        public void TitleInset(string title, int titleTypeId, int sortOrder, int pTOAccural, int? minimumSalary, int? maximumSalary, string userLogin)
        {
            try
            {
                TitleDal.TitleInset(title, titleTypeId, sortOrder, pTOAccural, minimumSalary, maximumSalary, userLogin);
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        public void TitleUpdate(int titleId, string title, int titleTypeId, int sortOrder, int pTOAccural, int? minimumSalary, int? maximumSalary, string userLogin)
        {
            try
            {
                TitleDal.TitleUpdate(titleId, title, titleTypeId, sortOrder, pTOAccural, minimumSalary, maximumSalary, userLogin);
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        public void TitleDelete(int titleId, string userLogin)
        {
            try
            {
                TitleDal.TitleDelete(titleId, userLogin);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}

