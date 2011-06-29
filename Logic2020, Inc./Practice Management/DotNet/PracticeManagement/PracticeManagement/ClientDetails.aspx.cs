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
        private const string lblStartRange = "lblStartRange";
        private const string lblEndRange = "lblEndRange";
        private const string lblColor = "lblColor";

        private bool userIsAdministrator;
        #endregion

        private ExceptionDetail InnerException { get; set; }

        private const string CLIENT_THRESHOLDS_LIST_KEY = "CLIENT_THRESHOLDS_LIST_KEY";

        private List<ClientMarginColorInfo> ClientMarginColorInfoList
        {
            get
            {
                if (ViewState[CLIENT_THRESHOLDS_LIST_KEY] != null)
                {
                    return ViewState[CLIENT_THRESHOLDS_LIST_KEY] as List<ClientMarginColorInfo>;
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
                    return null;
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
            DisableUsedItemsinDropDowns();

            if (gvClientThrsholds.Rows.Count == 5)
            {
                ddlEndRange.Enabled = ddlColor.Enabled = ddlStartRange.Enabled = btnAddThreshold.Enabled = false;
            }
            else if (chbMarginThresholds.Checked)
            {
                ddlEndRange.Enabled = ddlColor.Enabled = ddlStartRange.Enabled = btnAddThreshold.Enabled = true;
            }

        }

        private void DisableUsedItemsinDropDowns()
        {
            for (int i = 1; i < ddlColor.Items.Count; i++)
            {
                ddlColor.Items[i].Attributes.Remove("disabled");
            }

            if (ClientMarginColorInfoList != null)
            {
                foreach (ClientMarginColorInfo color in ClientMarginColorInfoList)
                {
                    for (int i = 1; i < ddlColor.Items.Count; i++)
                    {
                        int colorId = Convert.ToInt32(ddlColor.Items[i].Value);
                        if (color.ColorInfo.ColorId == colorId)
                        {
                            ddlColor.Items[i].Attributes["disabled"] = "true";
                        }
                    }

                    for (int i = 0; i < ddlStartRange.Items.Count; i++)
                    {
                        int val = Convert.ToInt32(ddlStartRange.Items[i].Value);
                        if (val <= color.EndRange && val >= color.StartRange)
                        {
                            ddlStartRange.Items[i].Attributes.Add("style", "color:gray;");
                            ddlStartRange.Items[i].Attributes["disabled"] = "true";
                            ddlEndRange.Items[i].Attributes.Add("style", "color:gray;");
                            ddlEndRange.Items[i].Attributes["disabled"] = "true";
                        }
                    }

                    if (ddlStartRange.SelectedItem.Attributes["disabled"] == "true")
                    {
                        for (int i = 0; i < ddlStartRange.Items.Count; i++)
                        {
                            if (ddlStartRange.Items[i].Attributes["disabled"] != "true")
                            {
                                ddlStartRange.SelectedIndex = i;
                                ddlEndRange.SelectedIndex = i;
                                break;
                            }
                        }
                    }

                }
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

                DataHelper.FillColorsList(ddlColor, string.Empty);

                FillRangeDropdown(ddlStartRange);
                FillRangeDropdown(ddlEndRange);

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
            if (!userIsAdministrator)
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
                Label lblSR = e.Row.FindControl(lblStartRange) as Label;
                Label lblER = e.Row.FindControl(lblEndRange) as Label;
                Table lblcolor = e.Row.FindControl("tableColor") as Table;


                lblSR.Text = string.Format("{0}%", clientMarginIfo.StartRange.ToString());
                lblER.Text = string.Format("{0}%", clientMarginIfo.EndRange.ToString());
                Color colorValueFrmHex = ColorTranslator.FromHtml(clientMarginIfo.ColorInfo.ColorValue);
                lblcolor.BackColor = colorValueFrmHex;
            }

            // Edit mode.
            if ((e.Row.RowState & DataControlRowState.Edit) != 0)
            {
                DropDownList ddlSR = e.Row.FindControl(gvddlStartRange) as DropDownList;
                DropDownList ddlER = e.Row.FindControl(gvddlEndRange) as DropDownList;
                DropDownList ddcolor = e.Row.FindControl(gvddlColor) as DropDownList;

                FillRangeDropdown(ddlSR);
                FillRangeDropdown(ddlER);
                DataHelper.FillColorsList(ddcolor, string.Empty);

                ddlSR.SelectedValue = clientMarginIfo.StartRange.ToString();
                ddlER.SelectedValue = clientMarginIfo.EndRange.ToString();
                ddcolor.SelectedValue = clientMarginIfo.ColorInfo.ColorId.ToString();
                ddcolor.Style["background-color"] = clientMarginIfo.ColorInfo.ColorValue;

                if (ClientMarginColorInfoList != null)
                {
                    List<ClientMarginColorInfo> cmciList = new List<ClientMarginColorInfo>();
                    for (int i = 0; i < ClientMarginColorInfoList.Count; i++)
                    {
                        if (i != e.Row.RowIndex)
                        {
                            cmciList.Add(ClientMarginColorInfoList[i]);
                        }

                    }

                    foreach (ClientMarginColorInfo color in cmciList)
                    {
                        for (int i = 1; i < ddcolor.Items.Count; i++)
                        {
                            int colorId = Convert.ToInt32(ddcolor.Items[i].Value);
                            if (color.ColorInfo.ColorId == colorId)
                            {
                                ddcolor.Items[i].Attributes["disabled"] = "true";
                            }
                        }

                        for (int i = 0; i < ddlSR.Items.Count; i++)
                        {
                            int val = Convert.ToInt32(ddlSR.Items[i].Value);
                            if (val <= color.EndRange && val >= color.StartRange)
                            {
                                ddlSR.Items[i].Attributes.Add("style", "color:gray;");
                                ddlSR.Items[i].Attributes["disabled"] = "true";
                                ddlER.Items[i].Attributes.Add("style", "color:gray;");
                                ddlER.Items[i].Attributes["disabled"] = "true";
                            }
                        }
                    }
                }


            }
        }

        protected void gvClientThrsholds_OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {

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

        protected void cvClientThresholds_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (userIsAdministrator && chbMarginThresholds.Checked)
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
                        for (int i = 0; i < ClientMarginColorInfoList.Count; i++)
                        {
                            if (i + 1 != ClientMarginColorInfoList.Count)
                            {
                                if (ClientMarginColorInfoList[i].EndRange + 1 != ClientMarginColorInfoList[i + 1].StartRange)
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

        protected void cvddlColor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (ddlColor.SelectedIndex == 0)
            {
                args.IsValid = false;
            }
        }

        protected void cvRange_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            int start = Convert.ToInt32(ddlStartRange.SelectedValue);
            int end = Convert.ToInt32(ddlEndRange.SelectedValue);
            if (start > end)
            {
                args.IsValid = false;
            }
        }

        protected void cvOverLapRange_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            int start = Convert.ToInt32(ddlStartRange.SelectedValue);
            int end = Convert.ToInt32(ddlEndRange.SelectedValue);
            if (ClientMarginColorInfoList != null)
            {
                if (ClientMarginColorInfoList.Any(k => k.StartRange >= start && k.StartRange <= end))
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
            if (ddcolor.SelectedIndex == 0)
            {
                args.IsValid = false;
            }
        }

        protected void cvgvRange_OnServerValidate(object source, ServerValidateEventArgs args)
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
            }
        }

        protected void cvgvOverLapRange_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator cvgvOverLapRange = source as CustomValidator;
            GridViewRow row = cvgvOverLapRange.NamingContainer as GridViewRow;

            DropDownList ddlSR = row.FindControl(gvddlStartRange) as DropDownList;
            DropDownList ddlER = row.FindControl(gvddlEndRange) as DropDownList;

            args.IsValid = true;
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

        protected void btnAddThreshold_OnClick(object sender, EventArgs e)
        {
            Page.Validate(vsumClientMargin.ValidationGroup);
            if (Page.IsValid)
            {
                var clientMarginColorInfo = new ClientMarginColorInfo();

                clientMarginColorInfo.ColorInfo = new ColorInformation();
                clientMarginColorInfo.ColorInfo.ColorId = Convert.ToInt32(ddlColor.SelectedValue);
                clientMarginColorInfo.ColorInfo.ColorDescription = ddlColor.SelectedItem.Attributes["Description"];
                clientMarginColorInfo.ColorInfo.ColorValue = ddlColor.SelectedItem.Attributes["colorValue"];
                clientMarginColorInfo.StartRange = Convert.ToInt32(ddlStartRange.SelectedValue);
                clientMarginColorInfo.EndRange = Convert.ToInt32(ddlEndRange.SelectedValue);

                if (ClientMarginColorInfoList != null)
                {
                    ClientMarginColorInfoList.Add(clientMarginColorInfo);
                }
                else
                {
                    ClientMarginColorInfoList = new List<ClientMarginColorInfo>();
                    ClientMarginColorInfoList.Add(clientMarginColorInfo);
                }

                ClientMarginColorInfoList = ClientMarginColorInfoList.OrderBy(k => k.StartRange).ToList();
                gvClientThrsholds.EditIndex = -1;
                DataBindClientThresholds(ClientMarginColorInfoList);

                ddlColor.SelectedIndex = 0;
            }
        }

        protected void btnDeleteRow_OnClick(object sender, EventArgs e)
        {
            ImageButton imgDelete = sender as ImageButton;
            GridViewRow gvRow = imgDelete.NamingContainer as GridViewRow;
            ClientMarginColorInfoList.RemoveAt(gvRow.RowIndex);
            ClientMarginColorInfoList = ClientMarginColorInfoList.OrderBy(k => k.StartRange).ToList();
            DataBindClientThresholds(ClientMarginColorInfoList);
        }

        protected void imgCancelClientThrsholds_OnClick(object sender, EventArgs e)
        {
            gvClientThrsholds.EditIndex = -1;
            DataBindClientThresholds(ClientMarginColorInfoList);
        }

        protected void imgEditClientThrsholds_OnClick(object sender, EventArgs e)
        {
            ImageButton imgEdit = sender as ImageButton;
            GridViewRow row = imgEdit.NamingContainer as GridViewRow;
            gvClientThrsholds.EditIndex = row.DataItemIndex;
            DataBindClientThresholds(ClientMarginColorInfoList);
        }

        protected void imgUpdateClientThrsholds_OnClick(object sender, EventArgs e)
        {
            ImageButton imgUpdate = sender as ImageButton;
            GridViewRow row = imgUpdate.NamingContainer as GridViewRow;
            DropDownList ddcolor = row.FindControl(gvddlColor) as DropDownList;

            Page.Validate(vsumEditClientMargin.ValidationGroup);
            if (Page.IsValid)
            {
                DropDownList ddlSR = row.FindControl(gvddlStartRange) as DropDownList;
                DropDownList ddlER = row.FindControl(gvddlEndRange) as DropDownList;

                int start = Convert.ToInt32(ddlSR.SelectedValue);
                int end = Convert.ToInt32(ddlER.SelectedValue);
                int colorid = Convert.ToInt32(ddcolor.SelectedValue);

                if (ClientMarginColorInfoList[row.RowIndex].StartRange != start ||
                    ClientMarginColorInfoList[row.RowIndex].EndRange != end ||
                    ClientMarginColorInfoList[row.RowIndex].ColorInfo.ColorId != colorid)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "setDirty();", true);
                }
                ClientMarginColorInfoList[row.RowIndex].StartRange = start;
                ClientMarginColorInfoList[row.RowIndex].EndRange = end;
                ClientMarginColorInfoList[row.RowIndex].ColorInfo.ColorId = colorid;
                ClientMarginColorInfoList[row.RowIndex].ColorInfo.ColorValue = ddcolor.SelectedItem.Attributes["colorValue"];
                ClientMarginColorInfoList[row.RowIndex].ColorInfo.ColorDescription = ddlColor.SelectedItem.Attributes["Description"];
                gvClientThrsholds.EditIndex = -1;
                ClientMarginColorInfoList = ClientMarginColorInfoList.OrderBy(k => k.StartRange).ToList();
                DataBindClientThresholds(ClientMarginColorInfoList);

            }
            else
            {
                ddcolor.Style["background-color"] = ddcolor.SelectedItem.Attributes["colorValue"];
            }
        }

        protected void cbMarginThresholds_OnCheckedChanged(object sender, EventArgs e)
        {
            EnableorDisableClientThrsholdControls(chbMarginThresholds.Checked);
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

            cvOverLapRange.Enabled = cvRange.Enabled = cvddlColor.Enabled = cvClientThresholds.Enabled = btnAddThreshold.Enabled = ddlColor.Enabled = ddlStartRange.Enabled = ddlEndRange.Enabled = gvClientThrsholds.Enabled = ischbMarginThresholdsChecked;

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

