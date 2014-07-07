using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ClientService;
using PraticeManagement.Configuration;
using PraticeManagement.Controls;
using PraticeManagement.Utils;
using DTO = DataTransferObjects;


namespace PraticeManagement
{
    public partial class ClientDetails : PracticeManagementPageBase, IPostBackEventHandler
    {
        #region Constants

        protected const string CloneCommandName = "Clone";

        private const string DuplClientName = "There is another Client with the same Name.";
        private const string gvddlStartRange = "gvddlStartRange";
        private const string gvddlEndRange = "gvddlEndRange";
        private const string gvddlColor = "gvddlColor";
        private bool userIsAdministrator;
        private bool userIsClientDirector;
        private bool userIsSeniorLeadership;
        #endregion

        public Controls.Clients.ClientProjects ProjectsControl
        {
            get
            {
                return ucProjects;
            }
        }

        public string DefaultSalesperson
        {
            get
            {
                if (ddlDefaultSalesperson.SelectedValue != "")
                    return ddlDefaultSalesperson.SelectedItem.Text;
                return string.Empty;
            }
        }

        public string DefaultDirector
        {
            get
            {
                if (ddlDefaultDirector.SelectedValue != "")
                    return ddlDefaultDirector.SelectedItem.Text;
                return string.Empty;
            }
        }

        private const string CLIENT_THRESHOLDS_LIST_KEY = "CLIENT_THRESHOLDS_LIST_KEY";

        private List<ClientMarginColorInfo> ClientMarginColorInfoList
        {
            get
            {
                if (ViewState[CLIENT_THRESHOLDS_LIST_KEY] != null)
                {
                    var output = ViewState[CLIENT_THRESHOLDS_LIST_KEY] as List<ClientMarginColorInfo>;
                    return output;
                }
                var isDeaultMarginInfoEnabled = Convert.ToBoolean(SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.IsDefaultMarginInfoEnabledForAllClientsKey));

                if (ClientId.HasValue)
                {
                    var client = GetClient(ClientId.Value);
                    if (client.IsMarginColorInfoEnabled != null && client.IsMarginColorInfoEnabled.HasValue)
                    {
                        using (var serviceClient = new ClientServiceClient())
                        {
                            try
                            {
                                var result = serviceClient.GetClientMarginColorInfo(ClientId.Value);

                                if (result != null)
                                {
                                    var clientInfoList = result.AsQueryable().ToList();
                                    ViewState[CLIENT_THRESHOLDS_LIST_KEY] = clientInfoList;
                                    return clientInfoList;
                                }
                                return SetSingleRowDataSource();
                            }
                            catch (FaultException<ExceptionDetail>)
                            {
                                serviceClient.Abort();
                                throw;
                            }
                        }
                    }
                    if (isDeaultMarginInfoEnabled)
                    {
                        return SetDefaultClientDataSource();
                    }
                }
                else if (isDeaultMarginInfoEnabled)
                {
                    return SetDefaultClientDataSource();
                }

                return SetSingleRowDataSource();
            }
            set { ViewState[CLIENT_THRESHOLDS_LIST_KEY] = value; }
        }

        private List<ClientMarginColorInfo> SetSingleRowDataSource()
        {
            var cmciList = new List<ClientMarginColorInfo>
            {
                new ClientMarginColorInfo {ColorInfo = new ColorInformation()}
            };
            ViewState[CLIENT_THRESHOLDS_LIST_KEY] = cmciList;
            return cmciList;
        }

        private List<ClientMarginColorInfo> SetDefaultClientDataSource()
        {
            var result = SettingsHelper.GetMarginColorInfoDefaults(DefaultGoalType.Client);
            if (result != null)
            {
                var clientInfoList = result.AsQueryable().ToList();
                ViewState[CLIENT_THRESHOLDS_LIST_KEY] = clientInfoList;
                return clientInfoList;
            }
            return SetSingleRowDataSource();
        }

        private bool IntialchbMarginThresholdsValue
        {
            get { return Convert.ToBoolean(ViewState["IntialchbMarginThresholdsValue"]); }
            set { ViewState["IntialchbMarginThresholdsValue"] = value; }
        }

