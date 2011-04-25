using System;
using System.Collections.Generic;
using System.Data;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;

namespace PracticeManagementScheduler
{
    public class Utils
    {
        private const string TokenFormat = "<{0}>";

        public static EmailData GetEmailData(EmailContext emailContext)
        {
            return ServiceCallers.Configuration(c => c.GetEmailData(emailContext));
        }

        public static string ReplaceValues(IEnumerable<string> tokens, DataRow row, string source)
        {
            foreach (var token in tokens)
            {
                var rowValue = row[token].ToString();
                source = source.Replace(String.Format(TokenFormat, token), rowValue);
            }

            return source;
        }

        public static IEnumerable<Email> PrepareEmails(EmailData emailData)
        {
            if (!emailData.IsEmpty)
            {
                var headers = emailData.DataHeaders;

                foreach (DataRow row in emailData.Data.Tables[0].Rows)
                {
                    var body = ReplaceValues(headers, row, emailData.EmailTemplate.Body);
                    var subj = ReplaceValues(headers, row, emailData.EmailTemplate.Subject);

                    yield return new Email
                                     {
                                         Body = body, 
                                         Subject = subj
                                     };
                }
            }
        }
    }
}

