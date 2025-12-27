using System.Web;
using System.Web.Optimization;

namespace Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        //"~/Scripts/jquery-{version}.js",
                        "~/Content/assets/plugins/jquery/jquery.min.js"
                        //"~/Content/assets/plugins/jquery-ui/jquery-ui.min.js",
                        //"~/Content/assets/plugins/jquery/jquery.slim.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        //"~/Scripts/jquery.validate*"
                        "~/Content/assets/plugins/jquery-validation/jquery.validate.min.js",
                        "~/Content/assets/plugins/jquery-validation/jquery.validate.unobtrusive.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      //"~/Scripts/bootstrap.js"
                      "~/Content/assets/plugins/bootstrap/js/bootstrap.bundle.min.js"
                      ));


            #region DataTables

            bundles.Add(new StyleBundle("~/bundles/DataTables").Include(
                      "~/Content/assets/plugins/datatables-bs4/css/dataTables.bootstrap4.min.css",
                      "~/Content/assets/plugins/datatables-responsive/css/responsive.bootstrap4.min.css",
                      "~/Content/assets/plugins/datatables-buttons/css/buttons.bootstrap4.min.css"));

            bundles.Add(new Bundle("~/plugin/DataTables").Include(
                      "~/Content/assets/plugins/datatables/jquery.dataTables.min.js",
                      "~/Content/assets/plugins/datatables-bs4/js/dataTables.bootstrap4.min.js",
                      "~/Content/assets/plugins/datatables-responsive/js/dataTables.responsive.min.js",
                      "~/Content/assets/datatables-responsive/js/responsive.bootstrap4.min.js",
                      "~/Content/assets/plugins/datatables-buttons/js/dataTables.buttons.min.js",
                      "~/Content/assets/plugins/datatables-buttons/js/buttons.bootstrap4.min.js",
                      "~/Content/assets/plugins/datatables-buttons/js/buttons.html5.min.js",
                      "~/Content/assets/plugins/datatables-buttons/js/buttons.print.min.js",
                      "~/Content/assets/plugins/datatables-buttons/js/buttons.colVis.min.js"
                      ));
            #endregion


            bundles.Add(new Bundle("~/plugin/adminLte").Include(
                      "~/Content/assets/js/adminlte.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/assets/plugins/jquery-ui/jquery-ui.min.css",
                      "~/Content/assets/plugins/fontawesome-free/css/all.min.css",
                      "~/Content/assets/css/Custom.css",
                      "~/Content/assets/css/adminlte.min.css"));
        }
    }
}
