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
            try
            {
                return TimeEntryDAL.GetAllTimeTypes();
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetAllTimeTypes", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Removes given time type
        /// </summary>
        /// <param name="timeType">Time type to remove</param>
        public void RemoveTimeType(TimeTypeRecord timeType)
        {
            try
            {
                TimeEntryDAL.RemoveTimeType(timeType);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "RemoveTimeType", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
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
                TimeEntryDAL.UpdateTimeType(timeType);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "UpdateTimeType", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
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
                return TimeEntryDAL.AddTimeType(timeType);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "AddTimeType", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        #endregion

        #region Time Zone

        public void SetTimeZone(Timezone timezone)
        {
            try
            {
                TimeEntryDAL.SetTimeZone(timezone);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "SetTimeZone", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public List<Timezone> TimeZonesAll()
        {
            try
            {
                return TimeEntryDAL.TimeZonesAll();
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "TimeZonesAll", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        #endregion

        #region Time entry

        public void RemoveTimeEntryById(int id)
        {
            try
            {
                RemoveTimeEntry(new TimeEntryRecord { Id = id });
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "RemoveTimeEntryById", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Removes entry
        /// </summary>
        /// <param name="timeEntry">Time entry to remove</param>
        public void RemoveTimeEntry(TimeEntryRecord timeEntry)
        {
            try
            {
                TimeEntryDAL.RemoveTimeEntry(timeEntry);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "RemoveTimeEntry", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Adds new time entry
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        /// <param name="defaultMpId">default milestone id</param>
        public int AddTimeEntry(TimeEntryRecord timeEntry, int defaultMpId)
        {
            try
            {
                return TimeEntryDAL.AddTimeEntry(timeEntry, defaultMpId);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "AddTimeEntry", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Updates time entry
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        public void UpdateTimeEntry(TimeEntryRecord timeEntry, int defaultMilestoneId)
        {
            try
            {
                TimeEntryDAL.UpdateTimeEntry(timeEntry, defaultMilestoneId);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "UpdateTimeEntry", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
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
            try
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
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ConstructAndUpdateTimeEntry", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Get time entries by person
        /// </summary>
        public TimeEntryRecord[] GetTimeEntriesForPerson(Person person, DateTime startDate, DateTime endDate)
        {
            try
            {
                return TimeEntryDAL.GetTimeEntriesForPerson(person, startDate, endDate);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetTimeEntriesForPerson", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public GroupedTimeEntries<Person> GetTimeEntriesProject(TimeEntryProjectReportContext reportContext)
        {
            try
            {
                return TimeEntryDAL.GetTimeEntriesByProject(reportContext);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetTimeEntriesProject", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public GroupedTimeEntries<TimeEntryHours> GetTimeEntriesProjectCumulative(TimeEntryPersonReportContext reportContext)
        {
            try
            {
                return TimeEntryDAL.GetTimeEntriesByProjectCumulative(reportContext);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetTimeEntriesProjectCumulative", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public GroupedTimeEntries<Project> GetTimeEntriesPerson(TimeEntryPersonReportContext reportContext)
        {
            try
            {
                return TimeEntryDAL.GetTimeEntriesByPerson(reportContext);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetTimeEntriesPerson", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Get milestones by person for given time period
        /// </summary>
        public MilestonePersonEntry[] GetCurrentMilestones(
            Person person, DateTime startDate, DateTime endDate, int defaultMilestoneId)
        {
            try
            {
                MilestonePersonEntry[] milestones = 
                    TimeEntryDAL.GetCurrentMilestones(person, startDate, endDate, defaultMilestoneId);
                return milestones;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetCurrentMilestones", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Get milestones by person for given time period exclusively for Time Entry page.
        /// </summary>
        public MilestonePersonEntry[] GetTimeEntryMilestones(Person person, DateTime startDate, DateTime endDate)
        {
            try
            {
                return TimeEntryDAL.GetTimeEntryMilestones(person, startDate, endDate);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetTimeEntryMilestones", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        // ReSharper disable InconsistentNaming
        public TimeEntryRecord[] GetAllTimeEntries(TimeEntrySelectContext selectContext, int startRow, int maxRows)
        // ReSharper restore InconsistentNaming
        {
            try
            {
                return TimeEntryDAL.GetAllTimeEntries(selectContext);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetAllTimeEntries", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public int GetTimeEntriesCount(TimeEntrySelectContext selectContext)
        {
            try
            {
                return TimeEntryDAL.GetTimeEntriesCount(selectContext);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetTimeEntriesCount", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }
        /// <summary>
        /// Toggle IsCorrect property
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        public void ToggleIsCorrect(TimeEntryRecord timeEntry)
        {
            try
            {
                TimeEntryDAL.ToggleIsCorrect(timeEntry);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ToggleIsCorrect", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Toggle IsReviewed property
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        public void ToggleIsReviewed(TimeEntryRecord timeEntry)
        {
            try
            {
                TimeEntryDAL.ToggleIsReviewed(timeEntry);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ToggleIsReviewed", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Toggle IsChargeable property
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        public void ToggleIsChargeable(TimeEntryRecord timeEntry)
        {
            try
            {
                TimeEntryDAL.ToggleIsChargeable(timeEntry);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ToggleIsChargeable", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public TimeEntrySums GetTimeEntrySums(TimeEntrySelectContext selectContext)
        {
            try
            {
                return TimeEntryDAL.GetTimeEntrySums(selectContext);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetTimeEntrySums", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public void RemoveTimeEntries(int milestonePersonId, int timeTypeId, DateTime startDate, DateTime endDate)
        {
            try
            {
                TimeEntryDAL.RemoveTimeEntries(milestonePersonId, timeTypeId, startDate, endDate);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "RemoveTimeEntries", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
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
            try
            {
                Project[] projects = TimeEntryDAL.GetAllTimeEntryProjects();
                Array.Sort(projects, ProjectNameComp);
                return projects;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetAllTimeEntryProjects", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Gets all projects that have TE records assigned to  particular clientId
        /// </summary>
        /// <returns>Projects list</returns>
        public Project[] GetTimeEntryProjectsByClientId(int? clientId, int? personId, bool showActiveAndInternalProjectsOnly)
        {
            try
            {
                Project[] projects = TimeEntryDAL.GetTimeEntryProjectsByClientId(clientId, personId, showActiveAndInternalProjectsOnly);
                Array.Sort(projects, ProjectNameComp);
                return projects;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetTimeEntryProjectsByClientId", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Gets all milestones that have TE records assigned to them
        /// </summary>
        /// <returns>Milestones list</returns>
        public Milestone[] GetAllTimeEntryMilestones()
        {
            try
            {
                Milestone[] milestones = TimeEntryDAL.GetAllTimeEntryMilestones();
                Array.Sort(milestones, MilestoneNameComp);
                return milestones;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetAllTimeEntryMilestones", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Gets all persons that have entered at least one TE
        /// </summary>
        /// <returns>List of persons</returns>
        public Person[] GetAllTimeEntryPersons(DateTime entryDateFrom, DateTime entryDateTo)
        {
            try
            {
                Person[] persons = TimeEntryDAL.GetAllTimeEntryPersons(entryDateFrom, entryDateTo);
                Array.Sort(persons, PersonNameComp);
                return persons;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetAllTimeEntryPersons", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public bool HasTimeEntriesForMilestone(int milestoneId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return TimeEntryDAL.HasTimeEntriesForMilestone(milestoneId, startDate, endDate);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "HasTimeEntriesForMilestone", "TimeEntryService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        #endregion
    }
}

