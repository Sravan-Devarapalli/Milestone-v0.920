using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PraticeManagement.Utils
{
    public static class OpportunitiesHelper
    {
        private static readonly IDictionary<string, string> _opportunityStatuses
           = new Dictionary<string, string>()
                {
                    {"Active", "AciveOpportunity"},
                    {"Won", "WonOpportunity"},
                    {"Lost", "LostOpportunity"},
                    {"Experimental", "ExperimentalOpportunity"},
                    {"Inactive", "InactiveOpportunity"}
                };

        public static string GetIndicatorClassByStatus(string statusName)
        {
            return _opportunityStatuses[statusName];
        }
    }
}
