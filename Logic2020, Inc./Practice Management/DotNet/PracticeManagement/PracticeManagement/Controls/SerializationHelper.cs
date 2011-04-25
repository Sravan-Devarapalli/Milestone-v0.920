using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace PraticeManagement.Controls
{
	/// <summary>
	/// Provides methods for the serialization/deserialization.
	/// </summary>
	public static class SerializationHelper
	{
		/// <summary>
		/// Serialize a specified object.
		/// </summary>
		/// <param name="value">An object to be serialized.</param>
		/// <returns>A BASE-64 encoded serialized data.</returns>
		public static string SerializeBase64(object value)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			using (MemoryStream stream = new MemoryStream())
			{
				formatter.Serialize(stream, value);

				return Convert.ToBase64String(stream.ToArray());
			}
		}

		/// <summary>
		/// Serialize a specified BASE-64 encoded string.
		/// </summary>
		/// <param name="value">A BASE-64 encoded serialized data.</param>
		/// <returns>Deserialized object or null when deserialization was net successfull.</returns>
		public static object DeserializeBase64(string value)
		{
			object result;
			BinaryFormatter formatter = new BinaryFormatter();
			try
			{
				using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(value)))
				{
					result = formatter.Deserialize(stream);
				}
			}
			catch (SerializationException)
			{
				result = null;
			}
			catch (FormatException)
			{
				result = null;
			}

			return result;
		}

		/// <summary>
		/// Serialize a value to BASE-64 encoded string and records it to the specified cookie.
		/// </summary>
		/// <param name="value">An object to be serialized.</param>
		/// <param name="cookieName">A name of cookie to be stored.</param>
		public static void SerializeCookie(object value, string cookieName)
		{
			if (HttpContext.Current != null)
			{
				string strValue = SerializeBase64(value);
				HttpContext.Current.Response.Cookies.Set(new HttpCookie(cookieName, strValue));
				HttpContext.Current.Response.Cookies[cookieName].Expires = DateTime.Today.AddYears(1);
			}
		}

		/// <summary>
		/// Retrieves a data from the specified cookie and desterilizes them.
		/// </summary>
		/// <param name="cookieName">A name of the cookie to retrieve a data from.</param>
		/// <returns>A desterilized object or null.</returns>
		public static object DeserializeCookie(string cookieName)
		{
			object result = null;
			if (HttpContext.Current != null)
			{
				HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
				if (cookie != null)
				{
					result = DeserializeBase64(cookie.Value);
				}
			}

			return result;
		}

        /// <summary>
        /// Retrieves a data from the specified cookie and desterilizes them.
        /// </summary>
        /// <param name="cookieName">A name of the cookie to retrieve a data from.</param>
        /// <returns>A desterilized object or null.</returns>
        public static T DeserializeCookie<T>(string cookieName) where T : class
        {
            object result = null;
            if (HttpContext.Current != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
                if (cookie != null)
                {
                    result = DeserializeBase64(cookie.Value);
                }
            }

            return result as T;
        }
    }
}

