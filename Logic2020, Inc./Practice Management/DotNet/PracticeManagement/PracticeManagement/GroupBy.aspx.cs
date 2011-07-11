using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using DataTransferObjects;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Web.Security;
using System.Configuration;
using PraticeManagement.Utils;
using Microsoft.WindowsAzure.ServiceRuntime;
using PraticeManagement.ProjectService;
using System.ServiceModel;
using PraticeManagement.Configuration;

namespace PraticeManagement
{
    public partial class GroupByDirector : PracticeManagementPageBase
    {
        #region "Constants"
        private const int NumberOfFixedColumns = 2;
        private const string GrandTotalCellTemplate = "<table><tr><td align='right'>{0}</td></tr><tr><td align='right'>{1}</td></tr></table>";
        private const string MonthCellTemplate = "<td align='right' style='padding-right:3px;'><table><tr><td align='right'>{0}</td></tr><tr><td align='right'>{1}</td></tr></table></td>";
        private const string CurrentMonthCellTemplate = "<td align='right' style='padding-right:3px;border-left:1px solid black;border-right:1px solid black;'><table><tr><td align='right'>{0}</td></tr><tr><td align='right'>{1}</td></tr></table></td>";
        private const string CurrentMonthLastCellTemplate = "<td align='right' style='padding-right:3px;border-left:1px solid black;border-right:1px solid black;border-bottom:1px solid black;'><table><tr><td align='right'>{0}</td></tr><tr><td align='right'>{1}</td></tr></table></td>";
        private const string RowHTMLTemplate =
                            @"</tr><tr {5} height = '35px' class='hidden'>
                            <td style='background-color : White !important;'></td>
                            <td style='padding-left:{0}px;'>{4}</td>{1}
                            <td align='right' style='padding-right:5px;'>
                                <table> <tr><td align='right'>{2}</td></tr><tr><td align='right'>{3}</td></tr></table>
                            </td>";
        private const string GrandTotalRowHTMLTemplate = "</tr><tr class='summary' style='border:1px solid black;' height = '40px'><td align='center' style='border-left:2px solid black;' >Grand Total</td><td></td>{0}<td align='right' style='padding-right:5px;'><table><tr><td>{1}</td></tr><tr><td>{2}</td></tr></table></td>";
        private const string CollpseExpandCellTemplate =
                   @"<table><tr><td width='15px'>
                  <img alt='Collapse' name='Collapse' {0} onclick='ExpandCollapseChilds(this);' src='Images/collapse.jpg' class='hidden'  />
                  <img alt='Expand' name='Expand' {0} onclick='ExpandCollapseChilds(this);' src='Images/expand.jpg' {3} />
                    </td><td style='padding-left:6px;'><font style='{1}'>{2}</font>
                    </td></tr></table>";
        private const int ProjectCellLeftPadding = 50;
        private const int ClientCellLeftPadding = 0;
        private const int ClientGroupCellLeftPadding = 20;
        private const string STRDirectorReportSortExpression = "DirectorReportSortExpression";
        private const string STRDirectorReportSortDirection = "DirectorReportSortDirection";
        private const string STRDirectorReportSortColumnId = "DirectorReportSortColumnId";

        private const string STRAMReportSortExpression = "AMReportSortExpression";
        private const string STRAMReportSortDirection = "AMReportSortDirection";
        private const string STRAMReportSortColumnId = "AMReportSortColumnId";

        private const string STRPMReportSortExpression = "PMReportSortExpression";
        private const string STRPMReportSortDirection = "PMReportSortDirection";
        private const string STRPMReportSortColumnId = "PMReportSortColumnId";

        private const string GroupByPersonFilterKey = "GroupByPersonFilterKey";
        private const string ProjectsGroupedByPracticeKey = "ProjectsGroupedByPractice";
        private const string GroupedByPracticeGrandTotalKey = "GroupedByPracticeGrandTotal";
        private const string ProjectBagroundStyle = " style='background-color : #EEF3F9;'";
        private const string ClientBagroundStyle = " style='background-color : #FFE0B2'";
        private const string PersonAttributeTemplate = " Person = '{0}' ";
        private const string ClientAttributeTemplate = " Client = '{0}' ";
        private const string ExpandCollapseStatusAttribute = " ExpandCollapseStatus = 'Collapse' ";
        private const string ClientGroupAttributeTemplate = " ClientGroup = '{0}' ";
        private const string ProjectsWithoutDirectorText = "Projects Without Client Director";
        private const string IncludeProjectsWithoutDirectorItemKey = "ShowWithoutDirector";
        private const string LinkHTMLTemplate = "<a href='{0}' target='_blank'>{1}</a>";
        private const string ProjectDetailPagePath = "ProjectDetail.aspx";

        #endregion

        #region "Properties"

        private int? DefaultProjectId
        {

            get
            {
                return MileStoneConfigurationManager.GetProjectId();
            }
        }

        private Project[] ProjectList
        {
            get
            {
                CompanyPerformanceState.Filter = GetFilterSettings();
                return CompanyPerformanceState.ProjectList;
            }
        }

        private List<ProjectsGroupedByPractice> GroupedPractices
        {
            get
            {
                if (Session[ProjectsGroupedByPracticeKey] == null)
                {
                    var filterSet = GetFilterSettings();
                    MilestonePerson[] milestonePersons;
                    using (var serviceClient = new ProjectServiceClient())
                    {
                        try
                        {

                            milestonePersons =
                                serviceClient.GetProjectListGroupByPracticeManagers(
                                filterSet.ClientIdsList,
                                filterSet.ShowProjected,
                                filterSet.ShowCompleted,
                                filterSet.ShowActive,
                                filterSet.ShowInternal,
                                filterSet.ShowExperimental,
                                filterSet.ShowInactive,
                                filterSet.PeriodStart,
                                filterSet.PeriodEnd,
                                filterSet.SalespersonIdsList,
                                filterSet.ProjectOwnerIdsList,
                                filterSet.PracticeIdsList,
                                filterSet.ProjectGroupIdsList,
                                filterSet.ExcludeInternalPractices);
                        }
                        catch (CommunicationException)
                        {
                            serviceClient.Abort();
                            throw;
                        }
                    }
                    var groupedPracticeManagers = GetGroupedPracticeManagers(milestonePersons.ToList());
                    Session[ProjectsGroupedByPracticeKey] = groupedPracticeManagers;
                    return groupedPracticeManagers;
                }
                else
                {
                    return Session[ProjectsGroupedByPracticeKey] as List<ProjectsGroupedByPractice>;
                }
            }
            set
            {
                Session["ProjectsGroupedByPractice"] = value;
            }
        }

        private List<ProjectsGroupedByPractice> GetGroupedPracticeManagers(List<MilestonePerson> milestonePersons)
        {
            var AllPracticesGrandTotals = new ProjectsGroupedByPractice();
            AllPracticesGrandTotals.ComputedFinancials = new ComputedFinancials();
            AllPracticesGrandTotals.ProjectedFinancialsByMonth = GetProjectedFinancials(GetMonthBegin(), GetPeriodLength());
            var GroupedPractices = new List<ProjectsGroupedByPractice>();
            var AllPractices = new List<Practice>();

            foreach (var milestonePerson in milestonePersons)
            {
                foreach (var practices in milestonePerson.PracticeList)
                {
                    AllPractices.Add(practices);
                }
            }
            IEnumerable<int> tempPracticeIdList = AllPractices.Select(p => p.Id);
            var practiceIdList = tempPracticeIdList.Distinct().ToList();
            foreach (var practiceId in practiceIdList)
            {
                var groupedPractice = new ProjectsGroupedByPractice();
                var practice = AllPractices.First(p => p.Id == practiceId);
                groupedPractice.PracticeId = practiceId;
                groupedPractice.PracticeManager = practice.PracticeOwner;
                groupedPractice.PreviousPracticeManagers = practice.PracticeManagers;
                groupedPractice.Name = practice.Name;
                FillPracticeFinancalDetails(groupedPractice, milestonePersons.FindAll(mp => mp.PracticeList.Any(p => p.Id == practiceId)), AllPracticesGrandTotals);
                GroupedPractices.Add(groupedPractice);
            }
            PracticeGrandTotals = AllPracticesGrandTotals;
            return GroupedPractices.OrderBy(gp => gp.Name).ToList();
        }

        private void FillPracticeFinancalDetails(ProjectsGroupedByPractice groupedPractice, List<MilestonePerson> milestonePersonList, ProjectsGroupedByPractice AllPracticesGrandTotals)
        {
            groupedPractice.ComputedFinancials = new ComputedFinancials();
            groupedPractice.ProjectedFinancialsByMonth = GetProjectedFinancials(GetMonthBegin(), GetPeriodLength());
            AddClientsToPractice(groupedPractice, milestonePersonList);
            foreach (var keyValPair in AllPracticesGrandTotals.ProjectedFinancialsByMonth)
            {
                var monthlyFinancial = groupedPractice.ProjectedFinancialsByMonth.Values.First(v => v.FinancialDate != null && v.FinancialDate == keyValPair.Key);
                if (monthlyFinancial != null)
                {
                    keyValPair.Value.Revenue += monthlyFinancial.Revenue;
                    keyValPair.Value.GrossMargin += monthlyFinancial.GrossMargin;
                }
            }
            AllPracticesGrandTotals.ComputedFinancials.Revenue += groupedPractice.ComputedFinancials.Revenue;
            AllPracticesGrandTotals.ComputedFinancials.GrossMargin += groupedPractice.ComputedFinancials.GrossMargin;

        }

