using System;
using System.Collections.Generic;
using System.Diagnostics;
using DataTransferObjects;
using DataTransferObjects.TimeEntry;

namespace PraticeManagement.Controls.TimeEntry
{
    /// <summary>
    /// Object behind TimeEntryBar.ascx control
    /// </summary>
    [DebuggerDisplay("TeGridRow; Cells = {Cells}, MilestoneBehind = {MilestoneBehind}")]
    public class TeGridRow : IEnumerable<TeGridCell>
    {
        #region Init constructors

        /// <summary>
        /// Init constructor of TeGridRow.
        /// </summary>
        public TeGridRow(CalendarItem[] calendar, MilestonePersonEntry milestoneBehind, TimeTypeRecord timeTypeBehind)
            : this(calendar, milestoneBehind)
        {
            TimeTypeBehind = timeTypeBehind;
        }

        /// <summary>
        /// Init constructor of TeGridRow.
        /// </summary>
        public TeGridRow(CalendarItem[] calendar, MilestonePersonEntry milestoneBehind) 
        {
            MilestoneBehind = milestoneBehind;
            Calendar = calendar;

            InitCells();
        }

        private void InitCells()
        {
            Cells = new List<TeGridCell>(Calendar.Length);
            foreach (var day in Calendar)
                Cells.Add(
                    new TeGridCell
                        {
                            Day = day,
                            MilestoneBehind = MilestoneBehind
                        });
        }

        #endregion

        #region Properties

        public List<TeGridCell> Cells { get; set; }

        public CalendarItem[] Calendar { get; set; }
        public MilestonePersonEntry MilestoneBehind { get; set; }
        public TimeTypeRecord TimeTypeBehind { get; set; }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Adds TE to the grid
        /// </summary>
        /// <param name="te">TE to add</param>
        /// <returns>True if added, false otherwise</returns>
        public bool AddTE(TimeEntryRecord te)
        {
            if (te.ParentMilestonePersonEntry.Equals(MilestoneBehind))
                return AddToTheRow(te);

            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Puts TE to the corresponding cell
        /// </summary>
        /// <param name="te">TE to add</param>
        /// <returns>True if added, false otherwise</returns>
        private bool AddToTheRow(TimeEntryRecord te)
        {
            //  If there's no time type selected for this row
            if (TimeTypeBehind == null)
            {
                //  Select the one that given TE represents
                TimeTypeBehind = te.TimeType;
            }

            //  If given TE has the same time type, put it to the cell
            if (te.TimeType.Equals(TimeTypeBehind))
            {
                int dateIndex = Array.IndexOf(Calendar, te.MilestoneDate);
                if (dateIndex >= 0)
                {
                    // Check if there's already a TE in that cell
                    var cell = Cells[dateIndex];

                    if (cell.TimeEntry == null)
                        cell.TimeEntry = te; // If not, put it there
                    else
                        return false; //    If there's one, return true in order to add a new row
                }

                return dateIndex >= 0;                
            }

            return false;
        }

        #endregion

        #region IEnumerable<TeGridCell> Members

        public IEnumerator<TeGridCell> GetEnumerator()
        {
            return Cells.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Cells.GetEnumerator();
        }

        #endregion
    }
}