        private List<ClientMarginColorInfo> IntialClientMarginColorInfoList
        {
            get { return ViewState["IsClientChangedColorSetting"] as List<ClientMarginColorInfo>; }
            set { ViewState["IsClientChangedColorSetting"] = value; }
        }

        private String ExMessage { get; set; }

        private int? ClientId
        {
            get
            {
                if (SelectedId.HasValue)
                {
                    return SelectedId;
                }
                int id;
                if (Int32.TryParse(hdnClientId.Value, out id))
                {
                    return id;
                }
                return null;
            }
            set
            {
                hdnClientId.Value = value.ToString();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (gvClientThrsholds.Rows.Count == 5)
            {
                btnAddThreshold.Enabled = false;
            }
            else if (chbMarginThresholds.Checked)
            {
                btnAddThreshold.Enabled = true;
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                // Salespersons
                DataHelper.FillSalespersonListOnlyActive(ddlDefaultSalesperson, "-- Select Salesperson --");

                //Directors
                DataHelper.FillDirectorsList(ddlDefaultDirector, "-- Select Client Director --");

                // Terms
                TermsConfigurationSection terms = TermsConfigurationSection.Current;
                ddlDefaultTerms.DataSource = terms != null ? terms.Terms : null;
                ddlDefaultTerms.DataBind();
                txtClientName.Focus();
            }
            mlConfirmation.ClearMessage();

            VerifyPrivileges();
        }

        private void VerifyPrivileges()
        {
            userIsAdministrator =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            userIsClientDirector =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);
            userIsSeniorLeadership =
                 Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName);// #2913: userIsSeniorLeadership is added as per the requirement.

