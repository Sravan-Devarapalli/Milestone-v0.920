using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using System.Web.Security;
using DataAccess;
using DataTransferObjects;

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
        public int? SaveClientDetail(Client client)
        {
            if (!client.Id.HasValue)
            {
                ClientDAL.ClientInsert(client);
                return client.Id;
            }
            else
            {
                ClientDAL.ClientUpdate(client);
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

        /// <summary>
        /// Inactivate (hide) a client
        /// </summary>
        /// <param name="client"><see cref="Client"/> to hide</param>
        /// <remarks>
        /// Uses the ClientId to hide record in data store
        /// </remarks>
        public void ClientInactivate(Client client)
        {
            ClientDAL.ClientInactivate(client);
        }

        /// <summary>
        /// Reactivate a client
        /// </summary>
        /// <param name="client">client whose ID will be reactivated</param>
        /// <remarks>
        /// Presumably the client was inactivated previosuly, but there is no restriction,
        /// i.e. active clients can be reactivated with no error.
        /// </remarks>
        public void ClientReactivate(Client client)
        {
            ClientDAL.ClientReactivate(client);
        }

        public void UpdateIsChargableForClient(int? clientId, bool isChargable)
        {
            ClientDAL.UpdateIsChargableForClient(clientId, isChargable);
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
            return ClientDAL.ClientListAllSecure(person, inactives);
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
        #endregion
    }
}
