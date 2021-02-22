using System.Globalization;
using System.Web;

namespace FindMyChair.Web.Utilities
{
    public class GlobalizationHelper
    {
        /// <summary>
        /// Sets the culture that is set in parameter [language] in querystring.
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns>Selected CultureInfo.Name as string</returns>
        public string GetCultureInfoNameFromQueryString(string queryString)
        {
            var languageParam = CultureInfo.CurrentCulture.Name;
            if (string.IsNullOrEmpty(queryString)) queryString = "?language=en-US";
            if (!string.IsNullOrEmpty(queryString) && 
                !string.IsNullOrWhiteSpace(HttpUtility.ParseQueryString(queryString).Get("language")))
            {
                languageParam = HttpUtility.ParseQueryString(queryString).Get("language").ToString();
            }   
            return GetFormattedCultureName(languageParam);
        }

        /// <summary>
        /// Return a passed parameter CultureInfo.Name as string.
        /// </summary>
        /// <param name="originalLanguage"></param>
        /// <returns>CulterInfo.Name as string</returns>
        public string GetFormattedCultureName(string originalLanguage)
        {
            originalLanguage = originalLanguage.ToUpper();
            if (string.IsNullOrWhiteSpace(originalLanguage)) originalLanguage = CultureInfo.CurrentCulture.Name;
            if (originalLanguage.Contains("-"))
            {
                var culturerray = originalLanguage.Split(new char[]{ '-' });
                if (culturerray.Length > 0)
                {
                    originalLanguage = culturerray[0].ToUpper();
                    if (culturerray.Length > 1)
                    {
                        var culture = culturerray[0].ToLower();
                        originalLanguage = culturerray[1].ToUpper();
                        originalLanguage = string.Format("{0}-{1}", culture, originalLanguage);
                    }
                }
            }
            return originalLanguage;
        }
    }
}