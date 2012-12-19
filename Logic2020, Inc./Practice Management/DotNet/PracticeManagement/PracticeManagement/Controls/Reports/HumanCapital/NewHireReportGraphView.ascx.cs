using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using DataTransferObjects;
using System.Web.UI;

namespace PraticeManagement.Controls.Reports.HumanCapital
{
    public partial class NewHireGraphView : System.Web.UI.UserControl
    {
        #region constant

        private const string MAIN_CHART_AREA_NAME = "MainArea";
        public const string SeeNewHiresbyTitle = "See New Hires by Title";
        public const string SeeNewHiresbyRecruiter = "See New Hires by Recruiter";
        private const int TitleLabelRow = 0;
        private const int TitleTypeLabelRow = 1;
        private const int TitleLabelLength = 20;
        #endregion

        #region properties

        private PraticeManagement.Reporting.NewHireReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.NewHireReport)Page); }
        }

        private Dictionary<string, int> Titles { get; set; }

        private Dictionary<string, int> Recruiters { get; set; }

        private List<TitleType> TitleTypeList { get; set; }

        private List<DataTransferObjects.Title> TitleList { get; set; }

        private List<Person> RecuriterList { get; set; }

        private bool IsTitleGraph
        {
            get
            {
                if (hlnkGraph.Text == SeeNewHiresbyTitle)
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
            if (hlnkGraph.Text == SeeNewHiresbyTitle)
            {
                hlnkGraph.Text = SeeNewHiresbyRecruiter;
            }
            else
            {
                hlnkGraph.Text = SeeNewHiresbyTitle;
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
            bool isTitle;
            Boolean.TryParse(postBackDetails[2], out isTitle);
            if (isTitle)
            {
                lbName.Text = "Title: " + selectedValue;
                data = data.Where(p => p.Title != null ? p.Title.TitleName == selectedValue : selectedValue == Constants.FilterKeys.Unassigned).ToList();
            }
            else
            {
                lbName.Text = "Recruiter: " + selectedValue;
                bool isUnassigned = selectedValue.Equals(Constants.FilterKeys.Unassigned);
                data = data.Where(p => p.RecruiterId.HasValue ? p.RecruiterLastFirstName == selectedValue : isUnassigned).ToList();
            }
            tpSummary.BtnExportToExcelButton.Attributes["IsTitle"] = isTitle.ToString();
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
            if (IsTitleGraph)
            {
                TitleTypeList = ServiceCallers.Custom.Title(p => p.GetTitleTypes()).OrderBy(s => s.TitleTypeId).ToList();
                TitleList = ServiceCallers.Custom.Title(p => p.GetAllTitles()).OrderBy(s => s.TitleType.TitleTypeId).ThenBy(s => s.SortOrder).ToList();
                if (data.Any(p => p.Title == null))
                {
                    TitleTypeList.Add(new TitleType() { TitleTypeId = 0, TitleTypeName = Constants.FilterKeys.Unassigned });
                    TitleList.Add(new DataTransferObjects.Title { TitleId = 0, TitleName = Constants.FilterKeys.Unassigned, TitleType = new TitleType() { TitleTypeId = 0, TitleTypeName = Constants.FilterKeys.Unassigned } });
                }
                if (Titles == null)
                    Titles = new Dictionary<string, int>();
                foreach (var S in TitleList)
                {
                    int count = data.Count(p => p.Title != null ? p.Title.TitleId == S.TitleId : S.TitleId == 0);
                    Titles.Add(S.TitleName, count);
                }
            }
            else
            {
                RecuriterList = ServiceCallers.Custom.Person(p => p.GetPersonListWithRole("Recruiter")).ToList();
                List<Person> _recurterList = data.Select(p => p.RecruiterId.HasValue ? new Person { LastName = p.RecruiterLastName, FirstName = p.RecruiterFirstName, Id = p.RecruiterId.Value } : new Person { LastName = Constants.FilterKeys.Unassigned, Id = 0 }).Distinct().ToList();
                if (_recurterList.Count > 0)
                {
                    RecuriterList = RecuriterList.Concat(_recurterList).Distinct().ToList();
                }

                if (Recruiters == null)
                    Recruiters = new Dictionary<string, int>();
                foreach (var R in RecuriterList)
                {
                    int count = data.Count(p => p.RecruiterId.HasValue ? p.RecruiterId.Value == R.Id.Value : R.Id.Value == 0);
                    Recruiters.Add(R.Id == 0 ? R.LastName : R.PersonLastFirstName, count);
                }

            }
        }

        private void LoadChartData(List<Person> data)
        {
            if (IsTitleGraph)
            {
                var titleList = Titles.Select(p => new { name = p.Key, count = p.Value }).ToList();
                chrtNewHireReportByTitle.Visible = true;
                chrtNewHireReportByRecruiter.Visible = false;
                chrtNewHireReportByTitle.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.CustomLabels.Clear();

                int startPosTitleType = 1;
                foreach (var sc in TitleTypeList)
                {
                    sc.StartPosition = startPosTitleType;
                    sc.EndPosition = startPosTitleType + TitleList.Count(p => p.TitleType.TitleTypeId == sc.TitleTypeId) - 1;

                    AddCustomLabelToAxis(sc.StartPosition, sc.EndPosition, sc.TitleTypeName, sc.TitleTypeName, TitleTypeLabelRow, LabelMarkStyle.LineSideMark, GridTickTypes.Gridline,
                        chrtNewHireReportByTitle.ChartAreas[MAIN_CHART_AREA_NAME].AxisX);
                    startPosTitleType = sc.EndPosition + 1;

                }

                double startPosTitle = 0.5;
                foreach (var t in TitleList)
                {
                    AddCustomLabelToAxis(startPosTitle, startPosTitle + 1, GetSubString(t.TitleName), t.TitleName, TitleLabelRow, LabelMarkStyle.None, GridTickTypes.None,
                        chrtNewHireReportByTitle.ChartAreas[MAIN_CHART_AREA_NAME].AxisX);
                    startPosTitle++;

                }
                chrtNewHireReportByTitle.DataSource = titleList;
                chrtNewHireReportByTitle.DataBind();
            }
            else
            {
                var recruiterList = Recruiters.Select(p => new { name = p.Key, count = p.Value }).ToList();
                recruiterList = recruiterList.OrderBy(p => p.name).ToList();
                chrtNewHireReportByTitle.Visible = false;
                chrtNewHireReportByRecruiter.Visible = true;
                chrtNewHireReportByRecruiter.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.CustomLabels.Clear();

                double startPosRecruiter = 0.5;
                foreach (var r in recruiterList)
                {
                    var name = r.name;
                    AddCustomLabelToAxis(startPosRecruiter, startPosRecruiter + 1, GetSubString(name), name, TitleLabelRow, LabelMarkStyle.None, GridTickTypes.None,
                        chrtNewHireReportByRecruiter.ChartAreas[MAIN_CHART_AREA_NAME].AxisX);
                    startPosRecruiter++;

                }
                chrtNewHireReportByRecruiter.DataSource = recruiterList;
                chrtNewHireReportByRecruiter.DataBind();
            }
            InitChart();
        }

        private void AddCustomLabelToAxis(double startPosition, double endPosition, string text, string toolTip, int labelRow, LabelMarkStyle labelMarkStyle, GridTickTypes gridTickType, Axis axis)
        {
            CustomLabel customLabel = new CustomLabel(startPosition, endPosition, text, labelRow, labelMarkStyle, gridTickType);
            customLabel.ToolTip = toolTip;
            customLabel.MarkColor = Color.Black;
            customLabel.ForeColor = Color.Black;
            axis.CustomLabels.Add(customLabel);
            axis.LabelStyle.Font = new Font("Arial", 10, FontStyle.Regular);
        }

        private string GetSubString(string text)
        {
            return (text.Length > TitleLabelLength) ? text.Substring(0, TitleLabelLength) + "..." : text;
        }

        private string GetTitleType(int i)
        {
            string titleType = string.Empty;
            foreach (var sc in TitleTypeList)
            {
                if (i >= sc.StartPosition && i <= sc.EndPosition)
                {
                    return sc.TitleTypeName;
                }
            }
            return titleType;
        }

        private Color GetTitleTypeColor(string sc)
        {
            if (sc == "Business")
            {
                return Color.FromArgb(59, 100, 150);
            }
            else if (sc == "Internal")
            {
                return Color.MediumTurquoise;
            }
            else if (sc == "Technology")
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
            if (IsTitleGraph)
            {
                for (int i = 0; i < chrtNewHireReportByTitle.Series[0].Points.Count; i++)
                {
                    var point = chrtNewHireReportByTitle.Series[0].Points[i];
                    string sc = GetTitleType(i + 1);
                    point.Color = GetTitleTypeColor(sc);
                }

                var pointWidth = (Titles.Count > 25) ? 70 - (Titles.Count - 25) * 5 : 70;
                pointWidth = pointWidth < 20 ? 20 : pointWidth;
                int width = Titles.Count * pointWidth < 400 ? 400 : Titles.Count * pointWidth;
                chrtNewHireReportByTitle.Width = width;// > HostingPage.TotalPageWidth - 150 ? HostingPage.TotalPageWidth - 150 : width;
                chrtNewHireReportByTitle.Height = (Titles.Max(c => c.Value) * 9 < 800) ? 800 : Titles.Max(c => c.Value) * 9;
                InitAxis(chrtNewHireReportByTitle.ChartAreas[MAIN_CHART_AREA_NAME].AxisX, "Title", false);
                InitAxis(chrtNewHireReportByTitle.ChartAreas[MAIN_CHART_AREA_NAME].AxisY, "Number of New Hires", true);

            }
            else
            {
                var pointWidth = (Recruiters.Count > 25) ? 70 - (Recruiters.Count - 25) * 5 : 70;
                pointWidth = pointWidth < 20 ? 20 : pointWidth;
                int width = Recruiters.Count * pointWidth < 400 ? 400 : Recruiters.Count * pointWidth;
                chrtNewHireReportByRecruiter.Width = width ;//> HostingPage.TotalPageWidth - 150 ? HostingPage.TotalPageWidth - 150 : width;
                chrtNewHireReportByRecruiter.Height = (Recruiters.Max(c => c.Value) * 9 < 800) ? 800 : Recruiters.Max(c => c.Value) * 9;
                InitAxis(chrtNewHireReportByRecruiter.ChartAreas[MAIN_CHART_AREA_NAME].AxisX, "Recruiter", false);
                InitAxis(chrtNewHireReportByRecruiter.ChartAreas[MAIN_CHART_AREA_NAME].AxisY, "Number of New Hires", true);
            }
            UpdateChartTitle();
        }

        private void UpdateChartTitle()
        {
            if (IsTitleGraph)
            {
                chrtNewHireReportByTitle.Titles.Clear();
                chrtNewHireReportByTitle.Titles.Add("New Hires By Title");
                chrtNewHireReportByTitle.Titles.Add(HostingPage.GraphRange);
                chrtNewHireReportByTitle.Titles[0].Font =
                chrtNewHireReportByTitle.Titles[1].Font = new Font("Arial", 16, FontStyle.Bold);
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

