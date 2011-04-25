using System;

namespace PraticeManagement
{
	public partial class AccessDenied : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			lnkAppRoot.NavigateUrl = ResolveClientUrl(Constants.ApplicationPages.AppRootUrl);
		}
	}
}

