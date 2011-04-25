using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using DataTransferObjects;
using PraticeManagement.Utils;

namespace PraticeManagement.Controls.Generic
{
    public partial class RecentNotes : System.Web.UI.UserControl
    {
        public const int StartRow = 0;
        public const int MaxRows = 1000;
        private const string NoteActivityName = "Note";

        public string PeriodFilter { get; set; }
        public string PersonId { get; set; }
        public string SourceFilter { get; set; }
        public string ProjectId { get; set; }
        public string OpportunityId { get; set; }
        public string MilestoneId { get; set; }
        public string TransformScript { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
        }

        public void Update()
        {
            lvRecentNotes.DataBind();
            pnlCanvas.Update();
        }

        public static ActivityLogItem[] GetActivities(
            string sourceFilter,
            string periodFilter,
            string personId,
            string projectId,
            string opportunityId,
            string milestoneId)
        {
            return ActivityLogHelper.GetActivities(
                    sourceFilter,
                    periodFilter,
                    personId,
                    projectId,
                    opportunityId,
                    milestoneId,
                    StartRow,
                    MaxRows)
                .Where(i => i.ActivityTypeId == 3 && i.LogData.Contains(NoteActivityName))
                .ToArray();
        }

        public static int GetActivitiesCount(
            string sourceFilter,
            string periodFilter,
            string personId,
            string projectId,
            string opportunityId,
            string milestoneId)
        {
            return ActivityLogHelper.GetActivitiesCount(
                    sourceFilter,
                    periodFilter,
                    personId,
                    projectId,
                    opportunityId,
                    milestoneId);
        }

        public string GetContent(ActivityLogItem dataItem)
        {
            var transform = new XslCompiledTransform();
            transform.Load(Server.MapPath(TransformScript));
            var doc = new XmlDocument();
            doc.LoadXml(dataItem.LogData);
            var navigator = doc.DocumentElement.CreateNavigator();
            var sb = new StringBuilder();
            var swriter = new StringWriter(sb);
            var writer = new XmlTextWriter(swriter);
            transform.Transform(navigator, null, writer);
            return sb.ToString();
        }
    }
}
