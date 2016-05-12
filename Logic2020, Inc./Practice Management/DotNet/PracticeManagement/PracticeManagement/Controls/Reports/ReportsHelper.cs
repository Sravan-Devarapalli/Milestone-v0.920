using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using DataTransferObjects.Reports;
using PraticeManagement.PersonService;

namespace PraticeManagement.Controls.Reports
{
    public class ReportsHelper
    {
        public static Project[] GetBenchList(DateTime start, DateTime end, string userName)
        {
            bool activePersons = true;
            bool projectedPersons = true;
            bool activeProjects = true;
            bool projectedProjects = true;
            bool experimentalProjects = true;
            string practicesIds = null;

            return GetBenchList(start, end, activePersons, projectedPersons, activeProjects, projectedProjects, experimentalProjects, userName, practicesIds);
        }

        public static Project[] GetBenchList(DateTime start, DateTime end, bool activePersons,
                                                        bool projectedPersons, bool activeProjects,
                                                        bool projectedProjects, bool experimentalProjects,
                                                        string userName,
                                                        string practicesIds,
                                                        bool completedProjects = false)
        {
            var context = new BenchReportContext
                {
                    Start = start,
                    End = end,
                    ActivePersons = activePersons,
                    ProjectedPersons = projectedPersons,
                    ActiveProjects = activeProjects,
                    ProjectedProjects = projectedProjects,
                    ExperimentalProjects = experimentalProjects,
                    CompletedProjects = completedProjects,
                    UserName = userName,
                    PracticeIds = practicesIds
                };

            return ServiceCallers.Custom.Project(c => c.GetBenchList(context));
        }

        public static Project[] GetBenchListWithoutBenchTotalAndAdminCosts(BenchReportContext context)
        {
            return ServiceCallers.Custom.Project(c => c.GetBenchListWithoutBenchTotalAndAdminCosts(context));
        }

        public static List<ConsultantUtilizationPerson> GetConsultantsTimelineReport(
            DateTime start, int granularity, int period,
            bool activePersons, bool projectedPersons, bool activeProjects,
            bool projectedProjects, bool experimentalProjects, bool internalProjects,bool proposedProjects,bool completedProjects,
            string timescaleIds, string practiceIdList, int sortId, string sortDirection,
            bool excludeInternalPractices, int utilizationType,bool includeBadgeStatus,bool isSampleReport,string divisionIds)
        {
            var context = new ConsultantTimelineReportContext
                              {
                                  Start = start,
                                  Granularity = granularity,
                                  Period = period,
                                  ProjectedPersons = projectedPersons,
                                  ProjectedProjects = projectedProjects,
                                  ProposedProjects = proposedProjects,
                                  CompletedProjects = completedProjects,
                                  ActivePersons = activePersons,
                                  ActiveProjects = activeProjects,
                                  ExperimentalProjects = experimentalProjects,
                                  TimescaleIdList = timescaleIds,
                                  PracticeIdList = practiceIdList,
                                  ExcludeInternalPractices = excludeInternalPractices,
                                  InternalProjects = internalProjects,
                                  SortId = sortId,
                                  SortDirection = sortDirection,
                                  IsSampleReport = isSampleReport,
                                  UtilizationType = utilizationType,
                                  IncludeBadgeStatus = includeBadgeStatus,
                                  //ExcludeInvestmentResource = excludeInvestmentResource,
                                  DivisionIdList = divisionIds
                              };

            var consultants = ServiceCallers.Custom.Person(
                client => client.GetConsultantUtilizationWeekly(context));
            var consultantsList = new List<ConsultantUtilizationPerson>();
            if (consultants != null && consultants.Any())
            {
                consultantsList.AddRange(consultants);
            }

            return consultantsList;
        }

        public static List<ConsultantDemandItem> GetConsultantDemand(DateTime startDate, DateTime endDate)
        {
            var consultants = ServiceCallers.Custom.Person(
                client => client.GetConsultantswithDemand(startDate, endDate));

            return consultants.ToList();
        }
    }
}