        private void AddClientsToPractice(ProjectsGroupedByPractice groupedPractice, List<MilestonePerson> milestonePersonList)
        {
            groupedPractice.GroupedClients = new List<ProjectsGroupedByClient>();
            var tempClientIdList = milestonePersonList.FindAll(mp1 => mp1.Milestone.Project.Client != null && mp1.Milestone.Project.Client.Id.HasValue)
                                                      .Select(mp2 => mp2.Milestone.Project.Client.Id.Value);
            var clientIdList = tempClientIdList.Distinct().ToList();
            foreach (var clientId in clientIdList)
            {
                var clientMilestonePersons = milestonePersonList.FindAll(mp => mp.Milestone.Project.Client != null
                                                                             && mp.Milestone.Project.Client.Id.HasValue
                                                                             && mp.Milestone.Project.Client.Id.Value == clientId);
                var groupedClient = new ProjectsGroupedByClient();
                groupedClient.Id = clientId;

                var client = clientMilestonePersons.First().Milestone.Project.Client;

                groupedClient.Name = client.Name;

                groupedClient.ComputedFinancials = new ComputedFinancials();

                groupedClient.ProjectedFinancialsByMonth = GetProjectedFinancials(GetMonthBegin(), GetPeriodLength());

                AddClientGroupsToClient(groupedClient, clientMilestonePersons, groupedPractice.PracticeId);


                var mpsWithoutClientGroups = clientMilestonePersons.FindAll(mp => mp.Milestone.Project.Group == null ||
                                                                    !mp.Milestone.Project.Group.Id.HasValue);
                groupedClient.ProjectsWithoutClientGroup = new List<Project>();
                var tempProjectIdsWithoutClientGroups = mpsWithoutClientGroups.FindAll(mp => mp.Milestone.Project.Practice.Id == groupedPractice.PracticeId)
                                                        .Select(mp => mp.Milestone.Project.Id.Value);

                var projectIdsWithoutClientGroups = tempProjectIdsWithoutClientGroups.Distinct().ToList();

                foreach (var projectId in projectIdsWithoutClientGroups)
                {
                    var clientProject = mpsWithoutClientGroups.First(mp => mp.Milestone.Project.Id.Value == projectId).Milestone.Project;

                    var project = new Project();
                    project.Id = clientProject.Id;
                    project.Name = clientProject.Name;
                    project.Milestones = clientProject.Milestones;
                    project.Group = clientProject.Group;
                    project.ComputedFinancials = new ComputedFinancials();

                    project.ProjectedFinancialsByMonth = GetProjectedFinancials(GetMonthBegin(), GetPeriodLength());
                    groupedClient.ProjectsWithoutClientGroup.Add(project);
                }

                foreach (var project in groupedClient.ProjectsWithoutClientGroup)
                {
                    var projectMilestonePersons = mpsWithoutClientGroups.FindAll(mp => mp.Milestone.Project.Id == project.Id);

                    AddFinancialsToProject(project, projectMilestonePersons, groupedPractice.PracticeId);

                    if (DefaultProjectId.HasValue && project.Id != DefaultProjectId)
                    {
                        if (project.ComputedFinancials != null)
                        {
                            groupedClient.ComputedFinancials.Revenue += project.ComputedFinancials.Revenue;
                            groupedClient.ComputedFinancials.GrossMargin += project.ComputedFinancials.GrossMargin;
                        }

                        foreach (var keyValPair in groupedClient.ProjectedFinancialsByMonth)
                        {
                            if (project.ProjectedFinancialsByMonth.Values.Any(v => v.FinancialDate != null && v.FinancialDate == keyValPair.Key))
                            {
                                var monthlyFinancial = project.ProjectedFinancialsByMonth.Values.First(v => v.FinancialDate != null && v.FinancialDate == keyValPair.Key);
                                keyValPair.Value.Revenue += monthlyFinancial.Revenue;
                                keyValPair.Value.GrossMargin += monthlyFinancial.GrossMargin;
                            }
                        }
                    }
                }
                foreach (var keyvalpair in groupedPractice.ProjectedFinancialsByMonth)
                {
                    var monthlyfinancial = groupedClient.ProjectedFinancialsByMonth.Values.First(v => v.FinancialDate != null && v.FinancialDate == keyvalpair.Key);
                    if (monthlyfinancial != null)
                    {
                        keyvalpair.Value.Revenue += monthlyfinancial.Revenue;
                        keyvalpair.Value.GrossMargin += monthlyfinancial.GrossMargin;
                    }
                }
                groupedPractice.ComputedFinancials.Revenue += groupedClient.ComputedFinancials.Revenue;
                groupedPractice.ComputedFinancials.GrossMargin += groupedClient.ComputedFinancials.GrossMargin;
                groupedPractice.GroupedClients.Add(groupedClient);
            }
            groupedPractice.GroupedClients = groupedPractice.GroupedClients.OrderBy(c => c.Name).ToList();
        }

        private void AddClientGroupsToClient(ProjectsGroupedByClient groupedClient, List<MilestonePerson> clientMilestonePersons, int practiceId)
        {
            groupedClient.GroupedClientGroups = new List<ProjectsGroupedByClientGroup>();
            var tempClientGroupIdList = clientMilestonePersons.FindAll(mp1 => mp1.Milestone.Project.Group != null
                                                                        && mp1.Milestone.Project.Group.Id.HasValue)
                                                               .Select(mp2 => mp2.Milestone.Project.Group.Id.Value);
            var clientGroupIdList = tempClientGroupIdList.Distinct().ToList();
            foreach (var clientGroupId in clientGroupIdList)
            {
                var tempclientGroupProjectIds = clientMilestonePersons.FindAll(mp => mp.Milestone.Project.Group != null
                                                                        && mp.Milestone.Project.Group.Id.HasValue
                                                                        && mp.Milestone.Project.Group.Id.Value == clientGroupId)
                                                                        .Select(mp => mp.Milestone.Project.Id.Value);
                var clientGroupProjectIds = tempclientGroupProjectIds.Distinct().ToList();

                var groupedClientGroup = new ProjectsGroupedByClientGroup();
                groupedClientGroup.Id = clientGroupId;
                var clientGroup = clientMilestonePersons.First(mp => mp.Milestone.Project.Group != null
                                                                       && mp.Milestone.Project.Group.Id.HasValue
                                                                       && mp.Milestone.Project.Group.Id.Value == clientGroupId).Milestone.Project.Group;

                groupedClientGroup.Name = clientGroup.Name;
                groupedClientGroup.ComputedFinancials = new ComputedFinancials();
                groupedClientGroup.ProjectedFinancialsByMonth = GetProjectedFinancials(GetMonthBegin(), GetPeriodLength());
                groupedClientGroup.Projects = new List<Project>();

                foreach (var projectId in clientGroupProjectIds)
                {
                    var clientProject = clientMilestonePersons.First(mp => mp.Milestone.Project.Group != null
                                                                         && mp.Milestone.Project.Group.Id.HasValue
                                                                         && mp.Milestone.Project.Group.Id.Value == clientGroupId
                                                                         && mp.Milestone.Project.Id.Value == projectId).Milestone.Project;
                    var project = new Project();
                    project.Id = clientProject.Id;
                    project.Name = clientProject.Name;
                    project.Milestones = clientProject.Milestones;
                    project.Group = clientProject.Group;
                    project.ComputedFinancials = new ComputedFinancials();
                    project.ProjectedFinancialsByMonth = GetProjectedFinancials(GetMonthBegin(), GetPeriodLength());

                    groupedClientGroup.Projects.Add(project);
                }

                groupedClientGroup.ComputedFinancials.Revenue += 0;
                groupedClientGroup.ComputedFinancials.GrossMargin += 0;

                foreach (var project in groupedClientGroup.Projects)
                {
                    var projectMilestonePersons = clientMilestonePersons.FindAll(mp => mp.Milestone.Project.Id == project.Id);

                    AddFinancialsToProject(project, projectMilestonePersons, practiceId);

                    if (DefaultProjectId.HasValue && project.Id != DefaultProjectId)
                    {
                        if (project.ComputedFinancials != null)
                        {
                            project.ComputedFinancials.Revenue += 0;
                            project.ComputedFinancials.GrossMargin += 0;
                            groupedClientGroup.ComputedFinancials.Revenue += project.ComputedFinancials.Revenue;
                            groupedClientGroup.ComputedFinancials.GrossMargin += project.ComputedFinancials.GrossMargin;
                        }
                        foreach (var keyValPair in groupedClientGroup.ProjectedFinancialsByMonth)
                        {
                            if (project.ProjectedFinancialsByMonth.Values.Any(v => v.FinancialDate != null && v.FinancialDate == keyValPair.Key))
                            {
                                var monthlyFinancial = project.ProjectedFinancialsByMonth.Values.First(v => v.FinancialDate != null && v.FinancialDate == keyValPair.Key);
                                keyValPair.Value.Revenue += monthlyFinancial.Revenue;
                                keyValPair.Value.GrossMargin += monthlyFinancial.GrossMargin;
                            }
                        }
                    }
                }

                foreach (var keyValPair in groupedClient.ProjectedFinancialsByMonth)
                {
                    var monthlyFinancial = groupedClientGroup.ProjectedFinancialsByMonth.Values.First(v => v.FinancialDate != null && v.FinancialDate == keyValPair.Key);
                    if (monthlyFinancial != null)
                    {
                        keyValPair.Value.Revenue += monthlyFinancial.Revenue;
                        keyValPair.Value.GrossMargin += monthlyFinancial.GrossMargin;
                    }
                }
                groupedClient.ComputedFinancials.Revenue += groupedClientGroup.ComputedFinancials.Revenue;
                groupedClient.ComputedFinancials.GrossMargin += groupedClientGroup.ComputedFinancials.GrossMargin;

                groupedClient.GroupedClientGroups.Add(groupedClientGroup);
            }
            groupedClient.GroupedClientGroups = groupedClient.GroupedClientGroups.OrderBy(cg => cg.Name).ToList();
        }

