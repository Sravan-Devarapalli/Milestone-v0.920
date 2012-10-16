﻿using System.Collections.Generic;
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
        void UpdatePractice(Practice practice);

        /// <summary>
        /// Inserts practice
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        [OperationContract]
        int InsertPractice(Practice practice);

        /// <summary>
        /// Removes practice
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        [OperationContract]
        void RemovePractice(Practice practice);

        /// <summary>
        /// Gets the list of practicecapabilities for the given practiceid and capabilityid
        /// If the practiceid and capabilityid are nulls then returns all practicecapabilities.
        /// </summary>
        /// <param name="practiceId"></param>
        /// <param name="capabilityId"></param>
        /// <returns>list of practicecapabilities</returns>
        [OperationContract]
        List<PracticeCapability> GetPracticeCapabilities(int? practiceId, int? capabilityId);
    }
}

