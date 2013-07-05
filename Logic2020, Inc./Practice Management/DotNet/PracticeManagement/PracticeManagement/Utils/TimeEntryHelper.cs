using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web.Security;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.CompositeObjects;
using DataTransferObjects.ContextObjects;
using DataTransferObjects.TimeEntry;
using PraticeManagement.Configuration;
using PraticeManagement.Controls.TimeEntry;
using PraticeManagement.MilestoneService;
using PraticeManagement.TimeEntryService;

namespace PraticeManagement.Utils
{
    /// <summary>
    /// Time entry utilities
    /// </summary>
    public class TimeEntryHelper
    {
        public static bool HasTimeEntriesForMilestone(int milestoneId, DateTime startDate, DateTime endDate)
        {
            using (var serv = new TimeEntryServiceClient())
            {
                return serv.HasTimeEntriesForMilestone(milestoneId, startDate, endDate);
            }
        }

        #region Data - Time Types

        /// <summary>
        /// Insert new time type
        /// </summary>
        /// <param name="name">Time type name</param>
        public static void AddTimeType(string name)
        {
            var timeType = new TimeTypeRecord { Name = name };
            ServiceCallers.Custom.TimeType(client => client.AddTimeType(timeType));
        }

        #endregion Data - Time Types

        #region Data - Time Entry General

        /// <summary>
        /// Returns all person's TEs for given period
        /// </summary>
        /// <param name="person">Person</param>
        /// <param name="startDate">Begining of the period</param>
        /// <param name="endDate">End of the period</param>
        /// <returns>Set of TE records</returns>
        public static TimeEntryRecord[]
            GetTimeEntriesForPerson(Person person, DateTime startDate, DateTime endDate)
        {
            return ServiceCallers.Custom.TimeEntry(client => client.GetTimeEntriesForPerson(person, startDate, endDate));
        }

        public static TimeEntryRecord[]
            GetTimeEntriesMilestonePerson(MilestonePerson mp)
        {
            using (var serv = new TimeEntryServiceClient())
            {
                if (mp.Person.Id != null)
                    return serv.GetAllTimeEntries(
                        new TimeEntrySelectContext
                        {
                            SortExpression = String.Empty,
                            RequesterId = mp.Person.Id.Value,
                            MilestonePersonId = mp.Id
                        }, 0, 0);

                return new TimeEntryRecord[] { };
            }
        }

        public static TimeEntryRecord[]
            GetTimeEntriesForMilestone(Milestone milestone)
        {
            TimeEntryRecord[] timeEntries;
            using (var serv = new TimeEntryServiceClient())
            {
                if (milestone.Id != null &&
                    milestone.MilestonePersons != null &&
                    milestone.MilestonePersons.Length > 0)
                {
                    timeEntries
                        = serv.GetAllTimeEntries(
                            new TimeEntrySelectContext
                            {
                                SortExpression = String.Empty,
                                RequesterId = milestone.MilestonePersons[0].Person.Id.Value,
                                MilestoneId = milestone.Id.Value
                            }, 0, 0);
                }
                else
                {
                    timeEntries = new TimeEntryRecord[] { };
                }
            }

            Array.Sort(timeEntries);

            return timeEntries;
        }

        /// <summary>
        /// Returns all person's TEs for given period
        /// </summary>
        /// <param name="person">Person</param>
        /// <param name="selectedDates">Dates to check for</param>
        /// <returns>Set of TE records</returns>
        public static TimeEntryRecord[]
            GetTimeEntriesForPerson(Person person, DateTime[] selectedDates)
        {
            DateTime startDate = selectedDates[0];
            DateTime endDate = selectedDates[selectedDates.Length - 1];

            return ServiceCallers.Custom.TimeEntry(client => client.GetTimeEntriesForPerson(person, startDate, endDate));
        }

        #endregion Data - Time Entry General

        #region Toggling

        private static void ToggleTimeEntryProperty(Action<TimeEntryServiceClient> action)
        {
            using (var serv = new TimeEntryServiceClient())
            {
                action(serv);
            }
        }

        public static void ToggleIsCorrect(TimeEntryRecord timeEntry)
        {
            ToggleTimeEntryProperty(c => c.ToggleIsCorrect(timeEntry));
        }

        public static void ToggleIsChargeable(TimeEntryRecord timeEntry)
        {
            ToggleTimeEntryProperty(c => c.ToggleIsChargeable(timeEntry));
        }

        public static void ToggleReviewStatus(TimeEntryRecord timeEntry)
        {
            ToggleTimeEntryProperty(c => c.ToggleIsReviewed(timeEntry));
        }

        #endregion Toggling

        #region Milestones

        /// <summary>
        /// Returns list of current milestones not including default one.
        /// </summary>
        /// <param name="person">Persons to extract milestones for</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        /// <returns>List of milestones</returns>
        public static MilestonePersonEntry[] GetCurrentMilestones(Person person, DateTime startDate, DateTime endDate)
        {
            return GetCurrentMilestones(person, startDate, endDate, false);
        }

