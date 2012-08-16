using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using System.Web.UI.DataVisualization.Charting;
using PraticeManagement.Utils;
using System.Drawing;

namespace PraticeManagement.Controls.Reports.HumanCapital
{
    public partial class NewHireGraphView : System.Web.UI.UserControl
    {
        #region constant

        private const string MAIN_CHART_AREA_NAME = "MainArea";

        #endregion

        #region properties

        private PraticeManagement.Reporting.NewHireReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.NewHireReport)Page); }
        }

        private Dictionary<string, int> Seniorities { get; set; }

        private Dictionary<string, int> Recruiters { get; set; }

        private List<Seniority> SeniorityList { get; set; }

        private List<Person> RecuriterList { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void chrtNewHireReport_Click(object sender, ImageMapEventArgs e)
        {
            string[] postBackDetails = e.PostBackValue.Split(',');
            
            lbTotalHires.Text = postBackDetails[1];
            var data = ServiceCallers.Custom.Report(r => r.NewHireReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.PersonStatus, HostingPage.PersonStatus, HostingPage.Practices, HostingPage.ExcludeInternalProjects, null, null, null, null)).ToList();
            bool isSeniority;
            Boolean.TryParse(postBackDetails[2], out isSeniority);
            if (isSeniority)
            {
                lbName.Text = "Seniority : " + postBackDetails[0];
                data = data.Where(p => p.Seniority.Name == postBackDetails[0]).ToList();
            }
            else
            {
                lbName.Text = "Recruiter : " + postBackDetails[0];
                data = data.Where(p => p.RecruiterCommission.Any() ? p.RecruiterCommission.First().Recruiter.PersonFirstLastName == postBackDetails[0] : false).ToList();
            }
            tpSummary.BtnExportToExcelButton.Attributes["IsSeniority"] = isSeniority.ToString();
            tpSummary.BtnExportToExcelButton.Attributes["FilterValue"] = postBackDetails[0];
            tpSummary.PopUpFilteredPerson = data;
            tpSummary.PopulateData(true);
            mpeDetailView.Show();
        }


        #endregion

        #region Methods

        public void PopulateGraph()
        {
            List<Person>  data = ServiceCallers.Custom.Report(r => r.NewHireReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.PersonStatus, HostingPage.PersonStatus, HostingPage.Practices, HostingPage.ExcludeInternalProjects, null, null, null, null)).ToList();
            HostingPage.PopulateHeaderSection(data);
            SeniorityList = data.Select(p => p.Seniority).Distinct().ToList();
            RecuriterList = data.Select(p => p.RecruiterCommission.Any() ? p.RecruiterCommission.First().Recruiter : null).Distinct().ToList();
            if (Seniorities == null)
                Seniorities = new Dictionary<string, int>();

            foreach (var S in SeniorityList)
            {
                int count = data.Count(p => p.Seniority.Id == S.Id);
                Seniorities.Add(S.Name, count);
            }
            if (Recruiters == null)
                Recruiters = new Dictionary<string, int>();
            foreach (var R in RecuriterList)
            {
                int count = data.Count(p => p.RecruiterCommission.Any() ? p.RecruiterCommission.First().Recruiter.Id.Value == R.Id.Value : false);
                Recruiters.Add(R.PersonFirstLastName, count);
            }
            LoadChartData(data);
        }

        private void LoadChartData(List<Person> data)
        {
            var seniorityList = Seniorities.Select(p => new { name = p.Key, count = p.Value }).ToList();
            var recruiterList = Recruiters.Select(p => new { name = p.Key, count = p.Value }).ToList();
            seniorityList = seniorityList.OrderBy(p => p.name).ToList();
            recruiterList = recruiterList.OrderBy(p => p.name).ToList();
            InitChart();
            chrtNewHireReportBySeniority.DataSource = seniorityList;
            chrtNewHireReportBySeniority.DataBind();

            chrtNewHireReportByRecruiter.DataSource = recruiterList;
            chrtNewHireReportByRecruiter.DataBind();
        }

        private void InitChart()
        {
            chrtNewHireReportBySeniority.Width = Seniorities.Count * 80;
            chrtNewHireReportBySeniority.Height = 500;
            InitAxis(chrtNewHireReportBySeniority.ChartAreas[MAIN_CHART_AREA_NAME].AxisX, "Seniority", false);
            InitAxis(chrtNewHireReportBySeniority.ChartAreas[MAIN_CHART_AREA_NAME].AxisY, "Number of Hires", true);

            chrtNewHireReportByRecruiter.Width = Recruiters.Count * 80;
            chrtNewHireReportByRecruiter.Height = 500;
            InitAxis(chrtNewHireReportByRecruiter.ChartAreas[MAIN_CHART_AREA_NAME].AxisX, "Recruiter", false);
            InitAxis(chrtNewHireReportByRecruiter.ChartAreas[MAIN_CHART_AREA_NAME].AxisY, "Number of Hires", true);

            UpdateChartTitle();
        }

        private void UpdateChartTitle()
        {
            chrtNewHireReportBySeniority.Titles.Clear();
            chrtNewHireReportBySeniority.Titles.Add(string.Format("New Hires By Seniority {0}", HostingPage.GraphRange));
            chrtNewHireReportBySeniority.Titles[0].Font = new Font("Arial", 18, FontStyle.Bold);

            chrtNewHireReportByRecruiter.Titles.Clear();
            chrtNewHireReportByRecruiter.Titles.Add(string.Format("New Hires By Recruiter {0}", HostingPage.GraphRange));
            chrtNewHireReportByRecruiter.Titles[0].Font = new Font("Arial", 18, FontStyle.Bold);
        }

        private void InitAxis(Axis horizAxis, string title, bool isVertical)
        {
            horizAxis.IsStartedFromZero = true;
            if (!isVertical)
                horizAxis.Interval = 1;
            horizAxis.TextOrientation = isVertical ? TextOrientation.Rotated270 : TextOrientation.Horizontal;
            horizAxis.LabelStyle.Angle = 0;
            horizAxis.TitleFont = new Font("Arial", 15, FontStyle.Bold);
            horizAxis.ArrowStyle = AxisArrowStyle.None;
            horizAxis.MajorGrid.Enabled = false;
            horizAxis.ToolTip = horizAxis.Title = title;
        }

        #endregion

    }
}
