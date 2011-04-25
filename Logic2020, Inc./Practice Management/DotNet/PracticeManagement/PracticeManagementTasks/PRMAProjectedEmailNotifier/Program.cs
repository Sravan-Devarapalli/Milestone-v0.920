using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PRMAProjectedEmailNotifier.ConfigurationService;

namespace PRMAProjectedEmailNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ConfigurationServiceClient service = new ConfigurationServiceClient();
                service.CheckProjectedProjects(PRMAProjectedEmailNotifier.Properties.Settings.Default.EmailTemplateID, PRMAProjectedEmailNotifier.Properties.Settings.Default.CompanyName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

