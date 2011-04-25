using System.Configuration;

namespace PraticeManagement.Configuration
{
	/// <summary>
	/// Provides an element for the collection of the Practice Management Terms.
	/// </summary>
	public class TermConfigurationElement : ConfigurationElement
	{
		#region Constants

		private const string NamePropertyName = "name";
		private const string FrequencyPropertyName = "frequency";

		#endregion

		#region Fields

		private static ConfigurationProperty nameProperty =
			new ConfigurationProperty(NamePropertyName,
				typeof(string),
				null,
				ConfigurationPropertyOptions.IsRequired);
		private static ConfigurationProperty frequencyProperty =
			new ConfigurationProperty(FrequencyPropertyName,
				typeof(int),
				1,
				ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);
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
		/// Gets or sets a term's name.
		/// </summary>
		[ConfigurationProperty(NamePropertyName, IsRequired = true)]
		public string Name
		{
			get
			{
				return (string)this[nameProperty];
			}
			set
			{
				this[nameProperty] = value;
			}
		}

		/// <summary>
		/// Gets or sets a term's frequincy.
		/// </summary>
		[ConfigurationProperty(FrequencyPropertyName, IsKey = true, IsRequired = true)]
		public int Frequency
		{
			get
			{
				return (int)this[frequencyProperty];
			}
			set
			{
				this[frequencyProperty] = value;
			}
		}

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a list of the configuration properties.
		/// </summary>
		static TermConfigurationElement()
		{
			propertiesValue = new ConfigurationPropertyCollection();
			propertiesValue.Add(nameProperty);
			propertiesValue.Add(frequencyProperty);
		}

		#endregion
	}
}

