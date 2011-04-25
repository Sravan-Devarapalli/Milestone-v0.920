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
	    /// Updates practice
	    /// </summary>
	    /// <returns>A list of <see cref="Practice"/>s in the system</returns>
	    public void UpdatePractice(Practice practice)
	    {
            PracticeDAL.UpdatePractice(practice);
	    }

	    /// <summary>
	    /// Inserts practice
	    /// </summary>
	    /// <returns>A list of <see cref="Practice"/>s in the system</returns>
	    public int InsertPractice(Practice practice)
	    {
	        return PracticeDAL.InsertPractice(practice);
	    }

	    /// <summary>
	    /// Removes practice
	    /// </summary>
	    /// <returns>A list of <see cref="Practice"/>s in the system</returns>
	    public void RemovePractice(Practice practice)
	    {
	        PracticeDAL.RemovePractice(practice);
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

	    #endregion               
    }
}
