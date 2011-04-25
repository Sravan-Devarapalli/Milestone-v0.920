using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using PracticeManagementScheduler.Settings;

namespace PracticeManagementScheduler
{
    public partial class Mainframe : ServiceBase
    {
        private readonly TaskSection _options;
        private readonly Configuration _config;
        private readonly Timer _timer;
        private const ConfigurationUserLevel ConfigurationLevel = ConfigurationUserLevel.None;
        private const string Source = "PracticeManagementServiceLogSource";
        private const string Log = "PracticeManagementServiceLogSource";


        public Mainframe()
        {

            InitializeComponent();
            if (!EventLog.SourceExists(Source))
                EventLog.CreateEventSource(Source, Log);

            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationLevel);
            _options = _config.GetSection(TaskSection.SectionName) as TaskSection;
            if (_options == null)
            {
                _options = new TaskSection();
                if (_options.Tasks != null)
                    _options.Tasks = new TasksCollection();
            }

            _timer = new Timer(_options.TasksPeriod*1000);
            _timer.Elapsed += TimerElapsed;

            eventLog1.Source = Source;
            eventLog1.Log = Log;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            TimerTick();
        }

        private void TimerTick()
        {
            var trace = new StringBuilder();
            var err = new StringBuilder();
            var warn = new StringBuilder();

            try
            {
                if (_options.Tasks != null)
                    foreach (TaskElement task in _options.Tasks)
                        if (task.IsTimeToTrigger())
                        {
                            // TODO: call service here!

                            task.UpdateLastRun();
                        }
            }
            catch (Exception e)
            {
                PrintError(e, err, 0);
            }
            finally
            {
                trace.AppendLine("PRMA successfully completed cycle");
                eventLog1.WriteEntry(trace.ToString(), EventLogEntryType.Information);
                if (warn.Length > 0)
                    eventLog1.WriteEntry(warn.ToString(), EventLogEntryType.Warning);
                if (err.Length > 0)
                    eventLog1.WriteEntry(err.ToString(), EventLogEntryType.Error);
            }
        }

        protected override void OnStart(string[] args)
        {
            _timer.Start();
            eventLog1.WriteEntry(string.Format("PRMA scheduler started (configuration file is here: {0}).", _config.FilePath));
        }

        protected override void OnStop()
        {
            _timer.Stop();
            eventLog1.WriteEntry("PRMA scheduler stopped.");
        }


        private static void PrintError(Exception e, StringBuilder error, int errorLevel)
        {
            var indent = string.Empty;
            for (var i = 0; i < errorLevel; i++)
                indent.Insert(0, "\t\t\t");

            error.AppendFormat(
                "{0}{1} Error Occurred : {2}{3}", 
                indent, DateTime.Now.ToLongTimeString(), e.Message, Environment.NewLine);

            if (!string.IsNullOrEmpty(e.StackTrace))
            {
                error.AppendLine("StackTrace");
                error.AppendFormat("{0}{1}{2}", indent, e.StackTrace, Environment.NewLine);
            }

            error.AppendLine();

            if (e.InnerException != null)
                PrintError(e.InnerException, error, errorLevel + 1);
        }
    }
}
