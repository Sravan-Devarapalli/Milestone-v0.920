using System.Collections.Generic;
using System.ServiceModel;
using DataTransferObjects;

namespace PracticeManagementService
{
    [ServiceContract(Namespace = "http://www.logic2020.com")]
    public interface IClientService
    {
        /// <summary>
        /// Change client information
        /// </summary>
        /// <param name="client"><see cref="Client"/> with information to be changed</param>
        [OperationContract]
        int? SaveClientDetail(Client client, string userLogin);

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

        [OperationContract]
        Client GetClientDetailsShort(int clientId);

        [OperationContract]
        void UpdateStatusForClient(int clientId, bool inActive, string userLogin);

        [OperationContract]
        void UpdateIsChargableForClient(int? clientId, bool isChargable, string userLogin);

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
        /// List all active and inactive clients in the system
        /// </summary>
        /// <param name="person">Person to restrict results to</param>
        /// <param name="inactives">Include inactive items</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="Client"/>s in the system</returns>
        [OperationContract]
        List<Client> ClientListAllSecureByNewRule(Person person, bool inactives, bool applyNewRule);

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

        [OperationContract]
        List<ColorInformation> GetAllColorsForMargin();

        [OperationContract]
        List<ClientMarginColorInfo> GetClientMarginColorInfo(int clientId);

        [OperationContract]
        List<Client> ClientListAllWithoutPermissions();

        [OperationContract]
        Client GetInternalAccount();

        [OperationContract]
        void ClientIsNoteRequiredUpdate(int clientId, bool isNoteRequired, string userLogin);

        [OperationContract]
        int PricingListInsert(PricingList pricingList, string userLogin);

        [OperationContract]
        void PricingListDelete(int pricingListId, string userLogin);

        [OperationContract]
        void PricingListUpdate(PricingList pricingList, string userLogin);

        [OperationContract]
        List<PricingList> GetPricingLists(int? clientId);
    }
}

