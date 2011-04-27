using System;
using System.ServiceModel;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ClientService;
using PraticeManagement.Controls;
namespace PraticeManagement.Config
{
    public partial class Clients : PracticeManagementPageBase
    {

        protected override void Display()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DisplayContent();
        }

        protected void DisplayContent()
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
            DisplayContent();
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
    }
}
