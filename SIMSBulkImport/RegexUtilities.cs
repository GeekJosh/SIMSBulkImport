using System;
using System.Globalization;
using System.Text.RegularExpressions;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    //  Reference: http://msdn.microsoft.com/en-us/library/01escwtf.aspx
    public class RegexUtilities
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private bool invalid;

        public bool IsValidEmail(string strIn)
        {
            logger.Log(LogLevel.Trace, "Trace:: RegexUtilities.IsValidEmail(strIn: " + strIn + ")");
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper);
            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                RegexOptions.IgnoreCase);
        }

        private string DomainMapper(System.Text.RegularExpressions.Match match)
        {
            logger.Log(LogLevel.Trace, "Trace:: RegexUtilities.DomainMapper(match: " + match + ")");
            // IdnMapping class with default property values.
            var idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }

        // Reference: http://regexlib.com/REDetails.aspx?regexp_id=1203
        public bool IsValidTelephone(string subjectString)
        {
            logger.Log(LogLevel.Trace, "Trace:: RegexUtilities.IsValidTelephone(subjectString: " + subjectString + ")");
            var regexObj = new Regex(@"^[0-9\s\(\)\+\-]+$");
            if (regexObj.IsMatch(subjectString))
            {
                string formattedPhoneNumber =
                    regexObj.Replace(subjectString, "($1) $2-$3");
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}