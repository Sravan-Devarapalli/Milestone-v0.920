using System;
using System.Data.SqlClient;
using DataTransferObjects;

namespace DataAccess
{
	/// <summary>
	/// Provides a custom exception to be used for this assembly.
	/// </summary>
	[Serializable]
	public class DataAccessException : Exception
	{
		#region Properties

		/// <summary>
		/// Gets or sets a fault detail.
		/// </summary>
		public DataAccessFault Detail
		{
			get;
			set;
		}

		#endregion

		#region Construction

		/// <summary>
		/// Creates a new instance of the <see cref="DataAccessException"/> class with the Detail specified.
		/// </summary>
		/// <param name="detail">The exception detail.</param>
		public DataAccessException(DataAccessFault detail)
			: base(detail.ErrorCode.ToString(), detail.InnerException)
		{
			this.Detail = detail;
		}

		/// <summary>
		/// Creates a new instance of the <see cref="DataAccessException"/> class basing on the initial exception.
		/// </summary>
		/// <param name="innerException">The initial exception.</param>
		public DataAccessException(SqlException innerException)
			: this(new DataAccessFault(innerException.Number, innerException))
		{
		}

		/// <summary>
		/// Creates a new instance of the <see cref="DataAccessException"/> class with a custom message.
		/// </summary>
		/// <param name="message">An exception reason description.</param>
		/// <param name="innerException">The initial exception.</param>
		public DataAccessException(string message, SqlException innerException)
			: base(message, innerException)
		{
		}

		#endregion
	}
}

