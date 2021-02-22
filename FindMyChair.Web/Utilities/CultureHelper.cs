using FindMyChair.Web.Utilities;
using System.Threading;
using System.Globalization;
using System.Web;
using System;

namespace FindMyChair.Web.Utilities
{
    public class CultureHelper : GlobalizationHelper
    {
        /// <summary>
        /// Get CulturInfo.Name to set in application from parameter [language] in current QueryString.
        /// </summary>
        /// <param name="queryString">Current page full Url with language parameter</param>
        /// <returns>CultureInfo name as string</returns>
        public string GetCultureInfo(string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString)) queryString = "?language=en-US";
            string languageParam = (null != HttpContext.Current && null != HttpContext.Current.Session && 
                null != HttpContext.Current.Session["language"]) ?
                HttpContext.Current.Session["language"].ToString() :
                GetCultureInfoNameFromQueryString(queryString);
            if (!string.IsNullOrWhiteSpace(languageParam))
            {
                if (null != HttpContext.Current && null == HttpContext.Current.Session["language"]) HttpContext.Current.Session["language"] = languageParam;
                if (languageParam.ToLower() == "default") return CultureInfo.CurrentCulture.Name;
                return GetFormattedCultureName(languageParam);
            }
            var cultureName = CultureInfo.CurrentCulture.Name;
            SetCultureInfoOnThread(cultureName);
            return cultureName;
        }

        /// <summary>
        /// Set Thread Curren.CultureInfo on application
        /// </summary>
        /// <returns>CultureInfo name as string</returns>
        public void SetCultureInfoOnThread(string cultureString)
        {
            CultureInfo culture;
            if (Thread.CurrentThread.CurrentUICulture.Name == cultureString)
                culture = CultureInfo.CreateSpecificCulture(cultureString);
            else
                culture = CultureInfo.CreateSpecificCulture(cultureString);

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// Returns CultureInformation based on a passed parametes cultureName
        /// </summary>
        /// <param name="cultureString"></param>
        /// <returns>CultureInfo</returns>
        public CultureInfo GetCultureInfoFromCultureInfoName(string cultureString)
        {
            if (string.IsNullOrWhiteSpace(cultureString)) cultureString = "en-US";
            cultureString = GetFormattedCultureName(cultureString);
            var culture = CultureInfo.CreateSpecificCulture(cultureString);
            return culture;
        }
    }
}