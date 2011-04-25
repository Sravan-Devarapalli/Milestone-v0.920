using System.Configuration;

namespace PraticeManagement.Configuration
{
	/// <summary>
	/// Provides a default page path for the specific role.
	/// </summary>
	public class UrlRoleMappingElement : ConfigurationElement
	{
		#region Constants

		private const string RolePropertyName = "role";
		private const string UrlPropertyName = "url";

		#endregion

		#region Fields

		private static ConfigurationProperty roleProperty =
			new ConfigurationProperty(RolePropertyName,
				typeof(string),
				null,
				ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);
		private static ConfigurationProperty urlProperty =
			new ConfigurationProperty(UrlPropertyName,
				typeof(string),
				null,
				ConfigurationPropertyOptions.IsRequired);
		private static ConfigurationPropertyCollection propertiesValue;

		#endregion

		#region Properties

		/// <summary>
		/// Gets a list of the configuration properties.
		/// </summary>
		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return propertiesValue;
			}
		}

		/// <summary>
		/// Gets or sets a role name.
		/// </summary>
		[ConfigurationProperty(RolePropertyName, IsKey = true, IsRequired = true)]
		public string Role
		{
			get
			{
				return (string)this[roleProperty];
			}
			set
			{
				this[roleProperty] = value;
			}
		}

		/// <summary>
		/// Gets or sets an URL.
		/// </summary>
		[ConfigurationProperty(UrlPropertyName, IsRequired = true)]
		public string Url
		{
			get
			{
				return (string)this[urlProperty];
			}
			set
			{
				this[urlProperty] = value;
			}
		}

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a list of the configuration properties.
		/// </summary>
		static UrlRoleMappingElement()
		{
			propertiesValue = new ConfigurationPropertyCollection();
			propertiesValue.Add(roleProperty);
			propertiesValue.Add(urlProperty);
		}

		#endregion
	}
}

