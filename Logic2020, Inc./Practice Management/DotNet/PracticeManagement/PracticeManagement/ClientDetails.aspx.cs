using System;
using System.ServiceModel;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ClientService;
using PraticeManagement.Configuration;
using PraticeManagement.Controls;
using System.Web.UI;
using System.Web.Security;
using PraticeManagement.Utils;
using DTO = DataTransferObjects;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;


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
        #endregion

        private ExceptionDetail InnerException { get; set; }

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
                else
                {
                    if (ClientId.HasValue)
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
                            }
                            catch (FaultException<ExceptionDetail> ex)
                            {
                                serviceClient.Abort();
                                throw;
                            }
                        }
                    }

                    var cmci = new List<ClientMarginColorInfo>();
                    cmci.Add(new ClientMarginColorInfo() { ColorInfo = new ColorInformation() });
                    ViewState[CLIENT_THRESHOLDS_LIST_KEY] = cmci;
                    return cmci;
                }
            }
            set { ViewState[CLIENT_THRESHOLDS_LIST_KEY] = value; }
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
                else
                {
                    int id;
                    if (Int32.TryParse(hdnClientId.Value, out id))
                    {
                        return id;
                    }
                    else
                    {
                        return null;
                    }
                }
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
                DataHelper.FillSalespersonList(ddlDefaultSalesperson, string.Empty, false);

                //Directors
                DataHelper.FillDirectorsList(ddlDefaultDirector, "-- Select Client Director --");

                // Terms
                TermsConfigurationSection terms = TermsConfigurationSection.Current;
                ddlDefaultTerms.DataSource = terms != null ? terms.Terms : null;
                ddlDefaultTerms.DataBind();
                txtClientName.Focus();
                var marginInfo = ClientMarginColorInfoList;
                DataBindClientThresholds(marginInfo);
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

            if (!userIsAdministrator && !userIsClientDirector)
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
                DropDownList ddlSR = e.Row.FindControl(gvddlStartRange) as DropDownList;
                DropDownList ddlER = e.Row.FindControl(gvddlEndRange) as DropDownList;
                DropDownList ddcolor = e.Row.FindControl(gvddlColor) as DropDownList;

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
                    ddcolor.Style["background-color"] = "White";
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
                                            Text = string.Format("{0}%", i.ToString()),
                                            Value = i.ToString()
                                        }
                                  );
            }

        }

        protected void custClientName_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!String.IsNullOrEmpty(ExMessage))
            {
                args.IsValid = !(ExMessage == DuplClientName);
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

            if ((userIsAdministrator || userIsClientDirector) && chbMarginThresholds.Checked)
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
            CustomValidator cvcolor = source as CustomValidator;
            GridViewRow row = cvcolor.NamingContainer as GridViewRow;
            DropDownList ddcolor = row.FindControl(gvddlColor) as DropDownList;

            args.IsValid = true;
            if (chbMarginThresholds.Checked)
            {
                if (ddcolor.SelectedIndex == 0)
                {
                    args.IsValid = false;
                    cvgvddlColorClone.IsValid = false;
                }
            }
        }

        protected void cvgvRange_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            if (chbMarginThresholds.Checked)
            {
                CustomValidator cvgvRange = source as CustomValidator;
                GridViewRow row = cvgvRange.NamingContainer as GridViewRow;
                DropDownList ddlSR = row.FindControl(gvddlStartRange) as DropDownList;
                DropDownList ddlER = row.FindControl(gvddlEndRange) as DropDownList;

                args.IsValid = true;
                int start = Convert.ToInt32(ddlSR.SelectedValue);
                int end = Convert.ToInt32(ddlER.SelectedValue);
                if (start > end)
                {
                    args.IsValid = false;
                    cvgvRangeClone.IsValid = false;
                }
            }
        }

        protected void cvgvOverLapRange_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (chbMarginThresholds.Checked)
            {
                CustomValidator cvgvOverLapRange = source as CustomValidator;
                GridViewRow row = cvgvOverLapRange.NamingContainer as GridViewRow;

                DropDownList ddlSR = row.FindControl(gvddlStartRange) as DropDownList;
                DropDownList ddlER = row.FindControl(gvddlEndRange) as DropDownList;
                
                int start = Convert.ToInt32(ddlSR.SelectedValue);
                int end = Convert.ToInt32(ddlER.SelectedValue);
                if (ClientMarginColorInfoList != null)
                {
                    List<ClientMarginColorInfo> cmciList = new List<ClientMarginColorInfo>();
                    for (int i = 0; i < ClientMarginColorInfoList.Count; i++)
                    {
                        if (i != row.RowIndex)
                        {
                            cmciList.Add(ClientMarginColorInfoList[i]);
                        }

                    }

                    if (cmciList.Any(k => k.StartRange >= start && k.StartRange <= end))
                    {
                        args.IsValid = false;
                        cvgvOverLapRangeClone.IsValid = false;
                    }
                }
            }

        }

        protected void custClient_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!String.IsNullOrEmpty(ExMessage))
            {
                args.IsValid = !(ExMessage != DuplClientName);
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
                        mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Client"));
                    }

                    Redirect(Constants.ApplicationPages.ClientList);
                }
            }
        }

        protected void btnAddProject_Click(object sender, EventArgs e)
        {
            GetLatestMarginInfoValues();
            Page.Validate(vsumClient.ValidationGroup);
            if (Page.IsValid)
            {
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
                                                  this.ClientId);

                    Redirect(targetUrl, ClientId.Value.ToString());
                }
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
                DropDownList ddlSR = row.FindControl(gvddlStartRange) as DropDownList;
                DropDownList ddlER = row.FindControl(gvddlEndRange) as DropDownList;
                DropDownList ddcolor = row.FindControl(gvddlColor) as DropDownList;

                int start = Convert.ToInt32(ddlSR.SelectedValue);
                int end = Convert.ToInt32(ddlER.SelectedValue);
                int colorId = Convert.ToInt32(ddcolor.SelectedValue);
                string colorValue = ddcolor.SelectedItem.Attributes["colorValue"];
                string colorDescription = ddcolor.SelectedItem.Attributes["Description"];
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
            var clientMarginColorInfo = new ClientMarginColorInfo();
            clientMarginColorInfo.ColorInfo = new ColorInformation();

            int end = ClientMarginColorInfoList.Max(m => m.EndRange);
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
            ImageButton imgDelete = sender as ImageButton;
            GridViewRow gvRow = imgDelete.NamingContainer as GridViewRow;
            ClientMarginColorInfoList.RemoveAt(gvRow.RowIndex);

            if (gvClientThrsholds.Rows.Count == 1)
            {
                var cmci = new List<ClientMarginColorInfo>();
                cmci.Add(new ClientMarginColorInfo() { ColorInfo = new ColorInformation() });
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
                    PopulateControls(client);

                InitActionControls(client == null || !client.Id.HasValue ? string.Empty : client.Id.Value.ToString());
            }
            else
            {
                InitActionControls(string.Empty);
            }
        }

        private Client GetClient(int? clientId)
        {
            return ServiceCallers.Custom.Client(c => c.GetClientDetail(clientId.Value, DataHelper.CurrentPerson.Alias));
        }

        private void InitActionControls(string clientId)
        {
            var postBackEventReference = ClientScript.GetPostBackEventReference(this, clientId);
            var onClickAction = string.Format(Constants.Scripts.CheckDirtyWithPostback, postBackEventReference);

            btnAddProject.Attributes.Add(
                HtmlTextWriterAttribute.Onclick.ToString(),
                onClickAction);

            if (string.IsNullOrEmpty(clientId))
            {
                var isUserSalesPerson = Roles.IsUserInRole(DTO.Constants.RoleNames.SalespersonRoleName);
                ListItem item = ddlDefaultSalesperson.Items.FindByValue(DataHelper.CurrentPerson.Id.ToString());
                if (item != null)
                    ddlDefaultSalesperson.SelectedValue = DataHelper.CurrentPerson.Id.ToString();
            }
        }

        /// <summary>
        /// Saves the user's input.
        /// </summary>
        private int? SaveData()
        {
            var client = new Client();

            PopulateData(client);
            using (var serviceClient = new ClientServiceClient())
            {
                try
                {
                    return serviceClient.SaveClientDetail(client);
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
            if (client.DefaultDirectorId.HasValue)
            {
                ListItem selectedDefaultDirector = ddlDefaultDirector.Items.FindByValue(client.DefaultDirectorId.Value.ToString());
                if (selectedDefaultDirector == null)
                {
                    Person selectedPerson = DataHelper.GetPerson(client.DefaultDirectorId.Value);
                    selectedDefaultDirector = new ListItem(selectedPerson.PersonLastFirstName, selectedPerson.Id.Value.ToString());
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
            ddlDefaultTerms.SelectedIndex =
                ddlDefaultTerms.Items.IndexOf(ddlDefaultTerms.Items.FindByValue(client.DefaultTerms.ToString()));

            chbMarginThresholds.Checked = client.IsMarginColorInfoEnabled;
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
            client.IsChargeable = chbIsChar.Checked;
            client.DefaultTerms =
                !string.IsNullOrEmpty(ddlDefaultTerms.SelectedValue)
                    ? int.Parse(ddlDefaultTerms.SelectedValue)
                    : default(int);

            client.IsMarginColorInfoEnabled = chbMarginThresholds.Checked;
            client.ClientMarginInfo = ClientMarginColorInfoList;
        }

        #region Projects

        protected void btnProjectName_Command(object sender, CommandEventArgs e)
        {
            Redirect(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                   Constants.ApplicationPages.ProjectDetail,
                                   e.CommandArgument));
        }

        #endregion

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            GetLatestMarginInfoValues();
            Page.Validate(vsumClient.ValidationGroup);
            if (Page.IsValid)
            {
                var clientId = SaveData();
                if (clientId.HasValue)
                {
                    var query = Request.QueryString.ToString();

                    string backUrl;

                    if (Request.QueryString[Constants.QueryStringParameterNames.Id] == null)
                    {
                        backUrl = string.Format(
                            Constants.ApplicationPages.ClientDetailsWithReturnFormat,
                            Constants.ApplicationPages.ClientDetails,
                            query,
                            clientId.Value);
                    }
                    else
                    {
                        backUrl = string.Format(
                            Constants.ApplicationPages.ClientDetailsWithoutClientIdFormat,
                            Constants.ApplicationPages.ClientDetails,
                            query);
                    }
                    var id = -1;
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
            }
        }

        #endregion
    }
}