        /// <summary>
        /// Returns list of current milestones including default one.
        /// </summary>
        /// <param name="person">Persons to extract milestones for</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        /// <param name="includeDefault">Whether to include default milestone</param>
        /// <returns>List of milestones</returns>
        public static MilestonePersonEntry[]
            GetCurrentMilestones(Person person, DateTime startDate, DateTime endDate, bool includeDefault)
        {
            var milestones = new List<MilestonePersonEntry>();
            int? mileStoneId = MileStoneConfigurationManager.GetMileStoneId();
            int defaultMilestoneId = mileStoneId.HasValue ? mileStoneId.Value : 0;
            using (var serv = new TimeEntryServiceClient())
            {
                milestones.AddRange(serv.GetCurrentMilestones(person, startDate, endDate, defaultMilestoneId));
            }

            if (includeDefault)
            {
                // Needed for the default milestone issue
                //  Check if default milestone is already in the list
                var isDefaultThere = milestones.Any(mpe => mpe.ParentMilestone.Id.Value == defaultMilestoneId);

                //  If it's not, put it there
                if (!isDefaultThere && defaultMilestoneId != 0)
                    milestones.Add(CreateDefaultMpe(startDate, endDate, defaultMilestoneId));
            }
            return milestones.ToArray();
        }

        private static MilestonePersonEntry CreateDefaultMpe(DateTime startDate, DateTime endDate,
                                                             int defaultMilestoneId)
        {
            Milestone defaultMilestone;
            using (var serv = new MilestoneServiceClient())
            {
                defaultMilestone = serv.GetMilestoneDataById(defaultMilestoneId);
            }
            var defMpe = new MilestonePersonEntry
            {
                ParentMilestone = defaultMilestone,
                StartDate = startDate,
                EndDate = endDate
            };
            return defMpe;
        }

        #endregion Milestones

        #region Controls

        public static void FillProjectMilestones(
            DropDownList ddlProjectMilestones,
            IEnumerable<MilestonePersonEntry> milestonePersonEntries)
        {
            List<ListItem> listitems = new List<ListItem>();

            //according to Bug# 2821 we need to add a new item to promt user to select a project-Milestone.
            ddlProjectMilestones.Items.Add(new ListItem("1. Select Project - Milestone", "-1"));
            foreach (MilestonePersonEntry mpe in milestonePersonEntries)
            {
                ListItem item = new ListItem(mpe.ParentMilestone.ToString(Milestone.MilestoneFormat.ProjectMilestone), mpe.MilestonePersonId.ToString());
                if (listitems.Contains(item)) continue;
                listitems.Add(item);
                item.Attributes.Add("title", item.Text);
                ddlProjectMilestones.Items.Add(item);
            }
        }

        #endregion Controls

        #region Adapters

        /// <summary>
        /// Converts business objects to the structure suitable for databinding
        /// </summary>
        public static List<KeyValuePair<int, string>> GetCurrentMilestonesById(int personId, DateTime startDate, DateTime endDate)
        {
            MilestonePersonEntry[] currentMilestones =
                GetCurrentMilestones(new Person(personId), startDate, endDate, false);

            var res = new List<KeyValuePair<int, string>>(currentMilestones.Length);

            foreach (var mpe in currentMilestones)
                res.Add(
                    new KeyValuePair<int, string>(
                        mpe.MilestonePersonId,
                        mpe.ParentMilestone.ToString(
                            Milestone.MilestoneFormat.ClientProjectMilestone)));

            return res;
        }

        #endregion Adapters

        #region Reviewing

        public static IEnumerable<string> GetAllReviewStatuses()
        {
            return Enum.GetNames(typeof(ReviewStatus));
        }

        #endregion Reviewing

        #region Reports

        public static Dictionary<Person, TimeEntryRecord[]> GetTimeEntriesForProject(int projectId, DateTime? startDate, DateTime? endDate, IEnumerable<int> personIdList, int? milestoneId)
        {
            var reportContext = new TimeEntryProjectReportContext
            {
                ProjectId = projectId,
                StartDate = startDate,
                EndDate = endDate,
                PersonIds = personIdList,
                MilestoneId = milestoneId
            };

            var byProject = ServiceCallers.Custom.TimeEntry(client => client.GetTimeEntriesProject(reportContext));

            return byProject._groupedTimeEtnries;
        }

        public static PersonTimeEntries GetTimeEntriesForPerson(int personId, DateTime? startDate, DateTime? endDate, List<int> payTypeIds, List<int> practiceIds)
        {
            var reportContext = new TimeEntryPersonReportContext
            {
                PersonId = personId,
                StartDate = startDate,
                EndDate = endDate,
                PayTypeIds = payTypeIds,
                PracticeIds = practiceIds
            };

            var personTimeEntries = ServiceCallers.Custom.TimeEntry(te => te.GetTimeEntriesPerson(reportContext));

            if (!(personTimeEntries.GroupedTimeEtnries.Values.Count > 0))
            {
                personTimeEntries = new PersonTimeEntries();

                var persons = ServiceCallers.Custom.Person(p => p.GetPersonListByPersonIdsAndPayTypeIds(personId + ",", payTypeIds != null ? FormCSV(payTypeIds) : null, practiceIds != null ? FormCSV(practiceIds) : null, startDate.Value, endDate.Value));

                foreach (var person in persons)
                {
                    return new PersonTimeEntries
                      {
                          Person = new Person() { LastName = person.LastName, FirstName = person.FirstName, Id = personId }
                      };
                }
            }

            return personTimeEntries;
        }

        private static string FormCSV(IEnumerable<int> IdList)
        {
            StringBuilder sb = new StringBuilder(string.Empty);
            foreach (int Id in IdList)
            {
                sb.Append("," + Id);
            }
            return sb.ToString();
        }

        #endregion Reports
    }
}
