using System.Configuration;

namespace PraticeManagement.Configuration
{
	/// <summary>
	/// Provides a description tag to location element.
	/// </summary>
	public class LocationDescriptionConfigurationSection : ConfigurationSection
	{
		#region Constants

		private const string TitlePropertyName = "title";
		private const string DescriptionPropertyName = "description";

		#endregion

		#region Fields

		private static ConfigurationProperty titleProperty =
			new ConfigurationProperty(TitlePropertyName,
				typeof(string),
				null,
				ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);
		private static ConfigurationProperty descriptionProperty =
			new ConfigurationProperty(DescriptionPropertyName,
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
		/// Gets or sets a title.
		/// </summary>
		[ConfigurationProperty(TitlePropertyName, IsRequired = true, IsKey = true)]
		public string Title
		{
			get
			{
				return (string)this[titleProperty];
			}
			set
			{
				this[titleProperty] = value;
			}
		}

		/// <summary>
		/// Gets or sets a title.
		/// </summary>
		[ConfigurationProperty(DescriptionPropertyName, IsRequired = true)]
		public string Description
		{
			get
			{
				return (string)this[descriptionProperty];
			}
			set
			{
				this[descriptionProperty] = value;
			}
		}

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a list of the configuration properties.
		/// </summary>
		static LocationDescriptionConfigurationSection()
		{
			propertiesValue = new ConfigurationPropertyCollection();
			propertiesValue.Add(titleProperty);
			propertiesValue.Add(descriptionProperty);
		}

		#endregion
	}
}

