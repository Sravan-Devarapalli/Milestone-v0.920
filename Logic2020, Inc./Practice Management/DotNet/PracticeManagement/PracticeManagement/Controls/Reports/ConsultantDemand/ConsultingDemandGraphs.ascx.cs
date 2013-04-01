using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using PraticeManagement.Reports;


namespace PraticeManagement.Controls.Reports.ConsultantDemand
{
    public partial class ConsultingDemandGraphs : System.Web.UI.UserControl
    {
        private const string MAIN_CHART_AREA_NAME = "MainArea";

        public string PipeLineTitle = "Pipeline Title Demand By Month";

        public string PipeLineSkill = "Pipeline SkillSet Demand By Month";

        public LinkButton hlinkGraphs { get { return hlnkGraph; } }

        private PraticeManagement.Reports.ConsultingDemand_New HostingPage
        {
            get { return ((PraticeManagement.Reports.ConsultingDemand_New)Page); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                chartConsultnDemandPipeline.Visible = false;
            }
        }

        protected void chartConsultnDemandPipeline_Click(object sender, ImageMapEventArgs e)
        {
            string[] postBackDetails = e.PostBackValue.Split(',');
            ctrDetails.BtnExportPipeLineSelectedValue = postBackDetails[0].ToString();

            if (hlnkGraph.Text == PipeLineSkill)
            {
                HostingPage.GraphType = ConsultingDemand_New.PipelineTitle;
            }
            else
            {
                HostingPage.GraphType = ConsultingDemand_New.PipelineSkill;
            }
            lblMonth.Text = ctrDetails.BtnExportPipeLineSelectedValue;
            lblCount.Text = string.Empty;
            ctrDetails.PopulateData();
            mpeDetailView.Show();
        }

        protected void chartConsultngDemand_Click(object sender, ImageMapEventArgs e)
        {
            string[] postBackDetails = e.PostBackValue.Split(',');

            ctrDetails.BtnExportSelectedStartDate = Utils.Calendar.MonthStartDate(Convert.ToDateTime(postBackDetails[0])).ToString();
            ctrDetails.BtnExportSelectedEndDate = Utils.Calendar.MonthEndDate(Convert.ToDateTime(postBackDetails[0])).ToString();
            lblMonth.Text = "Month :" + "" + Utils.Calendar.MonthStartDate(Convert.ToDateTime(postBackDetails[0])).ToString(Constants.Formatting.FullMonthYearFormat);
            ctrDetails.PopulateData();
            lblCount.Text = HostingPage.RolesCount.ToString();
            if (lblCount.Text != "1")
            {
                lblCount.Text += " Roles";
            }
            else
            {
                lblCount.Text += " Role";
            }
            mpeDetailView.Show();
        }

        public void PopulateGraph()
        {
            Dictionary<string, int> data = new Dictionary<string, int>();
            if (HostingPage.GraphType == ConsultingDemand_New.TransactionTitle)
            {
                chartConsultnDemandPipeline.Visible = false;
                chartConsultngDemand.Visible = true;
                hlnkGraph.Visible = false;
                data = ServiceCallers.Custom.Report(r => r.ConsultingDemandGraphsByTitle(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.isSelectAllTitles ? null : HostingPage.hdnTitlesProp));
                chartConsultngDemand.DataSource = data.Select(p => new { month = p.Key, count = p.Value }).ToList();
                chartConsultngDemand.DataBind();
                InitChart(data.Count);
            }
            else if (HostingPage.GraphType == ConsultingDemand_New.TransactionSkill)
            {
                chartConsultnDemandPipeline.Visible = false;
                chartConsultngDemand.Visible = true;
                hlnkGraph.Visible = false;
                data = ServiceCallers.Custom.Report(r => r.ConsultingDemandGraphsBySkills(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.isSelectAllSkills ? null : HostingPage.hdnSkillsProp));
                chartConsultngDemand.DataSource = data.Select(p => new { month = p.Key, count = p.Value }).ToList();
                chartConsultngDemand.DataBind();
                InitChart(data.Count);
            }
            else
            {
                chartConsultnDemandPipeline.Visible = true;
                chartConsultngDemand.Visible = false;
                hlnkGraph.Visible = true;
                if (HostingPage.GraphType == ConsultingDemand_New.PipelineTitle)
                {
                    data = ServiceCallers.Custom.Report(r => r.ConsultingDemandGrphsGroupsByTitle(HostingPage.StartDate.Value, HostingPage.EndDate.Value));
                }
                else if (HostingPage.GraphType == ConsultingDemand_New.PipelineSkill)
                {
                    data = ServiceCallers.Custom.Report(r => r.ConsultingDemandGrphsGroupsBySkill(HostingPage.StartDate.Value, HostingPage.EndDate.Value));
                }
                chartConsultnDemandPipeline.DataSource = data.Select(p => new { title = p.Key, count = p.Value }).ToList();
                chartConsultnDemandPipeline.DataBind();
                InitChart(data.Count);
            }
        }