        private void EnsureComputedFinancials(Practice MilestonePersonPractice)
        {
            if (MilestonePersonPractice.ComputedFinancials == null)
            {
                MilestonePersonPractice.ComputedFinancials = new ComputedFinancials();
                foreach (var financials in MilestonePersonPractice.ProjectedFinancialsByMonth.Values)
                {
                    MilestonePersonPractice.ComputedFinancials.Revenue += financials.Revenue;
                    MilestonePersonPractice.ComputedFinancials.GrossMargin += financials.GrossMargin;
                }
            }

        }

        private void AddFinancialsToProject(Project project, List<MilestonePerson> projectMilestonePersons, int practiceId)
        {
            foreach (var milestonePerson in projectMilestonePersons)
            {
                var MilestonePersonPracticelist = milestonePerson.PracticeList.FindAll(p => p.Id == practiceId);

                foreach (var milestonePersonPractice in MilestonePersonPracticelist)
                {
                    EnsureComputedFinancials(milestonePersonPractice);

                    if (milestonePersonPractice.ComputedFinancials != null)
                    {
                        project.ComputedFinancials.Revenue += milestonePersonPractice.ComputedFinancials.Revenue;
                        project.ComputedFinancials.GrossMargin += milestonePersonPractice.ComputedFinancials.GrossMargin;
                    }

                    foreach (var keyValPair in project.ProjectedFinancialsByMonth)
                    {
                        if (milestonePersonPractice.ProjectedFinancialsByMonth.Values.Any(v => v.FinancialDate != null && v.FinancialDate == keyValPair.Key))
                        {
                            var monthlyFinancial = milestonePersonPractice.ProjectedFinancialsByMonth.Values.First(v => v.FinancialDate != null && v.FinancialDate == keyValPair.Key);
                            keyValPair.Value.Revenue += monthlyFinancial.Revenue;
                            keyValPair.Value.GrossMargin += monthlyFinancial.GrossMargin;
                        }
                    }
                }
            }
        }

        private ProjectsGroupedByPerson DirectorGrandTotals
        {
            get;
            set;
        }

        private ProjectsGroupedByPerson AccountManagerGrandTotals
        {
            get;
            set;
        }

        private ProjectsGroupedByPractice PracticeGrandTotals
        {
            get
            {
                return Session[GroupedByPracticeGrandTotalKey] as ProjectsGroupedByPractice;
            }
            set
            {
                Session[GroupedByPracticeGrandTotalKey] = value;
            }
        }

        private string PrevDirectorReportSortExpression
        {
            get { return ViewState[STRDirectorReportSortExpression] as string ?? string.Empty; }
            set { ViewState[STRDirectorReportSortExpression] = value; }
        }

        private string DirectorReportSortDirection
        {
            get { return ViewState[STRDirectorReportSortDirection] as string ?? "Ascending"; }
            set { ViewState[STRDirectorReportSortDirection] = value; }
        }

        private int DirectorReportSortColumnId
        {
            get { return ViewState[STRDirectorReportSortColumnId] != null ? (int)ViewState[STRDirectorReportSortColumnId] : 0; }
            set { ViewState[STRDirectorReportSortColumnId] = value; }
        }

        private string PrevAccountManagerSortExpression
        {
            get { return ViewState[STRAMReportSortExpression] as string ?? string.Empty; }
            set { ViewState[STRAMReportSortExpression] = value; }
        }

        private string AccountManagerSortDirection
        {
            get { return ViewState[STRAMReportSortDirection] as string ?? "Ascending"; }
            set { ViewState[STRAMReportSortDirection] = value; }
        }

        private int AccountManagerReportSortColumnId
        {
            get { return ViewState[STRAMReportSortColumnId] != null ? (int)ViewState[STRAMReportSortColumnId] : 0; }
            set { ViewState[STRAMReportSortColumnId] = value; }
        }

        private string PrevPMReportSortExpression
        {
            get { return ViewState[STRPMReportSortExpression] as string ?? string.Empty; }
            set { ViewState[STRPMReportSortExpression] = value; }
        }

        private string PMReportSortDirection
        {
            get { return ViewState[STRPMReportSortDirection] as string ?? "Ascending"; }
            set { ViewState[STRPMReportSortDirection] = value; }
        }

        private int PMReportSortColumnId
        {
            get { return ViewState[STRPMReportSortColumnId] != null ? (int)ViewState[STRPMReportSortColumnId] : 0; }
            set { ViewState[STRPMReportSortColumnId] = value; }
        }

        private string SelectedClientIds
        {
            get
            {
                return cblClient.SelectedItems;
            }
            set
            {
                cblClient.SelectedItems = value;
            }
        }

        private string SelectedSalespersonIds
        {
            get
            {
                return cblSalesperson.SelectedItems;
            }
            set
            {
                cblSalesperson.SelectedItems = value;
            }
        }

        private string SelectedPracticeIds
        {
            get
            {
                return cblPractice.SelectedItems;
            }
            set
            {
                cblPractice.SelectedItems = value;
            }
        }

        private string SelectedGroupIds
        {
            get
            {
                return cblProjectGroup.SelectedItems;
            }
            set
            {
                cblProjectGroup.SelectedItems = value;
            }
        }

        private string SelectedProjectOwnerIds
        {
            get
            {
                return cblProjectOwner.SelectedItems;
            }
            set
            {
                cblProjectOwner.SelectedItems = value;
            }
        }
        #endregion

        private CompanyPerformanceFilterSettings GetFilterSettings()
        {
            var filter =
                 new CompanyPerformanceFilterSettings
                 {
                     StartYear = this.mpFromControl.SelectedYear,
                     StartMonth = this.mpFromControl.SelectedMonth,
                     EndYear = this.mpToControl.SelectedYear,
                     EndMonth = this.mpToControl.SelectedMonth,
                     ClientIdsList = SelectedClientIds,
                     ProjectOwnerIdsList = SelectedProjectOwnerIds,
                     PracticeIdsList = SelectedPracticeIds,
                     SalespersonIdsList = SelectedSalespersonIds,
                     ProjectGroupIdsList = SelectedGroupIds,
                     ShowActive = chbActive.Checked,
                     ShowCompleted = chbCompleted.Checked,
                     ShowProjected = chbProjected.Checked,
                     ShowInternal = chbInternal.Checked,
                     ShowExperimental = chbExperimental.Checked,
                     ShowInactive = chbInactive.Checked,
                     ExcludeInternalPractices = chbExcludeInternalPractices.Checked,
                     TotalOnlySelectedDateWindow = true,
                     HideAdvancedFilter = false,
                     IsGroupByPersonPage = true
                 };
            return filter;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GroupedPractices = null;
            }
            var person = DataHelper.CurrentPerson;
            if (person == null || Seniority.GetSeniorityValueById(person.Seniority.Id) > 35)
            {
                Response.Redirect(@"~\GuestPages\AccessDenied.aspx");
            }
            if (hdnFiltersChanged.Value == "false")
            {
                btnResetFilter.Attributes.Add("disabled", "true");
            }
            else
            {
                btnResetFilter.Attributes.Remove("disabled");
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            SaveFilterSettings();
        }

        /// <summary>
        /// Executes preliminary operations to the view be ready to display the data.
        /// </summary>
        private void PreparePeriodView()
        {
            if (!IsPostBack)
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), GroupByPersonFilterKey, "Delete_Cookie('" + GroupByPersonFilterKey + "', '/', '')", true);
                HttpContext.Current.Request.Cookies.Remove(GroupByPersonFilterKey);
                var filter = InitFilter();

