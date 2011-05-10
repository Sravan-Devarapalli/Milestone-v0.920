using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using Microsoft.WindowsAzure.ServiceRuntime;
using PraticeManagement.MilestonePersonService;
using PraticeManagement.ActivityLogService;
using PraticeManagement.CalendarService;
using PraticeManagement.ClientService;
using PraticeManagement.ExpenseCategoryService;
using PraticeManagement.ExpenseService;
using PraticeManagement.MilestoneService;
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
using PraticeManagement.ConfigurationService;
using PraticeManagement.DefaultRecruiterCommissionService;
using PraticeManagement.DefaultCommissionService;
using PraticeManagement.MembershipService;
using PraticeManagement.AuthService;
using PraticeManagement.RoleService;
using PraticeManagement.Utils;
using System.Configuration;

namespace PraticeManagement.MilestonePersonService
{
    public partial class MilestonePersonServiceClient
    {
        public MilestonePersonServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("MilestonePersonServiceClient");
            }
        }
    }
}

namespace PraticeManagement.ActivityLogService
{
    public partial class ActivityLogServiceClient
    {
        public ActivityLogServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("ActivityLogServiceClient");
            }
        }
    }
}

namespace PraticeManagement.CalendarService
{
    public partial class CalendarServiceClient
    {
        public CalendarServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("CalendarServiceClient");
            }
        }
    }
}

namespace PraticeManagement.ClientService
{
    public partial class ClientServiceClient
    {
        public ClientServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("ClientServiceClient");
            }
        }
    }
}

namespace PraticeManagement.ExpenseCategoryService
{
    public partial class ExpenseCategoryServiceClient
    {
        public ExpenseCategoryServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("ExpenseCategoryServiceClient");
            }
        }
    }
}

namespace PraticeManagement.ExpenseService
{
    public partial class ExpenseServiceClient
    {
        public ExpenseServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("ExpenseServiceClient");
            }
        }
    }
}

namespace PraticeManagement.MilestoneService
{
    public partial class MilestoneServiceClient
    {
        public MilestoneServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("MilestoneServiceClient");
            }
        }
    }
}

namespace PraticeManagement.OpportunityService
{
    public partial class OpportunityServiceClient
    {
        public OpportunityServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("OpportunityServiceClient");
            }
        }
    }
}

namespace PraticeManagement.OverheadService
{
    public partial class OverheadServiceClient
    {
        public OverheadServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("OverheadServiceClient");
            }
        }
    }
}

namespace PraticeManagement.PersonRoleService
{
    public partial class PersonRoleServiceClient
    {
        public PersonRoleServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("PersonRoleServiceClient");
            }
        }
    }
}

namespace PraticeManagement.PersonService
{
    public partial class PersonServiceClient
    {
        public PersonServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("PersonServiceClient");
            }
        }
    }
}

namespace PraticeManagement.PersonStatusService
{
    public partial class PersonStatusServiceClient
    {
        public PersonStatusServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("PersonStatusServiceClient");
            }
        }
    }
}

namespace PraticeManagement.PracticeService
{
    public partial class PracticeServiceClient
    {
        public PracticeServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("PracticeServiceClient");
            }
        }
    }
}

namespace PraticeManagement.ProjectGroupService
{
    public partial class ProjectGroupServiceClient
    {
        public ProjectGroupServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("ProjectGroupServiceClient");
            }
        }
    }
}

namespace PraticeManagement.ProjectService
{
    public partial class ProjectServiceClient
    {
        public ProjectServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("ProjectServiceClient");
            }
        }
    }
}

namespace PraticeManagement.ProjectStatusService
{
    public partial class ProjectStatusServiceClient
    {
        public ProjectStatusServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("ProjectStatusServiceClient");
            }
        }
    }
}

namespace PraticeManagement.TimeEntryService
{
    public partial class TimeEntryServiceClient
    {
        public TimeEntryServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("TimeEntryServiceClient");
            }
        }
    }
}

namespace PraticeManagement.TimescaleService
{
    public partial class TimescaleServiceClient
    {
        public TimescaleServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("TimescaleServiceClient");
            }
        }
    }
}

namespace PraticeManagement.ConfigurationService
{
    public partial class ConfigurationServiceClient
    {
        public ConfigurationServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("ConfigurationServiceClient");
            }
        }
    }
}

namespace PraticeManagement.DefaultRecruiterCommissionService
{
    public partial class DefaultRecruiterCommissionServiceClient
    {
        public DefaultRecruiterCommissionServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("DefaultRecruiterCommissionServiceClient");
            }
        }
    }
}

namespace PraticeManagement.DefaultCommissionService
{
    public partial class DefaultCommissionServiceClient
    {
        public DefaultCommissionServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("DefaultCommissionServiceClient");
            }
        }
    }
}

namespace PraticeManagement.MembershipService
{
    public partial class MembershipServiceClient
    {
        public MembershipServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("MembershipServiceClient");
            }
        }
    }
}

namespace PraticeManagement.RoleService
{
    public partial class RoleServiceClient
    {
        public RoleServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("RoleServiceClient");
            }
        }
    }
}

namespace PraticeManagement.AuthService
{
    public partial class AuthServiceClient
    {
        public AuthServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                this.Endpoint.Address = WCFClientUtility.GetEndpointAddress("AuthServiceClient");
            }
        }
    }
}

namespace PraticeManagement.Utils
{
    public class WCFClientUtility
    {

        private static MilestonePersonServiceClient GetMilestonePersonServiceClient()
        {
            var client = new MilestonePersonServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("MilestonePersonServiceClient");
            }

            return client;
        }

