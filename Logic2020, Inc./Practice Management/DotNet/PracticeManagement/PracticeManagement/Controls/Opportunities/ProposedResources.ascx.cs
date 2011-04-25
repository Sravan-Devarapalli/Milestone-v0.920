using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.OpportunityService;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using System.ServiceModel;
using System.Text;

namespace PraticeManagement.Controls.Opportunities
{
    public partial class ProposedResources : System.Web.UI.UserControl
    {
        private const string OPPORTUNITY_KEY = "OPPORTUNITY_KEY";
        private const string PreviousReportContext_Key = "PREVIOUSREPORTCONTEXT_KEY";
        private const string DistinctPotentialBoldPersons_Key = "DISTINCTPOTENTIALBOLDPERSONS_KEY";
        public Opportunity Opportunity
        {
            get
            {
                if (Cache[OPPORTUNITY_KEY] != null && OpportunityId.HasValue)
                {
                    if ((Cache[OPPORTUNITY_KEY] as Opportunity).Id == OpportunityId)
                    {
                        return Cache[OPPORTUNITY_KEY] as Opportunity;
                    }
                }

                if (OpportunityId.HasValue)
                {
                    using (var serviceClient = new OpportunityServiceClient())
                    {
                        try
                        {
                            Cache[OPPORTUNITY_KEY] = serviceClient.GetById(OpportunityId.Value);
                            return Cache[OPPORTUNITY_KEY] as Opportunity;
                        }
                        catch (CommunicationException ex)
                        {
                            serviceClient.Abort();
                            throw;
                        }
                    }
                }
                return null;
            }
        }

        public int? OpportunityId
        {
            get
            {
                int id;
                if (Int32.TryParse(hdnOpportunityIdValue.Value, out id))
                {
                    return id;
                }

                return null;
            }
            set
            {
                hdnOpportunityIdValue.Value = value.ToString();
            }
        }

        public bool hintDateVisible
        {
            get { return hintDate.Visible; }

            set { hintDate.Visible = value; }
        }

