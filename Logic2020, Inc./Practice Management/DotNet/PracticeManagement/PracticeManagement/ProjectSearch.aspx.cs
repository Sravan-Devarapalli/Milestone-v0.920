using System;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.ProjectService;

namespace PraticeManagement
{
	public partial class ProjectSearch : PracticeManagementPageBase
	{
		private const string RegexSpecialChars = ".[]()\\-*%+|$?^";

		private string RegexReplace
		{
			get;
			set;
		}

		protected void Project_Command(object sender, CommandEventArgs e)
		{
            RedirectWithBack(
                string.Format(
                Constants.ApplicationPages.DetailRedirectFormat,
                Constants.ApplicationPages.ProjectDetail,
                e.CommandArgument),
                Constants.ApplicationPages.Projects);
		}

		protected void btnClientName_Command(object sender, CommandEventArgs e)
		{
            RedirectWithBack(
                string.Format(
                    Constants.ApplicationPages.DetailRedirectFormat,
                    Constants.ApplicationPages.ClientDetails,
                    e.CommandArgument),
                Constants.ApplicationPages.Projects);
		}

		protected void btnMilestoneName_Command(object sender, CommandEventArgs e)
		{
			string[] parts = e.CommandArgument.ToString().Split('_');

			if (parts.Length >= 2)
			{
                RedirectWithBack(string.Concat(
					string.Format(Constants.ApplicationPages.DetailRedirectFormat,
					    Constants.ApplicationPages.MilestoneDetail,
					    parts[0]), "&projectId=", parts[1]),
                     Constants.ApplicationPages.Projects);
			}
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			DisplaySearch();
		}

		/// <summary>
		/// Hightlight the text searched.
		/// </summary>
		/// <param name="result">The found value.</param>
		/// <returns>The text with searched substring highlighted.</returns>
		protected string HighlightFound(object result)
		{
			if (string.IsNullOrEmpty(RegexReplace))
			{
				RegexReplace = txtSearchText.Text;

				foreach (char specialChar in RegexSpecialChars)
				{
					RegexReplace =
						RegexReplace.Replace(specialChar.ToString(), string.Format("\\{0}", specialChar));
				}

				RegexReplace = RegexReplace.Replace(" ", "\\s");
			}

			string strResult;

			if (result != null)
			{
				strResult =
					Regex.Replace(
					result.ToString(),
					RegexReplace,
					Constants.Formatting.SearchResultFormat,
					RegexOptions.IgnoreCase);
			}
			else
			{
				strResult = string.Empty;
			}

			return strResult;
		}

		protected override void Display()
		{
			if (PreviousPage != null)
			{
				txtSearchText.Text = PreviousPage.SearchText;
				DisplaySearch();
			}
		}

		private void DisplaySearch()
		{
			Page.Validate();
			if (Page.IsValid)
			{
				using (var serviceClient = new ProjectServiceClient())
				{
					try
					{
						var projects =
							serviceClient.ProjectSearchText(
                                txtSearchText.Text, 
                                DataHelper.CurrentPerson.Id.Value);

						lvProjects.DataSource = projects;
						lvProjects.DataBind();
					}
					catch (CommunicationException)
					{
						serviceClient.Abort();
						throw;
					}
				}
			}
		}
	}
}

