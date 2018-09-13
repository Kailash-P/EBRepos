using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace ElectricityBoardApplication.App_Start
{
    public class BundleConfig 
    {
        public static void RegisterBundle(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jQueryUI.js",
                        "~/Scripts/HighCharts.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/umd/popper.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/FileSaver.js",
                        "~/Scripts/pnotify.custom.min.js",
                        "~/Scripts/Alert.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/font-awesome.css",
                      "~/Content/jQueryUICss.css",
                      "~/Content/W3.css",
                      "~/Content/pnotify.custom.min.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}