using System.Web;
using System.Web.Optimization;

namespace Wan
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.9.0.js",
                        "~/Scripts/underscore.js",
                        "~/Scripts/ajaxfileupload.js",
                        "~/Scripts/jquery.datetimepicker.js",
                        "~/Scripts/bootbox.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
                        "~/Scripts/jquery.signalR-2.0.3.js"
                        ));
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
            "~/Scripts/angular.js",
            "~/Scripts/angular-route.js",
            "~/Scripts/angular-resource.js"));

            bundles.Add(new ScriptBundle("~/bundles/joinme").Include(
                                    "~/AppScripts/Main.js", // must be first   
                                    "~/AppScripts/DataContext.js",
                                    "~/AppScripts/Controllers/MenuAfter.js",
                                    "~/AppScripts/Controllers/Home.js",
                                    "~/AppScripts/Controllers/CreateGroup.js",
                                    "~/AppScripts/Controllers/GroupDetails.js",
                                    "~/AppScripts/Controllers/UserLogin.js",
                                    "~/AppScripts/Controllers/UserSignup.js",
                                    "~/AppScripts/Controllers/MyAccount.js",
                                    "~/AppScripts/Controllers/Events.js",
                                    "~/AppScripts/Controllers/Event.js",
                                    "~/AppScripts/Controllers/Sponsor.js",
                                    "~/AppScripts/Controllers/User.js",
                                    "~/AppScripts/Controllers/Members.js",
                                    "~/AppScripts/Controllers/GroupPhoto.js",
                                    "~/AppScripts/Controllers/NotificationMessages.js",
                                    "~/AppScripts/startSignalR.js",
                                    "~/Scripts/bootstrap.js",
                                    "~/Scripts/angular-ui/ui-bootstrap-tpls.js",
                                    "~/Scripts/angular-multi-select.js"                                    
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
                                        "~/Content/font-awesome.css",
                                        "~/Content/jquery.datetimepicker.css",
                                        "~/Content/angular-multi-select.css"
                                        ));
          
        }
    }
}