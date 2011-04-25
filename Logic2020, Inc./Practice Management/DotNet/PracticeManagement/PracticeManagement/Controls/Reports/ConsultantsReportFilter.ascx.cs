using System;

namespace PraticeManagement.Controls.Reports
{
    public partial class ConsultantsReportFilter : System.Web.UI.UserControl
    {
        public DateTime MonthBegin { get { return mpControl.MonthBegin; } }
        public DateTime MonthEnd { get { return mpControl.MonthEnd; } }

        public bool ActivePersons { get { return chbActivePersons.Checked; } }
        public bool ProjectedPersons { get { return chbProjectedPersons.Checked; } }

        public bool ActiveProjects { get { return chbActiveProjects.Checked; } }
        public bool ProjectedProjects { get { return chbProjectedProjects.Checked; } }
        public bool ExperimentalProjects { get { return chbExperimentalProjects.Checked; } }
        public bool InternalProjects { get { return chbInternalProjects.Checked; } }

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        public void ResetBasicFilter()
        {
            this.chbActivePersons.Checked = true;
            this.chbActiveProjects.Checked = true;
            this.chbProjectedPersons.Checked = false;
            this.chbProjectedProjects.Checked = true;
            this.chbExperimentalProjects.Checked = false;
            this.mpControl.SelectedMonth = DateTime.Now.Month;
            this.mpControl.SelectedYear = DateTime.Now.Year;
        }
    }
}
