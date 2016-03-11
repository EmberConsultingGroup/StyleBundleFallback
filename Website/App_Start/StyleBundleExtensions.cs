using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Website
{
    public static class StyleBundleExtensions
    {
        /// <summary>
        /// Include a stylesheet to fallback to when external CdnPath does not load.
        ///
        /// Coppied from https://github.com/EmberConsultingGroup/StyleBundleFallback.
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="fallback">Virtual path to fallback stylesheet</param>
        /// <param name="className">Stylesheet class name applied to test DOM element</param>
        /// <param name="ruleName">Rule name to test when the class is applied ie. width</param>
        /// <param name="ruleValue">Value to test when the class is applied ie. 1px</param>
        /// <param name="javascriptDebugLog">Include javascript logging of the fallback treatment for debuggin in the browser</param>
        /// <returns></returns>
        public static StyleBundle IncludeFallback(this StyleBundle bundle, string fallback,
            string className = null, string ruleName = null, string ruleValue = null, bool javascriptDebugLog = false)
        {
            if (String.IsNullOrEmpty(bundle.CdnPath))
            {
                throw new Exception("CdnPath must be provided when specifying a fallback");
            }

            if (VirtualPathUtility.IsAppRelative(bundle.CdnPath))
            {
                bundle.CdnFallbackExpress(fallback);
            }
            else if (new[] { className, ruleName, ruleValue }.Any(String.IsNullOrEmpty))
            {
                throw new Exception(
                    "IncludeFallback for cross-domain CdnPath must provide values for parameters [className, ruleName, ruleValue].");
            }
            else
            {
                bundle.CdnFallbackExpress(fallback, className, ruleName, ruleValue, javascriptDebugLog);
            }

            return bundle;
        }

        private static StyleBundle CdnFallbackExpress(this StyleBundle bundle, string fallback,
            string className = null, string ruleName = null, string ruleValue = null, bool javascriptDebugLog = false)
        {
            bundle.Include(fallback);

            fallback = VirtualPathUtility.ToAbsolute(fallback);

            bundle.CdnFallbackExpression = String.IsNullOrEmpty(className) ?

                String.Format(@"function() {{
                var len = document.styleSheets.length;
                for (var i = 0; i < len; i++) {{
                    var sheet = document.styleSheets[i];
                    if (sheet.href.indexOf('{0}') !== -1) {{
                        var rules = sheet.rules || sheet.cssRules;
                        if (rules.length <= 0) {{
                            document.write('<link href=""{1}"" rel=""stylesheet"" type=""text/css"" />');
                        }}
                    }}
                }}
                return true;
                }}()", bundle.CdnPath, fallback) :

                String.Format(@"
                    function() {{
                        var len = document.styleSheets.length;
                        " + (javascriptDebugLog ? @"console.log(""## Testing fallback for {0}"");" : "") + @"
                        for (var i = 0; i < len; i++) {{
                            var sheet = document.styleSheets[i];
                            if (sheet.href && sheet.href.indexOf('{0}') !== -1) {{
                                var meta = document.createElement('meta');
                                meta.className = '{2}';
                                document.head.appendChild(meta);
                                var value = window.getComputedStyle(meta).getPropertyValue('{3}');
                                document.head.removeChild(meta);
                                " + (javascriptDebugLog ? @"console.log(""Comparing: "" + value);" : "") + @"
                                " + (javascriptDebugLog ? @"console.log(""To:        {4}"");" : "") + @"
                                if (value !== ""{4}"") {{
                                    " + (javascriptDebugLog ? @"console.log(""Fallback to "" + ""{1}"");" : "") + @"
                                    document.write('<link href=""{1}"" rel=""stylesheet"" type=""text/css"" />');
                                }}
                            }}
                        }}
                        return true;
                    }}()", bundle.CdnPath, fallback, className, ruleName, ruleValue);

            return bundle;
        }
    }
}
