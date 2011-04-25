using System;
using System.Web;

using DataTransferObjects;

namespace PraticeManagement.Controls
{
	public partial class ProjectInfo : System.Web.UI.UserControl
	{
		private const string ProjectIdKey = "ProjectId";

        public event NavigatingEventHandler Navigating;

		/// <summary>
		/// Gets or sets an ID of the selected project.
		/// </summary>
		private int? ProjectId
		{
			get
			{
				return (int?)ViewState[ProjectIdKey];
			}
			set
			{
				ViewState[ProjectIdKey] = value;
			}
		}

		/// <summary>
		/// Fills project info from project instance
		/// </summary>
		public void Populate(Project project)
		{
			ProjectId = project.Id;

			linkProjectName.Text = HttpUtility.HtmlEncode(project.Name);
            // linkProjectName.NavigateUrl = GetProjectDetailsUrl(project.Id.Value);
            linkProjectName.NavigateUrl = Utils.Generic.GetTargetUrlWithReturn(GetProjectDetailsUrl(project.Id.Value), Request.Url.AbsoluteUri);           

			if (project.Client != null)
			{
				lblClientName.Text = HttpUtility.HtmlEncode(project.Client.Name);
			}
			if (project.StartDate.HasValue)
			{
				lblStartDate.Text = project.StartDate.Value.ToShortDateString();
			}
			if (project.EndDate.HasValue)
			{
				lblEndDate.Text = project.EndDate.Value.ToShortDateString();
			}
		}

        private NavigatingEventArgs FireNavigating()
        {
            var ne = new NavigatingEventArgs();
            if (Navigating != null)
            {
                Navigating(this, ne);
            }
            return ne;
        }

        private string GetProjectDetailsUrl(int projectId)
        {
            return string.Format(
                                    Constants.ApplicationPages.DetailRedirectFormat,
                                    Constants.ApplicationPages.ProjectDetail,
                                    projectId);
        }
	}
}

