using System.Collections.Generic;
using System.ServiceModel;
using DataTransferObjects;

namespace PracticeManagementService
{
	[ServiceContract]
	public interface IExpenseCategoryService
	{
		/// <summary>
		/// Retrieves the list of the categories for the fixed expenses
		/// </summary>
		/// <returns>The list of the <see cref="ExpenseCategory"/> objects.</returns>
		[OperationContract]
		[FaultContract(typeof(DataAccessFault))]
		List<ExpenseCategory> GetCategoryList();

		/// <summary>
		/// Saves an <see cref="ExpenseCategory"/> into the database.
		/// </summary>
		/// <param name="category">The <see cref="ExpenseCategory"/> the data be saved from.</param>
		[OperationContract]
		[FaultContract(typeof(DataAccessFault))]
		void SaveExpenseCategoryDetail(ExpenseCategory category);

		/// <summary>
		/// Deletes an <see cref="ExpenseCategory"/> from the database.
		/// </summary>
		/// <param name="category">The <see cref="ExpenseCategory"/> the data be deleted from.</param>
		[OperationContract]
		[FaultContract(typeof(DataAccessFault))]
		void DeleteExpenseCategory(ExpenseCategory category);
	}
}