        public bool HasProposedPersons
        {
            get
            {
                if (cblProposedResources.Items.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        private BenchReportContext PreviousReportContext
        {
            get
            {
                if (Cache[PreviousReportContext_Key] != null)
                {
                    return Cache[PreviousReportContext_Key] as BenchReportContext;
                }
                return null;
            }
            set 
            {
                Cache[PreviousReportContext_Key] = value;
            }
           
        }

        private List<string> DistinctPotentialBoldPersons
        {
            get
            {
                if (Cache[DistinctPotentialBoldPersons_Key] != null)
                {
                    return Cache[DistinctPotentialBoldPersons_Key] as List<string>;
                }
                return null;
            }
            set
            {
                Cache[DistinctPotentialBoldPersons_Key] = value;
            }
        }

        private BenchReportContext ReportContext
        {
            get
            {
                BenchReportContext reportContext = new BenchReportContext
                {
                    Start = Opportunity.ProjectedStartDate.HasValue ? (DateTime)Opportunity.ProjectedStartDate : DateTime.Today,
                    End = Opportunity.ProjectedEndDate.HasValue ? (DateTime)Opportunity.ProjectedEndDate : DateTime.Today,
                    ActivePersons = true,
                    ProjectedPersons = true,
                    ActiveProjects = true,
                    ProjectedProjects = true,
                    ExperimentalProjects = true,
                    UserName = DataHelper.CurrentPerson.Alias,
                    PracticeIds = null
                };
                return reportContext;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Cache.Remove(PreviousReportContext_Key);
                Cache.Remove(DistinctPotentialBoldPersons_Key);
            }
        }

        private string GetSelectedItems(CheckBoxList cbl, CheckBoxList cbl2 = null)
        {
            var clientList = new StringBuilder();
            foreach (ListItem item in cbl.Items)
            {
                if (item.Selected)
                {
                    if (cbl2 != null)
                    {
                        if (!cbl2.Items.Contains(item))
                        {
                            clientList.Append(item.Value).Append(',');
                        }
                    }
                    else
                    {
                        clientList.Append(item.Value).Append(',');
                    }

                    item.Selected = false;
                }
            }
            return clientList.ToString();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            var selectedList = GetSelectedItems(cblPotentialResources, cblProposedResources);
            if (!string.IsNullOrEmpty(selectedList))
            {
                using (var serviceClient = new OpportunityServiceClient())
                {
                    serviceClient.OpportunityPersonInsert(OpportunityId.Value, selectedList);
                }
            }

            FillProposedResources();
            cblPotentialResources.DataBind();
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            var selectedList = GetSelectedItems(cblProposedResources);

            using (var serviceClient = new OpportunityServiceClient())
            {
                serviceClient.OpportunityPersonDelete(OpportunityId.Value, selectedList);
            }

            FillProposedResources();
            cblPotentialResources.DataBind();
        }

        public void cblPotentialResources_DataBound(object sender, EventArgs e)
        {
            List<string> distinctPotentialBoldPersons = GetDistinctPotentialBoldPersons();

            foreach (ListItem item in cblPotentialResources.Items)
            {
                item.Selected = false;
                item.Enabled = true;

                //item.Attributes["onclick"] = "SethdnPotentialResourcesValueToTrue();";

                foreach (string name in distinctPotentialBoldPersons)
                {
                    if (item.Text == name)
                    {
                        item.Attributes["style"] = "font-weight:bold;";
                        break;
                    }
                }
                foreach (ListItem item2 in cblProposedResources.Items)
                {
                    if (item2.Value == item.Value)
                    {
                        item.Selected = true;
                        item.Enabled = false;
                        break;
                    }
                }
            }

        }

        public void cblProposedResources_DataBound(object sender, EventArgs e)
        {
            //foreach (ListItem item in cblProposedResources.Items)
            //{
            //    item.Attributes["onclick"] = "SethdnProposedResourcesValueToTrue();";
            //}
        }

        private List<string> GetDistinctPotentialBoldPersons()
        {
            if (
                PreviousReportContext != null && 
                DistinctPotentialBoldPersons != null &&
                PreviousReportContext.Start == ReportContext.Start &&
                PreviousReportContext.End == ReportContext.End &&
                PreviousReportContext.ActivePersons == ReportContext.ActivePersons &&
                PreviousReportContext.ProjectedPersons == ReportContext.ProjectedPersons &&
                PreviousReportContext.ActiveProjects == ReportContext.ActiveProjects  &&
                PreviousReportContext.ProjectedProjects == ReportContext.ProjectedProjects &&
                PreviousReportContext.ExperimentalProjects == ReportContext.ExperimentalProjects &&
                PreviousReportContext.UserName == ReportContext.UserName &&
                PreviousReportContext.PracticeIds == ReportContext.PracticeIds
               )
            {
                return DistinctPotentialBoldPersons;
            }
            else
            {
                var potentialBoldProjects = ServiceCallers.Custom.Project(c => c.GetBenchListWithoutBenchTotalAndAdminCosts(ReportContext));

                List<string> potentialBoldPersons = new List<string>();

                var monthBegin = new DateTime(ReportContext.Start.Year, ReportContext.Start.Month, Constants.Dates.FirstDay);
                int periodlength = GetPeriodLength();
                var monthEnd = new DateTime(monthBegin.Year, monthBegin.Month, DateTime.DaysInMonth(monthBegin.Year, monthBegin.Month));

                foreach (Project project in potentialBoldProjects)
                {
                    foreach (KeyValuePair<DateTime, ComputedFinancials> interestValue in
                            project.ProjectedFinancialsByMonth)
                    {
                        if (IsInMonth(interestValue.Key, monthBegin, monthEnd) && !interestValue.Value.EndDate.HasValue)
                        {
                            potentialBoldPersons.Add(project.Name);
                        }
                    }
                }

                PreviousReportContext = ReportContext;
                DistinctPotentialBoldPersons = potentialBoldPersons.Distinct<string>().ToList<string>();

                return DistinctPotentialBoldPersons;
            }
        }

        private static bool IsInMonth(DateTime date, DateTime periodStart, DateTime periodEnd)
        {
            bool result =
                (date.Year > periodStart.Year ||
                (date.Year == periodStart.Year && date.Month >= periodStart.Month)) &&
                (date.Year < periodEnd.Year || (date.Year == periodEnd.Year && date.Month <= periodEnd.Month));

            return result;
        }

        private int GetPeriodLength()
        {
            int mounthsInPeriod =
                (ReportContext.End.Year - ReportContext.Start.Year) * Constants.Dates.LastMonth +
                (ReportContext.End.Month - ReportContext.Start.Month + 1);
            return mounthsInPeriod;
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {

        }

        public void FillPotentialResources()
        {
            var potentialPersons = ServiceCallers.Custom.Person(c => c.GetPersonListByStatusList("1,3", null));
            cblPotentialResources.DataSource = potentialPersons.OrderBy(c => c.LastName);
            cblPotentialResources.DataBind();
        }

        public void FillProposedResources()
        {
            using (var serviceClient = new OpportunityServiceClient())
            {
                cblProposedResources.DataSource = serviceClient.GetOpportunityPersons(OpportunityId.Value);
                cblProposedResources.DataBind();
            }
        }
    }
}
