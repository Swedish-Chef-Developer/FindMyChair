using System.Resources;
using System.Globalization;

namespace FindMyChair.Web.Utilities
{
    public static class Utilities
    {
        public static string GetStringByResourceName(string resourceName)
        {
            // Set resource manager.
            var cultureHelper = new CultureHelper();
            var currentCulture = "en-US";
            if (string.IsNullOrWhiteSpace(currentCulture)) currentCulture = "en-US";
            var cultureInfo = new CultureInfo(cultureHelper.GetCultureInfo(currentCulture));
            var resourceManager = new ResourceManager(typeof(Resources.Resources));
            if (null == resourceManager) return "No value found!"; // string.Empty;
            return GetStringByResourceName(resourceName, cultureInfo);
        }

        public static string GetStringByResourceName(string resourceName, CultureInfo cultureInfo)
        {
            // Set resource manager.
            var resourceManager = new ResourceManager(typeof(Resources.Resources));

            if (null == resourceManager) return "No value found!"; // string.Empty;
            return resourceManager.GetString(resourceName, cultureInfo);
        }
    }
}