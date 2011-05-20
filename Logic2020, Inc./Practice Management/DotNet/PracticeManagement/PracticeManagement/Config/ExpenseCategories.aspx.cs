using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ExpenseCategoryService;
using PraticeManagement.Controls;
using System.ServiceModel;
using System.ComponentModel;
namespace PraticeManagement.Config
{
    public partial class ExpenseCategories : PracticeManagementPageBase
    {
        protected override void Display()
        {
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            msgLabel.Text = string.Empty;
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
            {
                e.Cancel = true;
            }
            else
            {
                string newExpenseCategory = e.NewValues["Name"].ToString().Trim();
                string oldExpenseCategory = e.OldValues["Name"].ToString().Trim();

                if (newExpenseCategory != oldExpenseCategory)
                {
                    if (IsExpenseCategoryAlreadyExisting(newExpenseCategory))
                    {
                        CustomValidator cvCategoryExists = gvCategories.Rows[e.RowIndex].FindControl("cvCategoryExists") as CustomValidator;
                        cvCategoryExists.IsValid = false;
                        e.Cancel = true;
                    }
                }
            }
        }

        private bool IsExpenseCategoryAlreadyExisting(string newExpenseCategory)
        {
            using (var serviceClient = new ExpenseCategoryServiceClient())
            {
                ExpenseCategory[] categoryList = serviceClient.GetCategoryList();
                foreach (ExpenseCategory item in categoryList)
                {
                    if (item.Name.ToLower() == newExpenseCategory.ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        protected void ibtnInsertCategory_Click(object sender, EventArgs e)
        {
            showPlusIcon(false);
        }

        private void showPlusIcon(bool showPlus)
        {
            ibtnInsertCategory.Visible = showPlus;
            ibtnInsert.Visible = ibtnCancel.Visible = txtNewCategoryName.Visible = !showPlus;

            if (!showPlus)
            {
                txtNewCategoryName.Text = string.Empty;
            }
        }

        protected void ibtnInsert_Clicked(object sender, EventArgs e)
        {
            Page.Validate(txtNewCategoryName.ValidationGroup);
            if (Page.IsValid)
            {
                try
                {
                    SaveCategory(new ExpenseCategory() { Name = txtNewCategoryName.Text });

                    //txtNewCategoryName.Text = string.Empty;
                    gvCategories.DataBind();

                    showPlusIcon(true);
                }
                catch (FaultException<ExceptionDetail> ex)
                {
                    msgLabel.Text = ex.Message;
                }
            }
        }

        protected void ibtnCancel_Clicked(object sender, EventArgs e)
        {
            showPlusIcon(true);
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

            if (internalException != null)
            {
                custDataAccessError.ErrorMessage = custDataAccessError.ToolTip = internalException.Message;
            }
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

