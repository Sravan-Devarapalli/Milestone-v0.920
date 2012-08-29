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
        public const string SeeNewHiresbySeniority = "See New Hires by Seniority";
        public const string SeeNewHiresbyRecruiter = "See New Hires by Recruiter";

        #endregion

        #region properties

        private PraticeManagement.Reporting.NewHireReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.NewHireReport)Page); }
        }

        private Dictionary<string, int> Seniorities { get; set; }

        private Dictionary<string, int> Recruiters { get; set; }

        private List<SeniorityCategory> SeniorityCategoryList { get; set; }

        private List<Seniority> SeniorityList { get; set; }

        private List<Person> RecuriterList { get; set; }

        private bool IsSeniorityGraph
        {
            get
            {
                if (hlnkGraph.Text == SeeNewHiresbySeniority)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public LinkButton hlnkGraphHiddenField
        {
            get
            {
                return hlnkGraph;
            }
        }

        protected void hlnkGraph_Click(object sender, EventArgs e)
        {
            if (hlnkGraph.Text == SeeNewHiresbySeniority)
            {
                hlnkGraph.Text = SeeNewHiresbyRecruiter;
            }
            else
            {
                hlnkGraph.Text = SeeNewHiresbySeniority;
            }
            PopulateGraph();
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void chrtNewHireReport_Click(object sender, ImageMapEventArgs e)
        {
            string[] postBackDetails = e.PostBackValue.Split(':');
            string selectedValue = postBackDetails[0].Trim();
            lbTotalHires.Text = postBackDetails[1];
            var data = ServiceCallers.Custom.Report(r => r.NewHireReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.PersonStatus, HostingPage.PayTypes, HostingPage.Practices, HostingPage.ExcludeInternalProjects, null, null, null, null)).ToList();
            bool isSeniority;
            Boolean.TryParse(postBackDetails[2], out isSeniority);
            if (isSeniority)
            {
                lbName.Text = "Seniority: " + selectedValue;
                data = data.Where(p => p.Seniority != null ? p.Seniority.Name == selectedValue : selectedValue == Constants.FilterKeys.Unassigned).ToList();
            }
            else
            {
                lbName.Text = "Recruiter: " + selectedValue;
                bool isUnassigned = selectedValue.Equals(Constants.FilterKeys.Unassigned);
                data = data.Where(p => p.RecruiterCommission.Any() ? p.RecruiterCommission.First().Recruiter.PersonLastFirstName == selectedValue : isUnassigned).ToList();
            }
            tpSummary.BtnExportToExcelButton.Attributes["IsSeniority"] = isSeniority.ToString();
            tpSummary.BtnExportToExcelButton.Attributes["FilterValue"] = selectedValue;
            tpSummary.PopUpFilteredPerson = data;
            tpSummary.PopulateData(true);
            mpeDetailView.Show();
        }

        #endregion

        #region Methods

        public void PopulateGraph()
        {
            List<Person> data = ServiceCallers.Custom.Report(r => r.NewHireReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.PersonStatus, HostingPage.PayTypes, HostingPage.Practices, HostingPage.ExcludeInternalProjects, null, null, null, null)).ToList();
            HostingPage.PopulateHeaderSection(data);
            if (data.Count > 0)
            {
                PopulateGraphAxisData(data);
                LoadChartData(data);
                divEmptyMessage.Style["display"] = "none";
                NewHireReportChartDiv.Visible = true;
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                NewHireReportChartDiv.Visible = false;
            }
        }

        private void PopulateGraphAxisData(List<Person> data)
        {
            if (IsSeniorityGraph)
            {

                SeniorityCategoryList = ServiceCallers.Custom.Person(p => p.ListAllSeniorityCategories()).OrderBy(s => s.Id).ToList();
                SeniorityList = ServiceCallers.Custom.Person(p => p.ListSeniorities()).OrderBy(s => s.SeniorityCategory.Id).ThenByDescending(s=>s.SeniorityValue).ToList();
                if (data.Any(p => p.Seniority == null))
                {
                    SeniorityCategoryList.Add(new SeniorityCategory() { Id = 0, Name = Constants.FilterKeys.Unassigned });
                    SeniorityList.Add(new Seniority { Id = 0, Name = Constants.FilterKeys.Unassigned, SeniorityCategory = new SeniorityCategory() { Id = 0, Name = Constants.FilterKeys.Unassigned } });
                }
                if (Seniorities == null)
                    Seniorities = new Dictionary<string, int>();
                foreach (var S in SeniorityList)
                {
                    int count = data.Count(p => p.Seniority != null ? p.Seniority.Id == S.Id : S.Id == 0);
                    Seniorities.Add(S.Name, count);
                }

                int startPos = 1;
                foreach (var sc in SeniorityCategoryList)
                {
                    sc.StartPosition = startPos;
                    sc.EndPosition = startPos + SeniorityList.Count(p => p.SeniorityCategory.Id == sc.Id) - 1;
                    startPos = sc.EndPosition + 1;
                }

            }
            else
            {
                RecuriterList = ServiceCallers.Custom.Person(p => p.GetPersonListWithRole("Recruiter")).ToList();
                List<Person> _recurterList = data.Select(p => p.RecruiterCommission.Any() ? p.RecruiterCommission.First().Recruiter : new Person { LastName = Constants.FilterKeys.Unassigned, Id = 0 }).Distinct().ToList();
                if (_recurterList.Count > 0)
                {
                    RecuriterList = RecuriterList.Concat(_recurterList).Distinct().ToList();
                }

                if (Recruiters == null)
                    Recruiters = new Dictionary<string, int>();
                foreach (var R in RecuriterList)
                {
                    int count = data.Count(p => p.RecruiterCommission.Any() ? p.RecruiterCommission.First().Recruiter.Id.Value == R.Id.Value : R.Id.Value == 0);
                    Recruiters.Add(R.Id == 0 ? R.LastName : R.PersonLastFirstName, count);
                }

            }
        }

        private void LoadChartData(List<Person> data)
        {
            if (IsSeniorityGraph)
            {
                var seniorityList = Seniorities.Select(p => new { name = p.Key, count = p.Value }).ToList();
                chrtNewHireReportBySeniority.Visible = true;
                chrtNewHireReportByRecruiter.Visible = false;
                foreach (var sc in SeniorityCategoryList)
                {
                    CustomLabel sCLabel = new CustomLabel(sc.StartPosition, sc.EndPosition, sc.Name, 1, LabelMarkStyle.LineSideMark, GridTickTypes.Gridline);
                    sCLabel.ToolTip = sc.Name;
                    sCLabel.MarkColor = Color.Black;
                    sCLabel.ForeColor = Color.Black; 
                    chrtNewHireReportBySeniority.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.CustomLabels.Add(sCLabel);
                }
                chrtNewHireReportBySeniority.DataSource = seniorityList;
                chrtNewHireReportBySeniority.DataBind();
            }
            else
            {
                var recruiterList = Recruiters.Select(p => new { name = p.Key, count = p.Value }).ToList();
                recruiterList = recruiterList.OrderBy(p => p.name).ToList();
                chrtNewHireReportBySeniority.Visible = false;
                chrtNewHireReportByRecruiter.Visible = true;
                chrtNewHireReportByRecruiter.DataSource = recruiterList;
                chrtNewHireReportByRecruiter.DataBind();
            }
            InitChart();
        }

        private string GetSeniorityCategory(int i)
        {
            string seniorityCategory = string.Empty;
            foreach (var sc in SeniorityCategoryList)
            {
                if (i >= sc.StartPosition && i <= sc.EndPosition)
                {
                    return sc.Name;
                }
            }
            return seniorityCategory;
        }

        private Color GetSeniorityCategoryColor(string sc)
        {
            if (sc == "Business Track")
            {
                return Color.FromArgb(59, 100, 150);
            }
            else if (sc == "Internal")
            {
                return Color.MediumTurquoise;
            }
            else if (sc == "Technical Track")
            {
                return Color.FromArgb(59, 148, 237);
            }
            else
            {
                return Color.LightBlue;
            }
        }

        private void InitChart()
        {
            if (IsSeniorityGraph)
            {
                for (int i = 0; i < chrtNewHireReportBySeniority.Series[0].Points.Count; i++)
                {
                    var point = chrtNewHireReportBySeniority.Series[0].Points[i];
                    string sc = GetSeniorityCategory(i + 1);
                    point.Color = GetSeniorityCategoryColor(sc);
                }

                chrtNewHireReportBySeniority.Width = Seniorities.Count * 70 < 400 ? 400 : Seniorities.Count * 70;
                chrtNewHireReportBySeniority.Height = 600;
                InitAxis(chrtNewHireReportBySeniority.ChartAreas[MAIN_CHART_AREA_NAME].AxisX, "Seniority", false);
                InitAxis(chrtNewHireReportBySeniority.ChartAreas[MAIN_CHART_AREA_NAME].AxisY, "Number of New Hires", true);
            }
            else
            {
                chrtNewHireReportByRecruiter.Width = Recruiters.Count * 70;
                chrtNewHireReportByRecruiter.Height = 600;
                InitAxis(chrtNewHireReportByRecruiter.ChartAreas[MAIN_CHART_AREA_NAME].AxisX, "Recruiter", false);
                InitAxis(chrtNewHireReportByRecruiter.ChartAreas[MAIN_CHART_AREA_NAME].AxisY, "Number of New Hires", true);
            }
            UpdateChartTitle();
        }

        private void UpdateChartTitle()
        {
            if (IsSeniorityGraph)
            {
                chrtNewHireReportBySeniority.Titles.Clear();
                chrtNewHireReportBySeniority.Titles.Add("New Hires By Seniority");
                chrtNewHireReportBySeniority.Titles.Add(HostingPage.GraphRange);
                chrtNewHireReportBySeniority.Titles[0].Font =
                chrtNewHireReportBySeniority.Titles[1].Font = new Font("Arial", 16, FontStyle.Bold);
            }
            else
            {
                chrtNewHireReportByRecruiter.Titles.Clear();
                chrtNewHireReportByRecruiter.Titles.Add("New Hires By Recruiter");
                chrtNewHireReportByRecruiter.Titles.Add(HostingPage.GraphRange);
                chrtNewHireReportByRecruiter.Titles[0].Font =
                chrtNewHireReportByRecruiter.Titles[1].Font = new Font("Arial", 16, FontStyle.Bold);
            }
        }

        private void InitAxis(Axis horizAxis, string title, bool isVertical)
        {
            horizAxis.IsStartedFromZero = true;
            if (!isVertical)
                horizAxis.Interval = 1;
            horizAxis.TextOrientation = isVertical ? TextOrientation.Rotated270 : TextOrientation.Horizontal;
            horizAxis.LabelStyle.Angle = isVertical ? 0 : 90;
            horizAxis.LabelStyle.Font = new Font("Arial", 10, FontStyle.Regular);
            horizAxis.TitleFont = new Font("Arial", 14, FontStyle.Bold);
            horizAxis.ArrowStyle = AxisArrowStyle.None;
            horizAxis.MajorGrid.Enabled = false;
            horizAxis.ToolTip = horizAxis.Title = title;
        }

        #endregion

    }
}

