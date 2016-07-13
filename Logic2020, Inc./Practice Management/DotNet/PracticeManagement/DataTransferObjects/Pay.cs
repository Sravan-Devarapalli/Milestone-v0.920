using System;
using System.Runtime.Serialization;
using System.Web;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class Pay : ICloneable
    {
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

        [DataMember]
        public string TimescaleCode
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

        [DataMember]
        public int? DivisionId
        {
            get;
            set;
        }

        [DataMember]
        public string DivisionName
        {
            get;
            set;
        }

        public string HtmlEncodedDivisionName
        {
            get
            {
                return HttpUtility.HtmlEncode(DivisionName);
            }
        }

        public string HtmlEncodedPracticeName
        {
            get
            {
                return HttpUtility.HtmlEncode(PracticeName);
            }
        }

        [DataMember]
        public bool ValidateAttribution
        {
            set;
            get;
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

        [DataMember]
        public Vendor vendor
        {
            get;
            set;
        }

        public string HtmlEncodedTitleName
        {
            get
            {
                return HttpUtility.HtmlEncode(TitleName);
            }
        }

        public object Clone()
        {
            return new Pay
                {
                    PersonId = PersonId,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    OldStartDate = OldStartDate,
                    OldEndDate = OldEndDate,
                    Amount = Amount,
                    Timescale = Timescale,
                    TimescaleName = TimescaleName,
                    AmountHourly = AmountHourly,
                    VacationDays = VacationDays,
                    BonusAmount = BonusAmount,
                    BonusHoursToCollect = BonusHoursToCollect,
                    IsYearBonus = IsYearBonus,
                    TitleId = TitleId,
                    TitleName = TitleName,
                    DivisionId = DivisionId,
                    PracticeId = PracticeId,
                    PracticeName = PracticeName,
                    SLTApproval = SLTApproval,
                    SLTPTOApproval = SLTPTOApproval,
                    vendor = vendor
                };
        }

        #endregion Properties
    }
}

