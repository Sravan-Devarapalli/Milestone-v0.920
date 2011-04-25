using System;
using System.Transactions;

namespace PracticeManagementService
{
	/// <summary>
	/// Contains common constants.
	/// </summary>
	public static class ServiceConstants
	{
		/// <summary>
		/// Default options for a distributed transaction.
		/// </summary>
		public static readonly TransactionOptions DistributedTransactionOptions =
			new TransactionOptions()
			{
				IsolationLevel = IsolationLevel.ReadCommitted,
				Timeout = TimeSpan.FromSeconds(600)
			};
	}
}

