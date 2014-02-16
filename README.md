Style Bundle Fallback
===================

When using [Microsoft ASP.NET Web Optimization Framework](http://www.nuget.org/packages/Microsoft.AspNet.Web.Optimization/1.1.2) there is no reliable way to target a [CDN](http://en.wikipedia.org/wiki/Content_delivery_network) stylesheet and also provide a local fallback. There are two parts to this problem outlined below. 

### Style Bundle Fallback Solution
The [StyleBundleExtensions.cs](http://github.com/EmberConsultingGroup/StyleBundleFallback/blob/master/Website/App_Start/StyleBundleExtensions.cs) class provides an extension method to the StyleBundle, that injects a fallback script into the page that will load a local stylesheet when the CDN source fails. To use it, call the `.IncludeFallback()` extension method on the `StyleBundle` object. It is important to provide a class name, rule name and rule value from the stylesheet being loaded from an external CDN.

```
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;
            bundles.UseCdn = true;

            bundles.Add(new StyleBundle("~/bundles/bootstrap",
                "//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css")
                .IncludeFallback("~/Content/bootstrap/bootstrap.css", "sr-only", "width", "1px"));

            bundles.Add(new StyleBundle("~/bundles/bootstrap-theme",
                "//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap-theme.min.css")
                .IncludeFallback("~/Content/bootstrap/bootstrap-theme.css", "well", "border-color", "#dcdcdc"));
        }
```

It also determines if the stylesheet is on a CDN in another domain and provides a reliable javascript test for the resource. *This should be 99% of the time, because it doesn't really make sense to load resources from a CDN on your own network, but I guess it could happen in a corporate environment.* When the CDN is within the same app domain, the javascript is more robust.

* Don't `IncludeFallback` more than one file per CDN bundle... it doesn't make sense, there can only be one `StyleBundle.CdnPath` per bundle.

* It isn't necessary to provide class name, rule name and rule value for a fallback stylesheet coming from a local CDN.

### 1. Problem with the *Optimization Framework*

The first is a [documented issue](http://aspnetoptimization.codeplex.com/workitem/104) in the *Optimization Framework* where it incorrectly renders `<script>` instead of `<link>` when rendering the `StyleBundle.CdnFallbackExpression`.

The solution to this is to set the `StyleBundle.CdnFallbackExpression` to a javascript function that checks that the stylesheet is loaded and if not, loads the fallback from the local web server. The *Optimization Framework* will still output the invalid script, but it will be ignored and not cause a problem.

### 2. Problem determining if a stylesheet has been loaded

The second problem is generally *"How to determine when a stylesheet has successfully loaded"*? Some solutions I found use `document.styleSheets` and `rules || cssRules`, but this is a security violation on some browsers when the stylesheet is cross-domain. [YepNope](http://github.com/SlexAxton/yepnope.js/blob/master/plugins/yepnope.css.js) does it this way.

The solution is to inject an element into the page and apply a style from the stylesheet. If the stylesheet is loaded, the element will have the style's rules applied, if not the fallback should be loaded. You can see this method and a more robust solution for stylesheets in the domain in [StyleBundleFallback.cs](http://github.com/EmberConsultingGroup/StyleBundleFallback/blob/master/Website/App_Start/StyleBundleExtensions.cs)


#### Other Links

* http://stackoverflow.com/questions/21004913/mvc-cdn-fallback-for-style-bundle
* http://stackoverflow.com/questions/3794128/how-to-check-if-an-external-cross-domain-css-file-is-loaded-using-javascript
* http://stackoverflow.com/questions/17666785/check-external-stylesheet-has-loaded-for-fallback
* http://github.com/kenwarner/CssCdnFallback
* http://www.nuget.org/packages/Microsoft.AspNet.Web.Optimization/1.1.2
* http://aspnetoptimization.codeplex.com/workitem/104
* http://stackoverflow.com/questions/15519937/using-yepnope-how-can-i-know-if-a-css-resource-was-loaded-successfully
* http://stackoverflow.com/questions/21807253/determine-if-path-is-cross-domain

