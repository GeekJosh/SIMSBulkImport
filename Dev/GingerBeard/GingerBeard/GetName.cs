using System.IO;
using System.Reflection;

namespace GingerBeard
{
    class GetName
    {
        /// <summary>
        /// Gets the application version
        /// </summary>
        protected internal static string Version
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        /// <summary>
        /// Gets the applications Title.
        /// </summary>
        protected internal static string Title
        {
            get
            {
                object[] attributes =
                    System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(
                        typeof (AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    var titleAttribute = (AssemblyTitleAttribute) attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        /// <summary>
        /// Gets the applications Description.
        /// </summary>
        protected internal static string Description
        {
            get
            {
                object[] attributes =
                    System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(
                        typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    var descriptionAttribute = (AssemblyDescriptionAttribute)attributes[0];
                    if (descriptionAttribute.Description != "")
                    {
                        return descriptionAttribute.Description;
                    }
                }
                return Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        /// <summary>
        /// Gets the applications Copyright.
        /// </summary>
        protected internal static string Copyright
        {
            get
            {
                object[] attributes =
                    System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(
                        typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length > 0)
                {
                    var copyrightAttribute = (AssemblyCopyrightAttribute)attributes[0];
                    if (copyrightAttribute.Copyright != "")
                    {
                        return copyrightAttribute.Copyright;
                    }
                }
                return Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            }
        }
    }
}
