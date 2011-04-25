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
	public interface ITimeEntryService
	{
		#region Time types

		/// <summary>
		/// Retrieves all existing time types
		/// </summary>
		/// <returns>Collection of new time types</returns>
		[OperationContract]
		IEnumerable<TimeTypeRecord> GetAllTimeTypes();

		/// <summary>
		/// Removes given time type
		/// </summary>
		/// <param name="timeType">Time type to remove</param>
		[OperationContract]
		void RemoveTimeType(TimeTypeRecord timeType);

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

		#endregion    

		#region Time entries

        [OperationContract]
        void RemoveTimeEntries(int milestonePersonId, int timeTypeId, DateTime startDate, DateTime endDate);

		/// <summary>
		/// Removes entry
		/// </summary>
		/// <param name="timeEntry">Time entry to remove</param>
		[OperationContract]
		void RemoveTimeEntryById(int id);

		/// <summary>
		/// Removes entry
		/// </summary>
		/// <param name="timeEntry">Time entry to remove</param>
		[OperationContract]
		void RemoveTimeEntry(TimeEntryRecord timeEntry);

		/// <summary>
		/// Adds new time entry
		/// </summary>
		/// <param name="timeEntry">Time entry to add</param>
		[OperationContract]
		int AddTimeEntry(TimeEntryRecord timeEntry, int defaultMpId);

		/// <summary>
		/// Updates time entry
		/// </summary>
		/// <param name="timeEntry">Time entry to add</param>
		[OperationContract]
		void UpdateTimeEntry(TimeEntryRecord timeEntry, int defaultMilestoneId);

		/// <summary>
		/// Constructs the objects and passes it to update the time entry
		/// </summary>
		[OperationContract]
		void ConstructAndUpdateTimeEntry(
			int id,
			string milestoneDate,
			string entryDate,
			int milestonePersonId,
			double actualHours,
			double forecastedHours,
			int timeTypeId,
			string note,
			string isReviewed,
			bool isChargeable,
			bool isCorrect,
			int modifiedById
			);

		/// <summary>
		/// Get time entries by person
		/// </summary>
		[OperationContract]
		TimeEntryRecord[] GetTimeEntriesForPerson(Person person, DateTime startDate, DateTime endDate);

		/// <summary>
		/// Get time entries by project grouped by person
		/// </summary>
		[OperationContract]
		GroupedTimeEntries<Person> GetTimeEntriesProject(TimeEntryProjectReportContext reportContext);

		/// <summary>
		/// Get time entries by project grouped by person
		/// </summary>
		[OperationContract]
		GroupedTimeEntries<TimeEntryHours> GetTimeEntriesProjectCumulative(TimeEntryPersonReportContext reportContext);

		/// <summary>
		/// Get time entries by project grouped by person
		/// </summary>
		[OperationContract]
		GroupedTimeEntries<Project> GetTimeEntriesPerson(TimeEntryPersonReportContext reportContext);

		/// <summary>
		/// Get milestones by person for given time period
		/// </summary>
		[OperationContract]
		MilestonePersonEntry[] GetCurrentMilestones(
			Person person, DateTime startDate, DateTime endDate, int defaultMilestoneId);

        /// <summary>
        /// Get milestones by person for given time period exclusively for Time Entry page.
        /// </summary>
        [OperationContract]
        MilestonePersonEntry[] GetTimeEntryMilestones(Person person, DateTime startDate, DateTime endDate);

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

		#endregion

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

		#endregion

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
        Project[] GetTimeEntryProjectsByClientId(int? clientId);

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

		#endregion
	}
}

