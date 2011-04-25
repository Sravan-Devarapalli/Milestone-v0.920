using System;
using DataTransferObjects.TimeEntry;

namespace PraticeManagement.Controls.TimeEntry
{
    public class ReadyToUpdateTeArguments : EventArgs
    {
        public TimeEntryRecord TimeEntry { get; set; }

        /// <summary>
        /// Init constructor of ReadyToUpdateTeArguments.
        /// </summary>
        public ReadyToUpdateTeArguments(DataTransferObjects.TimeEntry.TimeEntryRecord timeEntry)
        {
            this.TimeEntry = timeEntry;
        }
    }
}

