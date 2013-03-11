using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents a results of rate computing with additional values.
	/// </summary>
	[DataContract]
	[Serializable]
	public class ComputedFinancialsEx : ComputedFinancials
	{

		/// <summary>
		/// Gets or sets a computed value of the loaded hourly rate (salary + overheads)
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency LoadedHourlyRate
		{
			get;
			set;
		}
 
        [DataMember]
        public PracticeManagementCurrency SemiLoadedHourlyRate
        {
            get;
            set;
        }

        [DataMember]
        public PracticeManagementCurrency SemiCOGS
        {
            get;
            set;
        }

        [DataMember]
        public PracticeManagementCurrency SaleCommissionPerHour
        {
            get;
            set;
        }

		/// <summary>
		/// Gets or sets a computed Overheads.
		/// </summary>
		[DataMember]
		public List<PersonOverhead> OverheadList
		{
			get;
			set;
		}

	}
}

