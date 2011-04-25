using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents a Person Role entity.
	/// </summary>
	[DataContract]
	[Serializable]
	public class PersonRole
	{
		/// <summary>
		/// Gets or sets an ID of the role
		/// </summary>
		[DataMember]
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a name of the role
		/// </summary>
		[DataMember]
		public string Name
		{
			get;
			set;
		}
	}
}

