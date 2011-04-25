using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// A commission paid to a person fro recruiting a consultant
    /// </summary>
    /// <remarks>
    /// A person in the role of recruiter will have one or two of these
    /// for each recruit.  The amount of the commission is constant for
    /// the recruiter and doesn't depend on who is recruited
    /// </remarks>
    [DataContract]
	[Serializable]
	public class RecruiterCommission : IComparable
    {
        /// <summary>
        /// Number of hours a recruit works before a recruiter
        /// gets comission
        /// </summary>
        [DataMember]
        public int HoursToCollect
        {
            get;
            set;
        }

        /// <summary>
        /// The amount of the comission awarded
        /// </summary>
        [DataMember]
        public PracticeManagementCurrency Amount
        {
            get;
            set;
        }

        /// <summary>
        /// This <see cref="Person"/> works some hours for this commission to be paid
        /// </summary>
        [DataMember]
		public Person Recruit
        {
            get;
            set;
        }

		/// <summary>
		/// Gets or sets an ID of a <see cref="Person"/> is a recruiter.
		/// </summary>
		[DataMember]
		public int RecruiterId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an old value of the HoursToCollect field.
		/// </summary>
		/// <remarks>Uses for updating the data.</remarks>
		[DataMember]
		public int? Old_HoursToCollect
		{
			get;
			set;
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			RecruiterCommission value = (RecruiterCommission)obj;
			return value.HoursToCollect - HoursToCollect;
		}

		#endregion
	}
}
