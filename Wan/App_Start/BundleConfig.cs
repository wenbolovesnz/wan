﻿using System.Web;
using System.Web.Optimization;

namespace Wan
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
            "~/Scripts/angular.js",
            "~/Scripts/angular-route.js",
            "~/Scripts/angular-resource.js"));

            bundles.Add(new ScriptBundle("~/bundles/joinme").Include(
                                    "~/AppScripts/Main.js", // must be first   
                                    "~/AppScripts/DataContext.js",
                                    "~/AppScripts/Controllers/Home.js"
                                    ));


            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(                
                                        "~/Content/site.css",
                                        "~/Content/bootstrap.css",
                                        "~/Content/font-awesome.css"));
          
        }
    }
}