                //  If current user is administrator, don't apply restrictions
                var person =
                    Roles.IsUserInRole(
                        DataHelper.CurrentPerson.Alias,
                        DataTransferObjects.Constants.RoleNames.AdministratorRoleName)
                    ? null : DataHelper.CurrentPerson;

                // If person is not administrator, return list of values when [All] is selected
                //      this is needed because we apply restrictions and don't want
                //      NULL to be returned, because that would mean all and restrictions
                //      are not going to be applied
                if (person != null)
                {
                    cblSalesperson.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                    cblProjectOwner.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                    cblClient.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                    cblProjectGroup.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                }

                PraticeManagement.Controls.DataHelper.FillSalespersonList(
                    person, cblSalesperson,
                    Resources.Controls.AllSalespersonsText,
                    false);

                PraticeManagement.Controls.DataHelper.FillProjectOwnerList(cblProjectOwner,
                    Resources.Controls.AllPracticeMgrsText,
                    null,
                    true,
                    person);


                PraticeManagement.Controls.DataHelper.FillClientsAndGroups(
                    cblClient, cblProjectGroup);

                // Set the default viewable interval.

                DataHelper.FillPracticeList(this.cblPractice, Resources.Controls.AllPracticesText);
                //logic to display from and to dates as current month - 2 and current month + 2 respectively.
                var FromSelectedmonth = DateTime.Now.Month - 3;
                var ToSelectedmonth = DateTime.Now.Month + 2;
                mpFromControl.SelectedYear = FromSelectedmonth < 1 ? DateTime.Now.Year - 1 : DateTime.Now.Year;
                mpFromControl.SelectedMonth = FromSelectedmonth < 1 ? 12 + FromSelectedmonth : FromSelectedmonth;

                mpToControl.SelectedYear = ToSelectedmonth > 12 ? DateTime.Now.Year + 1 : DateTime.Now.Year;
                mpToControl.SelectedMonth = ToSelectedmonth > 12 ? ToSelectedmonth - 12 : ToSelectedmonth;
                UpdateToDate();

                var thisMonth = DateTime.Today;
                thisMonth = new DateTime(thisMonth.Year, thisMonth.Month, Constants.Dates.FirstDay);

                // Set the default viewable interval.
                filter.StartYear = mpFromControl.SelectedYear;
                filter.StartMonth = mpFromControl.SelectedMonth;

                var periodEnd = thisMonth.AddMonths(Constants.Dates.DefaultViewableMonths);
                filter.EndYear = mpToControl.SelectedYear;
                filter.EndMonth = mpToControl.SelectedMonth;
                chbExcludeInternalPractices.Checked = filter.ExcludeInternalPractices;
                chbActive.Checked = filter.ShowActive;
                chbCompleted.Checked = filter.ShowCompleted;
                chbExperimental.Checked = filter.ShowExperimental;
                chbProjected.Checked = filter.ShowProjected;
                chbInternal.Checked = filter.ShowInternal;
                chbInactive.Checked = filter.ShowInactive;

