using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects.Utils
{
    public class Generic
    {
        public const char StringListSeparator = ',';
        public const char PersonNameSeparator = ';';
        private const string IdSeperator = "48429914-f383-4399-96c0-db719db82765" ;
        private const string FirstNameSeperator = "bc4ad2a9-2105-48b9-85e8-448408ba2a7a";
        public static string[] LastNameSeperator = { "8585ebd9-f14a-4729-9322-b0d834913e2e" };

        public static string[] Seperators = { IdSeperator, FirstNameSeperator };  
           
        public static int GetIntConfiguration(string key)
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings[key]);
        }

        public static string WalkStackTrace(StackTrace stackTrace)
        {
            var builder = new StringBuilder("Stack trace: \n");

            StackFrame frame;
            for (var i = 0; i < stackTrace.FrameCount; i++)
            {
                frame = stackTrace.GetFrame(i);
                builder.AppendFormat("{0} ({1}:{2})",
                                     frame.GetMethod(),
                                     frame.GetFileName(),
                                     frame.GetFileLineNumber()).
                    AppendLine();
            }

            return builder.ToString();
        }

        public static string IdsListToString<T>(IEnumerable<T> ids) where T : IIdNameObject
        {
            return EnumerableToCsv(from id in ids where id.Id.HasValue select id, id => id.Id.Value, StringListSeparator);
        }

        public static string EnumerableToCsv<T, TV>(IEnumerable<T> existingIds, Func<T, TV> func)
        {
            return EnumerableToCsv(existingIds, func, StringListSeparator);
        }

        public static string EnumerableToCsv<T, TV>(IEnumerable<T> existingIds, Func<T, TV> func, char stringListSeparator)
        {
            var sb = new StringBuilder();

            foreach (var item in existingIds)
                sb.Append(func(item)).Append(stringListSeparator);

            return sb.ToString();
        }

        public static IEnumerable<KeyValuePair<DateTime, double>> GetTotalsByDate<T>(Dictionary<T, TimeEntryRecord[]> groupedTimeEtnries) where T : IIdNameObject
        {
            var res = new SortedDictionary<DateTime, double>();

            foreach (var etnry in groupedTimeEtnries)
                foreach (var record in etnry.Value)
                {
                    var date = record.MilestoneDate;
                    var hours = record.ActualHours;

                    try
                    {
                        res[date] += hours;
                    }
                    catch (Exception)
                    {
                        res.Add(date, hours);
                    }
                }

            return res;
        }

        public static T ToEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static T ToEnum<T>(string value, T defaultValue)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static double AddTotalsByEntity<T>(Dictionary<T, TimeEntryRecord[]> groupedTimeEtnries) where T : IIdNameObject
        {
            return groupedTimeEtnries.SelectMany(etnry => etnry.Value).Sum(record => record.ActualHours);
        }

        public static double AddTotalsByEntity<T>(IEnumerable<KeyValuePair<T, TimeEntryRecord[]>> groupedTimeEtnries) where T : IIdNameObject
        {
            return groupedTimeEtnries.SelectMany(etnry => etnry.Value).Sum(record => record.ActualHours);
        }
    }
}
