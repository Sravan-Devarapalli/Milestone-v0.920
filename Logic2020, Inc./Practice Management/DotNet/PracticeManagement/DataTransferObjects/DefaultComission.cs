using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
	[Serializable]
	public class DefaultCommission
    {
        private DateTime _startDate;
        private DateTime _endDate;
        private CommissionType _type;
        private decimal _fractionOfMargin;

		/// <summary>
		/// Gets or sets an ID of the person the commission belongs to.
		/// </summary>
		[DataMember]
		public int PersonId
		{
			get;
			set;
		}

        /// <summary>
        /// Date when this became the default commission
        /// </summary>
		[DataMember]
		public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        /// <summary>
        /// Date when this will no longer be the default commission
        /// </summary>
		[DataMember]
		public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }

        /// <summary>
        /// What type of is this?
        /// </summary>
		[DataMember]
		public CommissionType TypeOfCommission
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Fraction of margin that is paid as commission
        /// </summary>
		[DataMember]
		public decimal FractionOfMargin
        {
            get { return _fractionOfMargin; }
            set { _fractionOfMargin = value; }
        }


		/// <summary>
		/// Gets or sets a type of the magring (own, sub-ordinates, etc.)
		/// </summary>
		[DataMember]
		public int? MarginTypeId
		{
			get;
			set;
		}
    }
}
