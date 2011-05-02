using System;
using System.Collections.Generic;
using System.Data;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
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
                                                        bool completedProjects =false)
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


        public static DataSet GetConsultantsTableReport(DateTime start, DateTime end, bool activePersons,
                                                        bool projectedPersons, bool activeProjects,
                                                        bool projectedProjects, bool internalProjects, bool experimentalProjects)
        {
            var context = new ConsultantTableReportContext
                              {
                                  Start = start,
                                  End = end,
                                  ProjectedPersons = projectedPersons,
                                  ProjectedProjects = projectedProjects,
                                  ActivePersons = activePersons,
                                  ActiveProjects = activeProjects,
                                  InternalProjects = internalProjects,
                                  ExperimentalProjects = experimentalProjects
                              };

            return ServiceCallers.Custom.Person(
                client => client.GetConsultantUtilizationReport(context));
        }

        public static Dictionary<Person, int[]> GetConsultantsTimelineReport(
            DateTime start, int granularity, int period,
            bool activePersons, bool projectedPersons, bool activeProjects,
            bool projectedProjects, bool experimentalProjects, bool internalProjects,
            string timescaleIds, string practiceIdList, int sortId, string sortDirection,
            bool excludeInternalPractices, bool isSampleReport)
        {
            var context = new ConsultantTimelineReportContext
                              {
                                  Start = start,
                                  Granularity = granularity,
                                  Period = period,
                                  ProjectedPersons = projectedPersons,
                                  ProjectedProjects = projectedProjects,
                                  ActivePersons = activePersons,
                                  ActiveProjects = activeProjects,
                                  ExperimentalProjects = experimentalProjects,
                                  TimescaleIdList = timescaleIds,
                                  PracticeIdList = practiceIdList,
                                  ExcludeInternalPractices = excludeInternalPractices,
                                  InternalProjects = internalProjects,
                                  SortId = sortId,
                                  SortDirection = sortDirection,
                                  IsSampleReport = isSampleReport
                              };

            return ServiceCallers.Custom.Person(
                client => client.GetConsultantUtilizationWeekly(context));
        }
    }
}
