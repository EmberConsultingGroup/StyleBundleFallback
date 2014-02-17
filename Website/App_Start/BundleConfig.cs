using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Website
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;
            bundles.UseCdn = true;

            bundles.Add(new StyleBundle("~/bundles/bootstrap",
                "//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css")
                .IncludeFallback("~/Content/bootstrap/bootstrap.min.css", "sr-only", "width", "1px"));

            bundles.Add(new StyleBundle("~/bundles/bootstrap-theme",
                "//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap-theme.min.css")
                .IncludeFallback("~/Content/bootstrap/bootstrap-theme.min.css", "well", "background-repeat", "repeat-x"));
        }
    }
}