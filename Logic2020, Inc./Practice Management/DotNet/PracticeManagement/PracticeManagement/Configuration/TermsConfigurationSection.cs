using System.Configuration;

namespace PraticeManagement.Configuration
{
	public class TermsConfigurationSection : ConfigurationSection
	{
		#region Constants

		private const string TermsPropertyName = "terms";
		private const string TermsSectionName = "practiceManagement/terms";

		#endregion

		#region Fields

		private static ConfigurationProperty termsProperty =
			new ConfigurationProperty(TermsPropertyName,
				typeof(TermConfigurationElementCollection),
				null,
				ConfigurationPropertyOptions.IsDefaultCollection);
		private static ConfigurationPropertyCollection propertiesValue;

		#endregion

		#region Properties

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return propertiesValue;
			}
		}

		/// <summary>
		/// Gets or sets a collection of the <see cref="TermConfigurationElement"/> objects.
		/// </summary>
		[ConfigurationProperty(TermsPropertyName, IsDefaultCollection = true)]
		public TermConfigurationElementCollection Terms
		{
			get
			{
				return (TermConfigurationElementCollection)this[TermsPropertyName];
			}
			set
			{
				this[TermsPropertyName] = value;
			}
		}

		public static TermsConfigurationSection Current
		{
			get
			{
				return ConfigurationManager.GetSection(TermsSectionName) as TermsConfigurationSection;
			}
		}

		#endregion

		#region Construction

		static TermsConfigurationSection()
		{
			propertiesValue = new ConfigurationPropertyCollection();
			propertiesValue.Add(termsProperty);
		}

		#endregion
	}
}

