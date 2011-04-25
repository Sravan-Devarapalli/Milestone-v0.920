using System.ServiceModel;
using DataTransferObjects;
using System.Collections.Generic;

namespace PracticeManagementService
{
	[ServiceContract]
    [ServiceKnownType(typeof(Timescale))]
    public interface ITimescaleService
	{
		/// <summary>
		/// Retrives a <see cref="Timescale"/> by its ID.
		/// </summary>
		/// <param name="timescaleId">An ID of the <see cref="Timescale"/> to be retrieved.</param>
		/// <returns>A <see cref="Timescale"/> object if found and null otherwise.</returns>
		[OperationContract]
		Timescale GetById(TimescaleType timescaleId);

        /// <summary>
        /// Retrives all the timescale types.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<Timescale> GetAll();
	}
}

