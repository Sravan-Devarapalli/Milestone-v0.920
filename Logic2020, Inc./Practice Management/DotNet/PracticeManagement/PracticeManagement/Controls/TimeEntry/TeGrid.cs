using System;
using System.Collections.Generic;
using System.Diagnostics;
using DataTransferObjects;
using DataTransferObjects.TimeEntry;
using System.Linq;

namespace PraticeManagement.Controls.TimeEntry
{
    /// <summary>
    /// Object behind TimeEntries.ascx control
    /// </summary>
    [DebuggerDisplay("TeGrid; Rows = {Rows}, TimeEntries = {TimeEntries}")]
    public class TeGrid : IEnumerable<TeGridRow>
    {
        #region Properties

        private List<TeGridRow> Rows { get; set; }

        public TimeEntryRecord[] TimeEntries { get; set; }
        public MilestonePersonEntry[] MilestonePersonEntries { get; set; }
        public CalendarItem[] Calendar { get; set; }

        /// <summary>
        /// Determines if the result is empty
        /// </summary>
        public bool IsEmptyResultSet
        {
            get
            {
                return Rows.Count == 0;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Init constructor of TeGrid.
        /// </summary>
        private TeGrid(CalendarItem[] calendar)
        {
            Calendar = calendar;
            Rows = new List<TeGridRow>();
        }


        /// <summary>
        /// Init constructor of TeGrid.
        /// </summary>
        private TeGrid(TimeEntryRecord[] timeEntries, CalendarItem[] calendar)
            : this(calendar)
        {
            TimeEntries = timeEntries;
        }

        /// <summary>
        /// Init constructor of TeGrid.
        /// </summary>
        public TeGrid(TimeEntryRecord[] timeEntries, MilestonePersonEntry[] milestonePersonEntries, CalendarItem[] calendar)
            : this(timeEntries, calendar)
        {
            MilestonePersonEntries = milestonePersonEntries;

            PutTimeEntriesOnTheGrid();
        }

        private void AddEmptyRowsForEachMilestonePerson()
        {
            foreach (var mpe in MilestonePersonEntries)
                Rows.Add(new TeGridRow(Calendar, mpe));
        }

        private void AddSingleTimeEntryRow()
        {
            var mCount = MilestonePersonEntries.Length;

            if (mCount > 0)
                Rows.Add(
                    new TeGridRow(
                        Calendar,
                        MilestonePersonEntries[mCount - 1]));

        }

        private void AddSingleTimeEntryRow(MilestonePersonEntry mPEOther)
        {
            var mCount = MilestonePersonEntries.Length;

            if (mCount > 0)
            {
                var mPE = MilestonePersonEntries.First(MPE => MPE.MilestonePersonId == mPEOther.MilestonePersonId);
                Rows.Add(new TeGridRow(Calendar,mPE));
            }

        }

        private void PutTimeEntriesOnTheGrid()
        {

            if (TimeEntries.Length > 0)
            {
                bool isRowAdded = false;
                foreach (var te in TimeEntries)
                {
                    if(te.ParentMilestonePersonEntry != null 
                       && MilestonePersonEntries.Any(MPE => MPE.MilestonePersonId == te.ParentMilestonePersonEntry.MilestonePersonId))
                    {
                        AddSingleTimeEntryRow(TimeEntries[0].ParentMilestonePersonEntry);
                        isRowAdded = true;
                        break;
                    }
                }
                if(!isRowAdded)
                {
                    AddSingleTimeEntryRow();
                }
                foreach (var te in TimeEntries)
                {
                    var teAdded = false;
                    foreach (var row in Rows)
                    {
                        teAdded = row.AddTE(te);
                        if (teAdded)
                            break;
                    }

                    if (!teAdded)
                    {
                        var mpe = Array.Find(
                                            MilestonePersonEntries,
                                            fmpe => fmpe.MilestonePersonId ==
                                                    te.ParentMilestonePersonEntry.MilestonePersonId);
                        var newRow =
                            new TeGridRow(
                                Calendar,
                                mpe,
                                te.TimeType);
                        newRow.AddTE(te);
                        Rows.Add(newRow);
                    }
                }
            }
            else
            {
                AddSingleTimeEntryRow();
            }
        }

        #endregion

        #region Methods

        public void AddEmptyRow()
        {
            Rows.Add(new TeGridRow(Calendar, MilestonePersonEntries[0]));
        }

        #endregion

        #region IEnumerable<TeGridRow> Members

        public IEnumerator<TeGridRow> GetEnumerator()
        {
            return Rows.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Rows.GetEnumerator();
        }

        #endregion
    }
}

