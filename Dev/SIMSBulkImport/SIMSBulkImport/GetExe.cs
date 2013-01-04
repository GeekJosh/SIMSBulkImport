/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System.IO;
using System.Reflection;

namespace Matt40k.SIMSBulkImport
{
    class GetExe
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

        /// <summary>
        /// Gets the applications FileName.
        /// </summary>
        protected internal static string FileName
        {
            get
            {
                return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            }
        }


        protected internal static string FilePath
        {
            get
            {
                return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Substring(6);
            }
        }

        /// <summary>
        /// Gets the applications url.
        /// </summary>
        protected internal static string AppUrl
        {
            get
            {
                return "http://simsbulkimport.codeplex.com/";
            }
        }
    }
}
