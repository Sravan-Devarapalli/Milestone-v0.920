using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DataTransferObjects.TimeEntry;
using DataTransferObjects.Reports;
using System.ComponentModel;

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
                    var date = record.ChargeCodeDate;
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

        public static int GetBillablePercentage(double billableHours, double nonBillableHours)
        {
            return (int)(100 * billableHours / (billableHours + nonBillableHours));
        }

        /// <summary>
        /// Returns the ProportionateRatio List of the given Height for the given ration List
        /// </summary>
        /// <param name="ratioList"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static List<int> GetProportionateRatio(List<int> ratioList, int height)
        {
            List<int> proportionateList = new List<int>();
            int sum = ratioList.Sum();
            if (sum > 0)
            {
                foreach (int no in ratioList)
                {
                    decimal noRatio = ((decimal)no / (decimal)sum) * (decimal)height;
                    int i = (int)Math.Round(noRatio);
                    proportionateList.Add(i);
                }
                return proportionateList;
            }
            else
            {
                return ratioList;
            }
            
        }

        public static List<GroupByDate> GetGroupByDateList(List<TimeEntriesGroupByClientAndProject> timeEntriesGroupByClientAndProjectList)
        {
            List<GroupByDate> groupByDateList = new List<GroupByDate>();
            if (timeEntriesGroupByClientAndProjectList.Count > 0)
            {
                foreach (var timeEntriesGroupByClientAndProject in timeEntriesGroupByClientAndProjectList)
                {

                    foreach (var byDateList in timeEntriesGroupByClientAndProject.DayTotalHours)
                    {

                        foreach (var byWorkType in byDateList.DayTotalHoursList)
                        {
                            GroupByDate groupByDate;
                            if (!groupByDateList.Any(g => g.Date == byDateList.Date))
                            {
                                groupByDate = new GroupByDate();
                                groupByDate.Date = byDateList.Date;
                                groupByDateList.Add(groupByDate);
                            }
                            else
                            {
                                groupByDate = groupByDateList.First(g => g.Date == byDateList.Date);
                            }

                            GroupByClientAndProject groupByClientAndProject;
                            if (groupByDate.ProjectTotalHours == null)
                            {
                                groupByDate.ProjectTotalHours = new List<GroupByClientAndProject>();
                            }
                            if (!groupByDate.ProjectTotalHours.Any(g => g.Project.ProjectNumber == timeEntriesGroupByClientAndProject.Project.ProjectNumber && g.Client.Code == timeEntriesGroupByClientAndProject.Client.Code))
                            {
                                groupByClientAndProject = new GroupByClientAndProject();
                                groupByClientAndProject.Client = timeEntriesGroupByClientAndProject.Client;
                                groupByClientAndProject.Project = timeEntriesGroupByClientAndProject.Project;
                                groupByDate.ProjectTotalHours.Add(groupByClientAndProject);
                            }
                            else
                            {
                                groupByClientAndProject = groupByDate.ProjectTotalHours.First(g => g.Project.ProjectNumber == timeEntriesGroupByClientAndProject.Project.ProjectNumber && g.Client.Code == timeEntriesGroupByClientAndProject.Client.Code);
                            }

                            if (groupByClientAndProject.ProjectTotalHoursList == null)
                            {
                                groupByClientAndProject.ProjectTotalHoursList = new List<TimeEntryByWorkType>();
                            }
                            groupByClientAndProject.ProjectTotalHoursList.Add(byWorkType);
                        }
                    }
                }
            }
            return groupByDateList;
        }

        public static List<GroupByDateByPerson> GetGroupByDateList(List<PersonLevelGroupedHours> personLevelGroupedHoursList)
        {
            List<GroupByDateByPerson> groupByDateByPersonList = new List<GroupByDateByPerson>();

            foreach (PersonLevelGroupedHours PLGH in personLevelGroupedHoursList)
            {
                if (PLGH.DayTotalHours != null)
                {
                    foreach (TimeEntriesGroupByDate TEGD in PLGH.DayTotalHours)
                    {
                        GroupByDateByPerson GDP;
                        GroupByPersonByWorktype GPW;
                        if (groupByDateByPersonList.Any(p => p.Date == TEGD.Date))
                        {
                            GDP = groupByDateByPersonList.First(p => p.Date == TEGD.Date);
                        }
                        else
                        {
                            GDP = new GroupByDateByPerson();
                            GDP.Date = TEGD.Date;
                            GDP.ProjectTotalHours = new List<GroupByPersonByWorktype>();
                            GDP.TimeEntrySectionId = PLGH.TimeEntrySectionId;
                            groupByDateByPersonList.Add(GDP);
                        }

                        if (GDP.ProjectTotalHours.Any(p => p.Person.Id == PLGH.Person.Id))
                        {
                            GPW = GDP.ProjectTotalHours.First(p => p.Person.Id == PLGH.Person.Id);
                        }
                        else
                        {
                            GPW = new GroupByPersonByWorktype();
                            GPW.Person = PLGH.Person;
                            GPW.ProjectTotalHoursList = new List<TimeEntryByWorkType>();
                            GDP.ProjectTotalHours.Add(GPW);
                        }
                        GPW.ProjectTotalHoursList.AddRange(TEGD.DayTotalHoursList);
                    }
                }
            }
            return groupByDateByPersonList;
        }

        public static string GetDescription(Enum enumerator)
        {
            Type type = enumerator.GetType();

            var memberInfo = type.GetMember(enumerator.ToString());

            if (memberInfo != null && memberInfo.Length > 0)
            {
                //we default to the first member info, as it's for the specific enum value
                object[] attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                //return the description if it's found
                if (attributes != null && attributes.Length > 0)
                    return ((DescriptionAttribute)attributes[0]).Description;
            }

            //if there's no description, return the string value of the enum
            return enumerator.ToString();
        }

        public static DateTime MonthStartDate(DateTime now)
        {
            return now.AddDays(1 - now.Day);
        }

        public static DateTime MonthEndDate(DateTime now)
        {
            return now.AddMonths(1).AddDays(-now.AddMonths(1).Day);
        }
    }
}

