using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// Commission paid as fraction of margin, usually for sales, sometimes
    /// for practice management
    /// </summary>
    [DataContract]
	[Serializable]
	public class Commission
    {
		public const int OwnMargin = 1;

        private Project _projectWithMargin;
        private decimal _fractionOfMargin;
        private CommissionType _type;

		/// <summary>
		/// Gets or sets an ID of the commission.
		/// </summary>
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public int? Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a person the commission belongs to.
		/// </summary>
		/// <remarks>The null value used for specifying the record should be deleted.</remarks>
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public int? PersonId
		{
			get;
			set;
		}

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string PersonFirstName
        {
            get;
            set;
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string PersonLastName
        {
            get;
            set;
        }

        /// <summary>
        /// Project used to calculate amount paid
        /// </summary>
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public Project ProjectWithMargin
        {
            get { return _projectWithMargin; }
            set { _projectWithMargin = value; }
        }

        /// <summary>
        /// portion of project's margin paid as commission
        /// </summary>
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public decimal FractionOfMargin
        {
            get { return _fractionOfMargin; }
            set { _fractionOfMargin = value; }
        }

        /// <summary>
        /// What kind of commissio?
        /// </summary>
        [DataMember(IsRequired=false, EmitDefaultValue=false)]
        public CommissionType TypeOfCommission
        {
            get { return _type; }
            set { _type = value; }
        }

		/// <summary>
		/// Gets or sets a margin type (own, sub-ordinates, etc.)
		/// </summary>
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public int? MarginTypeId
		{
			get;
			set;
		}
    }
}
