#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.CompositeObjects;
using DataTransferObjects.ContextObjects;
using PraticeManagement.ActivityLogService;
using PraticeManagement.CalendarService;
using PraticeManagement.ClientService;
using PraticeManagement.Controls.Reports;
using PraticeManagement.ExpenseCategoryService;
using PraticeManagement.ExpenseService;
using PraticeManagement.MilestonePersonService;
using PraticeManagement.MilestoneService;
using PraticeManagement.Objects;
using PraticeManagement.OpportunityService;
using PraticeManagement.OverheadService;
using PraticeManagement.PersonRoleService;
using PraticeManagement.PersonService;
using PraticeManagement.PersonStatusService;
using PraticeManagement.PracticeService;
using PraticeManagement.ProjectGroupService;
using PraticeManagement.ProjectService;
using PraticeManagement.ProjectStatusService;
using PraticeManagement.TimeEntryService;
using PraticeManagement.TimescaleService;
using PraticeManagement.Controls.Generic.Filtering;
using PraticeManagement.Controls.Generic;
using PraticeManagement.ConfigurationService;

#endregion

namespace PraticeManagement.Controls
{
    public class NameValuePair : IIdNameObject
    {
        public int? Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Provides commonly used operations with the data.
    /// </summary>
    public static class DataHelper
    {
        #region Constants

        private const string DefaultIdFieldName = "Id";
        private const string DefaultNameFieldName = "Name";
        private const string CurrentPersonKey = "CurrentPerson";

        #endregion

