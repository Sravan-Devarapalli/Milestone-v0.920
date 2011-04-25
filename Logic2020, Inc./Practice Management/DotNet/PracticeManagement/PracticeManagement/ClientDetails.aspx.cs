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

namespace PraticeManagement
{
    public partial class ClientDetails : PracticeManagementPageBase, IPostBackEventHandler
    {
        #region Constants

        protected const string CloneCommandName = "Clone";

        private const string DuplClientName = "There is another Client with the same Name.";

        #endregion

        private ExceptionDetail InnerException { get; set; }

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
            }
            mlConfirmation.ClearMessage();
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

                    if (ClientId.HasValue)
                    {
                        var client = GetClient(id);
                        if (client != null)
                        {
                            PopulateControls(client);
                            InitActionControls(client == null || !client.Id.HasValue ? string.Empty : client.Id.Value.ToString());
                        }
                    }
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
                    Page.Validate();
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

