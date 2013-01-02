/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Matt40k.SIMSBulkImport
{
    class Validation
    {
        protected internal static bool IsValidEmail(string email)
        {
            RegexUtilities util = new RegexUtilities();
            return util.IsValidEmail(email);
        }
    }
}
