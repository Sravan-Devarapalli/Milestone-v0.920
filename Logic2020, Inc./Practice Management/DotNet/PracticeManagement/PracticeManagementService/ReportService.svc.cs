using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using DataTransferObjects.Reports;
using DataAccess;

namespace PracticeManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ReportService : IReportService
    {
        public List<TimeEntriesGroupByClientAndProject> GetPersonTimeEntriesDetails(int personId, DateTime startDate, DateTime endDate)
        {
            return ReportDAL.GetPersonTimeEntriesDetails(personId,  startDate, endDate);
        }
    }
}