        private static CalendarServiceClient GetCalendarServiceClient()
        {
            var client = new CalendarServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("CalendarServiceClient");
            }

            return client;
        }

        private static ClientServiceClient GetClientServiceClient()
        {
            var client = new ClientServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("ClientServiceClient");
            }

            return client;
        }

        private static MilestoneServiceClient GetMilestoneServiceClient()
        {
            var client = new MilestoneServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("MilestoneServiceClient");
            }

            return client;
        }

        private static ProjectServiceClient GetProjectServiceClient()
        {
            var client = new ProjectServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("ProjectServiceClient");
            }

            return client;
        }

        private static PersonServiceClient GetPersonServiceClient()
        {
            var client = new PersonServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("PersonServiceClient");
            }

            return client;
        }

        private static TimescaleServiceClient GetTimescaleServiceClient()
        {
            var client = new TimescaleServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("TimescaleServiceClient");
            }

            return client;
        }

        private static TimeEntryServiceClient GetTimeEntryServiceClient()
        {
            var client = new TimeEntryServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("TimeEntryServiceClient");
            }

            return client;
        }

        private static ProjectGroupServiceClient GetProjectGroupServiceClient()
        {
            var client = new ProjectGroupServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("ProjectGroupServiceClient");
            }

            return client;
        }

        private static ExpenseServiceClient GetExpenseServiceClient()
        {
            var client = new ExpenseServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("ExpenseServiceClient");
            }

            return client;
        }

        private static ConfigurationServiceClient GetConfigurationServiceClient()
        {
            var client = new ConfigurationServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("ConfigurationServiceClient");
            }

            return client;
        }

        private static PracticeServiceClient GetPracticeServiceClient()
        {
            var client = new PracticeServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("PracticeServiceClient");
            }

            return client;
        }

        private static ActivityLogServiceClient GetActivityLogServiceClient()
        {
            var client = new ActivityLogServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("ActivityLogServiceClient");
            }

            return client;
        }

        private static OverheadServiceClient GetOverheadServiceClient()
        {
            var client = new OverheadServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("OverheadServiceClient");
            }

            return client;
        }

        private static ProjectStatusServiceClient GetProjectStatusServiceClient()
        {
            var client = new ProjectStatusServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("ProjectStatusServiceClient");
            }

            return client;
        }

        private static PersonRoleServiceClient GetPersonRoleServiceClient()
        {
            var client = new PersonRoleServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("PersonRoleServiceClient");
            }

            return client;
        }

        private static PersonStatusServiceClient GetPersonStatusServiceClient()
        {
            var client = new PersonStatusServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("PersonStatusServiceClient");
            }

            return client;
        }

        private static ExpenseCategoryServiceClient GetExpenseCategoryServiceClient()
        {
            var client = new ExpenseCategoryServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("ExpenseCategoryServiceClient");
            }

            return client;
        }

        private static OpportunityServiceClient GetOpportunityServiceClient()
        {
            var client = new OpportunityServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("OpportunityServiceClient");
            }

            return client;
        }

        private static DefaultRecruiterCommissionServiceClient GetDefaultRecruiterCommissionServiceClient()
        {
            var client = new DefaultRecruiterCommissionServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("DefaultRecruiterCommissionServiceClient");
            }

            return client;
        }

        private static DefaultCommissionServiceClient GetDefaultCommissionServiceClient()
        {
            var client = new DefaultCommissionServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("DefaultCommissionServiceClient");
            }

            return client;
        }

        private static MembershipServiceClient GetMembershipServiceClient()
        {
            var client = new MembershipServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("MembershipServiceClient");
            }

            return client;
        }

        private static AuthServiceClient GetAuthServiceClient()
        {
            var client = new AuthServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("AuthServiceClient");
            }

            return client;
        }

        private static RoleServiceClient GetRoleServiceClient()
        {
            var client = new RoleServiceClient();
            if (WCFClientUtility.IsWebAzureRole())
            {
                client.Endpoint.Address = GetEndpointAddress("RoleServiceClient");
            }

            return client;
        }
        public static AttachmentService.AttachmentService GetAttachmentService()
        {
            var service = new AttachmentService.AttachmentService();
            if (IsWebAzureRole())
            {
                service.Url = RoleEnvironment.GetConfigurationSettingValue("AttachmentServiceURL");
            }
            return service;
        }

        public static EndpointAddress GetEndpointAddress(string serviceClientName)
        {
            string url = "";
            switch (serviceClientName)
            {
                case "MilestonePersonServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "CalendarServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "ClientServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "MilestoneServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "ProjectServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "PersonServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "TimescaleServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "TimeEntryServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "ProjectGroupServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "ExpenseServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "ConfigurationServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "PracticeServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "ActivityLogServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "OverheadServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "ProjectStatusServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "PersonRoleServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "PersonStatusServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "ExpenseCategoryServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "OpportunityServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "DefaultRecruiterCommissionServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "DefaultCommissionServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "MembershipServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "AuthServiceClient": url = GetClientUrl(serviceClientName);
                    break;
                case "RoleServiceClient": url = GetClientUrl(serviceClientName);
                    break;
            }

            return new EndpointAddress(url);
        }

        private static string GetClientUrl(string serviceClientName)
        {
            return RoleEnvironment.GetConfigurationSettingValue(serviceClientName);
        }

        public static bool IsWebAzureRole()
        {
            try
            {
                if (RoleEnvironment.IsAvailable)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static string GetConfigValue(string key)
        {
            if (IsWebAzureRole())
            {
                return RoleEnvironment.GetConfigurationSettingValue(key);
            }
            else
            {
                return ConfigurationManager.AppSettings[key];
            }
        }
    }
}
