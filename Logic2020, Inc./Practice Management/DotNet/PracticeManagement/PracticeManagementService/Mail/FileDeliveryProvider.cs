using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace PracticeManagement
{
    public class FileDeliveryProvider: IEMailDeliveryProvider
    {
        #region IEMailDeliveryProvider Members

        public void SendMail(List<string> to, List<string> cc, string from, string subject, string body)
        {
            string mailsOutput = System.Configuration.ConfigurationManager.AppSettings["EmailsFilePath"];
            string outFolder = Path.Combine(mailsOutput, DateTime.Now.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(outFolder))
            { 
                Directory.CreateDirectory(outFolder);

            }
            StringBuilder builder = GetEmail(to, cc, from, subject, body);
            foreach (string t in to)
            {
                using (TextWriter tw = new StreamWriter(Path.Combine(outFolder, string.Format("{0}-{1}.mail", DateTime.Now.ToString("HHmm-ssfff"), t))))
                {
                    tw.WriteLine(builder.ToString());
                }
                System.Threading.Thread.Sleep(1); // files are created too fast, some emails can be dublicated
            }
        }

        #endregion
        private StringBuilder GetEmail(List<string> to, List<string> cc, string from, string subject, string body)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("FROM : {0}{1}", from, Environment.NewLine);
            builder.AppendLine("---------------------------------------------------------------");
            builder.Append("TO : ");
            FillAddresses(to, builder);
            builder.AppendLine("---------------------------------------------------------------");
            if (cc != null && cc.Count > 0)
            {
                builder.Append("CC : ");
                FillAddresses(cc, builder);
                builder.AppendLine("---------------------------------------------------------------");
            }
            builder.AppendFormat("SUBJECT : {0}{1}", subject, Environment.NewLine);
            builder.AppendLine("---------------------------------------------------------------");
            builder.AppendFormat("BODY : {1}{1}{0}{1}", body, Environment.NewLine);
            return builder;
        }

        private void FillAddresses(List<string> addresses, StringBuilder builder)
        {
            for (int i = 0; i < addresses.Count; i++)
            {
                builder.Append(addresses[i]);
                if (i < addresses.Count - 1)
                {
                    builder.Append(", ");
                }
                else
                {
                    builder.Append(Environment.NewLine);
                }
            }
        }
    }
}

