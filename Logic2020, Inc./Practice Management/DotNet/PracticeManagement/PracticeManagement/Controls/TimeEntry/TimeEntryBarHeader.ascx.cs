using System;
using System.Collections.Generic;

namespace PraticeManagement.Controls.TimeEntry
{
    public partial class TimeEntryBarHeader : System.Web.UI.UserControl
    {
        public IEnumerable<DateTime> SelectedDates { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void UpdateHeader()
        {
            repEntries.DataSource = SelectedDates;
            repEntries.DataBind();
        }
    }
}
