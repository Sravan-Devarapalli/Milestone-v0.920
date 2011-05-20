using System;
using System.ServiceModel;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ClientService;
using PraticeManagement.Controls;
using System.Linq;

namespace PraticeManagement.Config
{
    public partial class Clients : PracticeManagementPageBase
    {
        private string previousAlphabetLnkButtonId
        {
            get
            {
                string value;

                value = ViewState["PreviousAlphabet"] != null ? (string)ViewState["PreviousAlphabet"] : null;

                return value;
            }
            set
            {
                ViewState["PreviousAlphabet"] = value;
            }
        }

        private const string CLIENTS_LIST_KEY = "ClientsListKey";

        private Client[] ClientsList
        {
            get
            {
                if (Cache[CLIENTS_LIST_KEY] != null)
                {
                    return Cache[CLIENTS_LIST_KEY] as Client[];
                }
                else
                {
                    using (var serviceClient = new ClientServiceClient())
                    {
                        try
                        {
                            var result = chbShowActive.Checked
                                    ? serviceClient.ClientListAll()
                                    : serviceClient.ClientListAllWithInactive();
                            Cache[CLIENTS_LIST_KEY] = result;
                            return result;
                        }
                        catch (FaultException<ExceptionDetail>)
                        {
                            serviceClient.Abort();
                            throw;
                        }
                    }
                }
            }
        }

        protected override void Display()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Cache.Remove(CLIENTS_LIST_KEY);
                DataBindClients(ClientsList);
                previousAlphabetLnkButtonId = lnkbtnAll.ID;
            }

            AddAlphabetButtons();
        }

        protected void DataBindClients(Client[] clientsList)
        {
            gvClients.DataSource = clientsList;

            if (!IsPostBack && clientsList.Length > 0)
            {
                gvClients.SelectedIndex = 0;
            }

            gvClients.DataBind();
        }

        protected void chbShowActive_CheckedChanged(object sender, EventArgs e)
        {
            Cache.Remove(CLIENTS_LIST_KEY);

            LinkButton previousLinkButton = (LinkButton)tdAlphabeticalPaging.FindControl(previousAlphabetLnkButtonId);
            Client[] FilteredClientList = previousLinkButton != null && previousLinkButton.Text != "All" ? ClientsList.AsQueryable().Where(c => c.Name.ToUpperInvariant().StartsWith(previousLinkButton.Text.ToUpperInvariant())).ToArray() : ClientsList;
            DataBindClients(FilteredClientList);
        }

        protected void chbInactive_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chbInactive = sender as CheckBox;
            var ClientId = Convert.ToInt32(chbInactive.Attributes["ClientId"]);

            using (var serviceClient = new ClientServiceClient())
            {
                try
                {
                    if (chbInactive.Checked)
                    {
                        serviceClient.ClientReactivate(new Client() { Id = ClientId });
                    }
                    else
                    {
                        serviceClient.ClientInactivate(new Client() { Id = ClientId });
                    }
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }

            Client client = ClientsList.AsQueryable().Where(c => c.Id == ClientId).ToArray()[0];

            client.Inactive = !client.Inactive;
        }

        protected void chbIsChargeable_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chbIsChargeable = sender as CheckBox;
            var ClientId = Convert.ToInt32(chbIsChargeable.Attributes["ClientId"]);

            using (var serviceClient = new ClientServiceClient())
            {
                try
                {
                    serviceClient.UpdateIsChargableForClient(ClientId, chbIsChargeable.Checked);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }

            Client client = ClientsList.AsQueryable().Where(c => c.Id == ClientId).ToArray()[0];

            client.IsChargeable = !client.IsChargeable;
        }

        protected void btnClientName_Command(object sender, CommandEventArgs e)
        {
            object args = e.CommandArgument;
            Response.Redirect(GetClientDetailsUrl(args));
        }

        protected string GetClientDetailsUrlWithReturn(object args)
        {
            return PraticeManagement.Utils.Generic.GetTargetUrlWithReturn(GetClientDetailsUrl(args), Request.Url.AbsoluteUri);
        }

        private static string GetClientDetailsUrl(object args)
        {
            return string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                 Constants.ApplicationPages.ClientDetails,
                                 args);
        }

        private void AddAlphabetButtons()
        {
            if (tdAlphabeticalPaging.HasControls())
            {
                for (int index = 65; index <= 65 + 25; index++)
                {
                    char alphabet = Convert.ToChar(index);
                    string alphabetId = "lnkbtn" + alphabet;
                    LinkButton Alphabet = (LinkButton)tdAlphabeticalPaging.FindControl(alphabetId);
                    tdAlphabeticalPaging.Controls.Remove(Alphabet);
                }
            }

            for (int index = 65; index <= 65 + 25; index++)
            {
                char alphabet = Convert.ToChar(index);
                LinkButton Alphabet = new LinkButton();
                Alphabet.ID = "lnkbtn" + alphabet;
                Alphabet.Text = alphabet.ToString();
                Alphabet.Font.Underline = false;
                Alphabet.Style.Add("padding-left", "15px");
                Alphabet.Click += new EventHandler(Alphabet_Clicked);

                tdAlphabeticalPaging.Controls.Add(Alphabet);
            }
        }

        protected void Alphabet_Clicked(object sender, EventArgs e)
        {
            if (previousAlphabetLnkButtonId != null)
            {
                LinkButton previousLinkButton = (LinkButton)tdAlphabeticalPaging.FindControl(previousAlphabetLnkButtonId);
                previousLinkButton.Font.Bold = false;
            }

            LinkButton alpha = (LinkButton)sender;
            alpha.Font.Bold = true;
            hdnAlphabet.Value = alpha.Text != "All" ? alpha.Text : null;
            previousAlphabetLnkButtonId = alpha.ID;

            Client[] FilteredClientList = alpha.Text != "All" ? ClientsList.AsQueryable().Where(c => c.Name.ToUpperInvariant().StartsWith(alpha.Text.ToUpperInvariant())).ToArray() : ClientsList;

            DataBindClients(FilteredClientList);
        }
    }
}
