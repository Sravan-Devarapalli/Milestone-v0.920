using System;
using PracticeManagementDatabaseUI;

namespace PracticeManagementSchedulerTest
{
    internal class Program
    {
        private static void TestScheduler()
        {
            var emailData =
                            PracticeManagementScheduler.Utils.GetEmailData(new DataTransferObjects.ContextObjects.EmailContext
                                                   {
                                                       EmailTemplateId = 1,
                                                       StorerProcedureName = "EmailNotificationsListExpiredPersons"
                                                   });
            var emails = PracticeManagementScheduler.Utils.PrepareEmails(emailData);

            var i = 0;
            foreach (var email in emails)
                Console.WriteLine("{0}. {1}\n{2}\n\n", ++i, email.Subject, email.Body);

            Console.In.ReadLine();
        }

        private static void Main(string[] args)
        {
            // TestScheduler();

            var conn = new ConnectionProperties 
                        { 
                            DatabaseName = "pm_jun_30",
                            ServerName = "(local)"
                        };

            Console.WriteLine(conn.ConnectionString);
            Console.WriteLine(Utils.IsConnectionValid(conn));

            Utils.RunDeploymentProcess(conn.ServerName, conn.DatabaseName, "D:\\PRMA\\Trash\\Deploy");

            //Console.In.ReadLine();
        }
    }
}
