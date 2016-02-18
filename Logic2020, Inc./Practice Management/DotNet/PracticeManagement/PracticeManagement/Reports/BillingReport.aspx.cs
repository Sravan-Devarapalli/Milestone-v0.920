using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using PraticeManagement.Controls;
using PraticeManagement.ClientService;
using System.ServiceModel;
using DataTransferObjects.Filters;
using DataTransferObjects;
using PraticeManagement.Utils;

namespace PraticeManagement.Reports
{
    public partial class BillingReport : System.Web.UI.Page
    {
        public string AccountIds
        {
            get
            {
                if (cblAccount.Items.Count == 0)
                    return null;
                else
                {
                    var clientList = new StringBuilder();
                    foreach (ListItem item in cblAccount.Items)
                        if (item.Selected)
                            clientList.Append(item.Value).Append(',');
                    return clientList.ToString();
                }
              
            }
            set {
                cblAccount.SelectedItems = value;
            }
        }

        public string DirectorFilteredIds
        {
            get
            {
                return ViewState["DirectorFilteredIds"] as string;
            }
            set
            {
                ViewState["DirectorFilteredIds"] = value;
            }
        }

        public string AccountFilteredIds
        {
            get
            {
                return ViewState["AccountFilteredIds"] as string;
            }
            set
            {
                ViewState["AccountFilteredIds"] = value;
            }
        }

        public string PracticeFilteredIds
        {
            get
            {
                return ViewState["PracticeFilteredIds"] as string;
            }
            set
            {
                ViewState["PracticeFilteredIds"] = value;
            }
        }

        public string SalespersonFilteredIds
        {
            get
            {
                return ViewState["SalespersonFilteredIds"] as string;
            }
            set
            {
                ViewState["SalespersonFilteredIds"] = value;
            }
        }

        public string ProjectManagerFilteredIds
        {
            get
            {
                return ViewState["ProjectManagerFilteredIds"] as string;
            }
            set
            {
                ViewState["ProjectManagerFilteredIds"] = value;
            }
        }

        public string SeniorManagerFilteredIds
        {
            get
            {
                return ViewState["SeniorManagerFilteredIds"] as string;
            }
            set
            {
                ViewState["SeniorManagerFilteredIds"] = value;
            }
        }

        public bool IsHoursUnitOfMeasure
        {
            get
            {
                return ddlMeasureUnit.SelectedValue == "Hours";
            }
        }

        public string PracticeIds
        {
            get
            {
                if (cblPractices.Items[0].Selected == true)
                    return null;
                else if (cblPractices.Items.Count == 0)
                    return string.Empty;
                else
                {
                    var practiceList = new StringBuilder();
                    foreach (ListItem item in cblPractices.Items)
                        if (item.Selected)
                            practiceList.Append(item.Value).Append(',');
                    return practiceList.ToString();
                }
            }
            set {
                cblPractices.SelectedItems = value;
            }
        }

        public string BusinessUnitIds
        {
            get
            {
             if (cblProjectGroup.Items.Count == 0)
                    return null;
                else
                {
                    var groupList = new StringBuilder();
                    foreach (ListItem item in cblProjectGroup.Items)
                        if (item.Selected)
                            groupList.Append(item.Value).Append(',');
                    return groupList.ToString();
                }
            }
            set {
                cblProjectGroup.SelectedItems = value;
            }
        }

        public string DirectorIds
        {
            get
            {
              if (cblDirector.Items[0].Selected == true)
                    return null;
                else if (cblDirector.Items.Count == 0)
                    return string.Empty;
                else
                {
                    var directorsList = new StringBuilder();
                    foreach (ListItem item in cblDirector.Items)
                        if (item.Selected)
                            directorsList.Append(item.Value).Append(',');
                    return directorsList.ToString();
                }            }
            set {
                cblDirector.SelectedItems = value;
            }
        }

        public DateTime? StartDate
        {
            get
            {
                return diRange.FromDate.HasValue ? (DateTime?)diRange.FromDate.Value : null;
            }
            set {
                diRange.FromDate = value;
            }
        }

