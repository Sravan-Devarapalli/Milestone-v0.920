using System;
using System.ServiceModel;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ClientService;
using PraticeManagement.Controls;
using PraticeManagement.Utils;
using System.Web;


namespace PraticeManagement.Controls
{
    public partial class ClientListControl : System.Web.UI.UserControl
    {
        #region Properties

        private PraticeManagement.Config.GeneralConfiguration HostingPage
        {
            get { return ((PraticeManagement.Config.GeneralConfiguration)Page); }
        }

        #endregion Properties
        /// <summary>
        /// Retrieves the data and display them in the table.
        /// </summary>

        protected void Page_Load(object sender, EventArgs e)
        {
            Display();
        }

        protected void Display()
        {
            using (var serviceClient = new ClientServiceClient())
            {
                try
                {
                    Client[] clients =
                        chbShowActive.Checked
                            ? serviceClient.ClientListAll()
                            : serviceClient.ClientListAllWithInactive();
                    gvClients.DataSource = clients;

                    if (!IsPostBack && clients.Length > 0)
                    {
                        gvClients.SelectedIndex = 0;
                    }

                    gvClients.DataBind();
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void chbShowActive_CheckedChanged(object sender, EventArgs e)
        {
            Display();
        }


        protected void btnClientName_Command(object sender, CommandEventArgs e)
        {
            object args = e.CommandArgument;
            HostingPage.Redirect(GetClientDetailsUrl(args));
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


    }
}
