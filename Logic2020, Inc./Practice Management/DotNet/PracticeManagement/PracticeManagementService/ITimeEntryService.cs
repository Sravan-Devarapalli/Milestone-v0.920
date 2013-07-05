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
    [ServiceKnownType(typeof(TimeEntryRecord))]
    public interface ITimeEntryService
    {
        #region Time Zones

        /// <summary>
        /// Retrieves all existing time types
        /// </summary>
        /// <returns>Collection of new time types</returns>
        [OperationContract]
        List<Timezone> TimeZonesAll();

        /// <summary>
        /// Sets given time zone
        /// </summary>
        /// <param name="Timezone">Time zone to set</param>
        [OperationContract]
        void SetTimeZone(Timezone timezone);

        #endregion Time Zones

        #region Time entries

        /// <summary>
        /// Has time entries
        /// </summary>
        /// <param name="milestoneId">milestoneId</param>
        [OperationContract]
        bool HasTimeEntriesForMilestone(int milestoneId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get time entries by person
        /// </summary>
        [OperationContract]
        TimeEntryRecord[] GetTimeEntriesForPerson(Person person, DateTime startDate, DateTime endDate);

        [OperationContract]
        PersonTimeEntries GetTimeEntriesPerson(TimeEntryPersonReportContext reportContext);

        /// <summary>
        /// Get time entries by project grouped by person
        /// </summary>
        [OperationContract]
        GroupedTimeEntries<Person> GetTimeEntriesProject(TimeEntryProjectReportContext reportContext);

        /// <summary>
        /// Get milestones by person for given time period
        /// </summary>
        [OperationContract]
        MilestonePersonEntry[] GetCurrentMilestones(
            Person person, DateTime startDate, DateTime endDate, int defaultMilestoneId);

        /// <summary>
        /// Returns all time entries for given Context/Filter conditions
        /// </summary>
        [OperationContract]
        TimeEntryRecord[] GetAllTimeEntries(TimeEntrySelectContext selectContext, int startRow, int maxRows);

        /// <summary>
        /// Gets TimeEntries count for given Context/Filter conditions
        /// </summary>
        /// <param name="selectContext"></param>
        /// <returns></returns>
        [OperationContract]
        int GetTimeEntriesCount(TimeEntrySelectContext selectContext);

        /// <summary>
        /// Gets TimeEntries count for given Context/Filter conditions
        /// </summary>
        /// <param name="selectContext"></param>
        /// <returns></returns>
        [OperationContract]
        TimeEntrySums GetTimeEntrySums(TimeEntrySelectContext selectContext);

        [OperationContract]
        System.Data.DataSet TimeEntriesByPersonGetExcelSet(TimeEntryPersonReportContext reportContext);

        //new timetrack methods
        [OperationContract]
        void DeleteTimeEntry(int clientId, int projectId, int personId, int timetypeId, DateTime startDate, DateTime endDate, string userName);

        [OperationContract]
        void SaveTimeTrack(string timeEntriesXml, int personId, DateTime startDate, DateTime endDate, string userLogin);

        [OperationContract]
        Dictionary<DateTime, bool> GetIsChargeCodeTurnOffByPeriod(int personId, int clientId, int groupId, int projectId, int timeTypeId, DateTime startDate, DateTime endDate);

        [OperationContract]
        void SetPersonTimeEntryRecursiveSelection(int personId, int clientId, int projectGroupId, int projectId, int timeEntrySectionId, bool isRecursive, DateTime startDate);

        [OperationContract]
        void SetPersonTimeEntrySelection(int personId, int clientId, int projectGroupId, int projectId, int timeEntrySectionId, bool isDelete, DateTime startDate, DateTime endDate, string userName);

        #endregion Time entries

        #region Toggling

        /// <summary>
        /// Toggle IsCorrect property
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        [OperationContract]
        void ToggleIsCorrect(TimeEntryRecord timeEntry);

        /// <summary>
        /// Toggle IsReviewed property
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        [OperationContract]
        void ToggleIsReviewed(TimeEntryRecord timeEntry);

        /// <summary>
        /// Toggle IsChargeable property
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        [OperationContract]
        void ToggleIsChargeable(TimeEntryRecord timeEntry);

        #endregion Toggling

        #region Time entry filters

        /// <summary>
        /// Gets all projects that have TE records assigned to them
        /// </summary>
        /// <returns>Projects list</returns>
        [OperationContract]
        Project[] GetAllTimeEntryProjects();

        /// <summary>
        /// Gets all projects that have TE records assigned to particular clientId
        /// </summary>
        /// <returns>Projects list</returns>
        [OperationContract]
        Project[] GetTimeEntryProjectsByClientId(int? clientId, int? personId, bool showActiveAndInternalProjectsOnly);

        /// <summary>
        /// Gets all milestones that have TE records assigned to them
        /// </summary>
        /// <returns>Milestones list</returns>
        [OperationContract]
        Milestone[] GetAllTimeEntryMilestones();

        /// <summary>
        /// Gets all persons that have entered at least one TE
        /// </summary>
        /// <returns>List of persons</returns>
        [OperationContract]
        Person[] GetAllTimeEntryPersons(DateTime entryDateFrom, DateTime entryDateTo);

        [OperationContract]
        List<TimeEntrySection> PersonTimeEntriesByPeriod(int personId, DateTime startDate, DateTime endDate);

        #endregion Time entry filters
    }
}
