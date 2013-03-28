using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports.ConsultingDemand;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;


namespace PraticeManagement.Controls.Reports.ConsultantDemand
{
    public partial class ConsultingDemandGraphs : System.Web.UI.UserControl
    {
        private const string MAIN_CHART_AREA_NAME = "MainArea";
        public  string PipeLineTitle = "Pipeline Title Demand By Month";
        public  string PipeLineSkill = "Pipeline SkillSet Demand By Month";
        public  bool isTitle;
        public bool isPipeline;
        public bool isPipelineByTitle=true;
        public const string pipelineGraph = "3";
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
            string titleSkill = e.PostBackValue[0].ToString();
            if (hlnkGraph.Text == PipeLineSkill)
            {
                isTitle = true;
                List<ConsultantGroupbyTitle> data;
                data = ServiceCallers.Custom.Report(r => r.ConsultingDemandTransactionReportByTitle(HostingPage.StartDate.Value, HostingPage.EndDate.Value, "1099 Developer")).ToList();
                ctrDetails.PopUpFilteredTitle = data;
            }
            else
            {
                isTitle = false;
                List<ConsultantGroupbySkill> data;
                data = ServiceCallers.Custom.Report(r => r.ConsultingDemandTransactionReportBySkill(HostingPage.StartDate.Value, HostingPage.EndDate.Value, titleSkill)).ToList();
                ctrDetails.PopUpFilteredSkill = data;
            }
            ctrDetails.PopulateData(isTitle);
            mpeDetailView.Show();
        }

        protected void chartConsultngDemand_Click(object sender, ImageMapEventArgs e)
        {
            string[] postBackDetails = e.PostBackValue.Split(',');

            var startDate = Utils.Calendar.MonthStartDate(Convert.ToDateTime(postBackDetails[0]));
            var endDate = Utils.Calendar.MonthEndDate(Convert.ToDateTime(postBackDetails[0]));
            ctrDetails.BtnExportToExcelButton.Attributes["startDate"] = startDate.ToString();
            ctrDetails.BtnExportToExcelButton.Attributes["endDate"] = endDate.ToString();
            ctrDetails.BtnExportToExcelButton.Attributes["IsGraphViewPopUp"] = true.ToString();
            if (startDate.ToString("MMMM yyyy") == endDate.ToString("MMMM yyyy"))
            {
                lblMonth.Text = "Month :" + "" + startDate.ToString("MMMM yyyy");
            }
            else
            {
                lblMonth.Text = "Month :" + "" + startDate.ToString("MMMM yyyy") + "-" + endDate.ToString("MMMM yyyy");
            }
            if ( HostingPage.titleOrSkill == "Title")
            {
                List<ConsultantGroupbyTitle> data;
                string titles = HostingPage.hdnTitlesProp;
                data = ServiceCallers.Custom.Report(r => r.ConsultingDemandTransactionReportByTitle(startDate, endDate, titles)).ToList();
                ctrDetails.PopUpFilteredTitle = data;
                isTitle = true;
                lblCount.Text = data.Count.ToString();
            }
            else if (HostingPage.titleOrSkill == "Skill")
            {
                List<ConsultantGroupbySkill> data;
                string skills = HostingPage.hdnSkillsProp;
                data = ServiceCallers.Custom.Report(r => r.ConsultingDemandTransactionReportBySkill(startDate, endDate, skills)).ToList();
                ctrDetails.PopUpFilteredSkill = data;
                isTitle = false;
                lblCount.Text = data.Count.ToString();
            }
            if (lblCount.Text != "1")
            {
                lblCount.Text += "Roles";
            }
            else
            {
                lblCount.Text += "Role";
            }
            ctrDetails.PopulateData(isTitle);
            mpeDetailView.Show();
        }
        

