using System.IO;
using System.Reflection;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    internal class GetExe
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Gets the application version
        /// </summary>
        protected internal static string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        /// <summary>
        ///     Gets the applications Title.
        /// </summary>
        protected internal static string Title
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(
                        typeof (AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    var titleAttribute = (AssemblyTitleAttribute) attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        /// <summary>
        ///     Gets the applications Description.
        /// </summary>
        protected internal static string Description
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(
                        typeof (AssemblyDescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    var descriptionAttribute = (AssemblyDescriptionAttribute) attributes[0];
                    if (descriptionAttribute.Description != "")
                    {
                        return descriptionAttribute.Description;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        /// <summary>
        ///     Gets the applications Copyright.
        /// </summary>
        protected internal static string Copyright
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(
                        typeof (AssemblyCopyrightAttribute), false);
                if (attributes.Length > 0)
                {
                    var copyrightAttribute = (AssemblyCopyrightAttribute) attributes[0];
                    if (copyrightAttribute.Copyright != "")
                    {
                        return copyrightAttribute.Copyright;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        /// <summary>
        ///     Gets the applications FileName.
        /// </summary>
        protected internal static string FileName
        {
            get { return Path.GetFileName(Assembly.GetExecutingAssembly().CodeBase); }
        }


        protected internal static string FilePath
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Substring(6); }
        }

        /// <summary>
        ///     Gets the applications url.
        /// </summary>
        protected internal static string AppUrl
        {
            get { return "http://simsbulkimport.uk/"; }
        }
    }
}