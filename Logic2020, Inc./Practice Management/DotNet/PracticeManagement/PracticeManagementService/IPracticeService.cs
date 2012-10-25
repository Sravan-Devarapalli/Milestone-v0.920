using System.Collections.Generic;
using System.ServiceModel;
using DataTransferObjects;

namespace PracticeManagementService
{
    [ServiceContract]
    public interface IPracticeService
    {
        /// <summary>
        /// Get all practices
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        [OperationContract]
        List<Practice> GetPracticeList();

        /// <summary>
        /// Get all practices
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        [OperationContract]
        List<Practice> PracticeListAll(Person person);

        /// <summary>
        /// Get all practices With their capabilities
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        [OperationContract]
        List<Practice> PracticeListAllWithCapabilities();

        /// <summary>
        /// Get practices by id
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        [OperationContract]
        List<Practice> PracticeGetById(int? id);

        /// <summary>
        /// Updates practice
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        [OperationContract]
        void UpdatePractice(Practice practice, string userLogin);

        /// <summary>
        /// Inserts practice
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        [OperationContract]
        int InsertPractice(Practice practice, string userLogin);

        /// <summary>
        /// Removes practice
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        [OperationContract]
        void RemovePractice(Practice practice, string userLogin);

        /// <summary>
        /// Gets the list of practice capabilities for the given practiceid and capabilityid
        /// If the practiceid and capabilityid are nulls then returns all practicecapabilities.
        /// </summary>
        /// <param name="practiceId"></param>
        /// <param name="capabilityId"></param>
        /// <returns>list of practice capabilities</returns>
        [OperationContract]
        List<PracticeCapability> GetPracticeCapabilities(int? practiceId, int? capabilityId);

        /// <summary>
        /// Deletes the given capability
        /// </summary>
        /// <param name="capabilityId"></param>
        [OperationContract]
        void CapabilityDelete(int capabilityId, string userLogin);

        /// <summary>
        /// Updated the given capability
        /// </summary>
        /// <param name="capability"></param>
        [OperationContract]
        void CapabilityUpdate(PracticeCapability capability, string userLogin);

        /// <summary>
        /// Insert New capability
        /// </summary>
        /// <param name="capability"></param>
        [OperationContract]
        void CapabilityInsert(PracticeCapability capability, string userLogin);

    }
}

