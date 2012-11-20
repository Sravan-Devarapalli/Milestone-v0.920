using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataTransferObjects;

namespace PracticeManagementService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITitleService" in both code and config file together.
    [ServiceContract]
    [ServiceKnownType(typeof(Title))]
    public interface ITitleService
    {
        [OperationContract]
        List<Title> GetAllTitles();

        [OperationContract]
        Title GetTitleById(int titleId);

        [OperationContract]
        void TitleInset(string title, int titleTypeId, int sortOrder, int pTOAccural, int? minimumSalary, int? maximumSalary, string userLogin);

        [OperationContract]
        void TitleUpdate(int titleId, string title, int titleTypeId, int sortOrder, int pTOAccural, int? minimumSalary, int? maximumSalary, string userLogin);

        [OperationContract]
        void TitleDelete(int titleId, string userLogin);
    }
}

