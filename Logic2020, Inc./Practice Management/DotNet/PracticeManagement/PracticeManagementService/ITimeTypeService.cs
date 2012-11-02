using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataTransferObjects;
using DataTransferObjects.CompositeObjects;
using DataTransferObjects.ContextObjects;
using DataTransferObjects.TimeEntry;

namespace PracticeManagementService
{
    [ServiceContract]
    [ServiceKnownType(typeof(TimeTypeRecord))]
    public interface ITimeTypeService
    {
        #region Time types

        /// <summary>
        /// Retrieves all existing time types
        /// </summary>
        /// <returns>Collection of new time types</returns>
        [OperationContract]
        IEnumerable<TimeTypeRecord> GetAllTimeTypes();


        [OperationContract]
        List<TimeTypeRecord> GetAllAdministrativeTimeTypes(bool includePTO, bool includeHoliday, bool includeUnpaid,bool includeSickLeave);

        [OperationContract]
        Triple<int, int, int> GetAdministrativeChargeCodeValues(int timeTypeId);


        /// <summary>
        /// Removes given time type
        /// </summary>
        /// <param name="timeType">Time type to remove</param>
        [OperationContract]
        void RemoveTimeType(int timeTypeId);

        /// <summary>
        /// Updates given time type
        /// </summary>
        /// <param name="timeType">Time type to update</param>
        [OperationContract]
        void UpdateTimeType(TimeTypeRecord timeType);

        /// <summary>
        /// Adds new time type
        /// </summary>
        /// <param name="timeType">Time type to add</param>
        /// <returns>Id of added time type</returns>
        [OperationContract]
        int AddTimeType(TimeTypeRecord timeType);

        [OperationContract]
        string GetWorkTypeNameById(int worktypeId);

        [OperationContract]
        TimeTypeRecord GetWorkTypeById(int worktypeId);

        [OperationContract]
        TimeTypeRecord GetUnpaidTimeType();

        [OperationContract]
        TimeTypeRecord GetPTOTimeType();

        [OperationContract]
        TimeTypeRecord GetSickLeaveTimeType();

        #endregion


    }
}

