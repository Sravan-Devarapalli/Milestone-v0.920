﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataTransferObjects.Reports;

namespace PracticeManagementService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IReportService" in both code and config file together.
    [ServiceContract]
    public interface IReportService
    {

        List<TimeEntriesGroupByClientAndProject> GetPersonTimeEntriesDetails(int personId, DateTime startDate, DateTime endDate);
        
    }
}

