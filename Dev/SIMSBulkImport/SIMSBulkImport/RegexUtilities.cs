//  Ref: http://msdn.microsoft.com/en-us/library/01escwtf.aspx

using System;
using System.Globalization;
using System.Text.RegularExpressions;

public class RegexUtilities
{
    bool invalid = false;

    public bool IsValidEmail(string strIn)
    {
        invalid = false;
        if (String.IsNullOrEmpty(strIn))
            return false;

        // Use IdnMapping class to convert Unicode domain names.
        strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper);
        if (invalid)
            return false;

        // Return true if strIn is in valid e-mail format.
        return Regex.IsMatch(strIn,
               @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
               @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
               RegexOptions.IgnoreCase);
    }

    private string DomainMapper(Match match)
    {
        // IdnMapping class with default property values.
        IdnMapping idn = new IdnMapping();

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
        Regex regexObj = new Regex(@"^[0-9\s\(\)\+\-]+$");
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