            if (!userIsAdministrator && !userIsClientDirector && !userIsSeniorLeadership)
            {
                tpMarginGoals.Visible = false;
            }

        }

        private void DataBindClientThresholds(List<ClientMarginColorInfo> clientMarginColorInfoList)
        {
            gvClientThrsholds.DataSource = clientMarginColorInfoList;
            gvClientThrsholds.DataBind();
        }

        protected void gvClientThrsholds_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }
            var clientMarginIfo = e.Row.DataItem as ClientMarginColorInfo;

            if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState & DataControlRowState.Edit) == 0)
            {
                var ddlSR = e.Row.FindControl(gvddlStartRange) as DropDownList;
                var ddlER = e.Row.FindControl(gvddlEndRange) as DropDownList;
                var ddcolor = e.Row.FindControl(gvddlColor) as DropDownList;

                FillRangeDropdown(ddlSR);
                FillRangeDropdown(ddlER);
                DataHelper.FillColorsList(ddcolor, string.Empty);

                ddlSR.SelectedValue = clientMarginIfo.StartRange.ToString();
                ddlER.SelectedValue = clientMarginIfo.EndRange.ToString();

                if (clientMarginIfo.ColorInfo.ColorId != 0)
                {
                    ddcolor.SelectedValue = clientMarginIfo.ColorInfo.ColorId.ToString();
                    ddcolor.Style["background-color"] = clientMarginIfo.ColorInfo.ColorValue;
                }
                else
                {
                    ddcolor.SelectedValue = string.Empty;
                    ddcolor.Attributes["class"] = "BackGroundColorWhite";
                }
            }
        }

        private void FillRangeDropdown(DropDownList ddlRange)
        {
            ddlRange.Items.Clear();

            for (int i = 0; i < 151; i++)
            {
                ddlRange.Items.Add(
                                        new ListItem()
                                        {
                                            Text = string.Format("{0}%", i),
                                            Value = i.ToString()
                                        }
                                  );
            }

        }

        protected void custClientName_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!String.IsNullOrEmpty(ExMessage))
            {
                args.IsValid = ExMessage != DuplClientName;
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void cvColors_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (chbMarginThresholds.Checked)
            {
                int i = 0;
                foreach (var item in ClientMarginColorInfoList)
                {
                    if (ClientMarginColorInfoList.Any(c => c.ColorInfo.ColorId == ClientMarginColorInfoList[i].ColorInfo.ColorId && c != item && c.ColorInfo.ColorId != 0))
                    {
                        args.IsValid = false;
                        break;
                    }
                    i++;
                }
            }

        }

        protected void cvClientThresholds_ServerValidate(object source, ServerValidateEventArgs args)
        {

            args.IsValid = true;

            if ((userIsAdministrator || userIsClientDirector || userIsSeniorLeadership) && chbMarginThresholds.Checked)
            {
                if (ClientMarginColorInfoList != null && ClientMarginColorInfoList.Count > 0)
                {
                    int start = ClientMarginColorInfoList.Min(m => m.StartRange);
                    int end = ClientMarginColorInfoList.Max(m => m.EndRange);
                    if (start != 0 || end < 100)
                    {
                        args.IsValid = false;
                    }
                    else
                    {
                        var temp = ClientMarginColorInfoList.OrderBy(k => k.StartRange).ToList();
                        for (int i = 0; i < temp.Count; i++)
                        {
                            if (i + 1 != temp.Count)
                            {
                                if (temp[i].EndRange + 1 != temp[i + 1].StartRange)
                                {
                                    args.IsValid = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    args.IsValid = false;
                }
            }
        }

        protected void cvgvddlColor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var cvcolor = source as CustomValidator;
            var row = cvcolor.NamingContainer as GridViewRow;
            var ddcolor = row.FindControl(gvddlColor) as DropDownList;

            args.IsValid = true;
            if (!chbMarginThresholds.Checked) return;
            if (ddcolor.SelectedIndex != 0) return;
            args.IsValid = false;
            cvgvddlColorClone.IsValid = false;
        }

        protected void cvgvRange_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!chbMarginThresholds.Checked) return;
            var cvgvRange = source as CustomValidator;
            var row = cvgvRange.NamingContainer as GridViewRow;
            var ddlSR = row.FindControl(gvddlStartRange) as DropDownList;
            var ddlER = row.FindControl(gvddlEndRange) as DropDownList;

            args.IsValid = true;
            var start = Convert.ToInt32(ddlSR.SelectedValue);
            var end = Convert.ToInt32(ddlER.SelectedValue);
            if (start <= end) return;
            args.IsValid = false;
            cvgvRangeClone.IsValid = false;
        }

        protected void cvgvOverLapRange_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (chbMarginThresholds.Checked)
            {
                var cvgvOverLapRange = source as CustomValidator;
                var row = cvgvOverLapRange.NamingContainer as GridViewRow;

                var ddlSR = row.FindControl(gvddlStartRange) as DropDownList;
                var ddlER = row.FindControl(gvddlEndRange) as DropDownList;

                var start = Convert.ToInt32(ddlSR.SelectedValue);
                var end = Convert.ToInt32(ddlER.SelectedValue);
                if (ClientMarginColorInfoList == null) return;
                var cmciList = ClientMarginColorInfoList.Where((t, i) => i != row.RowIndex).ToList();

                if (!cmciList.Any(k => k.StartRange >= start && k.StartRange <= end)) return;
                args.IsValid = false;
                cvgvOverLapRangeClone.IsValid = false;
            }

        }

        protected void custClient_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!String.IsNullOrEmpty(ExMessage))
            {
                args.IsValid = ExMessage == DuplClientName;
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            GetLatestMarginInfoValues();
            Page.Validate(vsumClient.ValidationGroup);
            if (Page.IsValid)
            {
                int? id = SaveData();

                if (Page.IsValid)
                {
                    if (id.HasValue)
                    {
                        ClientId = id;
                        ClearDirty();
                        mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Account"));
                    }

                    Redirect(Constants.ApplicationPages.ClientList);
                }
            }
        }

        protected void btnAddProject_Click(object sender, EventArgs e)
        {
            GetLatestMarginInfoValues();
            Page.Validate(vsumClient.ValidationGroup);
            if (!Page.IsValid) return;
            if (!ClientId.HasValue)
            {
                int? clientId = SaveData();
                var targetUrl = string.Format(Constants.ApplicationPages.ProjectDetailRedirectWithReturnFormat,
                    Constants.ApplicationPages.ProjectDetail,
                    clientId.Value);

                Redirect(targetUrl, clientId.Value.ToString());
            }
            else if (!SaveDirty || ValidateAndSave())
            {
                var targetUrl = string.Format(Constants.ApplicationPages.ProjectDetailRedirectWithReturnFormat,
                    Constants.ApplicationPages.ProjectDetail,
                    ClientId);

                Redirect(targetUrl, ClientId.Value.ToString());
            }
        }

        private void GetLatestMarginInfoValues()
        {
            while (ClientMarginColorInfoList.Count > 0)
            {
                ClientMarginColorInfoList.RemoveAt(0);
            }

            foreach (GridViewRow row in gvClientThrsholds.Rows)
            {
                var ddlSR = row.FindControl(gvddlStartRange) as DropDownList;
                var ddlER = row.FindControl(gvddlEndRange) as DropDownList;
                var ddcolor = row.FindControl(gvddlColor) as DropDownList;

                var start = Convert.ToInt32(ddlSR.SelectedValue);
                var end = Convert.ToInt32(ddlER.SelectedValue);
                var colorId = Convert.ToInt32(ddcolor.SelectedValue);
                var colorValue = ddcolor.SelectedItem.Attributes["colorValue"];
                var colorDescription = ddcolor.SelectedItem.Attributes["Description"];
                ClientMarginColorInfoList.Add(
                    new ClientMarginColorInfo()
                    {
                        ColorInfo = new ColorInformation()
                        {
                            ColorId = colorId,
                            ColorValue = colorValue,
                            ColorDescription = colorDescription

                        },
                        StartRange = start,
                        EndRange = end
                    });

            }
        }

        protected void btnAddThreshold_OnClick(object sender, EventArgs e)
        {
            GetLatestMarginInfoValues();
            var clientMarginColorInfo = new ClientMarginColorInfo { ColorInfo = new ColorInformation() };

            var end = ClientMarginColorInfoList.Max(m => m.EndRange);
            if (end != 150)
            {
                end = end + 1;
            }
            clientMarginColorInfo.StartRange = end;
            clientMarginColorInfo.EndRange = end;


            ClientMarginColorInfoList.Add(clientMarginColorInfo);
            DataBindClientThresholds(ClientMarginColorInfoList);

        }

        protected void btnDeleteRow_OnClick(object sender, EventArgs e)
        {
            GetLatestMarginInfoValues();
            var imgDelete = sender as ImageButton;
            var gvRow = imgDelete.NamingContainer as GridViewRow;
            ClientMarginColorInfoList.RemoveAt(gvRow.RowIndex);

            if (gvClientThrsholds.Rows.Count == 1)
            {
                var cmci = new List<ClientMarginColorInfo>
                {
                    new ClientMarginColorInfo {ColorInfo = new ColorInformation()}
                };
                ClientMarginColorInfoList = cmci;
            }

            DataBindClientThresholds(ClientMarginColorInfoList);
        }

        protected void cbMarginThresholds_OnCheckedChanged(object sender, EventArgs e)
        {
            GetLatestMarginInfoValues();
            EnableorDisableClientThrsholdControls(chbMarginThresholds.Checked);
            DataBindClientThresholds(ClientMarginColorInfoList);
        }

        /// <summary>
        /// Retrieves the data and display them.
        /// </summary>
        protected override void Display()
        {
            int? id = ClientId;
            if (id.HasValue)
            {
                var client = GetClient(id);

                if (client != null)
                {
                    PopulateControls(client);
                }

                InitActionControls(client == null || !client.Id.HasValue ? string.Empty : client.Id.Value.ToString());
            }
            else
            {
                InitActionControls(string.Empty);
                var IsDeaultMarginInfoEnabled = Convert.ToBoolean(SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.IsDefaultMarginInfoEnabledForAllClientsKey));
                chbMarginThresholds.Checked = IsDeaultMarginInfoEnabled;
                EnableorDisableClientThrsholdControls(chbMarginThresholds.Checked);
            }

            IntialchbMarginThresholdsValue = chbMarginThresholds.Checked;
            var marginInfo = ClientMarginColorInfoList;
            IntialClientMarginColorInfoList = marginInfo;
            DataBindClientThresholds(marginInfo);
            LoadAllView();
        }

        private Client GetClient(int? clientId)
        {
            return ServiceCallers.Custom.Client(c => c.GetClientDetail(clientId.Value, Page.User.Identity.Name));
        }

        private void InitActionControls(string clientId)
        {
            var postBackEventReference = ClientScript.GetPostBackEventReference(this, clientId);
            var onClickAction = string.Format(Constants.Scripts.CheckDirtyWithPostback, postBackEventReference);

            btnAddProject.Attributes.Add(
                HtmlTextWriterAttribute.Onclick.ToString(),
                onClickAction);

            if (!string.IsNullOrEmpty(clientId)) return;
            var item = ddlDefaultSalesperson.Items.FindByValue(DataHelper.CurrentPerson.Id.ToString());
            if (item != null)
                ddlDefaultSalesperson.SelectedValue = DataHelper.CurrentPerson.Id.ToString();
        }

        /// <summary>
        /// Saves the user's input.
        /// </summary>
        private int? SaveData()
        {
            var client = new Client();
            int businessGroupId = 0;
            PopulateData(client);
            using (var serviceClient = new ClientServiceClient())
            {
                try
                {
                    client.DefaultDirector = DefaultDirector;
                    client.DefaultSalesperson = DefaultSalesperson;
                    client.LoginPerson = DataHelper.CurrentPerson.PersonFirstLastName;
                    var id = serviceClient.SaveClientDetail(client, User.Identity.Name);
                    if (!ClientId.HasValue)
                    {
                        foreach (var g in ucProjectGoups.ClientGroupsList)
                        {
                            using (var serviceGroups = new ProjectGroupService.ProjectGroupServiceClient())
                            {
                                if (g.Code != ProjectGroup.DefaultGroupCode)
                                {
                                    var projectGroup = new ProjectGroup
                                    {
                                        Name = g.Name,
                                        IsActive = g.IsActive,
                                        BusinessGroupId = businessGroupId,
                                        ClientId = id.Value
                                    };
                                    int result = serviceGroups.ProjectGroupInsert(projectGroup, Page.User.Identity.Name);
                                }
                                else if (g.Code == ProjectGroup.DefaultGroupCode)
                                {
                                    int groupId = ServiceCallers.Custom.Group(s => s.GroupListAll(id.Value, null).ToList()).First(s => s.Code == ProjectGroup.DefaultGroupCode).Id.Value;
                                    var projectGroup = new ProjectGroup
                                    {
                                        Name = g.Name,
                                        Id = groupId,
                                        IsActive = g.IsActive,
                                        ClientId = id.Value
                                    };
                                    var businessGroupList = serviceGroups.GetBusinessGroupList(null, groupId);
                                    projectGroup.BusinessGroupId = businessGroupList.First().Id.Value;
                                    businessGroupId = projectGroup.BusinessGroupId;
                                    serviceGroups.ProjectGroupUpdate(projectGroup, Page.User.Identity.Name);
                                }
                            }
                        }
                        foreach (var g in ucPricingList.ClientPricingLists)
                        {
                            using (var serviceGroups = new ClientServiceClient())
                            {
                                if (!g.IsDefault)
                                {
                                    var pricingList = new PricingList { ClientId = id.Value, Name = g.Name };
                                    int result = serviceGroups.PricingListInsert(pricingList, User.Identity.Name);
                                }
                                else if (g.IsDefault && g.Name != PricingList.DefaultPricingListName)
                                {
                                    int pricingListId = ServiceCallers.Custom.Client(s => s.GetPricingLists(id.Value).ToList()).First(s => s.IsDefault).PricingListId.Value;
                                    var pricingList = new PricingList
                                    {
                                        ClientId = id.Value,
                                        Name = g.Name,
                                        PricingListId = pricingListId
                                    };
                                    serviceGroups.PricingListUpdate(pricingList, User.Identity.Name);
                                }
                            }
                        }
                        foreach (var g in ucBusinessGroups.ClientGroupsList)
                        {
                            using (var serviceGroups = new ProjectGroupService.ProjectGroupServiceClient())
                            {
                                if (g.Code != BusinessGroup.DefaultBusinessGroupCode)
                                {
                                    var businessGroup = new BusinessGroup
                                    {
                                        Name = g.Name,
                                        IsActive = g.IsActive,
                                        ClientId = id.Value
                                    };
                                    int result = serviceGroups.BusinessGroupInsert(businessGroup, User.Identity.Name);
                                }
                                else if (g.Code == BusinessGroup.DefaultBusinessGroupCode && g.Name != BusinessGroup.DefaultBusinessGroupName)
                                {
                                    int groupId = ServiceCallers.Custom.Group(s => s.GetBusinessGroupList(id.Value.ToString(), null).ToList()).First(s => s.Code == BusinessGroup.DefaultBusinessGroupCode).Id.Value;
                                    var businessGroup = new BusinessGroup
                                    {
                                        Name = g.Name,
                                        Id = groupId,
                                        IsActive = g.IsActive
                                    };
                                    serviceGroups.BusinessGroupUpdate(businessGroup, User.Identity.Name);
                                }
                            }
                        }
                    }
                    return id;
                }
                catch (Exception ex)
                {
                    serviceClient.Abort();
                    ExMessage = ex.Message;
                    Page.Validate(vsumClient.ValidationGroup);
                }
            }

            return null;
        }

        private Person PersonById(int personId)
        {
            using (var serviceClient = new PersonService.PersonServiceClient())
            {
                return serviceClient.GetPersonDetailsShort(personId);
            }
        }

        /// <summary>
        /// Fill the controls with the specified data.
        /// </summary>
        /// <param name="client">The client's data.</param>
        private void PopulateControls(Client client)
        {
            txtClientName.Text = client.Name;
            ddlDefaultSalesperson.SelectedIndex =
                ddlDefaultSalesperson.Items.IndexOf(
                                                       ddlDefaultSalesperson.Items.FindByValue(
                                                                                                  client.
                                                                                                      DefaultSalespersonId
                                                                                                      .ToString()));
            chbIsNoteRequired.Checked = client.IsNoteRequired;
            if (client.DefaultDirectorId.HasValue)
            {
                var selectedDefaultDirector = ddlDefaultDirector.Items.FindByValue(client.DefaultDirectorId.Value.ToString());
                if (selectedDefaultDirector == null)
                {
                    var selectedPerson = PersonById(client.DefaultDirectorId.Value);
                    selectedDefaultDirector = new ListItem(selectedPerson.PersonLastFirstName, client.DefaultDirectorId.Value.ToString());
                    ddlDefaultDirector.Items.Add(selectedDefaultDirector);
                    ddlDefaultDirector.SortByText();
                }

                ddlDefaultDirector.SelectedValue = selectedDefaultDirector.Value;

            }

            txtDefaultDiscount.Text = client.DefaultDiscount.ToString();
            chbActive.Checked = !client.Inactive;
            if (chbActive.Checked)
            {
                btnAddProject.Enabled = true;
                hdnchbActive.Value = "true";
                btnAddProject.CssClass = "add-btn-project";
            }
            else
            {
                btnAddProject.Enabled = false;
                hdnchbActive.Value = "false";
                btnAddProject.CssClass = "darkadd-btn-project";
            }
            chbIsChar.Checked = client.IsChargeable;
            chbIsInternal.Checked = client.IsInternal;
            chbHouseAccount.Checked = client.IsHouseAccount;
            ddlDefaultTerms.SelectedIndex =
                ddlDefaultTerms.Items.IndexOf(ddlDefaultTerms.Items.FindByValue(client.DefaultTerms.ToString()));

            if (client.IsMarginColorInfoEnabled != null && client.IsMarginColorInfoEnabled.HasValue)
            {
                chbMarginThresholds.Checked = client.IsMarginColorInfoEnabled.Value;
            }
            else if (Convert.ToBoolean(SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.IsDefaultMarginInfoEnabledForAllClientsKey)))
            {
                chbMarginThresholds.Checked = true;
            }
            else
            {
                chbMarginThresholds.Checked = false;
            }

            EnableorDisableClientThrsholdControls(chbMarginThresholds.Checked);
        }

        private void EnableorDisableClientThrsholdControls(bool ischbMarginThresholdsChecked)
        {
            cvClientThresholds.Enabled = btnAddThreshold.Enabled = gvClientThrsholds.Enabled = ischbMarginThresholdsChecked;
        }

        private void PopulateData(Client client)
        {
            client.Id = ClientId;
            client.Name = txtClientName.Text;
            client.DefaultSalespersonId = Int32.Parse(ddlDefaultSalesperson.SelectedValue);

            if (ddlDefaultDirector.SelectedIndex > 0)
            {
                client.DefaultDirectorId = Int32.Parse(ddlDefaultDirector.SelectedValue);
            }

            client.DefaultDiscount = decimal.Parse(txtDefaultDiscount.Text);
            client.Inactive = !chbActive.Checked;
            client.IsHouseAccount = chbHouseAccount.Checked;
            client.IsChargeable = chbIsChar.Checked;
            client.IsInternal = chbIsInternal.Checked;
            client.IsNoteRequired = chbIsNoteRequired.Checked;
            client.DefaultTerms =
                !string.IsNullOrEmpty(ddlDefaultTerms.SelectedValue)
                    ? int.Parse(ddlDefaultTerms.SelectedValue)
                    : default(int);

            client.IsMarginColorInfoEnabled = chbMarginThresholds.Checked;

            client.ClientMarginInfo = ClientMarginColorInfoList;
        }

        private void LoadAllView()
        {
            switch (tcFilters.ActiveTabIndex)
            {
                case 0: ucProjects.DisplayProjects();
                    break;
                case 1: ucProjectGoups.DisplayGroups(null, true);
                    break;
                case 2: ucBusinessGroups.DisplayGroups(null, true);
                    break;
                case 3: ucPricingList.DisplayPricingList(null, true);
                    break;
            }
        }

        #region Projects

        protected void btnProjectName_Command(object sender, CommandEventArgs e)
        {
            Redirect(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                   Constants.ApplicationPages.ProjectDetail,
                                   e.CommandArgument));


        }

        protected void lnkProjects_Click(object sender, EventArgs e)
        {
            ucProjects.DisplayProjects();
            tcFilters.ActiveTabIndex = 0;
        }

        protected void lnkBusinessUnit_Click(object sender, EventArgs e)
        {
            ucProjectGoups.DisplayGroups(null, true);
            tcFilters.ActiveTabIndex = 1;
        }

        protected void lnkBusinessGroup_Click(object sender, EventArgs e)
        {
            ucBusinessGroups.DisplayGroups(null, true);
            tcFilters.ActiveTabIndex = 2;
        }

        protected void lnkPricingList_Click(object sender, EventArgs e)
        {
            ucPricingList.DisplayPricingList(null, true);
            tcFilters.ActiveTabIndex = 3;
        }
        #endregion




        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            GetLatestMarginInfoValues();
            Page.Validate(vsumClient.ValidationGroup);
            if (!Page.IsValid) return;
            var clientId = SaveData();
            if (!clientId.HasValue) return;
            var query = Request.QueryString.ToString();
            var backUrl = string.Format(Constants.ApplicationPages.ClientDetailsWithoutClientIdFormat,
                Constants.ApplicationPages.ClientDetails, query);
            backUrl = GetBackUrlWithId(backUrl, clientId.Value.ToString());

            int id;
            if (string.IsNullOrEmpty(eventArgument) || int.TryParse(eventArgument, out id))
            {
                RedirectWithBack(
                    string.Format(
                        Constants.ApplicationPages.ProjectDetailRedirectWithReturnFormat,
                        Constants.ApplicationPages.ProjectDetail,
                        clientId.Value),
                    backUrl);
            }
            else
            {
                RedirectWithBack(
                    string.Format(
                        Constants.ApplicationPages.ProjectDetailRedirectWithReturnFormat,
                        eventArgument,
                        clientId.Value),
                    backUrl);
            }
        }

        #endregion
    }
}

