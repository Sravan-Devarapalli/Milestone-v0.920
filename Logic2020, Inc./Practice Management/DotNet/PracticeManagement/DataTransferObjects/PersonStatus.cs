using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents the status of the <see cref="Person"/>.
	/// </summary>
	[DataContract]
	[Serializable]
	public class PersonStatus : IEquatable<PersonStatus>
	{
		#region Properties

		/// <summary>
		/// Gets or sets an ID of the status.
		/// </summary>
		[DataMember]
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Sets or sets a name of the status.
		/// </summary>
		[DataMember]
		public string Name
		{
			get;
			set;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns the hash code for the instance.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		/// <summary>
		/// Determines whether the instance is equals to the another one.
		/// </summary>
		/// <param name="obj">The object to compare with.</param>
		/// <returns>true if the objects are equal and false otherwise.</returns>
		public override bool Equals(object obj)
		{
			return (obj is PersonStatus) && ((PersonStatus)obj).Id == Id;
		}        

        public PersonStatusType ToStatusType()
        {
            return ToStatusType(this.Id);
        }

        public static PersonStatusType? ToStatusType(string status)
        {
            if (string.IsNullOrEmpty(status))
                return null;

            return ToStatusType(Convert.ToInt32(status));
        }

        public static PersonStatusType ToStatusType(int statusId)
        {
            return (PersonStatusType) statusId;
        }



		#endregion

        #region IEquatable<PersonStatus> Members

        public bool Equals(PersonStatus other)
        {
            return this.Id == other.Id;
        }

        #endregion
    }
}
