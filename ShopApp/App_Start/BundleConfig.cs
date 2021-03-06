﻿using System.Web;
using System.Web.Optimization;

namespace ShopApp
{
    public class BundleConfig
    {
        // Aby uzyskać więcej informacji o grupowaniu, odwiedź stronę https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery351").Include(
                        "~/Scripts/jquery-3.5.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapjs").Include(
                         "~/Scripts/bootstrap.js",
                         "~/Scripts/umd/popper.js",
                         "~/Scripts/bootstrap-input-spinner.js"));

            bundles.Add(new ScriptBundle("~/bundles/Userjs").Include(
                       "~/Scripts/register.js",
                       "~/Scripts/login.js"));

            bundles.Add(new ScriptBundle("~/bundles/RestorePasswordjs").Include(
                       "~/Scripts/passwordRestore.js"));

            bundles.Add(new ScriptBundle("~/bundles/Layoutjs").Include(
                        "~/Scripts/layout.js"));

            bundles.Add(new ScriptBundle("~/bundles/Offerjs").Include(
                        "~/Scripts/Offer.js"));

            bundles.Add(new ScriptBundle("~/bundles/Bucketjs").Include(
                        "~/Scripts/bucket.js"));

            bundles.Add(new ScriptBundle("~/bundles/Katjs").Include(
                        "~/Scripts/kat.js"));

            bundles.Add(new ScriptBundle("~/bundles/BucketAndFavjs").Include(
                        "~/Scripts/BucketAndFavouriteManagement.js",
                        "~/Scripts/Bucket.js"));

            bundles.Add(new ScriptBundle("~/bundles/Favouritejs").Include(
                        "~/Scripts/Fav.js"));

            bundles.Add(new ScriptBundle("~/bundles/Orderjs").Include(
                        "~/Scripts/Order.js"));

            bundles.Add(new ScriptBundle("~/bundles/UserInformationjs").Include(
                        "~/Scripts/UserInformation.js"));

            // Użyj wersji deweloperskiej biblioteki Modernizr do nauki i opracowywania rozwiązań. Następnie, kiedy wszystko będzie
            // gotowe do produkcji, użyj narzędzia do kompilowania ze strony https://modernizr.com, aby wybrać wyłącznie potrzebne testy.
            /* bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            // TODO: Ogarnięcie co to jest ten modernizr i jak z tego korzystać( bo to może być fajne narzędzie)
             */

            bundles.Add(new StyleBundle("~/bundles/bootstrap").Include(
                      "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/bundles/Layout").Include(
                       "~/Content/Layout.css"));

            bundles.Add(new StyleBundle("~/bundles/UserPanel").Include(
                      "~/Content/UserPanel.css"));

            bundles.Add(new StyleBundle("~/bundles/LoginAndRegistration").Include(
                      "~/Content/loginregister.css"));

            bundles.Add(new StyleBundle("~/bundles/Offer").Include(
                      "~/Content/offer.css"));
            
            bundles.Add(new StyleBundle("~/bundles/Error").Include(
                      "~/Content/error.css"));

            bundles.Add(new StyleBundle("~/bundles/Categories").Include(
                       "~/Content/categories.css"));

            bundles.Add(new StyleBundle("~/bundles/Bucket").Include(
                       "~/Content/bucket.css"));

            bundles.Add(new StyleBundle("~/bundles/Kat").Include(
                       "~/Content/kat.css"));

            bundles.Add(new StyleBundle("~/bundles/Order").Include(
                "~/Content/CashOut.css"));

            bundles.Add(new StyleBundle("~/bundles/UserInformation").Include(
                "~/Content/User.css"));

        }
    }
}
