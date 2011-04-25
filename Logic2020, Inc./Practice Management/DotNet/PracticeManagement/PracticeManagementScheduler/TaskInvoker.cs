using DataTransferObjects.ContextObjects;
using PracticeManagementScheduler.Settings;

namespace PracticeManagementScheduler
{
    public class TaskInvoker
    {
        public static void Invoke(TaskElement task)
        {
            var emailContext = new EmailContext
                                   {
                                       EmailTemplateId = task.TemplateId,
                                       StorerProcedureName = task.StoredProcName
                                   };

            var emailData = Utils.GetEmailData(emailContext);

            var emails = Utils.PrepareEmails(emailData);
        }
    }
}

