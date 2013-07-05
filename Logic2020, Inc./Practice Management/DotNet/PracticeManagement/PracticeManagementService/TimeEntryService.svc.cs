using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using System.Web;
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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        #endregion Time Zone

        #region Time entry

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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public PersonTimeEntries GetTimeEntriesPerson(TimeEntryPersonReportContext reportContext)
        {
            try
            {
                return TimeEntryDAL.GetTimeEntriesByPerson(reportContext);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetTimeEntriesPerson", "TimeEntryService.svc", string.Empty,
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        #endregion Time entry

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

        #endregion Event handlers

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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
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
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        #endregion Time entry filters

        public List<TimeEntrySection> PersonTimeEntriesByPeriod(int personId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return TimeEntryDAL.PersonTimeEntriesByPeriod(personId, startDate, endDate);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "PersonTimeEntriesByPeriod", "TimeEntryService.svc", string.Empty,
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public System.Data.DataSet TimeEntriesByPersonGetExcelSet(TimeEntryPersonReportContext reportContext)
        {
            return TimeEntryDAL.TimeEntriesByPersonGetExcelSet(reportContext);
        }

        #region TimeTrack Methods

        public void DeleteTimeEntry(int clientId, int projectId, int personId, int timetypeId, DateTime startDate, DateTime endDate, string userName)
        {
            try
            {
                TimeEntryDAL.DeleteTimeEntry(clientId, projectId, personId, timetypeId, startDate, endDate, userName);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "DeleteTimeTrack", "TimeEntryService.svc", string.Empty,
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public void SaveTimeTrack(string timeEntriesXml, int personId, DateTime startDate, DateTime endDate, string userLogin)
        {
            try
            {
                TimeEntryDAL.SaveTimeTrack(timeEntriesXml, personId, startDate, endDate, userLogin);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "SaveTimeTrack", "TimeEntryService.svc", string.Empty,
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public Dictionary<DateTime, bool> GetIsChargeCodeTurnOffByPeriod(int personId, int clientId, int groupId, int projectId, int timeTypeId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return TimeEntryDAL.GetIsChargeCodeTurnOffByPeriod(personId, clientId, groupId, projectId, timeTypeId, startDate, endDate);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetIsChargeCodeTurnOffByPeriod", "TimeEntryService.svc", string.Empty,
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public void SetPersonTimeEntryRecursiveSelection(int personId, int clientId, int projectGroupId, int projectId, int timeEntrySectionId, bool isRecursive, DateTime startDate)
        {
            try
            {
                TimeEntryDAL.SetPersonTimeEntryRecursiveSelection(personId, clientId, projectGroupId, projectId, timeEntrySectionId, isRecursive, startDate);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SetPersonTimeEntrySelection(int personId, int clientId, int projectGroupId, int projectId, int timeEntrySectionId, bool isDelete, DateTime startDate, DateTime endDate, string userName)
        {
            try
            {
                TimeEntryDAL.SetPersonTimeEntrySelection(personId, clientId, projectGroupId, projectId, timeEntrySectionId, isDelete, startDate, endDate, userName);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "SetPersonTimeEntrySelection", "TimeEntryService.svc", string.Empty,
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        #endregion TimeTrack Methods
    }
}
