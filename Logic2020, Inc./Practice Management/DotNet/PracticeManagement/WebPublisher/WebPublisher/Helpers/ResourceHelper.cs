using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Resources;
using System.Windows;

namespace WebPublisher.Helpers
{
    public class ResourceHelper
    {

        public static string ExecutingAssemblyName
        {
            get
            {
                string name = System.Reflection.Assembly.GetExecutingAssembly().FullName;
                return name.Substring(0, name.IndexOf(','));
            }
        }

        public static Uri GetUri(string relativeUri, string assemblyName)
        {
            Uri uri = new Uri(assemblyName + ";component/" + relativeUri, UriKind.Relative);
            return uri;
        }

        public static Stream GetStream(string relativeUri, string assemblyName)
        {
            Uri uri = new Uri(assemblyName + ";component/" + relativeUri, UriKind.Relative);
            StreamResourceInfo res = null;
            res = Application.GetResourceStream(uri);
            if (res == null)
            {
                res = Application.GetResourceStream(new Uri(relativeUri, UriKind.Relative));
            }
            if (res != null)
            {
                return res.Stream;
            }
            else
            {
                return null;
            }
        }

        public static Stream GetStream(string relativeUri)
        {
            return GetStream(relativeUri, ExecutingAssemblyName);
        }

        public static BitmapImage GetBitmapByUri(string relativeUri, string assemblyName)
        {
            BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = GetUri(relativeUri, assemblyName);
                bmp.EndInit();
                return bmp;
        }
        public static BitmapImage GetBitmap(string relativeUri, string assemblyName)
        {
            Stream s = GetStream(relativeUri, assemblyName);
            if (s == null) return null;
            using (s)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = s;
                bmp.EndInit();
                return bmp;
            }
        }

        public static BitmapImage GetBitmap(string relativeUri)
        {
            return GetBitmap(relativeUri, ExecutingAssemblyName);
        }

        public static string GetString(string relativeUri, string assemblyName)
        {
            Stream s = GetStream(relativeUri, assemblyName);
            if (s == null) return null;
            using (StreamReader reader = new StreamReader(s))
            {
                return reader.ReadToEnd();
            }
        }

        public static string GetString(string relativeUri)
        {
            return GetString(relativeUri, ExecutingAssemblyName);
        }

        //public static FontSource GetFontSource(string relativeUri, string assemblyName)
        //{
        //    Stream s = GetStream(relativeUri, assemblyName);
        //    if (s == null) return null;
        //    using (s)
        //    {
        //        return new FontSource(s);
        //    }
        //}

        //public static FontSource GetFontSource(string relativeUri)
        //{
        //    return GetFontSource(relativeUri, ExecutingAssemblyName);
        //}

        //public static object GetXamlObject(string relativeUri, string assemblyName)
        //{
        //    string str = GetString(relativeUri, assemblyName);
        //    if (str == null) return null;
        //    object obj = System.Windows.Markup.XamlReader.Load(str);
        //    return obj;
        //}

        //public static object GetXamlObject(string relativeUri)
        //{
        //    return GetXamlObject(relativeUri, ExecutingAssemblyName);
        //}

    }
}

