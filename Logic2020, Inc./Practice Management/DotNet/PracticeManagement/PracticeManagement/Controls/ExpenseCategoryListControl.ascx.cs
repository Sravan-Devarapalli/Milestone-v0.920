using System;
using System.ComponentModel;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ExpenseCategoryService;

namespace PraticeManagement.Controls
{
	public partial class ExpenseCategoryListControl : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
        #region Fields

        private ExceptionDetail internalException;

        #endregion

        #region Methods

        protected void Page_Init(object sender, EventArgs e)
        {
            odsCategories.DataObjectTypeName = typeof(ExpenseCategory).AssemblyQualifiedName;
            odsCategories.TypeName = GetType().AssemblyQualifiedName;
        }

        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            Page.Validate(btnAddCategory.ValidationGroup);
            if (Page.IsValid)
            {
                try
                {
                    SaveCategory(new ExpenseCategory() { Name = txtNewCategoryName.Text });

                    txtNewCategoryName.Text = string.Empty;
                    gvCategories.DataBind();
                }
                catch (FaultException<ExceptionDetail>)
                {
                    Page.Validate();
                }
            }
        }

        /// <summary>
        /// Handles the data access errors
        /// </summary>
        /// <param name="e"></param>
        protected void odsCategories_Change(object sender, ObjectDataSourceStatusEventArgs e)
        {
            FaultException<ExceptionDetail> ex =
                e.Exception != null ? e.Exception.InnerException as FaultException<ExceptionDetail> : null;

            if (ex != null)
            {
                internalException = ex.Detail;
                Page.Validate(custCategoriyDelete.ValidationGroup);
                e.ExceptionHandled = true;
            }
        }

        protected void gvCategories_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            Page.Validate(this.vsumUpdate.ValidationGroup);
            if (!Page.IsValid)
                e.Cancel = true;
        }

        #region Validation

        protected void custCategoriyDelete_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid =
                internalException == null ||
                internalException.Message != ErrorCode.ExpenseCategoryIsInUse.ToString();
        }

        protected void custDataAccessError_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid =
                internalException == null ||
                internalException.Message == ErrorCode.ExpenseCategoryIsInUse.ToString();

            custDataAccessError.ErrorMessage = custDataAccessError.ToolTip = internalException.Message;
        }
        #endregion

        #region Data Access

        /// <summary>
        /// Retrives the list of the categories from the WCF Service.
        /// </summary>
        /// <returns>An array of the <see cref="ExpenseCategory"/> objects.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public static ExpenseCategory[] GetCategoryList()
        {
            using (ExpenseCategoryServiceClient serviceClient = new ExpenseCategoryServiceClient())
            {
                try
                {
                    return serviceClient.GetCategoryList();
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
                catch (FaultException)
                {
                    serviceClient.Abort();
                    throw;
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Saves the <see cref="ExpenseCategory"/> to the data store.
        /// </summary>
        /// <param name="category">The <see cref="ExpenseCategory"/> object to the data be saved from.</param>
        [DataObjectMethod(DataObjectMethodType.Update)]
        public static void SaveCategory(ExpenseCategory category)
        {
            using (ExpenseCategoryServiceClient serviceClient = new ExpenseCategoryServiceClient())
            {
                try
                {
                    serviceClient.SaveExpenseCategoryDetail(category);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
                catch (FaultException)
                {
                    serviceClient.Abort();
                    throw;
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes the <see cref="ExpenseCategory"/> from the data store.
        /// </summary>
        /// <param name="category">The <see cref="ExpenseCategory"/> object to the data be deleted from.</param>
        public static void DeleteCategory(ExpenseCategory category)
        {
            using (ExpenseCategoryServiceClient serviceClient = new ExpenseCategoryServiceClient())
            {
                try
                {
                    serviceClient.DeleteExpenseCategory(category);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
                catch (FaultException)
                {
                    serviceClient.Abort();
                    throw;
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        #endregion

        #endregion
	}
}
