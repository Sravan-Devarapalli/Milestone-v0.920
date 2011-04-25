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
		private PracticeManagementCurrency marginWithoutRecruitingValue;
		private PracticeManagementCurrency cogsWithoutRecruitingValue;

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
        public PracticeManagementCurrency SemiLoadedHourlyRateWithoutRecruiting
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
        public PracticeManagementCurrency SemiCOGSWithoutRecruiting
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

        /// <summary>
        /// Gets or sets a computed COGS excluding the recruiting costs.
        /// </summary>
        [DataMember]
        public PracticeManagementCurrency CogsWithoutRecruiting
        {
            get
            {
                return cogsWithoutRecruitingValue;
            }
            set
            {
                cogsWithoutRecruitingValue = value;
                cogsWithoutRecruitingValue.FormatStyle = NumberFormatStyle.Cogs;
            }
        }

        [DataMember]
        public PracticeManagementCurrency SaleCommissionPerHour
        {
            get;
            set;
        }

		/// <summary>
		/// Gets or sets a computed margin excluding the recruiting costs.
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency MarginWithoutRecruiting
		{
			get
			{
				return marginWithoutRecruitingValue;
			}
			set
			{
				marginWithoutRecruitingValue = value;
				marginWithoutRecruitingValue.FormatStyle = NumberFormatStyle.Margin;
			}
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

		/// <summary>
		/// Gets a margin without recruiting to revenue ratio in percentage.
		/// </summary>
		public decimal TargetMarginWithoutRecruiting
		{
			get
			{
				return Revenue != 0M ? MarginWithoutRecruiting.Value * 100M / Revenue.Value : 0M;
			}
		}
	}
}