        public void PopulateGraph(string value,string param=null)
        {
            Dictionary<string, int> data = new Dictionary<string, int>();
            if (value == "1")
            {
                chartConsultnDemandPipeline.Visible = false;
                chartConsultngDemand.Visible = true;
                isPipeline = false;
                hlnkGraph.Visible = false;
                isTitle=true;
                data = ServiceCallers.Custom.Report(r => r.ConsultingDemandGraphsByTitle(HostingPage.StartDate.Value, HostingPage.EndDate.Value, param));
                chartConsultngDemand.DataSource = data.Select(p => new { month = p.Key, count = p.Value }).ToList();
                chartConsultngDemand.DataBind();
                InitChart(data.Count);
                chartConsultngDemand.Series["chartSeries"].XValueMember = "month";
                chartConsultngDemand.Series["chartSeries"].YValueMembers = "count";  
            }
            else if (value == "2")
            {
                chartConsultnDemandPipeline.Visible = false;
                chartConsultngDemand.Visible = true;
                isPipeline = false;
                hlnkGraph.Visible = false;
                isTitle=false;
                data = ServiceCallers.Custom.Report(r => r.ConsultingDemandGraphsBySkills(HostingPage.StartDate.Value, HostingPage.EndDate.Value, param));
                chartConsultngDemand.DataSource = data.Select(p => new { month = p.Key, count = p.Value }).ToList();
                chartConsultngDemand.DataBind();
                InitChart(data.Count);
                chartConsultngDemand.Series["chartSeries"].XValueMember = "month";
                chartConsultngDemand.Series["chartSeries"].YValueMembers = "count";
            }
            else
            {
                chartConsultnDemandPipeline.Visible = true;
                chartConsultngDemand.Visible = false;
                hlnkGraph.Visible = true;
                isPipeline = true;
                if (isPipelineByTitle)
                {
                    data = ServiceCallers.Custom.Report(r => r.ConsultingDemandGrphsGroupsByTitle(HostingPage.StartDate.Value, HostingPage.EndDate.Value));
                }
                else
                {
                    data = ServiceCallers.Custom.Report(r => r.ConsultingDemandGrphsGroupsBySkill(HostingPage.StartDate.Value, HostingPage.EndDate.Value));
                }
                chartConsultnDemandPipeline.DataSource = data.Select(p => new { title = p.Key, count = p.Value }).ToList();
                chartConsultnDemandPipeline.DataBind();
                chartConsultnDemandPipeline.Series["seriesPipeline"].XValueMember = "title";
                chartConsultnDemandPipeline.Series["seriesPipeline"].YValueMembers = "count";
                InitChart(data.Count);
            }
        }

        private void InitChart(int count)
        {
            if (!isPipeline)
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
                InitAxis(chartConsultnDemandPipeline.ChartAreas[0].AxisY, "Quantity", false);
                if (isPipelineByTitle)
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
            if (!isPipeline)
            {
                chartConsultngDemand.Titles.Clear();
                if (isTitle)
                {
                    chartConsultngDemand.Titles.Add("Resource Demand By Titles");
                }
                else
                {
                    chartConsultngDemand.Titles.Add("Resource Demand By Skills");
                }
                chartConsultngDemand.Titles[0].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
                //chartConsultngDemand.Titles[1].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
            }
            else
            {
                chartConsultnDemandPipeline.Titles.Clear();
                if (hlnkGraph.Text == PipeLineTitle)
                {
                    chartConsultnDemandPipeline.Titles.Add("PipeLine SkillSet Demand by Month");
                }
                else
                {
                    chartConsultnDemandPipeline.Titles.Add("PipeLine Titles Demand by Month");
                }
                chartConsultnDemandPipeline.Titles.Add(HostingPage.StartDate.Value.ToString("MMMM yyyy") + "-" + HostingPage.EndDate.Value.ToString("MMMM yyyy"));
                chartConsultnDemandPipeline.Titles[0].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
                chartConsultnDemandPipeline.Titles[1].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
            }
        }

        /// <summary>
        /// Intiates axis's style.
        /// </summary
        private void InitAxis(Axis horizAxis, string title, bool isVertical)
        {
            horizAxis.IsStartedFromZero = true;
            if (!isVertical)
                horizAxis.Interval = 1;
            horizAxis.TextOrientation = isVertical ? TextOrientation.Rotated270 : TextOrientation.Horizontal;
            horizAxis.LabelStyle.Angle = isVertical ? 0 : 45;
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
                isPipelineByTitle = true;
            }
            else
            {
                hlnkGraph.Text = PipeLineTitle;
                isPipelineByTitle = false;
            }
            PopulateGraph(pipelineGraph);
        }

    }
}

