using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using DataAccess;
using DataTransferObjects;
using DataTransferObjects.CompositeObjects;
using DataTransferObjects.ContextObjects;
using DataTransferObjects.TimeEntry;

namespace PracticeManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TimeEntryService : ITimeEntryService
    {
        #region Time type

        /// <summary>
        /// Retrieves all existing time types
        /// </summary>
        /// <returns>Collection of new time types</returns>
        public IEnumerable<TimeTypeRecord> GetAllTimeTypes()
        {
            return TimeEntryDAL.GetAllTimeTypes();
        }

        /// <summary>
        /// Removes given time type
        /// </summary>
        /// <param name="timeType">Time type to remove</param>
        public void RemoveTimeType(TimeTypeRecord timeType)
        {
            TimeEntryDAL.RemoveTimeType(timeType);
        }

        /// <summary>
        /// Updates given time type
        /// </summary>
        /// <param name="timeType">Time type to update</param>
        public void UpdateTimeType(TimeTypeRecord timeType)
        {
            TimeEntryDAL.UpdateTimeType(timeType);
        }

        /// <summary>
        /// Adds new time type
        /// </summary>
        /// <param name="timeType">Time type to add</param>
        /// <returns>Id of added time type</returns>
        public int AddTimeType(TimeTypeRecord timeType)
        {
            return TimeEntryDAL.AddTimeType(timeType);
        }

        #endregion

        #region Time entry

        public void RemoveTimeEntryById(int id)
        {
            RemoveTimeEntry(new TimeEntryRecord { Id = id });
        }

        /// <summary>
        /// Removes entry
        /// </summary>
        /// <param name="timeEntry">Time entry to remove</param>
        public void RemoveTimeEntry(TimeEntryRecord timeEntry)
        {
            TimeEntryDAL.RemoveTimeEntry(timeEntry);
        }

        /// <summary>
        /// Adds new time entry
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        /// <param name="defaultMpId">default milestone id</param>
        public int AddTimeEntry(TimeEntryRecord timeEntry, int defaultMpId)
        {
            return TimeEntryDAL.AddTimeEntry(timeEntry, defaultMpId);
        }

        /// <summary>
        /// Updates time entry
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        public void UpdateTimeEntry(TimeEntryRecord timeEntry, int defaultMilestoneId)
        {
            TimeEntryDAL.UpdateTimeEntry(timeEntry, defaultMilestoneId);
        }

        /// <summary>
        /// Constructs the objects and passes it to update the time entry
        /// </summary>
        public void ConstructAndUpdateTimeEntry(
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
            int modifiedById)
        {
            var te = new TimeEntryRecord
                         {
                             Id = id,
                             MilestoneDate = DateTime.Parse(milestoneDate),
                             EntryDate = DateTime.Parse(entryDate),
                             ParentMilestonePersonEntry = new MilestonePersonEntry(milestonePersonId),
                             TimeType = new TimeTypeRecord(timeTypeId),
                             ActualHours = actualHours,
                             ForecastedHours = forecastedHours,
                             Note = note,
                             IsReviewed = (ReviewStatus) Enum.Parse(typeof (ReviewStatus), isReviewed),
                             IsChargeable = isChargeable,
                             IsCorrect = isCorrect,
                             ModifiedDate = DateTime.Now,
                             ModifiedBy = new Person(modifiedById)
                         };

            UpdateTimeEntry(te, 0);
        }

        /// <summary>
        /// Get time entries by person
        /// </summary>
        public TimeEntryRecord[] GetTimeEntriesForPerson(Person person, DateTime startDate, DateTime endDate)
        {
            return TimeEntryDAL.GetTimeEntriesForPerson(person, startDate, endDate);
        }

        public GroupedTimeEntries<Person> GetTimeEntriesProject(TimeEntryProjectReportContext reportContext)
        {
            return TimeEntryDAL.GetTimeEntriesByProject(reportContext);
        }

        public GroupedTimeEntries<TimeEntryHours> GetTimeEntriesProjectCumulative(TimeEntryPersonReportContext reportContext)
        {
            return TimeEntryDAL.GetTimeEntriesByProjectCumulative(reportContext);
        }

        public GroupedTimeEntries<Project> GetTimeEntriesPerson(TimeEntryPersonReportContext reportContext)
        {
            return TimeEntryDAL.GetTimeEntriesByPerson(reportContext);
        }

        /// <summary>
        /// Get milestones by person for given time period
        /// </summary>
        public MilestonePersonEntry[] GetCurrentMilestones(
            Person person, DateTime startDate, DateTime endDate, int defaultMilestoneId)
        {
            MilestonePersonEntry[] milestones = 
                TimeEntryDAL.GetCurrentMilestones(person, startDate, endDate, defaultMilestoneId);
            return milestones;
        }

        /// <summary>
        /// Get milestones by person for given time period exclusively for Time Entry page.
        /// </summary>
        public MilestonePersonEntry[] GetTimeEntryMilestones(Person person, DateTime startDate, DateTime endDate)
        {
            return TimeEntryDAL.GetTimeEntryMilestones(person, startDate, endDate);
        }

        // ReSharper disable InconsistentNaming
        public TimeEntryRecord[] GetAllTimeEntries(TimeEntrySelectContext selectContext, int startRow, int maxRows)
        // ReSharper restore InconsistentNaming
        {
            return TimeEntryDAL.GetAllTimeEntries(selectContext);
        }

        public int GetTimeEntriesCount(TimeEntrySelectContext selectContext)
        {
            return TimeEntryDAL.GetTimeEntriesCount(selectContext);
        }
        /// <summary>
        /// Toggle IsCorrect property
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        public void ToggleIsCorrect(TimeEntryRecord timeEntry)
        {
            TimeEntryDAL.ToggleIsCorrect(timeEntry);
        }

        /// <summary>
        /// Toggle IsReviewed property
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        public void ToggleIsReviewed(TimeEntryRecord timeEntry)
        {
            TimeEntryDAL.ToggleIsReviewed(timeEntry);
        }

        /// <summary>
        /// Toggle IsChargeable property
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        public void ToggleIsChargeable(TimeEntryRecord timeEntry)
        {
            TimeEntryDAL.ToggleIsChargeable(timeEntry);
        }

        public TimeEntrySums GetTimeEntrySums(TimeEntrySelectContext selectContext)
        {
            return TimeEntryDAL.GetTimeEntrySums(selectContext);
        }

        public void RemoveTimeEntries(int milestonePersonId, int timeTypeId, DateTime startDate, DateTime endDate)
        {
            TimeEntryDAL.RemoveTimeEntries(milestonePersonId, timeTypeId, startDate, endDate);
        }

        #endregion

        #region Event handlers

        private static int ProjectNameComp(Project x, Project y)
        {
            return x.Name.CompareTo(y.Name);
        }

        private static int MilestoneNameComp(Milestone x, Milestone y)
        {
            int result = x.Project.Name.CompareTo(y.Project.Name);
            if (result == 0)
            {
                result = x.Description.CompareTo(y.Description);
            }

            return result;
        }

        private static int PersonNameComp(Person x, Person y)
        {
            return x.PersonLastFirstName.CompareTo(y.PersonLastFirstName);
        }

        #endregion

        #region Time entry filters

        /// <summary>
        /// Gets all projects that have TE records assigned to them
        /// </summary>
        /// <returns>Projects list</returns>
        public Project[] GetAllTimeEntryProjects()
        {
            Project[] projects = TimeEntryDAL.GetAllTimeEntryProjects();
            Array.Sort(projects, ProjectNameComp);
            return projects;
        }

        /// <summary>
        /// Gets all projects that have TE records assigned to  particular clientId
        /// </summary>
        /// <returns>Projects list</returns>
        public Project[] GetTimeEntryProjectsByClientId(int? clientId)
        {
            Project[] projects = TimeEntryDAL.GetTimeEntryProjectsByClientId(clientId);
            Array.Sort(projects, ProjectNameComp);
            return projects;
        }



        /// <summary>
        /// Gets all milestones that have TE records assigned to them
        /// </summary>
        /// <returns>Milestones list</returns>
        public Milestone[] GetAllTimeEntryMilestones()
        {
            Milestone[] milestones = TimeEntryDAL.GetAllTimeEntryMilestones();
            Array.Sort(milestones, MilestoneNameComp);
            return milestones;
        }

        /// <summary>
        /// Gets all persons that have entered at least one TE
        /// </summary>
        /// <returns>List of persons</returns>
        public Person[] GetAllTimeEntryPersons(DateTime entryDateFrom, DateTime entryDateTo)
        {
            Person[] persons = TimeEntryDAL.GetAllTimeEntryPersons(entryDateFrom, entryDateTo);
            Array.Sort(persons, PersonNameComp);
            return persons;
        }

        #endregion
    }
}

