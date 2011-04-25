using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents a timescale entity
	/// </summary>
	[DataContract]
	[Serializable]
	public class Timescale
	{
		/// <summary>
		/// Gets or sets a <see cref="Timescale"/> Id value.
		/// </summary>
		[DataMember]
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a name of the <see cref="Timescale"/>.
		/// </summary>
		[DataMember]
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a default Terms for the <see cref="Timescale"/>.
		/// </summary>
		[DataMember]
		public int? DefaultTerms
		{
			get;
			set;
		}
	}
}

