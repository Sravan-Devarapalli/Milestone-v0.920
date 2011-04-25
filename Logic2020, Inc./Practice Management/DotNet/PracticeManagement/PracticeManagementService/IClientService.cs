using System.Collections.Generic;
using System.ServiceModel;
using DataTransferObjects;

namespace PracticeManagementService
{
    [ServiceContract(Namespace="http://www.logic2020.com")]
    public interface IClientService
    {
        /// <summary>
        /// Change client information
        /// </summary>
        /// <param name="client"><see cref="Client"/> with information to be changed</param>
        [OperationContract]
		int? SaveClientDetail(Client client);

        /// <summary>
        /// Get a client
        /// </summary>
        /// <param name="clientId">Id of the client to get</param>
        /// <param name="viewerUsername"></param>
        /// <returns>Client matching <paramref name="clientId"/>, or <value>null</value> if the client is not in the system</returns>
        /// <remarks>
        /// Presumably the id is obtained form a previous call to GetClientList but
        /// there is no system restriction on the value for the identifier in this call.
        /// </remarks>
        [OperationContract]
		Client GetClientDetail(int clientId, string viewerUsername);

        /// <summary>
        /// Inactivate (hide) a client
        /// </summary>
        /// <param name="client"><see cref="Client"/> to hide</param>
        /// <remarks>
        /// Uses the ClientId to hide record in data store
        /// </remarks>
        [OperationContract]
        void ClientInactivate(Client client);

        /// <summary>
        /// Reactivate a client
        /// </summary>
        /// <param name="client">client whose ID will be reactivated</param>
        /// <remarks>
        /// Presumably the client was inactivated previosuly, but there is no restriction,
        /// i.e. active clients can be reactivated with no error.
        /// </remarks>
        [OperationContract]
        void ClientReactivate(Client client);

        /// <summary>
        /// List all active clients in the system
        /// </summary>
        /// <returns><see cref="List{T}"/> of all active <see cref="Client"/>s in the system</returns>
        [OperationContract]
        List<Client> ClientListAll();

        /// <summary>
        /// List all active and inactive clients in the system
        /// </summary>
        /// <param name="person">Person to restrict results to</param>
        /// <param name="inactives">Include inactive items</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="Client"/>s in the system</returns>
        [OperationContract]
        List<Client> ClientListAllSecure(Person person, bool inactives);

        /// <summary>
        /// List all clients, including inactive clients
        /// </summary>
        /// <returns><see cref="List{T}"/> of all active and inactive <see cref="Client"/>s in the system</returns>
        [OperationContract]
        List<Client> ClientListAllWithInactive();

		/// <summary>
		/// Retrives the list clients available for the specific project.
		/// </summary>
		/// <param name="projectId">An ID of the project to retrive the data for.</param>
		/// <returns>The list of the <see cref="Client"/> objects.</returns>
		[OperationContract]
        List<Client> ClientListAllForProject(int? projectId, int? loggedInPersonId);

    }
}
