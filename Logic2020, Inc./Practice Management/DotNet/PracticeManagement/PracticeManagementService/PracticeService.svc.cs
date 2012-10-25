using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;

using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
    /// <summary>
    /// Practice information supplied
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PracticeService : IPracticeService
    {
        #region IPracticeService Members

        /// <summary>
        /// Get all practices
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        public List<Practice> GetPracticeList()
        {
            return this.PracticeListAll(null);
        }

        /// <summary>
        /// Get all practices
        /// </summary>
        /// <param name="person">Person to restrict practices to</param>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        public List<Practice> PracticeListAll(Person person)
        {
            return PracticeDAL.PracticeListAll(person);
        }

        /// <summary>
        /// Get all practices With there capabilities
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        public List<Practice> PracticeListAllWithCapabilities()
        {
            return PracticeDAL.PracticeListAllWithCapabilities();
        }

        /// <summary>
        /// Updates practice
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        public void UpdatePractice(Practice practice, string userLogin)
        {
            PracticeDAL.UpdatePractice(practice, userLogin);
        }

        /// <summary>
        /// Inserts practice
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        public int InsertPractice(Practice practice, string userLogin)
        {
            return PracticeDAL.InsertPractice(practice, userLogin);
        }

        /// <summary>
        /// Removes practice
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        public void RemovePractice(Practice practice, string userLogin)
        {
            PracticeDAL.RemovePractice(practice, userLogin);
        }

        /// <summary>
        /// Get practice by id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>List of practices</returns>
        public List<Practice> PracticeGetById(int? id)
        {
            return PracticeDAL.PracticeGetById(id);
        }

        /// <summary>
        /// Gets the list of practicecapabilities for the given practiceid and capabilityid
        /// If the practiceid and capabilityid are nulls then returns all practicecapabilities.
        /// </summary>
        /// <param name="practiceId"></param>
        /// <param name="capabilityId"></param>
        /// <returns>list of practicecapabilities</returns>
        public List<PracticeCapability> GetPracticeCapabilities(int? practiceId, int? capabilityId)
        {
            return PracticeDAL.GetPracticeCapabilities(practiceId, capabilityId);
        }

        /// <summary>
        /// Deletes the given capability
        /// </summary>
        /// <param name="capabilityId"></param>
        public void CapabilityDelete(int capabilityId, string userLogin)
        {
            PracticeDAL.CapabilityDelete(capabilityId, userLogin);
        }

        /// <summary>
        /// Updated the given capability
        /// </summary>
        /// <param name="capability"></param>
        public void CapabilityUpdate(PracticeCapability capability, string userLogin)
        {
            PracticeDAL.CapabilityUpdate(capability, userLogin);
        }

        /// <summary>
        /// Insert New capability
        /// </summary>
        /// <param name="capability"></param>
        public void CapabilityInsert(PracticeCapability capability, string userLogin)
        {
            PracticeDAL.CapabilityInsert(capability, userLogin);
        }

        #endregion
    }
}