        public DateTime? EndDate
        {
            get
            {
                return diRange.ToDate.HasValue ? (DateTime?)diRange.ToDate.Value : null;
            }
            set {
                diRange.ToDate = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillInitAccountsList();
                FillInitProjectGroupList();
                FillInitPracticesList();
                FillInitDirectorsList();
                GetFilterValuesForSession();
            }
        }

        private void FillInitAccountsList()
        {
            var allClients = ServiceCallers.Custom.Client(c => c.ClientListAllWithoutPermissions());
            DataHelper.FillListDefaultWithEncodedName(cblAccount, "All Accounts", allClients,
                                             false);
            cblAccount.SelectAll();
        }

        private void FillInitProjectGroupList()
        {
            DataHelper.FillBusinessUnitsByClients(cblProjectGroup, AccountIds, "All Business Units", false);
            cblProjectGroup.SelectAll();
        }

        private void FillInitPracticesList()
        {
            DataHelper.FillPracticeList(cblPractices, Resources.Controls.AllPracticesText);
            cblPractices.SelectAll();
        }

        private void FillInitDirectorsList()
        {
            DataHelper.FillDirectorsList(cblDirector, "All Executives in Charge", null,true);
            cblDirector.SelectAll();
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            Page.Validate(valSum.ValidationGroup);
            if (Page.IsValid)
            {
                SelectView();
                lblRange.Text = StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
                SaveFilterValuesForSession();
            }
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {

            int viewIndex = int.Parse((string)e.CommandArgument);

            SwitchView((Control)sender, viewIndex);
        }

        protected void cblAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            AccountFilteredIds = null;
            using (var serviceClient = new ClientServiceClient())
            {
                try
                {
                    FillInitProjectGroupList();
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
            EnableDisableViewReportButton();
        }

        protected void cblProjectGroup_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            EnableDisableViewReportButton();
        }

        protected void cblPractices_SelectedIndexChanged(object sender, EventArgs e)
        {
            PracticeFilteredIds = null;
            EnableDisableViewReportButton();
        }

        protected void cblDirector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            DirectorFilteredIds = null;
            EnableDisableViewReportButton();
        }

        private void EnableDisableViewReportButton()
        {
            btnUpdateView.Enabled = cblPractices.isSelected && cblAccount.isSelected && cblProjectGroup.isSelected && cblDirector.isSelected;
        }

        private void SelectView()
        {
            if (StartDate.HasValue && EndDate.HasValue && (!string.IsNullOrEmpty(AccountIds)) && (!string.IsNullOrEmpty(BusinessUnitIds)) && (PracticeIds != string.Empty) && (DirectorIds != string.Empty))
            {
                divWholePage.Style.Remove("display");
                LoadActiveView();
            }
            else
            {
                divWholePage.Style.Add("display", "none");
            }
        }

        private void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
            SelectView();
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblProjectViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        public void SelectView(Control sender, int viewIndex)
        {
            mvAccountReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        private void LoadActiveView()
        {
            int activeView = mvAccountReport.ActiveViewIndex;
            switch (activeView)
            {
                case 0:
                    billingSummary.PopulateData(true);
                    break;
            }
        }

        public void SaveFilterValuesForSession()
        {
            BillingReportFilters filter = new BillingReportFilters();
            filter.AccountIds = AccountIds;
            filter.PracticeIds = PracticeIds;
            filter.BusinessUnitIds = BusinessUnitIds;
            filter.ExecutiveInchargeIds = DirectorIds;
            filter.StartDate = StartDate;
            filter.EndDate = EndDate;
            filter.UnitOfMeassure = ddlMeasureUnit.SelectedValue;
            ReportsFilterHelper.SaveFilterValues(ReportName.BillingReport, filter);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.BillingReport) as BillingReportFilters;
            if (filters != null)
            {
                AccountIds = filters.AccountIds;
                StartDate = filters.StartDate;
                EndDate = filters.EndDate;
                PracticeIds = filters.PracticeIds;
                FillInitProjectGroupList();
                BusinessUnitIds = filters.BusinessUnitIds;
                DirectorIds = filters.ExecutiveInchargeIds;
                ddlMeasureUnit.SelectedValue = filters.UnitOfMeassure;
                SelectView();
                lblRange.Text = StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            }
        }
    }
}

