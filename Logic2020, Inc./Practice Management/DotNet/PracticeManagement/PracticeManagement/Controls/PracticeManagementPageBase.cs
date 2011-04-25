using System;
using System.Web;
using System.Web.UI;

namespace PraticeManagement.Controls
{
	/// <summary>
	/// Provides a base WEB page class for the application.
	/// </summary>
	public abstract class PracticeManagementPageBase : Page
	{
		#region Constants

		private const string ReturnUrlKey = "returnTo";
	    private const string IdArgument = "Id";
		private const string IdArgumentFormat1 = "?id={0}";
		private const string IdArgumentFormat2 = "&id={0}";
		private const string ValidatedKey = "PageValidated";

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value of the query string argument which must specify a return URL.
		/// </summary>
		protected string ReturnUrl
		{
			get
			{
				return Request.QueryString[ReturnUrlKey];
			}
		}

		/// <summary>
		/// Gets a value if the "ID" query string argument
		/// </summary>
		public int? SelectedId
		{
			get
			{
				return GetArgumentInt32(IdArgument);
			}
		}

		/// <summary>
		/// Gets a specified value from the query string and tries to convert it into the int value.
		/// </summary>
		/// <param name="argumentName">The name of the argument to be converted.</param>
		/// <returns>The conversion result whenever it's possible and null otherwise.</returns>
		protected int? GetArgumentInt32(string argumentName)
		{
			string tmpId = Request.QueryString[argumentName];
			int result;
			if (!int.TryParse(tmpId, out result))
			{
				return null;
			}

			return result;
		}

		/// <summary>
		/// Gets a specified value from the query string and tries to convert it into the DateTime value.
		/// </summary>
		/// <param name="argumentName">The name of the argument to be converted.</param>
		/// <returns>The conversion result whenever it's possible and null otherwise.</returns>
		protected DateTime? GetArgumentDateTime(string argumentName)
		{
			string tmpValue = Request.QueryString[argumentName];
			DateTime result;
			if (!DateTime.TryParse(tmpValue, out result))
			{
				return null;
			}

			return result;
		}

		protected bool IsValidated
		{
			get
			{
				return Convert.ToBoolean(ViewState[ValidatedKey]);
			}
			private set
			{
				ViewState[ValidatedKey] = value;
			}
		}

		/// <summary>
		/// Gets or sets if the page's data is dirty (not saved)
		/// </summary>
		public bool IsDirty
		{
			get
			{
                return Master is PracticeManagementMain ? ((PracticeManagementMain) Master).IsDirty : false;
			}
			set
			{
                if (Master is PracticeManagementMain)
				{
                    ((PracticeManagementMain) Master).IsDirty = value;
				}
			}
		}

		/// <summary>
		/// Gets whether the user selected save dirty data.
		/// </summary>
		protected bool SaveDirty
		{
			get
			{
                return Master is PracticeManagementMain ? ((PracticeManagementMain) Master).SaveDirty : false;
			}
			set
			{
                if (Master is PracticeManagementMain)
				{
                    ((PracticeManagementMain) Master).SaveDirty = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets if continue without saving is allowed.
		/// </summary>
		protected bool AllowContinueWithoutSave
		{
			get
			{
                return Master is PracticeManagementMain ? ((PracticeManagementMain) Master).AllowContinueWithoutSave : true;
			}
			set
			{
                if (Master is PracticeManagementMain)
				{
                    ((PracticeManagementMain) Master).AllowContinueWithoutSave = value;
				}
			}
		}

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			//ScriptManager scriptManager = ScriptManager.GetCurrent(Page);
            //  Commented out to fix Chrome issues.
            //if (scriptManager != null)
            //{
            //    scriptManager.EnablePartialRendering = Request.UserAgent.ToLower().IndexOf("safari") < 0;
            //}

            if (Master is PracticeManagementMain)
			{
                ((PracticeManagementMain) Master).Navigating += Master_Navigating;
			}
		}

		/// <summary>
		/// Displayes the data.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!IsPostBack)
			{
				Display();
			}

            //Response.CacheControl = Constants.HttpHeaders.CacheControlNoCache;
            //Response.Cache.SetNoStore();
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

            //if (IsPostBack && IsDirty)
            //{
            //    ScriptManager.RegisterStartupScript(this, GetType(), "SetDirty", "setDirty();", true);
            //}
		}

		// Just for debugging purposes
		protected override void OnError(EventArgs e)
		{
			base.OnError(e);
		}

		protected void ClearDirty()
		{
			IsDirty = false;
			SaveDirty = false;
			ScriptManager.RegisterStartupScript(this, GetType(), "SetDirty", "clearDirty();", true);
		}

		/// <summary>
		/// Displayes the data.
		/// </summary>
		protected abstract void Display();

		protected virtual bool ValidateAndSave()
		{
			return true;
		}

		/// <summary>
		/// Remembers if the page was already validated.
		/// </summary>
		public override void Validate()
		{
			base.Validate();

			IsValidated = true;
		}

		/// <summary>
		/// Performs a redirect to the page which is specified with the value of <see cref="ReturnUrl"/> property.
		/// </summary>
		public void ReturnToPreviousPage()
		{
			if (!string.IsNullOrEmpty(ReturnUrl))
			{
				Response.Redirect(ReturnUrl);
			}
		}

		/// <summary>
		/// Redirects to the specified URL.
		/// </summary>
		/// <param name="redirectUrl">The URL to redirect to.</param>
		public void Redirect(string redirectUrl)
		{
			RedirectWithBack(redirectUrl, Request.Url.AbsoluteUri);
		}

		/// <summary>
		/// Redirects to the specified URL and provides the back redirect for new records.
		/// </summary>
		/// <param name="redirectUrl">The URL to redirect to.</param>
		/// <param name="newId">An ID of the new record.</param>
		public void Redirect(string redirectUrl, string newId)
		{
			string backUrl = Request.Url.AbsoluteUri;
			if (!SelectedId.HasValue)
			{
				backUrl +=
					string.Format(
					backUrl.IndexOf(Constants.QueryStringParameterNames.QueryStringSeparator) < 0 ? IdArgumentFormat1 : IdArgumentFormat2,
					newId);
			}
			RedirectWithBack(redirectUrl, backUrl);
		}

		/// <summary>
		/// Redirects to the specified page and supply a query string argument for bacjward redirect.
		/// </summary>
		/// <param name="redirectUrl">An URL to redirect to.</param>
		/// <param name="backUrl">A back URL.</param>
		protected virtual void RedirectWithBack(string redirectUrl, string backUrl)
		{
		    Utils.Generic.RedirectWithReturnTo(redirectUrl, backUrl, Response);
		}

	    /// <summary>
		/// Redirects the user to the previous page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnCancel_Click(object sender, EventArgs e)
		{
			ReturnToPreviousPage();
		}

		private void Master_Navigating(object sender, NavigatingEventArgs e)
		{
			e.Cancel = SaveDirty && !ValidateAndSave() && !Page.IsValid;
		}

		#endregion
	}
}

