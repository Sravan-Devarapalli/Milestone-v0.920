using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace PracticeManagement
{
    public class EmailMessageDeliveryProvider : IEMailDeliveryProvider
    {

        #region IEMailDeliveryProvider Members

        public void SendMail(List<string> to, List<string> cc, string from, string subject, string body)
        {
            MailMessage message = new MailMessage();
            DateTime now = DateTime.Now;

            foreach (string email in to)
            {
                message.To.Add(new MailAddress(email));
            }
            foreach (string email in cc)
            {
                message.CC.Add(new MailAddress(email));
            }
            message.From = new MailAddress(from);
            message.Subject = subject;
            message.Body = body;

            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            client.Send(message);
        }

        #endregion
    }
}

