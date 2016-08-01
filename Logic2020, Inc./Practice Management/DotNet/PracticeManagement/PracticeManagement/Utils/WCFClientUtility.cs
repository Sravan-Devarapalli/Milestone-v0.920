using System.Configuration;
using System.ServiceModel;
using Microsoft.WindowsAzure.ServiceRuntime;
using PraticeManagement.ActivityLogService;
using PraticeManagement.AuthService;
using PraticeManagement.CalendarService;
using PraticeManagement.ClientService;
using PraticeManagement.ConfigurationService;
using PraticeManagement.ExpenseCategoryService;
using PraticeManagement.ExpenseService;
using PraticeManagement.MembershipService;
using PraticeManagement.MilestonePersonService;
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
using PraticeManagement.RoleService;
using PraticeManagement.TimeEntryService;
using PraticeManagement.TimescaleService;
using PraticeManagement.Utils;

namespace PraticeManagement.MilestonePersonService
{
    public partial class MilestonePersonServiceClient
    {
        public MilestonePersonServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("MilestonePersonServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("ActivityLogServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("CalendarServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("ClientServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("ExpenseCategoryServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("ExpenseServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("MilestoneServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("OpportunityServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("OverheadServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("PersonRoleServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("PersonServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("PersonStatusServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("PracticeServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("ProjectGroupServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("ProjectServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("ProjectStatusServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("TimeEntryServiceClient");
            }
        }
    }
}

namespace PraticeManagement.TimeTypeService
{
    public partial class TimeTypeServiceClient
    {
        public TimeTypeServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("TimeTypeServiceClient");
            }
        }
    }
}

namespace PraticeManagement.ReportService
{
    public partial class ReportServiceClient
    {
        public ReportServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("ReportServiceClient");
            }
        }
    }
}

namespace PraticeManagement.TitleService
{
    public partial class TitleServiceClient
    {
        public TitleServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("TitleServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("TimescaleServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("ConfigurationServiceClient");
            }
        }
    }
}

namespace PraticeManagement.PersonSkillService
{
    public partial class PersonSkillServiceClient
    {
        public PersonSkillServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("PersonSkillServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("MembershipServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("RoleServiceClient");
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
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("AuthServiceClient");
            }
        }
    }
}

namespace PraticeManagement.VendorService
{
    public partial class VendorServiceClient
    {
        public VendorServiceClient()
        {
            if (WCFClientUtility.IsWebAzureRole())
            {
                Endpoint.Address = WCFClientUtility.GetEndpointAddress("VendorServiceClient");
            }
        }
    }
}

namespace PraticeManagement.Utils
{
    public class WCFClientUtility
    {
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
            string url = GetClientUrl(serviceClientName);
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
                return RoleEnvironment.IsAvailable;
            }
            catch
            {
                return false;
            }
        }

        public static string GetConfigValue(string key)
        {
            return IsWebAzureRole() ? RoleEnvironment.GetConfigurationSettingValue(key) : ConfigurationManager.AppSettings[key];
        }
    }
}
