using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HireDateExpiredEmailNotifier.ConfigurationService;

namespace HireDateExpiredEmailNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ConfigurationServiceClient service = new ConfigurationServiceClient();
                service.CheckProjectedProjectsByHireDate(Properties.Settings.Default.EmailTemplateID, Properties.Settings.Default.CompanyName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

