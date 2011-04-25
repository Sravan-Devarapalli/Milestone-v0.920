using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents the stutus of the <see cref="Project"/>.
	/// </summary>
	[DataContract]
	[Serializable]
	public class ProjectStatus
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

        #region Computed values

        public ProjectStatusType StatusType
        {
            get
            {
                return (ProjectStatusType)Id;
            }
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
			return (obj is ProjectStatus) && ((ProjectStatus)obj).Id == Id;
		}

		#endregion
	}
}
