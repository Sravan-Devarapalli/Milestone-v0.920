using System.ServiceModel.Activation;
using DataAccess;
using DataTransferObjects;
using System.Collections.Generic;

namespace PracticeManagementService
{
	// NOTE: If you change the class name "TimescaleService" here, you must also update the reference to "TimescaleService" in Web.config.
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class TimescaleService : ITimescaleService
	{
		#region ITimescaleService Members

		/// <summary>
		/// Retrives a <see cref="Timescale"/> by its ID.
		/// </summary>
		/// <param name="timescaleId">An ID of the <see cref="Timescale"/> to be retrieved.</param>
		/// <returns>A <see cref="Timescale"/> object if found and null otherwise.</returns>
		public Timescale GetById(DataTransferObjects.TimescaleType timescaleId)
		{
			return TimescaleDAL.GetById(timescaleId);
		}

        /// <summary>
        /// Retrives all the timescale types.
        /// </summary>
        /// <returns></returns>
        public List<Timescale> GetAll()
        {
            return TimescaleDAL.GetAll();
        }
		#endregion
	}
}

