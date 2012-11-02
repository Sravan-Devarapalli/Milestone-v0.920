using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using DataAccess;
using DataTransferObjects;
using DataTransferObjects.CompositeObjects;
using DataTransferObjects.ContextObjects;
using DataTransferObjects.TimeEntry;
using System.Web;

namespace PracticeManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TimeTypeService : ITimeTypeService
    {

        /// <summary>
        /// Retrieves all existing time types
        /// </summary>
        /// <returns>Collection of new time types</returns>
        public IEnumerable<TimeTypeRecord> GetAllTimeTypes()
        {
            try
            {
                return TimeTypeDAL.GetAllTimeTypes();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<TimeTypeRecord> GetAllAdministrativeTimeTypes(bool includePTO, bool includeHoliday, bool includeUnpaid, bool includeSickLeave)
        {
            try
            {
                return TimeTypeDAL.GetAllAdministrativeTimeTypes(includePTO, includeHoliday, includeUnpaid, includeSickLeave);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Triple<int, int, int> GetAdministrativeChargeCodeValues(int timeTypeId)
        {
            try
            {
                return TimeTypeDAL.GetAdministrativeChargeCodeValues(timeTypeId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Removes given time type
        /// </summary>
        /// <param name="timeType">Time type to remove</param>
        public void RemoveTimeType(int timeTypeId)
        {
            try
            {
                TimeTypeDAL.RemoveTimeType(timeTypeId);
            }
            catch (Exception e) 
            {
                throw e;
            }
        }

        /// <summary>
        /// Updates given time type
        /// </summary>
        /// <param name="timeType">Time type to update</param>
        public void UpdateTimeType(TimeTypeRecord timeType)
        {
            try
            {
                TimeTypeDAL.UpdateTimeType(timeType);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        /// <summary>
        /// Adds new time type
        /// </summary>
        /// <param name="timeType">Time type to add</param>
        /// <returns>Id of added time type</returns>
        public int AddTimeType(TimeTypeRecord timeType)
        {
            try
            {
                return TimeTypeDAL.AddTimeType(timeType);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public string GetWorkTypeNameById(int worktypeId)
        {
            try
            {
                return TimeTypeDAL.GetWorkTypeNameById(worktypeId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public TimeTypeRecord GetWorkTypeById(int worktypeId)
        {
            try
            {
                return TimeTypeDAL.GetWorkTypeById(worktypeId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public TimeTypeRecord GetUnpaidTimeType()
        {
            return TimeTypeDAL.GetUnpaidTimeType();
        }

        public TimeTypeRecord GetPTOTimeType()
        {
            return TimeTypeDAL.GetPTOTimeType();
        }

        public TimeTypeRecord GetSickLeaveTimeType()
        {
            return TimeTypeDAL.GetSickLeaveTimeType();
        }

    }
}


