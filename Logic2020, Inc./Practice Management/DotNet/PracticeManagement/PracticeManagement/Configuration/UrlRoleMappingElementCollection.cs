using System;
using System.Configuration;

namespace PraticeManagement.Configuration
{
	/// <summary>
	/// Provides a collection of the mappings default page - role.
	/// </summary>
	public class UrlRoleMappingElementCollection : ConfigurationElementCollection
	{
		#region Properties

		/// <summary>
		/// Gets or sets a Term configurtation by its index.
		/// </summary>
		/// <param name="index">An index of the requested configuration element.</param>
		/// <returns>The configuration element.</returns>
		public new UrlRoleMappingElement this[string role]
		{
			get
			{
				return (UrlRoleMappingElement)BaseGet(role);
			}
			set
			{
				if (BaseGet(role) != null)
				{
					BaseRemove(role);
				}
				BaseAdd(value);
			}
		}

		#endregion

		#region ConfigurationElementCollection members

		protected override ConfigurationElement CreateNewElement()
		{
			return new UrlRoleMappingElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((UrlRoleMappingElement)element).Role;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Searches for an URL in the configuration.
		/// </summary>
		/// <param name="roles"></param>
		/// <returns></returns>
		public string FindFirstUrl(string[] roles)
		{
			string result = string.Empty;
			if (roles != null && roles.Length > 0)
			{
				for (int i = 0; i < Count; i++)
				{
					UrlRoleMappingElement element = BaseGet(i) as UrlRoleMappingElement;
					if (Array.IndexOf<string>(roles, element.Role) >= 0)
					{
						result = element.Url;
						break;
					}
				}
			}
			else
			{
				for (int i = 0; i < Count; i++)
				{
					UrlRoleMappingElement element = BaseGet(i) as UrlRoleMappingElement;
					if (element.Role == string.Empty)
					{
						result = element.Url;
						break;
					}
				}
			}

			return result;
		}

		#endregion
	}
}

