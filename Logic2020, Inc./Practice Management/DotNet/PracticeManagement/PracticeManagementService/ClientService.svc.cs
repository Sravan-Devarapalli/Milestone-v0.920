using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using System.Web.Security;
using DataAccess;
using DataTransferObjects;
using System.Web;

namespace PracticeManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ClientService : IClientService
    {
        #region IClientService Members

        /// <summary>
        /// Commit data about a <see cref="Client"/> to the system store
        /// </summary>
        /// <param name="client"><see cref="Client"/> with information to be changed</param>
        public int? SaveClientDetail(Client client, string userLogin)
        {
            if (!client.Id.HasValue)
            {
                ClientDAL.ClientInsert(client, userLogin);
                return client.Id;
            }
            else
            {
                ClientDAL.ClientUpdate(client, userLogin);
                return client.Id;
            }
        }

        /// <summary>
        /// Get a client
        /// </summary>
        /// <param name="clientId">Id of the client to get</param>
        /// <param name="viewerUsername"></param>
        /// <returns>Client matching <paramref name="clientId"/>, or <value>null</value> if the client is not in the system</returns>
        public Client GetClientDetail(int clientId, string viewerUsername)
        {
            return ClientDAL.GetById(clientId, IsAdminOrSales(viewerUsername) ? null : viewerUsername);
        }

        /// <summary>
        /// Is given user admin or sales
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>True if admin or sales</returns>
        private static bool IsAdminOrSales(string username)
        {
            var roles = Roles.GetRolesForUser(username);
            return Array.FindIndex(roles, s => s == Constants.RoleNames.AdministratorRoleName) >= 0 ||
                   Array.FindIndex(roles, s => s == Constants.RoleNames.SalespersonRoleName) >= 0;
        }

        public Client GetClientDetailsShort(int clientId)
        {
            return ClientDAL.GetDetailsShortById(clientId);
        }

        public void UpdateStatusForClient(int clientId, bool inActive, string userLogin)
        {
            ClientDAL.UpdateStatusForClient(clientId, inActive, userLogin);
        }

        public void UpdateIsChargableForClient(int? clientId, bool isChargable, string userLogin)
        {
            ClientDAL.UpdateIsChargableForClient(clientId, isChargable, userLogin);
        }

        /// <summary>
        /// List all active clients in the system
        /// </summary>
        /// <returns><see cref="List{T}"/> of all active <see cref="Client"/>s in the system</returns>
        public List<Client> ClientListAll()
        {
            return ClientDAL.ClientListAll();
        }

        /// <summary>
        /// List all clients, including inactive clients
        /// </summary>
        /// <returns><see cref="List{T}"/> of all active and inactive <see cref="Client"/>s in the system</returns>
        public List<Client> ClientListAllWithInactive()
        {
            return ClientDAL.ClientListAllWithInactive();
        }

        /// <summary>
        /// List all active and inactive clients in the system
        /// </summary>
        /// <param name="person">Person to restrict results to</param>
        /// <param name="inactives">Include inactive items</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="Client"/>s in the system</returns>
        public List<Client> ClientListAllSecure(Person person, bool inactives)
        {
            try
            {
                return ClientDAL.ClientListAllSecure(person, inactives);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ClientListAllSecure", "ClientService.svc", string.Empty,
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }
        
        /// <summary>
        /// List all active and inactive clients in the system
        /// </summary>
        /// <param name="person">Person to restrict results to</param>
        /// <param name="inactives">Include inactive items</param>
        /// <param name="applyNewRule">Permissions as per the New rules.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="Client"/>s in the system</returns>
        public List<Client> ClientListAllSecureByNewRule(Person person, bool inactives, bool applyNewRule)
        {
            try
            {
                return ClientDAL.ClientListAllSecure(person, inactives, applyNewRule);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ClientListAllSecureByNewRule", "ClientService.svc", string.Empty,
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Retrives the list clients available for the specific project.
        /// </summary>
        /// <param name="projectId">An ID of the project to retrive the data for.</param>
        /// <returns>The list of the <see cref="Client"/> objects.</returns>
        public List<Client> ClientListAllForProject(int? projectId, int? loggedInPersonId)
        {
            List<Client> result = ClientDAL.ClientListAllForProject(projectId, loggedInPersonId);

            return result;
        }

        public List<ColorInformation> GetAllColorsForMargin()
        {
            return ClientDAL.GetAllColorsForMargin();
        }

        public List<ClientMarginColorInfo> GetClientMarginColorInfo(int clientId)
        {
            return ClientDAL.GetClientMarginColorInfo(clientId);
        }

        public List<Client> ClientListAllWithoutPermissions()
        {
            return ClientDAL.ClientListAllWithoutPermissions();
        }

        public Client GetInternalAccount()
        {
            return ClientDAL.GetInternalAccount();
        }

        public void ClientIsNoteRequiredUpdate(int clientId, bool isNoteRequired, string userLogin)
        {
            try
            {
                ClientDAL.ClientIsNoteRequiredUpdate(clientId, isNoteRequired, userLogin);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ClientIsNoteRequiredUpdate", "ClientService.svc", string.Empty,
                    HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public int PricingListInsert(PricingList pricingList, string userLogin)
        {
            return ClientDAL.PricingListInsert(pricingList, userLogin);
        }

        public void PricingListDelete(int pricingListId, string userLogin)
        {
            ClientDAL.PricingListDelete(pricingListId, userLogin);
        }

        public void PricingListUpdate(PricingList pricingList, string userLogin)
        {
            ClientDAL.PricingListUpdate(pricingList, userLogin);
        }

        public List<PricingList> GetPricingLists(int? clientId)
        {
            return ClientDAL.GetPricingLists(clientId);
        }

        #endregion
    }
}