                SelectedClientIds = filter.ClientIdsList;
                SelectedPracticeIds = filter.PracticeIdsList;
                SelectedProjectOwnerIds = filter.ProjectOwnerIdsList;
                SelectedSalespersonIds = filter.SalespersonIdsList;
                SelectedGroupIds = filter.ProjectGroupIdsList;
            }
            AddAttributesToCheckBoxes(this.cblPractice);
            AddAttributesToCheckBoxes(this.cblSalesperson);
            AddAttributesToCheckBoxes(this.cblProjectOwner);
            AddAttributesToCheckBoxes(this.cblProjectGroup);
            AddAttributesToCheckBoxes(this.cblClient);
        }

        private static CompanyPerformanceFilterSettings InitFilter()
        {
            return SerializationHelper.DeserializeCookie(GroupByPersonFilterKey) as CompanyPerformanceFilterSettings ??
                   new CompanyPerformanceFilterSettings();
        }

        /// <summary>
        /// Stores a current filter set.
        /// </summary>
        private void SaveFilterSettings()
        {
            CompanyPerformanceFilterSettings filter = GetFilterSettings();
            SerializationHelper.SerializeCookie(filter, GroupByPersonFilterKey);
        }

        private void AddAttributesToCheckBoxes(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Attributes.Add("onclick", "EnableResetButton();");
            }
        }

        private DateTime GetMonthBegin()
        {
            return new DateTime(mpFromControl.SelectedYear,
                    mpFromControl.SelectedMonth,
                    Constants.Dates.FirstDay);
        }

        private static DateTime GetMonthEnd(ref DateTime monthBegin)
        {
            return new DateTime(monthBegin.Year,
                    monthBegin.Month,
                    DateTime.DaysInMonth(monthBegin.Year, monthBegin.Month));
        }

        private int GetPeriodLength()
        {
            int mounthsInPeriod =
            (mpToControl.SelectedYear - mpFromControl.SelectedYear) * Constants.Dates.LastMonth +
            (mpToControl.SelectedMonth - mpFromControl.SelectedMonth + 1);

            return mounthsInPeriod;
        }

        private List<ProjectsGroupedByPerson> GetGroupedDirectors(List<Project> ProjectList)
        {
            EnsureComputedFinancials(ProjectList);
            var AllPersonsGrandTotals = new ProjectsGroupedByPerson();
            AllPersonsGrandTotals.ComputedFinancials = new ComputedFinancials();
            AllPersonsGrandTotals.ProjectedFinancialsByMonth = GetProjectedFinancials(GetMonthBegin(), GetPeriodLength());
            var GroupedDirectors = new List<ProjectsGroupedByPerson>();
            IEnumerable<int> tempDirectorIdList =
                    ProjectList.FindAll(p => p.Director != null && p.Director.Id.HasValue).Select(q => q.Director.Id.Value);

            var personIdList = tempDirectorIdList.Distinct().ToList();

            foreach (var personId in personIdList)
            {
                List<Project> DirectorProjects = ProjectList.FindAll(p => p.Director != null && p.Director.Id.HasValue
                                                                      && p.Director.Id.Value == personId);
                var groupedPerson = new ProjectsGroupedByPerson();
                groupedPerson.PersonId = personId;
                var director = DirectorProjects[0].Director;
                groupedPerson.FirstName = director.FirstName;
                groupedPerson.LastName = director.LastName;
                CreatePersonAndFillDetails(DirectorProjects, groupedPerson, AllPersonsGrandTotals);
                GroupedDirectors.Add(groupedPerson);
            }
            //var IncludeProjectsWithoutDirectorItem = false;

            if (IsProjectsWithoutDirectorItemRequired())
            {
                List<Project> ProjectsWithoutDirector = ProjectList.FindAll(p => p.Director == null || !p.Director.Id.HasValue);
                if (ProjectsWithoutDirector.Any())
                {
                    var groupedDirector = new ProjectsGroupedByPerson();
                    groupedDirector.PersonId = -1;
                    groupedDirector.LastName = ProjectsWithoutDirectorText;
                    groupedDirector.FirstName = string.Empty;
                    CreatePersonAndFillDetails(ProjectsWithoutDirector, groupedDirector, AllPersonsGrandTotals);
                    GroupedDirectors.Add(groupedDirector);
                }
            }
            DirectorGrandTotals = AllPersonsGrandTotals;
            return GroupedDirectors;
        }

        private bool IsProjectsWithoutDirectorItemRequired()
        {
            bool IncludeProjectsWithoutDirectorItem = false;
            try
            {
                if (WCFClientUtility.IsWebAzureRole())
                {
                    IncludeProjectsWithoutDirectorItem = bool.Parse(RoleEnvironment.GetConfigurationSettingValue(IncludeProjectsWithoutDirectorItemKey));
                }
                else
                {
                    IncludeProjectsWithoutDirectorItem = bool.Parse(ConfigurationManager.AppSettings[IncludeProjectsWithoutDirectorItemKey]);
                }
            }
            catch
            {
                IncludeProjectsWithoutDirectorItem = false;
            }
            return IncludeProjectsWithoutDirectorItem;
        }

        private List<ProjectsGroupedByPerson> GetGroupedAccountManagers(List<Project> ProjectList)
        {
            EnsureComputedFinancials(ProjectList);
            var AllPersonsGrandTotals = new ProjectsGroupedByPerson();
            AllPersonsGrandTotals.ComputedFinancials = new ComputedFinancials();
            AllPersonsGrandTotals.ProjectedFinancialsByMonth = GetProjectedFinancials(GetMonthBegin(), GetPeriodLength());
            var GroupedAMs = new List<ProjectsGroupedByPerson>();
            IEnumerable<int> tempAccountManagerIdList = ProjectList.FindAll(p => p.SalesCommission != null
                                                                            && p.SalesCommission.Any(c => c.PersonId.HasValue && c.TypeOfCommission == CommissionType.Sales))
                                                                   .Select(q => q.SalesCommission.First(c => c.PersonId.HasValue && c.TypeOfCommission == CommissionType.Sales).PersonId.Value);

            var AccountManagerIdList = tempAccountManagerIdList.Distinct().ToList();

            foreach (var personId in AccountManagerIdList)
            {
                List<Project> personProjects;

                personProjects = ProjectList.FindAll(p => p.SalesCommission != null
                                                    && p.SalesCommission.Any(c => c.PersonId.HasValue
                                                                            && c.PersonId.Value == personId
                                                                            && c.TypeOfCommission == CommissionType.Sales)
                                                    );
                var groupedPerson = new ProjectsGroupedByPerson();
                groupedPerson.PersonId = personId;
                var salesCommission = personProjects.First().SalesCommission.First();
                groupedPerson.FirstName = salesCommission.PersonFirstName;
                groupedPerson.LastName = salesCommission.PersonLastName;
                CreatePersonAndFillDetails(personProjects, groupedPerson, AllPersonsGrandTotals);
                GroupedAMs.Add(groupedPerson);
            }
            AccountManagerGrandTotals = AllPersonsGrandTotals;
            return GroupedAMs;
        }

        private void EnsureComputedFinancials(List<Project> ProjectList)
        {
            foreach (var project in ProjectList)
            {
                if (project.ComputedFinancials == null)
                {
                    project.ComputedFinancials = new ComputedFinancials();
                    foreach (var financial in project.ProjectedFinancialsByMonth)
                    {
                        project.ComputedFinancials.Revenue += financial.Value.Revenue;
                        project.ComputedFinancials.GrossMargin += financial.Value.GrossMargin;
                    }
                }
            }
        }

        private ProjectsGroupedByPerson CreatePersonAndFillDetails(List<Project> personProjects, ProjectsGroupedByPerson groupedPerson, ProjectsGroupedByPerson AllPersonsGrandTotals)
        {
            groupedPerson.ComputedFinancials = new ComputedFinancials();
            groupedPerson.ProjectedFinancialsByMonth = GetProjectedFinancials(GetMonthBegin(), GetPeriodLength());
            AddClientsToPerson(groupedPerson, personProjects);
            foreach (var keyValPair in AllPersonsGrandTotals.ProjectedFinancialsByMonth)
            {
                var monthlyFinancial = groupedPerson.ProjectedFinancialsByMonth.Values.First(v => v.FinancialDate != null && v.FinancialDate == keyValPair.Key);
                if (monthlyFinancial != null)
                {
                    keyValPair.Value.Revenue += monthlyFinancial.Revenue;
                    keyValPair.Value.GrossMargin += monthlyFinancial.GrossMargin;
                }
            }

            AllPersonsGrandTotals.ComputedFinancials.Revenue += groupedPerson.ComputedFinancials.Revenue;
            AllPersonsGrandTotals.ComputedFinancials.GrossMargin += groupedPerson.ComputedFinancials.GrossMargin;

            return groupedPerson;
        }

        private void AddClientsToPerson(ProjectsGroupedByPerson groupedPerson, List<Project> personProjects)
        {
            groupedPerson.GroupedClients = new List<ProjectsGroupedByClient>();
            var tempClientIdList = personProjects.FindAll(p => p.Client != null && p.Client.Id.HasValue)
                                                 .Select(q => q.Client.Id.Value);
            var clientIdList = tempClientIdList.Distinct().ToList();
            foreach (var clientId in clientIdList)
            {
                var clientProjects = personProjects.FindAll(p => p.Client != null
                                                            && p.Client.Id.HasValue
                                                            && p.Client.Id.Value == clientId);
                var groupedClient = new ProjectsGroupedByClient();
                groupedClient.Id = clientId;
                var client = clientProjects.First().Client;
                groupedClient.Name = client.Name;
                groupedClient.ComputedFinancials = new ComputedFinancials();
                groupedClient.ProjectedFinancialsByMonth = GetProjectedFinancials(GetMonthBegin(), GetPeriodLength());
                AddClientGroupsToClient(groupedClient, clientProjects);
                groupedClient.ProjectsWithoutClientGroup = clientProjects.FindAll(p => p.Group == null || !p.Group.Id.HasValue)
                                                                        .OrderBy(q => q.Name).ToList();
                foreach (var project in groupedClient.ProjectsWithoutClientGroup)
                {
                    if (DefaultProjectId.HasValue && project.Id != DefaultProjectId)
                    {
                        if (project.ComputedFinancials != null)
                        {
                            groupedClient.ComputedFinancials.Revenue += project.ComputedFinancials.Revenue;
                            groupedClient.ComputedFinancials.GrossMargin += project.ComputedFinancials.GrossMargin;
                        }

                        foreach (var keyValPair in groupedClient.ProjectedFinancialsByMonth)
                        {
                            if (project.ProjectedFinancialsByMonth.Values.Any(v => v.FinancialDate != null && v.FinancialDate == keyValPair.Key))
                            {
                                var monthlyFinancial = project.ProjectedFinancialsByMonth.Values.First(v => v.FinancialDate != null && v.FinancialDate == keyValPair.Key);
                                keyValPair.Value.Revenue += monthlyFinancial.Revenue;
                                keyValPair.Value.GrossMargin += monthlyFinancial.GrossMargin;
                            }
                        }
                    }
                }

                foreach (var keyvalpair in groupedPerson.ProjectedFinancialsByMonth)
                {
                    var monthlyfinancial = groupedClient.ProjectedFinancialsByMonth.Values.First(v => v.FinancialDate != null && v.FinancialDate == keyvalpair.Key);
                    if (monthlyfinancial != null)
                    {
                        keyvalpair.Value.Revenue += monthlyfinancial.Revenue;
                        keyvalpair.Value.GrossMargin += monthlyfinancial.GrossMargin;
                    }
                }

                groupedPerson.ComputedFinancials.Revenue += groupedClient.ComputedFinancials.Revenue;
                groupedPerson.ComputedFinancials.GrossMargin += groupedClient.ComputedFinancials.GrossMargin;
                groupedPerson.GroupedClients.Add(groupedClient);
            }
            groupedPerson.GroupedClients = groupedPerson.GroupedClients.OrderBy(c => c.Name).ToList();
        }

        private void AddClientGroupsToClient(ProjectsGroupedByClient groupedClient, List<Project> clientProjects)
        {
            groupedClient.GroupedClientGroups = new List<ProjectsGroupedByClientGroup>();
            var tempClientGroupIdList = clientProjects.FindAll(p => p.Group != null && p.Group.Id.HasValue)
                                                 .Select(q => q.Group.Id.Value);
            var clientGroupIdList = tempClientGroupIdList.Distinct().ToList();

            foreach (var clientGroupId in clientGroupIdList)
            {
                var clientGroupProjects = clientProjects.FindAll(p => p.Group != null && p.Group.Id.HasValue
                                                            && p.Group.Id.Value == clientGroupId);
                var groupedClientGroup = new ProjectsGroupedByClientGroup();
                groupedClientGroup.Id = clientGroupId;
                var clientGroup = clientGroupProjects.First().Group;
                groupedClientGroup.Name = clientGroup.Name;
                groupedClientGroup.ComputedFinancials = new ComputedFinancials();
                groupedClientGroup.ProjectedFinancialsByMonth = GetProjectedFinancials(GetMonthBegin(), GetPeriodLength());
                groupedClientGroup.Projects = clientGroupProjects.OrderBy(p => p.Name).ToList();
                groupedClientGroup.ComputedFinancials.Revenue += 0;
                groupedClientGroup.ComputedFinancials.GrossMargin += 0;
                foreach (var project in clientGroupProjects)
                {
                    if (DefaultProjectId.HasValue && project.Id != DefaultProjectId)
                    {
                        if (project.ComputedFinancials != null)
                        {
                            project.ComputedFinancials.Revenue += 0;
                            project.ComputedFinancials.GrossMargin += 0;
                            groupedClientGroup.ComputedFinancials.Revenue += project.ComputedFinancials.Revenue;
                            groupedClientGroup.ComputedFinancials.GrossMargin += project.ComputedFinancials.GrossMargin;
                        }

                        foreach (var keyValPair in groupedClientGroup.ProjectedFinancialsByMonth)
                        {
                            if (project.ProjectedFinancialsByMonth.Values.Any(v => v.FinancialDate != null && v.FinancialDate == keyValPair.Key))
                            {
                                var monthlyFinancial = project.ProjectedFinancialsByMonth.Values.First(v => v.FinancialDate != null && v.FinancialDate == keyValPair.Key);
                                keyValPair.Value.Revenue += monthlyFinancial.Revenue;
                                keyValPair.Value.GrossMargin += monthlyFinancial.GrossMargin;
                            }
                        }
                    }
                }

                foreach (var keyValPair in groupedClient.ProjectedFinancialsByMonth)
                {
                    var monthlyFinancial = groupedClientGroup.ProjectedFinancialsByMonth.Values.First(v => v.FinancialDate != null && v.FinancialDate == keyValPair.Key);
                    if (monthlyFinancial != null)
                    {
                        keyValPair.Value.Revenue += monthlyFinancial.Revenue;
                        keyValPair.Value.GrossMargin += monthlyFinancial.GrossMargin;
                    }
                }
                groupedClient.ComputedFinancials.Revenue += groupedClientGroup.ComputedFinancials.Revenue;
                groupedClient.ComputedFinancials.GrossMargin += groupedClientGroup.ComputedFinancials.GrossMargin;

                groupedClient.GroupedClientGroups.Add(groupedClientGroup);
            }
            groupedClient.GroupedClientGroups = groupedClient.GroupedClientGroups.OrderBy(cg => cg.Name).ToList();
        }

        private Dictionary<DateTime, ComputedFinancials> GetProjectedFinancials(DateTime monthBegin, int periodLength)
        {
            var projectedFinancials = new Dictionary<DateTime, ComputedFinancials>();

            for (int k = 0; k < periodLength; k++, monthBegin = monthBegin.AddMonths(1))
            {
                var computedFinancials = new ComputedFinancials();
                computedFinancials.FinancialDate = monthBegin;
                computedFinancials.Revenue += 0;
                computedFinancials.GrossMargin += 0;
                projectedFinancials.Add(monthBegin, computedFinancials);
            }
            return projectedFinancials;
        }

        protected override void Display()
        {
            // Clean up the cached values
            CompanyPerformanceState.Clear();
            PreparePeriodView();
        }

        protected void mpFromControl_OnSelectedValueChanged(object sender, EventArgs e)
        {
            UpdateToDate();
        }

        protected void mpToControl_OnSelectedValueChanged(object sender, EventArgs e)
        {
            var selectedYear = mpToControl.SelectedYear;
            var selectedMonth = mpToControl.SelectedMonth;
            UpdateToDate();
            if (mpFromControl.SelectedYear == selectedYear && mpFromControl.SelectedMonth > selectedMonth)
            {
                mpToControl.SelectedYear = mpFromControl.SelectedYear;
                mpToControl.SelectedMonth = mpFromControl.SelectedMonth;
            }
        }

        private void UpdateToDate()
        {
            DropDownList monthToControl = mpToControl.FindControl("ddlMonth") as DropDownList;
            DropDownList yearToControl = mpToControl.FindControl("ddlYear") as DropDownList;

            //remove all the year items less than mpFromControl.SelectedYear in mpToControl.
            RemoveToControls(mpFromControl.SelectedYear, yearToControl);

            if (mpFromControl.SelectedYear >= mpToControl.SelectedYear)
            {
                //remove all the month items less than mpFromControl.SelectedMonth in mpToControl.
                RemoveToControls(mpFromControl.SelectedMonth, monthToControl);

                if (mpFromControl.SelectedYear > mpToControl.SelectedYear ||
                    mpFromControl.SelectedMonth > mpToControl.SelectedMonth)
                {
                    mpToControl.SelectedYear = mpFromControl.SelectedYear;
                    mpToControl.SelectedMonth = mpFromControl.SelectedMonth;
                }
            }
            else
            {
                RemoveToControls(0, monthToControl);
            }
        }

        private void RemoveToControls(int FromSelectedValue, DropDownList yearToControl)
        {
            foreach (ListItem toYearItem in yearToControl.Items)
            {
                var toYearItemInt = Convert.ToInt32(toYearItem.Value);
                if (toYearItemInt < FromSelectedValue)
                {
                    toYearItem.Enabled = false;
                }
                else
                {
                    toYearItem.Enabled = true;
                }
            }
        }

        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {
            lblDirectorEmptyMessage.Visible = false;
            lblPracticeEmptyMessage.Visible = false;
            lblAccountManagerEmptyMessage.Visible = false;
            if (chkShowDirector.Checked && chkShowPractice.Checked)
            {
                hrDirectorAndPracticeSeperator.Visible = true;
            }
            else
            {
                hrDirectorAndPracticeSeperator.Visible = false;
            }
            if ((chkShowDirector.Checked || chkShowPractice.Checked) && chkShowAccountManager.Checked)
            {
                hrPracticeAndACMgrSeperator.Visible = true;
            }
            else
            {
                hrPracticeAndACMgrSeperator.Visible = false;
            }
            if (chkShowDirector.Checked)
            {
                DirectorReportSortDirection = "Ascending";
                PrevDirectorReportSortExpression = "0";
                if (ProjectList.Any())
                {
                    lvGroupByDirector.DataSource = SortList(GetGroupedDirectors(ProjectList.ToList()), PrevDirectorReportSortExpression, DirectorReportSortDirection);

                    lvGroupByDirector.DataBind();
                    lvGroupByDirector.Visible = true;
                    lblDirectorEmptyMessage.Visible = false;
                }
                else
                {
                    lblDirectorEmptyMessage.Visible = true;
                    lvGroupByDirector.Visible = false;
                }
            }
            else
            {
                lvGroupByDirector.Visible = false;
            }
            if (chkShowPractice.Checked)
            {
                PMReportSortDirection = "Ascending";
                PrevPMReportSortExpression = "0";
                if (hdnFiltersChangedSinceLastUpdate.Value == "true")
                {
                    GroupedPractices = null;
                    hdnFiltersChangedSinceLastUpdate.Value = "false";
                }
                if (GroupedPractices.Any())
                {

                    lvGroupByPractice.DataSource = GroupedPractices;
                    lvGroupByPractice.DataBind();
                    lvGroupByPractice.Visible = true;
                    lblPracticeEmptyMessage.Visible = false;
                }
                else
                {
                    lblPracticeEmptyMessage.Visible = true;
                    lvGroupByPractice.Visible = false;
                }
            }
            else
            {
                lvGroupByPractice.Visible = false;
            }
            if (chkShowAccountManager.Checked)
            {
                AccountManagerSortDirection = "Ascending";
                PrevAccountManagerSortExpression = "0";
                if (ProjectList.Any())
                {
                    lvGroupByAccountManager.DataSource = SortList(GetGroupedAccountManagers(ProjectList.ToList()), PrevAccountManagerSortExpression, AccountManagerSortDirection);
                    lvGroupByAccountManager.DataBind();
                    lvGroupByAccountManager.Visible = true;
                    lblAccountManagerEmptyMessage.Visible = false;
                }
                else
                {
                    lblAccountManagerEmptyMessage.Visible = true;
                    lvGroupByAccountManager.Visible = false;
                }
            }
            else
            {
                lvGroupByAccountManager.Visible = false;
            }
            updGroupByDirector.Update();
            updGroupByPractice.Update();
            updGroupByAccountManager.Update();
        }

        protected void lvGroupByPerson_OnDataBinding(object sender, EventArgs e)
        {
            var lvGroupByPerson = sender as ListView;
            var periodStart = GetMonthBegin();
            var monthsInPeriod = GetPeriodLength();
            var row = lvGroupByPerson.FindControl("lvHeader") as System.Web.UI.HtmlControls.HtmlTableRow;
            AddMonthColumn(row, periodStart, monthsInPeriod, NumberOfFixedColumns);
            row.Cells[row.Cells.Count - 1].InnerHtml = " <div class='ie-bg' style='padding-right:3px;'>Grand Total</div>";
        }

        private void AddMonthColumn(HtmlTableRow row, DateTime periodStart, int monthsInPeriod, int insertPosition)
        {
            if (row != null)
            {
                while (row.Cells.Count > NumberOfFixedColumns + 1)
                {
                    row.Cells.RemoveAt(NumberOfFixedColumns);
                }

                row.Cells[row.Cells.Count - 1].Attributes["Style"] = "";

                for (int i = insertPosition, k = 0; k < monthsInPeriod; i++, k++)
                {
                    var newColumn = new HtmlTableCell("td");
                    row.Cells.Insert(i, newColumn);

                    row.Cells[i].InnerHtml = "<div class='ie-bg no-wrap'>" + periodStart.ToString(Constants.Formatting.MonthYearFormat) + "</div>";
                    row.Cells[i].Attributes["class"] = "MonthSummary ie-bg";

                    if (periodStart.Month == DateTime.Now.Month && periodStart.Year == DateTime.Now.Year)
                    {
                        row.Cells[i].Attributes["Style"] = "border-left:1px solid black;border-right:1px solid black;border-bottom:1px solid black;";
                    }

                    periodStart = periodStart.AddMonths(1);
                }
            }
        }

        protected void lvGroupByPerson_OnItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var groupedDir = (e.Item as ListViewDataItem).DataItem as ProjectsGroupedByPerson;
                var row = e.Item.FindControl("testTr") as HtmlTableRow;
                var attributes = string.Format(PersonAttributeTemplate, groupedDir.PersonId);
                row.Attributes.Add("Person", groupedDir.PersonId.ToString());
                row.Cells[0].InnerHtml = string.Format(CollpseExpandCellTemplate, attributes,
                                                        string.Empty, groupedDir.LastName +
                                                        (!string.IsNullOrEmpty(groupedDir.FirstName) ? ", " + groupedDir.FirstName : string.Empty),
                                                        string.Empty);

                row.Cells[NumberOfFixedColumns - 1].InnerHtml += getMonthCellsHTML(groupedDir.ProjectedFinancialsByMonth);
                row.Cells[row.Cells.Count - 1].InnerHtml = string.Format(GrandTotalCellTemplate, groupedDir.ComputedFinancials.Revenue, groupedDir.ComputedFinancials.GrossMargin);
                row.Cells[row.Cells.Count - 1].InnerHtml += GetClientRowsHtml(groupedDir.GroupedClients, attributes);
            }
        }

        protected void lvGroupByPractice_OnItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var groupedPractice = (e.Item as ListViewDataItem).DataItem as ProjectsGroupedByPractice;
                var row = e.Item.FindControl("testTr") as HtmlTableRow;
                var attributes = string.Format(PersonAttributeTemplate, groupedPractice.PracticeId);

                row.Attributes.Add("Person", groupedPractice.PracticeId.ToString());
                var PracticeManagersName = GetPracticeManagersName(groupedPractice.PreviousPracticeManagers);
                var CurrentPracticeManagerName = GetCurrentPracticeManagerName(groupedPractice.PracticeManager);
                row.Cells[0].InnerHtml = string.Format(CollpseExpandCellTemplate, attributes,
                                                    string.Empty, groupedPractice.Name + "<br/>(" + PracticeManagersName
                                                    + CurrentPracticeManagerName + ")",
                                                    string.Empty);

                row.Cells[NumberOfFixedColumns - 1].InnerHtml += getMonthCellsHTML(groupedPractice.ProjectedFinancialsByMonth);
                row.Cells[row.Cells.Count - 1].InnerHtml = string.Format(GrandTotalCellTemplate, groupedPractice.ComputedFinancials.Revenue, groupedPractice.ComputedFinancials.GrossMargin);
                row.Cells[row.Cells.Count - 1].InnerHtml += GetClientRowsHtml(groupedPractice.GroupedClients, attributes);
            }
        }

        private string GetCurrentPracticeManagerName(Person person)
        {
            return person.LastName + (!string.IsNullOrEmpty(person.FirstName) ? " " + person.FirstName : string.Empty) + "*";
        }

        private string GetPracticeManagersName(List<PracticeManagerHistory> PracticeManagers)
        {
            var personIds = PracticeManagers.Select(p => p.PracticeManagerId).Distinct();
            var sb = new StringBuilder(string.Empty);
            foreach (var personId in personIds)
            {
                var practicemanager = PracticeManagers.First(p => p.PracticeManagerId == personId);
                sb.Append(practicemanager.PracticeManagerLastName +
                    (!string.IsNullOrEmpty(practicemanager.PracticeManagerFirstName) ? " " + practicemanager.PracticeManagerFirstName : string.Empty)
                    + ",<br/>");

            }
            return sb.ToString();
        }

        protected void lvGroupByPerson_OnPreRender(object sender, EventArgs e)
        {
            var lvGroupByPerson = sender as ListView;
            var grandtotals = lvGroupByPerson.ID == "lvGroupByDirector" ? DirectorGrandTotals : AccountManagerGrandTotals;
            if (lvGroupByPerson.Items.Any() && grandtotals != null)
            {
                var lastItem = lvGroupByPerson.Items.Last();
                var row = lastItem.FindControl("testTr") as HtmlTableRow;
                row.Cells[row.Cells.Count - 1].InnerHtml += string.Format(GrandTotalRowHTMLTemplate,
                    getMonthCellsHTML(grandtotals.ProjectedFinancialsByMonth, true),
                    grandtotals.ComputedFinancials.Revenue, grandtotals.ComputedFinancials.GrossMargin);
            }
        }

        protected void lvGroupByPractice_OnPreRender(object sender, EventArgs e)
        {
            var lvGroupByPerson = sender as ListView;
            //var grandtotals = lvGroupByPerson.ID == "lvGroupByDirector" ? DirectorGrandTotals : PracticeManagerGrandTotals;
            var grandtotals = PracticeGrandTotals;
            if (lvGroupByPerson.Items.Any() && grandtotals != null)
            {
                var lastItem = lvGroupByPerson.Items.Last();
                var row = lastItem.FindControl("testTr") as HtmlTableRow;
                row.Cells[row.Cells.Count - 1].InnerHtml += string.Format(GrandTotalRowHTMLTemplate,
                    getMonthCellsHTML(grandtotals.ProjectedFinancialsByMonth, true),
                    grandtotals.ComputedFinancials.Revenue, grandtotals.ComputedFinancials.GrossMargin);
            }
        }

        protected void PersonListView_OnSorting(object sender, ListViewSortEventArgs e)
        {
            var PersonListView = sender as ListView;

            if (PersonListView.ID == "lvGroupByDirector")
            {
                if (PrevDirectorReportSortExpression != e.SortExpression)
                {
                    PrevDirectorReportSortExpression = e.SortExpression;
                    DirectorReportSortDirection = e.SortDirection.ToString();
                }
                else
                {
                    DirectorReportSortDirection = GetSortDirection(DirectorReportSortDirection);
                }
                DirectorReportSortColumnId = GetSortColumnId(e.SortExpression);

                lvGroupByDirector.DataSource = SortList(GetGroupedDirectors(ProjectList.ToList()), PrevDirectorReportSortExpression, DirectorReportSortDirection);
                lvGroupByDirector.DataBind();
            }
            else if (PersonListView.ID == "lvGroupByAccountManager")
            {
                if (PrevAccountManagerSortExpression != e.SortExpression)
                {
                    PrevAccountManagerSortExpression = e.SortExpression;
                    AccountManagerSortDirection = e.SortDirection.ToString();
                }
                else
                {
                    AccountManagerSortDirection = GetSortDirection(AccountManagerSortDirection);
                }
                AccountManagerReportSortColumnId = GetSortColumnId(e.SortExpression);

                lvGroupByAccountManager.DataSource = SortList(GetGroupedAccountManagers(ProjectList.ToList()), PrevAccountManagerSortExpression, AccountManagerSortDirection);
                lvGroupByAccountManager.DataBind();
            }
            else if (PersonListView.ID == "lvGroupByPractice")
            {
                if (PrevPMReportSortExpression != e.SortExpression)
                {
                    PrevPMReportSortExpression = e.SortExpression;
                    PMReportSortDirection = e.SortDirection.ToString();
                }
                else
                {
                    PMReportSortDirection = GetSortDirection(PMReportSortDirection);
                }
                PMReportSortColumnId = GetSortColumnId(e.SortExpression);
                lvGroupByPractice.DataSource = SortList(GroupedPractices);
                lvGroupByPractice.DataBind();
            }
        }

        protected void lvGroupByPerson_Sorted(object sender, EventArgs e)
        {
            var lvGroupByPerson = sender as ListView;
            var row = lvGroupByPerson.FindControl("lvHeader") as System.Web.UI.HtmlControls.HtmlTableRow;
            for (int i = 0; i < row.Cells.Count; i++)
            {
                HtmlTableCell cell = row.Cells[i];

                if (cell.HasControls())
                {
                    foreach (var ctrl in cell.Controls)
                    {
                        if (ctrl is LinkButton)
                        {
                            var lb = ctrl as LinkButton;
                            lb.CssClass = "arrow";
                            if (lvGroupByPerson.ID == "lvGroupByDirector")
                            {
                                if (i == DirectorReportSortColumnId)
                                {
                                    lb.CssClass += string.Format(" sort-{0}", DirectorReportSortDirection == "Ascending" ? "up" : "down");
                                }
                            }
                            else if (lvGroupByPerson.ID == "lvGroupByAccountManager")
                            {
                                if (i == AccountManagerReportSortColumnId)
                                {
                                    lb.CssClass += string.Format(" sort-{0}", AccountManagerSortDirection == "Ascending" ? "up" : "down");
                                }
                            }
                            else if (lvGroupByPerson.ID == "lvGroupByPractice")
                            {
                                if (i == PMReportSortColumnId)
                                {
                                    lb.CssClass += string.Format(" sort-{0}", PMReportSortDirection == "Ascending" ? "up" : "down");
                                }
                            }
                        }
                    }
                }
            }
        }

        private int GetSortColumnId(string sortExpression)
        {
            int sortColumn = -1;
            return int.TryParse(sortExpression, out sortColumn) ? sortColumn : 0;
        }

        private string GetSortDirection(string sortDirection)
        {
            switch (sortDirection)
            {
                case "Ascending":
                    sortDirection = "Descending";
                    break;
                case "Descending":
                    sortDirection = "Ascending";
                    break;
            }
            return sortDirection;
        }

        private object SortList(List<ProjectsGroupedByPractice> GroupedPractices)
        {
            List<ProjectsGroupedByPractice> sortedpractices = GroupedPractices;
            if (GroupedPractices != null && GroupedPractices.Count > 0)
            {
                if (!string.IsNullOrEmpty(PrevPMReportSortExpression) && PMReportSortDirection != "Ascending")
                {
                    sortedpractices = GroupedPractices.OrderByDescending(practice => practice.Name).ToList();
                }
                else
                {
                    sortedpractices = GroupedPractices.OrderBy(practice => practice.Name).ToList();
                }
            }
            return sortedpractices;
        }

        private List<ProjectsGroupedByPerson> SortList(List<ProjectsGroupedByPerson> groupedDirectors, string prevSortDirection, string sortDirection)
        {
            if (groupedDirectors != null && groupedDirectors.Count > 0)
            {
                if (!string.IsNullOrEmpty(prevSortDirection))
                {
                    if (sortDirection != "Ascending")
                    {
                        groupedDirectors = groupedDirectors.OrderByDescending(person => person.LastName + " , " + person.FirstName).ToList();
                    }
                    else
                    {
                        groupedDirectors = groupedDirectors.OrderBy(person => person.LastName + " , " + person.FirstName).ToList();
                    }
                }

                if (groupedDirectors.Any(p => p.LastName == ProjectsWithoutDirectorText))
                {
                    var Emptyperson = groupedDirectors.FindAll(p => p.LastName == ProjectsWithoutDirectorText).First();
                    groupedDirectors.RemoveAt(groupedDirectors.IndexOf(Emptyperson));
                    groupedDirectors.Add(Emptyperson);
                }
            }
            return groupedDirectors;
        }

        private string GetClientRowsHtml(List<ProjectsGroupedByClient> groupedClients, string attributes)
        {
            var clientSb = new StringBuilder(string.Empty);
            foreach (var client in groupedClients)
            {
                var ClientAttributes = attributes + string.Format(ClientAttributeTemplate, client.Id);
                clientSb.Append(string.Format(RowHTMLTemplate, ClientCellLeftPadding,
                    getMonthCellsHTML(client.ProjectedFinancialsByMonth), client.ComputedFinancials.Revenue,
                    client.ComputedFinancials.GrossMargin, GetClientNameCellHtml(client, ClientAttributes),
                    ClientBagroundStyle + ClientAttributes + ExpandCollapseStatusAttribute));
                clientSb.Append(GetClientGroupRowsHtml(client.GroupedClientGroups, ClientAttributes));
                if (client.ProjectsWithoutClientGroup != null)
                {
                    clientSb.Append(GetProjectRowsHtml(client.ProjectsWithoutClientGroup, ClientAttributes));
                }
                else
                {
                    clientSb.Append(GetMilestonePersonRowsHtml(client.MilestonePersonsWithoutClientGroup, ClientAttributes));
                }
            }
            return clientSb.ToString();
        }

        private string GetMilestonePersonRowsHtml(List<MilestonePerson> MilestonePersonList, string ClientAttributes)
        {
            var projectSb = new StringBuilder(string.Empty);

            foreach (var milestonePerson in MilestonePersonList)
            {
                ComputedFinancials temp = new ComputedFinancials();
                temp.Revenue = 0;
                temp.GrossMargin = 0;
                var practiceFinancials = milestonePerson.PracticeList.First().ComputedFinancials;

                if (practiceFinancials != null)
                {
                    temp = practiceFinancials;
                }

                var projectAttribures = ClientAttributes + (MilestonePersonList.IndexOf(milestonePerson) % 2 == 0 ? ProjectBagroundStyle : string.Empty);
                projectSb.Append(string.Format(RowHTMLTemplate, ProjectCellLeftPadding, getMonthCellsHTML(milestonePerson.PracticeList.First().ProjectedFinancialsByMonth.OrderByDescending(kv => kv.Key).ToDictionary(v => v.Key, v => v.Value)),
                                                temp.Revenue, temp.GrossMargin,
                                                GetProjectNameHTML(milestonePerson),
                                                 projectAttribures));
            }
            return projectSb.ToString();
        }

        private string GetClientGroupRowsHtml(List<ProjectsGroupedByClientGroup> GroupedClientGroups, string attributes)
        {
            var clientGroupSb = new StringBuilder(string.Empty);
            var isClientGroupRowRequired = true;
            if (!GroupedClientGroups.Any())
            {
                return string.Empty;
            }
            if (GroupedClientGroups.Count == 1 && GroupedClientGroups[0].Name == "Default Group")
            {
                isClientGroupRowRequired = false;
            }
            foreach (var clientGroup in GroupedClientGroups)
            {
                var clientGroupAttributes = attributes + string.Format(ClientGroupAttributeTemplate, clientGroup.Id);
                if (isClientGroupRowRequired)
                {
                    clientGroupSb.Append(string.Format(RowHTMLTemplate, ClientGroupCellLeftPadding,
                        getMonthCellsHTML(clientGroup.ProjectedFinancialsByMonth), clientGroup.ComputedFinancials.Revenue,
                        clientGroup.ComputedFinancials.GrossMargin, GetClientGroupNameCellHTML(clientGroup, clientGroup.Projects, clientGroupAttributes),
                        clientGroupAttributes + ExpandCollapseStatusAttribute));
                }
                if (clientGroup.Projects != null)
                {
                    clientGroupSb.Append(GetProjectRowsHtml(clientGroup.Projects, isClientGroupRowRequired ? clientGroupAttributes : attributes));
                }
                else
                {
                    clientGroupSb.Append(GetMilestonePersonRowsHtml(clientGroup.MilestonePersons, isClientGroupRowRequired ? clientGroupAttributes : attributes));
                }
            }
            return clientGroupSb.ToString();
        }

        private string GetProjectRowsHtml(List<Project> projectList, string attributes)
        {
            var projectSb = new StringBuilder(string.Empty);

            foreach (var project in projectList)
            {
                ComputedFinancials temp = new ComputedFinancials();
                temp.Revenue = 0;
                temp.GrossMargin = 0;

                if (project.ComputedFinancials != null)
                {
                    temp = project.ComputedFinancials;
                }

                var projectAttribures = attributes + (projectList.IndexOf(project) % 2 == 0 ? ProjectBagroundStyle : string.Empty);
                projectSb.Append(string.Format(RowHTMLTemplate, ProjectCellLeftPadding, getMonthCellsHTML(project.ProjectedFinancialsByMonth.OrderByDescending(kv => kv.Key).ToDictionary(v => v.Key, v => v.Value)),
                                                temp.Revenue, temp.GrossMargin, GetProjectNameHTML(project), projectAttribures));
            }
            return projectSb.ToString();
        }

        private string GetProjectNameHTML(Project project)
        {
            return string.Format(LinkHTMLTemplate,
                                    string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                                  ProjectDetailPagePath,
                                                  project.Id),
                                    project.Name);

        }

        private string GetProjectNameHTML(MilestonePerson milestonePerson)
        {
            var projectname = milestonePerson.Milestone.Project.Name + "-" +
                              milestonePerson.Milestone.Description + "-" +
                              milestonePerson.Person.LastName + ", " + milestonePerson.Person.FirstName;
            return string.Format(LinkHTMLTemplate,
                                    string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                                  ProjectDetailPagePath,
                                                  milestonePerson.Milestone.Project.Id),
                                    projectname);

        }

        private string getMonthCellsHTML(Dictionary<DateTime, ComputedFinancials> FinancialsByMonth, bool IsGrandTotalRow = false)
        {
            var financials = GetProjectedFinancials(GetMonthBegin(), GetPeriodLength());
            var MonthCellSb = new StringBuilder(string.Empty);
            foreach (var financial in financials)
            {
                KeyValuePair<DateTime, ComputedFinancials> monthFinancial = financial;
                if (FinancialsByMonth.Any(kv => kv.Key == financial.Key))
                {
                    monthFinancial = FinancialsByMonth.First(kv => kv.Key == financial.Key);
                }

                if (monthFinancial.Key.Month == DateTime.Now.Month && monthFinancial.Key.Year == DateTime.Now.Year)
                {
                    MonthCellSb.Append(string.Format(IsGrandTotalRow ? CurrentMonthLastCellTemplate : CurrentMonthCellTemplate, monthFinancial.Value.Revenue, monthFinancial.Value.GrossMargin));
                }
                else
                {
                    MonthCellSb.Append(string.Format(MonthCellTemplate, monthFinancial.Value.Revenue, monthFinancial.Value.GrossMargin));
                }

            }
            return MonthCellSb.ToString();
        }

        private string GetClientNameCellHtml(ProjectsGroupedByClient client, string attributes)
        {
            //string expandImageClass = string.Empty;
            //if (client.GroupedClientGroups.Count == 0 || (client.GroupedClientGroups.Count == 1 && client.GroupedClientGroups[0].Name == "Default Group"))
            //{
            //    expandImageClass = "class='hidden'";
            //}
            var clientAttributes = attributes + string.Format(ClientAttributeTemplate, client.Name);
            var clientNameHTML = string.Format(CollpseExpandCellTemplate, clientAttributes, string.Empty, client.Name, string.Empty);

            return clientNameHTML;
        }

        private string GetClientGroupNameCellHTML(ProjectsGroupedByClientGroup clientGroup, List<Project> projects, string attributes)
        {
            var clientGroupAttributes = attributes + string.Format(ClientGroupAttributeTemplate, clientGroup.Name);

            var clientGroupNameHTML = string.Format(CollpseExpandCellTemplate, clientGroupAttributes, string.Empty, clientGroup.Name, string.Empty);

            return clientGroupNameHTML;
        }

    }
}

