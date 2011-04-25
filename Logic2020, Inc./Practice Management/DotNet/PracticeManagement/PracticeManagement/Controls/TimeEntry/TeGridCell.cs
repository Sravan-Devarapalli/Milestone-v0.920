using System;
using System.Diagnostics;
using DataTransferObjects;
using DataTransferObjects.TimeEntry;

namespace PraticeManagement.Controls.TimeEntry
{
    /// <summary>
    /// 	Object behind SingleTimeEntry.ascx control
    /// </summary>
    [DebuggerDisplay("TeGridCell; Date = {Date}, TimeEntry = {TimeEntry}")]
    public class TeGridCell
    {
        #region Properties

        public CalendarItem Day { get; set; }
        public TimeEntryRecord TimeEntry { get; set; }
        public MilestonePersonEntry MilestoneBehind { get; set; }

        #endregion
    }
}
