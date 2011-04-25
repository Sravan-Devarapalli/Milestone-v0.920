using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents the OverheadRateType entity.
	/// </summary>
	[DataContract]
	[Serializable]
	public class OverheadRateType
	{
		#region Properties

		/// <summary>
		/// Gets or sets an ID of the <see cref="OverheadRateType"/> entity.
		/// </summary>
		[DataMember]
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a name of the <see cref="OverheadRateType"/> entity.
		/// </summary>
		[DataMember]
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets if the <see cref="OverheadRateType"/> entity is for a percentage rate.
		/// </summary>
		[DataMember]
		public bool IsPercentage
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a number of the hours the <see cref="OverheadRateType"/> entity is for.
		/// </summary>
		[DataMember]
		public int HoursToCollect
		{
			get;
			set;
		}

		#endregion
	}
}

