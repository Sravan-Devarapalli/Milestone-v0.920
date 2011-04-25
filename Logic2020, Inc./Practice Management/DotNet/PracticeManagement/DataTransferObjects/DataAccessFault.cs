using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Provides the application specific Exception to be used when access the data.
	/// </summary>
	[DataContract]
	[Serializable]
	public class DataAccessFault
	{
		#region Properties

		/// <summary>
		/// Gets or sets an <see cref="ErrorCode"/> of the exceptional situation.
		/// </summary>
		[DataMember]
		public ErrorCode ErrorCode
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the fault reason description.
		/// </summary>
		[DataMember]
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an initial exception.
		/// </summary>
		[DataMember]
		public Exception InnerException
		{
			get;
			set;
		}

		#endregion

		#region Construction

		/// <summary>
		/// Creates a new instance of the <see cref="DataAccessException"/> class.
		/// </summary>
		/// <param name="errorCode">The error code of teh original exception.</param>
		/// <param name="innerException">The original exception.</param>
		public DataAccessFault(int errorCode, Exception innerException)
		{
			ErrorCode =
				Enum.GetName(typeof(ErrorCode), errorCode) != null ? (ErrorCode)errorCode : ErrorCode.Unknown;
			Message = innerException != null ? innerException.Message : string.Empty;
			this.InnerException = innerException;
		}

		#endregion
	}
}

