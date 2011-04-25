using System.Configuration;

namespace PraticeManagement.Configuration
{
	public class UrlRoleMappingElementSection : ConfigurationSection
	{
		#region Constants

		private const string MappingPropertyName = "mapping";
		private const string MappingSectionName = "practiceManagement/mapping";

		#endregion

		#region Fields

		private static ConfigurationProperty mappingProperty =
			new ConfigurationProperty(MappingPropertyName,
				typeof(UrlRoleMappingElementCollection),
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
		[ConfigurationProperty(MappingPropertyName, IsDefaultCollection = true)]
		public UrlRoleMappingElementCollection Mapping
		{
			get
			{
				return (UrlRoleMappingElementCollection)this[MappingPropertyName];
			}
			set
			{
				this[MappingPropertyName] = value;
			}
		}

		public static UrlRoleMappingElementSection Current
		{
			get
			{
				return ConfigurationManager.GetSection(MappingSectionName) as UrlRoleMappingElementSection;
			}
		}

		#endregion
		
		#region Construction

		static UrlRoleMappingElementSection()
		{
			propertiesValue = new ConfigurationPropertyCollection();
			propertiesValue.Add(mappingProperty);
		}

		#endregion
	}
}

