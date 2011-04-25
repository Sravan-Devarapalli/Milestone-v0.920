using System.Configuration;

namespace PraticeManagement.Configuration
{
	/// <summary>
	/// Provides a list of the Practice Management Terms
	/// </summary>
	public class TermConfigurationElementCollection : ConfigurationElementCollection
	{
		#region Properties

		/// <summary>
		/// Gets or sets a Term configurtation by its index.
		/// </summary>
		/// <param name="index">An index of the requested configuration element.</param>
		/// <returns>The configuration element.</returns>
		public TermConfigurationElement this[int index]
		{
			get
			{
				return (TermConfigurationElement)BaseGet(index);
			}
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}

		#endregion

		#region ConfigurationElementCollection members

		protected override ConfigurationElement CreateNewElement()
		{
			return new TermConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((TermConfigurationElement)element).Frequency;
		}

		/// <summary>
		/// Determines an index of the specified <see cref="TermConfigurationElement"/> object.
		/// </summary>
		/// <param name="term">The <see cref="TermConfigurationElement"/> object to be searched.</param>
		/// <returns>An in dex of the specified <see cref="TermConfigurationElement"/> object.</returns>
		public int IndexOf(TermConfigurationElement term)
		{
			return BaseIndexOf(term);
		}

		/// <summary>
		/// Adds a specified <see cref="TermConfigurationElement"/> object to the collection.
		/// </summary>
		/// <param name="term">The <see cref="TermConfigurationElement"/> object to be added to.</param>
		public void Add(TermConfigurationElement term)
		{
			BaseAdd(term);
		}

		/// <summary>
		/// Removes a specified <see cref="TermConfigurationElement"/> object from the collection.
		/// </summary>
		/// <param name="term">The <see cref="TermConfigurationElement"/> object to be removed from.</param>
		public void Remove(TermConfigurationElement term)
		{
			if (BaseIndexOf(term) >= 0)
			{
				BaseRemove(BaseIndexOf(term));
			}
		}

		/// <summary>
		/// Removes a specified <see cref="TermConfigurationElement"/> object by its index.
		/// </summary>
		/// <param name="index">
		/// The index of the <see cref="TermConfigurationElement"/> object to be removed.
		/// </param>
		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

		/// <summary>
		/// Clears the collection.
		/// </summary>
		public void Clear()
		{
			BaseClear();
		}

		#endregion
	}
}

