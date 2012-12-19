using System;
using System.Runtime.Serialization;
using System.Web;

namespace DataTransferObjects
{
    [DataContract]
	[Serializable]
	public class Pay : ICloneable
	{
		#region Constants

		public const int DefaultHoursPerYear = 2080;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets an ID of the person the pay intended for.
		/// </summary>
		[DataMember]
		public int PersonId
		{
			get;
			set;
		}

        /// <summary>
        /// When pay is effective
        /// </summary>
        [DataMember]
		public DateTime StartDate
        {
            get;
            set;
        }

        /// <summary>
        /// When pay is no longer effective
        /// </summary>
		[DataMember]
		public DateTime? EndDate
        {
            get;
            set;
        }

		/// <summary>
		/// Gets or sets an old StartDate value to be used during update
		/// </summary>
		[DataMember]
		public DateTime? OldStartDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an old EndDate value to be used during update
		/// </summary>
		[DataMember]
		public DateTime? OldEndDate
		{
			get;
			set;
		}

        private PracticeManagementCurrency _amount;
        /// <summary>
        /// Rate of pay
        /// </summary>
        [DataMember]
        public PracticeManagementCurrency Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
            }
        }

        private TimescaleType _timescale;
        /// <summary>
        /// per annum, hourly, ...
        /// </summary>
        [DataMember]
        public TimescaleType Timescale
        {
            get
            {
                return _timescale;
            }
            set
            {
                _timescale = value;

                _amount.FormatStyle = 
                _amountHourly.FormatStyle = 
                    _timescale == TimescaleType.PercRevenue 
                    ? 
                    NumberFormatStyle.Percent : NumberFormatStyle.General;
            }
        }

		/// <summary>
		/// The name for the <see cref="Timescale"/>
		/// </summary>
		[DataMember]
		public string TimescaleName
		{
			get;
			set;
		}

        private PracticeManagementCurrency _amountHourly;
        /// <summary>
        /// Gets or sets an amount per hour.
        /// </summary>
        [DataMember]
        public PracticeManagementCurrency AmountHourly
        {
            get
            {
                return _amountHourly;
            }
            set
            {
                _amountHourly = value;
            }
        }

		/// <summary>
		/// Gets or sets a number of the vacation days per year.
		/// </summary>
		[DataMember]
		public int? VacationDays
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a bonus amount.
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency BonusAmount
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a periodicity of the bonus payments.
		/// </summary>
		[DataMember]
		public int? BonusHoursToCollect
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets if the bonus is year one.
		/// </summary>
		[DataMember]
		public bool IsYearBonus
		{
			get;
			set;
		}

        /// <summary>
        /// TitleId of the person which will be active from the start date.
        /// </summary>
        [DataMember]
        public int? TitleId
        {
            set;
            get;
        }

        /// <summary>
        /// TitleName of the person which will be active from the start date.
        /// </summary>
        [DataMember]
        public string TitleName
        {
            set;
            get;
        }

        /// <summary>
        /// Practice Id of the person which will be active from the start date.
        /// </summary>
        [DataMember]
        public int? PracticeId
        {
            set;
            get;
        }

        /// <summary>
        /// Practice Name of the person which will be active from the start date.
        /// </summary>
        [DataMember]
        public string PracticeName
        {
            set;
            get;
        }

        public string HtmlEncodedPracticeName
        {   
             get
            {
                return HttpUtility.HtmlEncode(PracticeName);
            }
        }

        [DataMember]
        public bool SLTApproval
        {
            set;
            get;
        }

        [DataMember]
        public bool SLTPTOApproval
        {
            set;
            get;
        }

        public object Clone()
        {
            return (object) new Pay()
            {
                PersonId = this.PersonId,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                OldStartDate = this.OldStartDate,
                OldEndDate = this.OldEndDate,
                Amount = this.Amount,
                Timescale = this.Timescale,
                TimescaleName = this.TimescaleName,
                AmountHourly = this.AmountHourly,
                VacationDays = this.VacationDays,
                BonusAmount = this.BonusAmount,
                BonusHoursToCollect = this.BonusHoursToCollect,
                IsYearBonus = this.IsYearBonus,
                TitleId = this.TitleId,
                TitleName = this.TitleName,
                PracticeId = this.PracticeId,
                PracticeName = this.PracticeName,
                SLTApproval = this.SLTApproval,
                SLTPTOApproval = this.SLTPTOApproval
            };
        }

		#endregion
    }
}