        private void InitChart(int count)
        {
            if (HostingPage.GraphType == ConsultingDemand_New.TransactionTitle || HostingPage.GraphType == ConsultingDemand_New.TransactionSkill)
            {
                chartConsultngDemand.Width = ((count < 5) ? 5 : count) * 70;
                chartConsultngDemand.Height = 500;
                InitAxis(chartConsultngDemand.ChartAreas[MAIN_CHART_AREA_NAME].AxisX, "Month", false);
                InitAxis(chartConsultngDemand.ChartAreas[MAIN_CHART_AREA_NAME].AxisY, "Number of Resources", true);
            }
            else
            {
                chartConsultnDemandPipeline.Width = ((count < 5) ? 5 : count) * 120;
                chartConsultnDemandPipeline.Height = 500;
                InitAxis(chartConsultnDemandPipeline.ChartAreas[0].AxisY, "Quantity", false, 0);
                if (HostingPage.GraphType == ConsultingDemand_New.PipelineTitle)
                {
                    InitAxis(chartConsultnDemandPipeline.ChartAreas[0].AxisX, "Title", true);
                }
                else
                {
                    InitAxis(chartConsultnDemandPipeline.ChartAreas[0].AxisX, "Skill Set", true);
                }
            }
            chartConsultnDemandPipeline.ChartAreas[0].AxisY2.TextOrientation = TextOrientation.Rotated90;
            UpdateChartTitle();
        }

        /// <summary>
        /// Updates the title of graph depends on link button's text.
        /// </summary>
        private void UpdateChartTitle()
        {
            int multipleSelected;
            chartConsultngDemand.Titles.Clear();
            chartConsultnDemandPipeline.Titles.Clear();
            if (HostingPage.GraphType == ConsultingDemand_New.TransactionTitle)
            {
                multipleSelected = HostingPage.hdnTitlesProp.Where(t => t == ',').Count();
                chartConsultngDemand.Titles.Add("Resource Demand By Titles");
                Title title = new Title();
                title.Text = HostingPage.isSelectAllTitles ? "All Titles" : (multipleSelected > 1 ? "Multiple Titles Selected" : HostingPage.hdnTitlesProp.TrimEnd(','));
                title.ToolTip = HostingPage.isSelectAllTitles ? "All Titles" : HostingPage.hdnTitlesProp.TrimEnd(',');
                chartConsultngDemand.Titles.Add(title);
                chartConsultngDemand.Titles[0].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
                chartConsultngDemand.Titles[1].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
            }
            else if (HostingPage.GraphType == ConsultingDemand_New.TransactionSkill)
            {
                multipleSelected = HostingPage.hdnSkillsProp.Where(s => s == ',').Count();
                chartConsultngDemand.Titles.Add("Resource Demand By Skills");
                Title title = new Title();
                title.Text = HostingPage.isSelectAllSkills ? "All Skills" : (multipleSelected > 1 ? "Multiple Skills Selected" : HostingPage.hdnSkillsProp.TrimEnd(','));
                title.ToolTip = HostingPage.isSelectAllSkills ? "All Skills" : HostingPage.hdnSkillsProp.TrimEnd(',');
                chartConsultngDemand.Titles.Add(title);
                chartConsultngDemand.Titles[0].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
                chartConsultngDemand.Titles[1].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
            }
            else if (HostingPage.GraphType == ConsultingDemand_New.PipelineTitle)
            {
                chartConsultnDemandPipeline.Titles.Add("PipeLine Title Demand by Month");
                chartConsultnDemandPipeline.Titles.Add(HostingPage.StartDate.Value.ToString(Constants.Formatting.FullMonthYearFormat) + "-" + HostingPage.EndDate.Value.ToString(Constants.Formatting.FullMonthYearFormat));
                chartConsultnDemandPipeline.Titles[0].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
                chartConsultnDemandPipeline.Titles[1].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
            }
            else
            {
                chartConsultnDemandPipeline.Titles.Add("PipeLine SkillSet Demand by Month");
                chartConsultnDemandPipeline.Titles.Add(HostingPage.StartDate.Value.ToString(Constants.Formatting.FullMonthYearFormat) + "-" + HostingPage.EndDate.Value.ToString(Constants.Formatting.FullMonthYearFormat));
                chartConsultnDemandPipeline.Titles[0].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
                chartConsultnDemandPipeline.Titles[1].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
            }
        }

        /// <summary>
        /// Intiates axis's style.
        /// </summary
        private void InitAxis(Axis horizAxis, string title, bool isVertical, int labelAngle = -1)
        {
            horizAxis.IsStartedFromZero = true;
            if (!isVertical)
                horizAxis.Interval = 1;
            horizAxis.TextOrientation = isVertical ? TextOrientation.Rotated270 : TextOrientation.Horizontal;
            horizAxis.LabelStyle.Angle = labelAngle != -1 ? labelAngle : isVertical ? 0 : 45;
            horizAxis.TitleFont = new System.Drawing.Font("Arial", 14, FontStyle.Bold);
            horizAxis.ArrowStyle = AxisArrowStyle.None;
            horizAxis.MajorGrid.Enabled = false;
            horizAxis.ToolTip = horizAxis.Title = title;
        }

        protected void hlnkGraph_Click(object sender, EventArgs e)
        {
            if (hlnkGraph.Text == PipeLineTitle)
            {
                hlnkGraph.Text = PipeLineSkill;
                HostingPage.GraphType = ConsultingDemand_New.PipelineTitle;
            }
            else
            {
                hlnkGraph.Text = PipeLineTitle;
                HostingPage.GraphType = ConsultingDemand_New.PipelineSkill;
            }
            PopulateGraph();
        }
    }
}

