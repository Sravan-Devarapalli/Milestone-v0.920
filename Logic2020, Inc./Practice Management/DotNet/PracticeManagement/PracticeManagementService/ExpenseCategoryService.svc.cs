using System.Collections.Generic;
using System.ServiceModel.Activation;
using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class ExpenseCategoryService : IExpenseCategoryService
	{
		/// <summary>
		/// Retrieves the list of the categories for the fixed expenses
		/// </summary>
		/// <returns>The list of the <see cref="ExpenseCategory"/> objects.</returns>
		public List<ExpenseCategory> GetCategoryList()
		{
			return ExpenseCategoryDAL.ExpenseCategoryListAll();
		}

		/// <summary>
		/// Saves an <see cref="ExpenseCategory"/> into the database.
		/// </summary>
		/// <param name="category">The <see cref="ExpenseCategory"/> the data be saved from.</param>
		public void SaveExpenseCategoryDetail(ExpenseCategory category)
		{
			if (category.Id == default(int))
			{
				ExpenseCategoryDAL.ExpenseCategoryInsert(category);
			}
			else
			{
				ExpenseCategoryDAL.ExpenseCategoryUpdate(category);
			}
		}

		/// <summary>
		/// Deletes an <see cref="ExpenseCategory"/> from the database.
		/// </summary>
		/// <param name="category">The <see cref="ExpenseCategory"/> the data be deleted from.</param>
		public void DeleteExpenseCategory(ExpenseCategory category)
		{
			ExpenseCategoryDAL.ExpenseCategoryDelete(category);
		}
	}
}