        public static Opportunity[] GetOpportunitiesPrevNext(int? opportunityId)
        {
            if (opportunityId.HasValue)
            {
                var opportunities = GetFilteredOpportunities();

                var oppCount = opportunities.Length;
                var currentOppIndex = Array.FindIndex(opportunities, opp => opp.Id.Value == opportunityId.Value);

                if (currentOppIndex >= 0)
                {
                    if (oppCount > 2)
                        return new Opportunity[]
                        {
                            opportunities[(oppCount + currentOppIndex - 1) % oppCount],
                            opportunities[(oppCount + currentOppIndex + 1) % oppCount]
                        };

                    if (oppCount == 2)
                        return new Opportunity[]
                        {
                            opportunities[(oppCount + currentOppIndex + 1) % oppCount]
                        };
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a list of <see cref="Opportunity"></see> objects by the specified conditions.
        /// </summary>
        public static Opportunity[] GetFilteredOpportunities()
        {
            var opportunities =
                ServiceCallers.Custom.Opportunity(c => c.OpportunityListAll(OpportunityFilter.Filter));

            var sortingFilter = Controls.Generic.OpportunityList.Filter;
            var comp = new OpportunityComparer(sortingFilter);

            if (comp.SortOrder != OpportunitySortOrder.None)
                Array.Sort(opportunities, comp);

            return opportunities;
        }

        public static Opportunity[] GetFilteredOpportunitiesForDiscussionReview2()
        {
            var opportunities =
                ServiceCallers.Custom.Opportunity(c => c.OpportunityListAll(new OpportunityListContext { IsDiscussionReview2 = true }
                    ));
            var sortingFilter = Controls.Opportunities.OpportunityListControl.Filter;
            var comp = new OpportunityComparer(sortingFilter);

            if (comp.SortOrder != OpportunitySortOrder.None)
                Array.Sort(opportunities, comp);

            return opportunities;
        }

        /// <summary>
        /// Gets a list of <see cref="Opportunity"></see> objects by the specified conditions.
        /// </summary>
        public static Opportunity[] GetOpportunitiesForTargetPerson(int? personId)
        {
            //var comp = new OpportunityComparer(Controls.Generic.OpportunityList.Filter);
            //Array.Sort(opportunities, comp);

            return personId.HasValue ?
                ServiceCallers.Custom.Opportunity(
                    c => c.OpportunityListAll(
                        new OpportunityListContext
                        {
                            TargetPersonId = personId.Value,
                            ActiveClientsOnly = false
                        }))
                        :
                        new Opportunity[] { };
        }

        public static Person CurrentPerson
        {
            get
            {
                if (HttpContext.Current.Items[CurrentPersonKey] == null)
                {
                    using (var serviceClient = new PersonServiceClient())
                    {
                        try
                        {
                            HttpContext.Current.Items[CurrentPersonKey] =
                                serviceClient.GetPersonByAlias(HttpContext.Current.User.Identity.Name);
                        }
                        catch (CommunicationException)
                        {
                            serviceClient.Abort();
                            throw;
                        }
                    }
                }
                return HttpContext.Current.Items[CurrentPersonKey] as Person;
            }
        }

        public static List<DatePoint> GetDatePointsForPerson(DateTime startDate, DateTime endDate, Person person)
        {
            CalendarItem[] calendar;
            using (var client = new CalendarServiceClient())
                calendar = client.GetCalendar(startDate, endDate, person.Id, null);

            return calendar.Select(item => DatePoint.Create(item)).ToList();
        }

        public static Person GetPerson(int personId)
        {
            using (var client = new PersonServiceClient())
                return client.GetPersonDetail(personId);
        }

        public static Person GetPersonWithoutFinancials(int personId)
        {
            using (var client = new PersonServiceClient())
                return client.GetPersonById(personId);
        }

        public static PersonStatus GetPersonStatus(Person person)
        {
            using (var client = new PersonServiceClient())
            {
                person = client.GetPersonDetail(person.Id.Value);

                if (person != null && person.Status != null)
                    return person.Status;
            }

            return null;
        }

        public static void SetNewDefaultManager(Person person)
        {
            using (var client = new PersonServiceClient())
            {
                client.SetAsDefaultManager(person);
            }
        }

        /// <summary>
        /// Enlists number of requested projects by client.
        /// </summary>
        public static int ProjectCountByClient(int clientId)
        {
            using (var client = new ProjectServiceClient())
            {
                return client.ProjectCountByClient(clientId);
            }
        }

        /// <summary>
        /// Checks if the person is a manager to somebody
        /// </summary>
        public static bool IsSomeonesManager(Person person)
        {
            using (var client = new PersonServiceClient())
            {
                return client.IsSomeonesManager(person);
            }
        }

        /// <summary>
        /// Lists managers subordinates
        /// </summary>
        /// <param name="person">Manager</param>
        /// <returns>List of subordinates</returns>
        public static Person[] ListManagersSubordinates(Person person)
        {
            using (var client = new PersonServiceClient())
            {
                return client.ListManagersSubordinates(person);
            }
        }

        /// <summary>
        /// Set new manager
        /// </summary>
        public static void SetNewManager(Person oldManager, Person newManager)
        {
            using (var client = new PersonServiceClient())
            {
                client.SetNewManager(oldManager, newManager);
            }
        }

        private static int Comp(Triple<Person, int[], int> x, Triple<Person, int[], int> y)
        {
            return x.Third.CompareTo(y.Third);
        }

        /// <summary>
        /// Retrives consultans report: Person - load per range - avarage u%
        /// </summary>
        public static List<Triple<Person, int[], int>> GetConsultantsWeeklyReport
            (DateTime startDate,
            int step,
            int duration,
            bool activePersons,
            bool projectedPersons,
            bool activeProjects,
            bool projectedProjects,
            bool experimentalProjects,
            bool internalProjects,
            string timescaleIds,
            string practiceIdList,
            int avgUtil,
            int sortId,
            string sortDirection,
            bool excludeInternalPractices, bool isSampleReport = false)
        {

            var consultants =
                ReportsHelper.GetConsultantsTimelineReport(
                    startDate, duration, step, activePersons, projectedPersons,
                    activeProjects, projectedProjects, experimentalProjects, internalProjects,
                    timescaleIds, practiceIdList, sortId, sortDirection, excludeInternalPractices, isSampleReport);


            return consultants;
        }

        public static List<Triple<Person, int[], int>> ConsultantUtilizationDailyByPerson
            (DateTime startDate,
            int duration,
            bool activeProjects,
            bool projectedProjects,
            bool internalProjects,
            bool experimentalProjects,
            int personId)
        {

            var context = new ConsultantTimelineReportContext
            {
                Start = startDate,
                Period = duration,
                ActiveProjects = activeProjects,
                ProjectedProjects = projectedProjects,
                InternalProjects = internalProjects,
                ExperimentalProjects = experimentalProjects
            };

            var consultants = ServiceCallers.Custom.Person(
                client => client.ConsultantUtilizationDailyByPerson(personId, context));
            var consultantsList = new List<Triple<DataTransferObjects.Person, int[], int>>();
            if (consultants != null && consultants.Any())
                consultantsList.AddRange(consultants);

            return consultantsList;
        }

        public static List<DetailedUtilizationReportBaseItem> GetMilestonePersons(int personId, DateTime startDate, DateTime endDate, bool incActive, bool incProjected, bool incInternal, bool incExperimental)
        {
            var result = new List<DetailedUtilizationReportBaseItem>();

            var context = new ConsultantMilestonesContext
                              {
                                  PersonId = personId,
                                  StartDate = startDate,
                                  EndDate = endDate,
                                  IncludeActiveProjects = incActive,
                                  IncludeProjectedProjects = incProjected,
                                  IncludeCompletedProjects = false,
                                  IncludeInternalProjects = incInternal,
                                  IncludeExperimentalProjects = incExperimental,
                                  IncludeInactiveProjects = false,
                                  IncludeDefaultMileStone = false
                              };

            var personEntries =
                ServiceCallers.Custom.MilestonePerson(
                    client => client.GetConsultantMilestones(context));

            var opportTransition = ServiceCallers.Custom.Opportunity(
                client => client.GetOpportunityTransitionsByPerson(personId));

            foreach (var entry in personEntries)
                result.Add(new DetailedUtilizationReportMilestoneItem(startDate, endDate, entry));

            foreach (var transition in opportTransition)
                result.Add(new DetailedUtilizationReportOpportunityItem(startDate, endDate, transition));

            return result;
        }

        public static void InsertExportActivityLogMessage(string source)
        {
            using (var logClient = new ActivityLogServiceClient())
            {
                try
                {
                    Person cp = CurrentPerson;
                    logClient.ActivityLogInsert(Constants.ActityLog.ExportMessageId,
                                                String.Format(Constants.ActityLog.ExportLogMessage,
                                                              cp.LastName + ", " + cp.FirstName, source));
                }
                catch (CommunicationException)
                {
                    logClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Returns database version
        /// </summary>
        /// <returns>Returns database version</returns>
        public static string GetDatabaseVersion()
        {
            using (var client = new ActivityLogServiceClient())
            {
                try
                {
                    return client.GetDatabaseVersion();
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        /// <summary>
        /// Check's if there's compensation record covering milestone/
        /// See #886 for details.
        /// </summary>
        /// <param name="person">Person to check against</param>
        /// <returns>True if there's such record, false otherwise</returns>
        public static bool IsCompensationCoversMilestone(Person person)
        {
            return IsCompensationCoversMilestone(person, null, null);
        }

        /// <summary>
        /// Verifies whether a user has compensation at this moment
        /// </summary>
        /// <param name="personId">Id of the person</param>
        /// <returns>True if a person has active compensation, false otherwise</returns>
        public static bool CurrentPayExists(int personId)
        {
            return ServiceCallers.Custom.Person(c => c.CurrentPayExists(personId));
        }
        /// <summary>
        /// Check's if there's compensation record covering milestone/
        /// See #886 for details.
        /// </summary>
        /// <param name="person">Person to check against</param>
        /// <returns>True if there's such record, false otherwise</returns>
        public static bool IsCompensationCoversMilestone(Person person, DateTime? start, DateTime? end)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    return serviceClient.IsCompensationCoversMilestone(person, start, end);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Moves milestone
        /// </summary>
        /// <param name="shiftDays">Number of days to move milestone</param>
        /// <param name="selectedIdValue">Id of the milestone to move</param>
        /// <param name="moveFutureMilestones">Whether to move future milestones</param>
        public static void ShiftMilestone(int shiftDays, int selectedIdValue, bool moveFutureMilestones)
        {
            using (var serviceClient = new MilestoneServiceClient())
            {
                try
                {
                    serviceClient.MilestoneMove(selectedIdValue, shiftDays, moveFutureMilestones);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Moves milestone
        /// </summary>
        /// <param name="shiftDays">Number of days to move milestone</param>
        /// <param name="selectedIdValue">Id of the milestone to move</param>
        public static void ShiftMilestoneEnd(int shiftDays, int milestonePersonId, int selectedIdValue)
        {
            using (var serviceClient = new MilestoneServiceClient())
            {
                try
                {
                    serviceClient.MilestoneMoveEnd(selectedIdValue, milestonePersonId, shiftDays);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of practices.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillPracticeList(ListControl control, string firstItemText)
        {
            using (var serviceClient = new PracticeServiceClient())
            {
                try
                {
                    Practice[] practices = serviceClient.GetPracticeList();

                    FillListDefault(control, firstItemText, practices, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of practices.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillPracticeListOnlyActive(ListControl control, string firstItemText)
        {
            using (var serviceClient = new PracticeServiceClient())
            {
                try
                {
                    // Add separate SP for this later.
                    Practice[] practices = GetActivePractices(serviceClient.GetPracticeList());

                    FillListDefault(control, firstItemText, practices, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of Timescale types.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="firstItemText"></param>
        public static void FillTimescaleList(ListControl control, string firstItemText)
        {
            using (var serviceClient = new TimescaleServiceClient())
            {
                try
                {
                    Timescale[] Timescales = serviceClient.GetAll();

                    FillListDefault(control, firstItemText, Timescales, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }



        /// <summary>
        /// Fills the list control with the list of practices.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillPracticeWithOwnerList(ListControl control, string firstItemText)
        {
            using (var serviceClient = new PracticeServiceClient())
            {
                try
                {
                    Practice[] practices = serviceClient.GetPracticeList();

                    FillListDefault(
                        control, firstItemText, practices, false,
                        DefaultIdFieldName, "PracticeWithOwner");
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of practices.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillPracticeWithOwnerListOnlyActive(ListControl control, string firstItemText)
        {
            using (var serviceClient = new PracticeServiceClient())
            {
                try
                {
                    // Create separate SP for this later.
                    Practice[] practices = GetActivePractices(serviceClient.GetPracticeList());

                    FillListDefault(
                        control, firstItemText, practices, false,
                        DefaultIdFieldName, "PracticeWithOwner");
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Select only active practices.
        /// </summary>
        /// <param name="practices"></param>
        /// <returns></returns>
        private static Practice[] GetActivePractices(Practice[] practices)
        {
            return practices.AsQueryable().Where(p => p.IsActive).ToArray();
        }
        /// <summary>
        /// Fills the list control with the list of practices.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillPracticeList(Person person, ListControl control, string firstItemText)
        {
            Practice[] practices = GetPractices(person);

            FillListDefault(control, firstItemText, practices, false);
        }

        public static Practice[] GetPractices(Person person)
        {
            using (var serviceClient = new PracticeServiceClient())
            {
                try
                {
                    return serviceClient.PracticeListAll(person);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static Practice[] GetPracticeById(int? id)
        {
            using (var serviceClient = new PracticeServiceClient())
            {
                try
                {
                    return serviceClient.PracticeGetById(id);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of active persons.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillPersonList(ListControl control, string firstItemText)
        {
            FillPersonList(control, firstItemText, DateTime.MinValue, DateTime.MinValue);
        }

        /// <summary>
        /// Fills the list control with the list of active persons.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        /// <param name="startDate">mileStone start date</param>
        /// <param name="endDate">mileStone end date</param>
        public static void FillPersonList(ListControl control, string firstItemText, DateTime startDate,
                                          DateTime endDate)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person[] persons = serviceClient.PersonListAllShort(null, null, startDate, endDate);

                    Array.Sort(persons);

                    FillPersonList(control, firstItemText, persons, String.Empty);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static IEnumerable<Person> GetAllPersons()
        {
            return ServiceCallers.Custom.Person(c => c.PersonListAllShort(null, null, DateTime.MinValue, DateTime.MaxValue));
        }

        public static void FillPersonListForImpersonate(ListControl control)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person[] persons = serviceClient.GetPersonListWithCurrentPay(null, true, Int16.MaxValue, 0,
                                                                   String.Empty, null,
                                                                   Thread.CurrentPrincipal.Identity.Name, null);
                    control.Items.Clear();
                    if (persons.Length == 0)
                        control.Items.Add(new ListItem(Resources.Controls.NotAvailableText, null));
                    else
                        foreach (Person person in persons)
                        {
                            if (person.Status != null && person.Status.Id == (int)PersonStatusType.Active &&
                                !person.LockedOut)
                                control.Items.Add(new ListItem(
                                                      person.PersonLastFirstName,
                                                      person.Alias));
                        }
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of active persons who are not in the Administration practice.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        /// <param name="startDate">mileStone start date</param>
        /// <param name="endDate">mileStone end date</param>
        public static void FillPersonListForMilestone(
            ListControl control,
            string firstItemText,
            int? milestonePersonId,
            DateTime startDate,
            DateTime endDate)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    var persons = serviceClient.PersonListAllForMilestone(milestonePersonId, startDate, endDate);
                    // persons = persons.ToList().FindAll(item => item.HireDate <= startDate).ToArray();

                    Array.Sort(persons);

                    FillPersonList(control, firstItemText, persons, String.Empty);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of subordinated persons.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        /// <param name="practiceManagerId">An ID of the practice manager to retrieve the list for.</param>
        public static void FillSubordinatesList(ListControl control, string firstItemText, int practiceManagerId)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person[] persons = serviceClient.GetSubordinates(practiceManagerId);

                    FillPersonList(control, firstItemText, persons, practiceManagerId.ToString());
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of One-Off persons.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        /// <param name="today">A date today</param>
        public static void FillOneOffList(ListControl control, string firstItemText, DateTime today)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    var persons = serviceClient.GetOneOffList(today, HttpContext.Current.User.Identity.Name);

                    Array.Sort(persons);

                    FillPersonList(control, firstItemText, persons, "-1");
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of active recruiters.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        /// <param name="personId">An ID of the <see cref="Person"/> to fill the list for.</param>
        /// <param name="hireDate">A Hire Date of the person.</param>
        public static void FillRecruiterList(ListControl control,
                                             string firstItemText,
                                             int? personId,
                                             DateTime? hireDate)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person[] persons = serviceClient.GetRecruiterList(personId, hireDate);

                    FillPersonList(control, firstItemText, persons, String.Empty);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of active salespersons.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        public static void FillSalespersonList(ListControl control, string firstItemText, bool includeInactive)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person[] persons = serviceClient.GetSalespersonList(includeInactive);

                    FillPersonList(control, firstItemText, persons, String.Empty);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of active persons having  Director Seniority.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="firstItemText"></param>

        public static void FillDirectorsList(ListControl control, string firstItemText)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person[] persons = serviceClient.PersonListShortByRoleAndStatus((int)DataTransferObjects.PersonStatusType.Active, DataTransferObjects.Constants.RoleNames.DirectorRoleName);

                    FillPersonList(control, firstItemText, persons, String.Empty);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static void FillSalespersonListOnlyActive(ListControl control, string firstItemText)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person[] persons = GetActivePersons(serviceClient.GetSalespersonList(false));

                    FillPersonList(control, firstItemText, persons, String.Empty);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private static Person[] GetActivePersons(Person[] persons)
        {
            return persons.AsQueryable().Where(p => p.Status.Id == 1).ToArray(); // Here Status.Id == 1 means only active person. (Not projected)
        }
        /// <summary>
        /// Fills the list control with the list of active salespersons.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        public static void FillSalespersonList(Person person, ListControl control, string firstItemText,
                                               bool includeInactive)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person[] persons = serviceClient.PersonListSalesperson(person, includeInactive);

                    FillPersonList(control, firstItemText, persons, String.Empty);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }


        /// <summary>
        /// Fills the list control with the list of active practice managers.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        /// <param name="projectId">An ID of the project to the Practice Maneger be selected for.</param>
        /// <param name="endDate">An End Date of the project to the Practice Maneger be selected for.</param>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        public static void FillPracticeManagerList(ListControl control,
                                                   string firstItemText,
                                                   int? projectId,
                                                   DateTime? endDate,
                                                   bool includeInactive)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person[] persons = serviceClient.GetPracticeManagerList(projectId, endDate, includeInactive);

                    FillPersonList(control, firstItemText, persons, String.Empty);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of active practice managers.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        /// <param name="endDate">An End Date of the project to the Practice Maneger be selected for.</param>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        public static void FillProjectOwnerList(
            ListControl control,
            string firstItemText,
            DateTime? endDate,
            bool includeInactive)
        {
            FillProjectOwnerList(control, firstItemText, endDate, includeInactive, null);
        }

        /// <summary>
        /// Fills the list control with the list of active practice managers.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        /// <param name="endDate">An End Date of the project to the Practice Maneger be selected for.</param>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        /// <param name="person">Person who requests the info</param>
        public static void FillProjectOwnerList(
            ListControl control,
            string firstItemText,
            DateTime? endDate,
            bool includeInactive,
            Person person)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    var persons = serviceClient.PersonListProjectOwner(endDate, includeInactive, person);

                    FillPersonList(control, firstItemText, persons, String.Empty);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of active practice managers.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        /// <param name="projectId">An ID of the project to the Practice Maneger be selected for.</param>
        /// <param name="endDate">An End Date of the project to the Practice Maneger be selected for.</param>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        public static void FillPracticeManagerList(
            Person person,
            ListControl control,
            string firstItemText,
            int? projectId,
            DateTime? endDate,
            bool includeInactive)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person[] persons = serviceClient.PersonListPracticeManager(person, projectId, endDate,
                                                                               includeInactive);

                    FillPersonList(control, firstItemText, persons, String.Empty);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private static void FillPersonList(ListControl control, string firstItemText, Person[] persons,
                                           string firstItemValue)
        {
            control.Items.Clear();

            if (!string.IsNullOrEmpty(firstItemText))
            {
                control.Items.Add(new ListItem(firstItemText, firstItemValue));
            }

            if (persons.Length > 0)
            {
                foreach (Person person in persons)
                {
                    control.Items.Add(new ListItem(
                                          person.PersonLastFirstName,
                                          person.Id.Value.ToString()));
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of registered overhead rate types.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillOverheadRateTypeList(ListControl control, string firstItemText)
        {
            using (var serviceClient = new OverheadServiceClient())
            {
                try
                {
                    OverheadRateType[] types = serviceClient.GetRateTypes();

                    FillListDefault(control, firstItemText, types, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of project statuses.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillProjectStatusList(ListControl control, string firstItemText)
        {
            using (var serviceClient = new ProjectStatusServiceClient())
            {
                try
                {
                    ProjectStatus[] statuses = serviceClient.GetProjectStatuses();

                    FillListDefault(control, firstItemText, statuses, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of time entry projects.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillTimeEntryProjectsList(ListControl control, string firstItemText, int? selectedValue)
        {
            var projects = ServiceCallers.Custom.TimeEntry(c => c.GetAllTimeEntryProjects());
            FillCheckBoxList(control, firstItemText, selectedValue, projects, "-1");
        }

        /// <summary>
        /// Fills the list control with the list of time entry projects.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillTimeEntryPersonList(ListControl control, string firstItemText, int? selectedValue)
        {
            var persons = ServiceCallers.Custom.Person(c => c.PersonListAllShort(null, null, DateTime.MinValue, DateTime.MaxValue));
            FillCheckBoxList(control, firstItemText, selectedValue, persons, "-1");
        }

        public static void FillTimeEntryPersonList(ListControl control, string firstItemText, int? selectedValue, string personStatusIdsList, int? personId)
        {
            var persons = ServiceCallers.Custom.Person(c => c.GetPersonListByStatusList(personStatusIdsList, personId));
            FillCheckBoxList(control, firstItemText, selectedValue, persons, "-1");
        }

        /// <summary>
        /// Fills the list control with the list of persons.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="persons">Persons which needs to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillTimeEntryPersonList(ListControl control, string firstItemText, int? selectedValue, List<Person> persons)
        {
            FillCheckBoxList(control, firstItemText, selectedValue, persons.ToArray(), "-1");
        }

        /// <summary>
        /// Fills the list control with the list of projects.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static IEnumerable<Project> FillProjectsList(ListControl control, string firstItemText)
        {
            using (var serviceClient = new ProjectServiceClient())
            {
                try
                {
                    return serviceClient.GetProjectListCustom(true, true, true, true);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private static void FillCheckBoxList<T>(
            ListControl control, string firstItemText, int? selectedValue, T[] items, string firstItemValue) where T : IIdNameObject
        {
            control.Items.Clear();
            control.Items.Add(new ListItem(firstItemText, firstItemValue));

            foreach (var project in items.OrderBy(p => p.Name))
            {
                var listitem = new ListItem(
                    project.ToString(),
                    project.Id.Value.ToString());

                control.Items.Add(listitem);

                if (selectedValue.HasValue && project.Id.Value == selectedValue)
                    listitem.Selected = true;
            }
        }

        public static string FormatDetailedProjectName(Project project)
        {
            return project.DetailedProjectTitle;
        }

        /// <summary>
        /// Fills the list control with the list of person roles.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillPersonRoleList(ListControl control, string firstItemText)
        {
            using (var serviceClient = new PersonRoleServiceClient())
            {
                try
                {
                    PersonRole[] roles = serviceClient.GetPersonRoles();

                    FillListDefault(control, firstItemText, roles, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of project statuses.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillPersonStatusList(ListControl control)
        {
            using (var serviceClient = new PersonStatusServiceClient())
            {
                try
                {
                    PersonStatus[] statuses = serviceClient.GetPersonStatuses();

                    FillListDefault(control, String.Empty, statuses, true);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of expense categories.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillExpenseCategoriesList(ListControl control, string firstItemText)
        {
            using (var serviceClient = new ExpenseCategoryServiceClient())
            {
                try
                {
                    ExpenseCategory[] categories = serviceClient.GetCategoryList();

                    FillListDefault(control, firstItemText, categories, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of expense bases.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillExpenseBasesList(ListControl control, string firstItemText)
        {
            using (var serviceClient = new ExpenseServiceClient())
            {
                try
                {
                    ExpenseBasis[] bases = serviceClient.GetExpenseBasisList();

                    FillListDefault(control, firstItemText, bases, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of week paid options.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillWeekPaidOptionsList(ListControl control, string firstItemText)
        {
            using (var serviceClient = new ExpenseServiceClient())
            {
                try
                {
                    WeekPaidOption[] bases = serviceClient.GetWeekPaidOptionList();

                    FillListDefault(control, firstItemText, bases, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of person's seniorities.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        public static void FillSenioritiesList(ListControl control, string firstItemText = null)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    var seniorities = serviceClient.ListSeniorities();

                    FillListDefault(control, firstItemText, seniorities, firstItemText == null);
                    if (firstItemText == null)
                        control.SelectedIndex = control.Items.Count - 1;
                    else
                        control.SelectedIndex = 0;
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static void FillProjectGroupList(ListControl control, int? clientId, int? projectId)
        {
            using (var serviceClient = new ProjectGroupServiceClient())
            {
                try
                {
                    ProjectGroup[] groups = serviceClient.GroupListAll(clientId, projectId);
                    groups = groups.AsQueryable().Where(g => g.IsActive).ToArray();

                    FillListDefault(control, null, groups, true);

                    // control.Items.Insert(0, new ListItem(Resources.Controls.DefaultGroup, string.Empty));
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of active clients.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillClientList(ListControl control, string firstItemText)
        {
            FillClientListWithInactive(control, firstItemText, false);
        }

        /// <summary>
        /// Fills the list control with the list of active clients.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillClientListWithInactive(ListControl control, string firstItemText)
        {
            FillClientListWithInactive(control, firstItemText, true);
        }

        /// <summary>
        /// Fills the client list control with the list of active clients and groups list control with corresponding groups
        /// </summary>
        /// <param name="clientList">Clients list control</param>
        /// <param name="groupList">Groups list control</param>
        public static void FillClientsAndGroups(CascadingMsdd clientList, ListControl groupList)
        {
            //  If current user is administrator, don't apply restrictions
            Person person =
                Roles.IsUserInRole(
                    CurrentPerson.Alias,
                    DataTransferObjects.Constants.RoleNames.AdministratorRoleName)
                    ? null
                    : CurrentPerson;

            IEnumerable<Client> clients = GetAllClientsSecure(person, false);


            PrepareClientList(clientList, clients);
            PrepareGroupList(clientList, groupList, clients);
        }

        /// <summary>
        /// Fills the client list control with the list of active clients and groups list control with corresponding groups in person detail page.
        /// </summary>
        /// <param name="clientList">Clients list control</param>
        /// <param name="groupList">Groups list control</param>
        public static void FillClientsAndGroupsCheckBoxListInPersonDetailPage(CascadingMsdd clientList, ListControl groupList)
        {

            //  If current user is administrator, don't apply restrictions
            Person person =
                (
                Roles.IsUserInRole(CurrentPerson.Alias, DataTransferObjects.Constants.RoleNames.AdministratorRoleName) ||
                Roles.IsUserInRole(CurrentPerson.Alias, DataTransferObjects.Constants.RoleNames.HRRoleName)
                )
                    ? null
                    : CurrentPerson;

            IEnumerable<Client> clients = GetAllClientsSecure(person, false);


            PrepareClientList(clientList, clients);
            PrepareGroupList(clientList, groupList, clients);
        }

        public static IEnumerable<Person> GetPersonsInMilestone(Milestone milestone)
        {
            MilestonePersonServiceClient serviceClient = null;
            MilestonePerson[] milestonePersonList;
            using (serviceClient = new MilestonePersonServiceClient())
            {
                try
                {
                    milestonePersonList = serviceClient.GetMilestonePersonListByMilestone(milestone.Id.Value);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }

            if (milestonePersonList != null)
                foreach (MilestonePerson mp in milestonePersonList)
                    yield return mp.Person;
            else
                yield return null;
        }

        public static IEnumerable<Person> GetPersonsInMilestone(Project project)
        {
            MilestonePersonServiceClient serviceClient = null;
            MilestonePerson[] milestonePersonsList = null;
            using (serviceClient = new MilestonePersonServiceClient())
            {
                try
                {
                    milestonePersonsList = serviceClient.GetMilestonePersonListByProjectWithoutPay(project.Id.Value);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }

            if (milestonePersonsList != null)
            {
                var persons = new List<DataTransferObjects.Person>();
                foreach (MilestonePerson milestonePerson in milestonePersonsList)
                    persons.Add(milestonePerson.Person);
                return persons;
            }
            else
                return null;
        }

        public static Milestone GetMileStoneById(int mileStoneId)
        {
            MilestoneServiceClient serviceClient = null;
            using (serviceClient = new MilestoneServiceClient())
            {
                try
                {
                    return serviceClient.GetMilestoneById(mileStoneId);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Populates groups list and initializes dependencies between source and target controls
        /// </summary>
        /// <param name="clientList">Clients list control</param>
        /// <param name="groupList">Groups list control</param>
        /// <param name="clients">Collection of clients</param>
        private static void PrepareGroupList(CascadingMsdd clientList, ListControl groupList,
                                             IEnumerable<Client> clients)
        {
            groupList.Items.Clear();

            int clientIndex = 1; // Client list checkbox index
            int groupIndex = 1; // Group list checkbox index
            //  Iterates through clients and corresponding groups
            foreach (Client client in clients)
            {
                foreach (ProjectGroup group in client.Groups)
                {
                    //  Add item to the groups list
                    string itemText = String.Format(
                        Resources.Controls.GroupNameFormat,
                        client.Name, group.Name);
                    groupList.Items.Add(
                        new ListItem(itemText, group.Id.Value.ToString()));

                    // Add dependence between client and group
                    clientList.AddDependence(
                        new CascadingMsdd.ControlDependence(
                            clientList.ClientID, clientIndex,
                            groupList.ClientID, groupIndex));

                    groupIndex++;
                }

                clientIndex++;
            }

            if (groupList.Items.Count > 0)
            {
                groupList.Items.Insert(0,
                                       new ListItem(Resources.Controls.AllGroupsText, String.Empty));
            }
        }

        /// <summary>
        /// Fills client list with values
        /// </summary>
        /// <param name="clientList">Client list control</param>
        /// <param name="clients">Collection to databind to the list</param>
        private static void PrepareClientList(CascadingMsdd clientList, IEnumerable<Client> clients)
        {
            clientList.Items.Clear();

            clientList.DataTextField = DefaultNameFieldName;
            clientList.DataValueField = DefaultIdFieldName;
            clientList.DataSource = clients;
            clientList.DataBind();

            if (clientList.Items.Count > 0)
            {
                clientList.Items.Insert(0,
                                        new ListItem(Resources.Controls.AllClientsText, String.Empty));
            }
        }

        /// <summary>
        /// Fills the list of clients available for the specific project.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        /// <param name="projectId">An ID of the project to retrive the data for.</param>
        public static void FillClientListForProject(ListControl control, string firstItemText, int? projectId)
        {
            using (var serviceClient = new ClientServiceClient())
            {
                try
                {
                    Client[] clients = serviceClient.ClientListAllForProject(projectId, CurrentPerson.Id);

                    FillListDefault(control, firstItemText, clients, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }


        public static IEnumerable<Client> GetAllClientsSecure(Person person, bool inactives)
        {
            using (var serviceClient = new ClientServiceClient())
            {
                try
                {
                    Client[] clients = serviceClient.ClientListAllSecure(person, inactives);
                    Array.Sort(clients, (c1, c2) => c1.Name.CompareTo(c2.Name));
                    return clients;
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private static void FillClientListWithInactive(ListControl control, string firstItemText, bool includeInactive)
        {
            using (var serviceClient = new ClientServiceClient())
            {
                try
                {
                    Client[] clients =
                        includeInactive
                            ?
                                serviceClient.ClientListAllWithInactive()
                            : serviceClient.ClientListAll();

                    FillListDefault(control, firstItemText, clients, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static Project[] ListProjectsByClient(int? clientId, string sortBy)
        {
            return ServiceCallers.Invoke<ProjectServiceClient, Project[]>(
                client => client.ListProjectsByClientWithSort(clientId, /*CurrentPerson.Alias*/null, sortBy));
        }

        public static Project[] GetTimeEntryProjectsByClientId(int? clientId)
        {
            using (var serviceClient = new TimeEntryServiceClient())
            {
                try
                {
                    Project[] projects = serviceClient.GetTimeEntryProjectsByClientId(clientId);

                    return projects;
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of opportunity statuses.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillOpportunityStatusList(ListControl control, string firstItemText)
        {
            using (var serviceClient = new OpportunityServiceClient())
            {
                try
                {
                    OpportunityStatus[] statuses = serviceClient.OpportunityStatusListAll();

                    FillListDefault(control, firstItemText, statuses, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static OpportunityPriority[] GetOpportunityPrioritiesListAll()
        {
            using (var serviceClient = new OpportunityServiceClient())
            {
                try
                {
                    OpportunityPriority[] opportunityPriorities = serviceClient.GetOpportunityPrioritiesListAll();
                    return opportunityPriorities;

                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Fills the list control with the list of opportunity transition statuses.
        /// </summary>
        /// <param name="control">The control to be filled.</param>
        /// <param name="firstItemText">The text to be displayed by default.</param>
        public static void FillOpportinityTransitionStatusList(ListControl control, string firstItemText)
        {
            using (var serviceClient = new OpportunityServiceClient())
            {
                try
                {
                    OpportunityTransitionStatus[] statuses = serviceClient.OpportunityTransitionStatusListAll();

                    FillListDefault(control, firstItemText, statuses, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static void FillListDefault(ListControl control, string firstItemText, object[] statuses,
                                            bool noFirstItem)
        {
            FillListDefault(control, firstItemText, statuses, noFirstItem, DefaultIdFieldName, DefaultNameFieldName);
        }

        public static void FillListDefault(ListControl control, string firstItemText, object[] statuses,
                                            bool noFirstItem, string valueField, string NameField)
        {
            control.AppendDataBoundItems = true;
            control.Items.Clear();
            if (!noFirstItem && statuses != null && statuses.Length > 0)
            {
                control.Items.Add(new ListItem(firstItemText, String.Empty));
            }
            if (statuses == null || !statuses.Any())
            {
                var item = new ListItem(firstItemText, String.Empty);
                item.Enabled = false;
                control.Items.Add(item);
            }
            else
            {
                control.DataValueField = valueField;
                control.DataTextField = NameField;
                control.DataSource = statuses;
                control.DataBind();
            }
        }

        public static int CloneProject(ProjectCloningContext context)
        {
            using (var serviceClient = new ProjectServiceClient())
            {
                try
                {
                    return serviceClient.CloneProject(context);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static PersonPermission GetPermissions(Person person)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    return serviceClient.GetPermissions(person);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }


        public static void SaveResourceKeyValuePairs(SettingsType settingType, Dictionary<string, string> dictionary)
        {
            using (var serviceClient = new ConfigurationServiceClient())
            {
                try
                {
                    serviceClient.SaveResourceKeyValuePairs(settingType, dictionary); ;
                }
                catch (FaultException<ExceptionDetail> ex)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static Dictionary<string, string> GetResourceKeyValuePairs(SettingsType settingType)
        {
            using (var serviceClient = new ConfigurationServiceClient())
            {
                try
                {
                    return serviceClient.GetResourceKeyValuePairs(settingType); ;
                }
                catch (FaultException<ExceptionDetail> ex)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static void FillBusinessDevelopmentManagersList(ListControl control, string firstItemText, DateTime startDate, DateTime endDate)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person[] persons = serviceClient.PersonListByCategoryTypeAndPeriod(DataTransferObjects.BudgetCategoryType.BusinessDevelopmentManager,
                                                                                        startDate,
                                                                                        endDate);

                    FillListDefault(control, firstItemText, persons, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static void FillOpportunityPrioritiesList(DropDownList ddlPriority, string firstItemText)
        {
            OpportunityPriority[] priorities = DataHelper.GetOpportunityPrioritiesListAll();

            FillListDefault(ddlPriority, firstItemText, priorities, false, "Id", "Priority");

        }
    }
}

