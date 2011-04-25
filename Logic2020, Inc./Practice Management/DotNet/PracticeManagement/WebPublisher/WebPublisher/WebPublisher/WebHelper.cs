using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WebPublisher
{
    public class WebHelper
    {
        public string PublishWebSite(string siteUrl, string virtualPath, string sourcesPath, string outputPath)
        {
            string tempFileName = Path.GetTempFileName();
            StringBuilder output = new StringBuilder();
            StringBuilder buildTemplate = new StringBuilder();
            string appPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            using (TextReader streamReader = new StreamReader(Path.Combine(appPath, @"Templates\Build.xml")))
            {
                buildTemplate.AppendLine(streamReader.ReadToEnd());
            }
            var tempFolder = Path.Combine(Path.GetTempPath(), Path.GetFileName(sourcesPath));
            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            string cur = System.IO.Directory.GetCurrentDirectory();

            buildTemplate = buildTemplate.Replace("%Build_Path%", tempFolder);
            buildTemplate = buildTemplate.Replace("%Sources_Path%", sourcesPath);
            buildTemplate = buildTemplate.Replace("%Ouput_Path%", outputPath);
            buildTemplate = buildTemplate.Replace("%Sdc_Path%", Path.Combine(appPath, "SDC"));
            buildTemplate = buildTemplate.Replace("%Virtual_Folder_Name%", virtualPath);

            

            var tmpFile = Path.GetTempFileName();
            using (TextWriter streamWriter = new StreamWriter(tmpFile))
            {
                streamWriter.Write(buildTemplate.ToString());
            }

            string msbuildPath = Settings.SettingsManager.Instance.WebSites.MSBuildPath;
            if (!Directory.Exists(Path.GetDirectoryName(msbuildPath)) || !File.Exists(msbuildPath))
            {
                throw new FileNotFoundException(string.Format("msbuild.exe not found. Please (Re-)install framework 3.5"));
            }
            var proc = new System.Diagnostics.Process();

            System.IO.File.Copy(tmpFile, @"d:\builds.xml", true);

            proc.StartInfo.FileName = msbuildPath;
            proc.StartInfo.CreateNoWindow = false;
            proc.StartInfo.Arguments = tmpFile;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.StartInfo.UseShellExecute = false;
            output.AppendFormat("{0} {1}", proc.StartInfo.FileName, proc.StartInfo.Arguments);
            output.AppendLine();
            proc.Start();
            proc.WaitForExit();
            int i = proc.ExitCode;
            //output.AppendLine(proc.StandardOutput.ReadToEnd());

            System.IO.File.Delete(tmpFile);
            Directory.Delete(tempFolder, true);
            return output.ToString();

        }
    }
}

