using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WebPublisher
{
    public class DataBaseHelper
    {
        public delegate void LogMessage(string message);
        public event LogMessage OnLogMessage;

        public string ServerName {get; set;}
        public string InstanceName {get; set;}
        public string DBName {get; set;}
        public string UserName {get; set;}
        public string Password {get; set;}

        public string BuildDataBase(List<string> scripts, bool updateExisting)
        {
            StringBuilder output = new StringBuilder();

            PrintHeader(string.Format("Creating database {0} :", DBName), output);
            CreateDataBase(output);
            PrintFooter(output);
            TraceMessage(output.ToString());

            PrintHeader(string.Format("Creating ASP NET Objects {0} :", DBName), output);
            CreateAspNETObjects(output);
            PrintFooter(output);
            TraceMessage(output.ToString());
            
            foreach (var script in scripts)
            {
                output = new StringBuilder();
                PrintHeader(string.Format("Building {0}", Path.GetFileName(script)), output);
                BuildSript(script, output, DBName);
                PrintFooter(output);
                TraceMessage(output.ToString());
            }

            return output.ToString();
            //todo scripts building
        }

        private void TraceMessage(string message)
        {
            if (OnLogMessage != null)
            {
                OnLogMessage(message);
            }
        }
        private void PrintHeader(string message, StringBuilder output)
        {
            if (!string.IsNullOrEmpty(message))
                output.AppendLine(message);

            output.AppendFormat("*** Begin: {0}*******************************{1}", DateTime.Now.ToLongTimeString(), Environment.NewLine);
            output.AppendLine();
        }

        private void PrintFooter(StringBuilder output)
        {
            PrintFooter(null, output);
        }
        private void PrintFooter(string message, StringBuilder output)
        {
            if (!string.IsNullOrEmpty(message))
                output.AppendLine(message);

            output.AppendLine();
            output.AppendFormat("*** End: {0}*******************************{1}", DateTime.Now.ToLongTimeString(), Environment.NewLine);
            output.AppendLine();
            output.AppendLine();
        }

        private void CreateDataBase(StringBuilder output)
        {
            string createDbSample =string.Empty;

            string appDir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            using (TextReader streamReader = new StreamReader(Path.Combine(appDir, @"Templates\CreateDatabase.tmpl")))
            {
                createDbSample= streamReader.ReadToEnd();
            }

            createDbSample = createDbSample.Replace("%Database_Name%", DBName);

            var tmpFile = Path.GetTempFileName();
            using (TextWriter streamWriter = new StreamWriter(tmpFile))
            {
                streamWriter.Write(createDbSample);
            }

            BuildSript(tmpFile, null, null);
            
            
            System.IO.File.Delete(tmpFile);
            output.AppendFormat("Database {0} created...", DBName);
        }

        private void CreateAspNETObjects(StringBuilder output)
        {
            string createDbSample = string.Empty;

            string appDir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            using (TextReader streamReader = new StreamReader(Path.Combine(appDir, @"Templates\aspnet.sql")))
            {
                createDbSample = streamReader.ReadToEnd();
            }

            createDbSample = createDbSample.Replace("%Database_Name%", DBName);

            var tmpFile = Path.GetTempFileName();
            using (TextWriter streamWriter = new StreamWriter(tmpFile))
            {
                streamWriter.Write(createDbSample);
            }

            BuildSript(tmpFile, null, null);


            System.IO.File.Delete(tmpFile);
            output.AppendFormat("Database {0} created...", DBName);
        }


        private void BuildSript(string path, StringBuilder output, string dbName)
        {
            var proc = new System.Diagnostics.Process();

            proc.StartInfo.FileName = "sqlcmd";
            proc.StartInfo.CreateNoWindow = !string.IsNullOrEmpty(dbName);
            proc.StartInfo.Arguments = string.Format("-S {0}",ServerName);
            if (!string.IsNullOrEmpty(InstanceName))
            {
                proc.StartInfo.Arguments += string.Format(" \\{0}", InstanceName);
            }
            proc.StartInfo.Arguments += string.Format(" -U {0} -P {1}", UserName,Password);

            if (!string.IsNullOrEmpty(dbName))
            {
                proc.StartInfo.Arguments += string.Format(" -d {0}", dbName);
            }
            
            
            proc.StartInfo.Arguments += string.Format(" -i {0}", path);

            proc.StartInfo.RedirectStandardOutput = !string.IsNullOrEmpty(dbName);
            proc.StartInfo.UseShellExecute = false;
            if (output != null)
            {
                output.AppendFormat("{0} {1}", proc.StartInfo.FileName, proc.StartInfo.Arguments);
                output.AppendLine();
            }
            proc.Start();

            if (output != null)
            {
                output.AppendLine(proc.StandardOutput.ReadToEnd());
            }

            proc.WaitForExit();
            int i = proc.ExitCode;

            
            proc.Close();
        }
    }
}

