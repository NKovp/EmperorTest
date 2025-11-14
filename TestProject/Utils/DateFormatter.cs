using System.Globalization;

namespace TestProject.Utils
{
    /// <summary>
    /// Helper class for formatting dates.
    /// </summary>
    public static class DateFormatter
    {
        /// <summary>
        /// Formats the given UTC date into a string with an ordinal day suffix.
        /// </summary>
        /// <param name="utcDate">UTC date</param>
        /// <returns></returns>
        public static string FormatWithOrdinal(DateTime? utcDate)
        {
            if (utcDate is null)
                return string.Empty;

            var date = utcDate.Value;
            int day = date.Day;

            string suffix = "th";
            if (day % 10 == 1 && day != 11) 
            { 
                suffix = "st"; 
            }
            else if (day % 10 == 2 && day != 12) 
            { 
                suffix = "nd"; 
            }
            else if (day % 10 == 3 && day != 13) 
            { 
                suffix = "rd"; 
            }

            var month = date.ToString("MMMM", CultureInfo.InvariantCulture);
            var year = date.Year;

            // "12th November 2025"
            return $"{day}{suffix} {month} {year}";
        }

    }
}
