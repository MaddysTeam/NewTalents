using System.Web.Optimization;

namespace Talents
{
   public class BundleConfig
   {
      // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
      public static void RegisterBundles(BundleCollection bundles)
      {
         // JS				jQuery
         bundles.Add(new ScriptBundle("~/js/jquery").Include(
                     "~/assets/plugins/jquery-1.11.1/jquery-1.11.1.min.js",
                     "~/assets/plugins/bootstrap-daterangepicker-2.1.13/moment.js"
                     ));

         // JS				jQuery Cookie
         bundles.Add(new ScriptBundle("~/js/jqueryCookie").Include(
                     "~/assets/plugins/jquery-cookie-master/src/jquery.cookie.js"
                     ));

         // JS       Moment
         bundles.Add(new ScriptBundle("~/js/moment").Include(
                     "~/assets/plugins/moment-2.10.6/moment.min.js"));

         // JS & CSS		Bootstrap core 
         bundles.Add(new ScriptBundle("~/js/bootstrap").Include(
                     "~/assets/plugins/bootstrap-3.3.2/js/bootstrap.min.js"
                     ));
         bundles.Add(new StyleBundle("~/css/bootstrap").Include(
                     "~/assets/plugins/bootstrap-3.3.2/css/bootstrap.css"
                     ));

			// JS & CSS		multiselect core 
			bundles.Add(new ScriptBundle("~/js/multiselect").Include(
							"~/assets/plugins/bootstrap-multiselect-0.9.8/js/bootstrap-multiselect.min.js"
							));
			bundles.Add(new StyleBundle("~/css/multiselect").Include(
							"~/assets/plugins/bootstrap-multiselect-0.9.8/css/bootstrap-multiselect.css"
							));

			// CSS			Font Awesome
			bundles.Add(new StyleBundle("~/css/font-awesome").Include(
                     "~/assets/plugins/font-awesome-4.4.0/css/font-awesome.min.css"
                     ));

         // JS & CSS		Bootstrap datepicker 
         bundles.Add(new ScriptBundle("~/js/datepicker").Include(
                     "~/assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js",
                     "~/assets/plugins/bootstrap-datepicker/js/locales/bootstrap-datepicker.zh-CN.js"
                     ));
         bundles.Add(new StyleBundle("~/css/datepicker").Include(
                     "~/assets/plugins/bootstrap-datepicker/css/datepicker.css"
                     ));

         // JS & CSS		Bootstrap daterangepicker 
         bundles.Add(new ScriptBundle("~/js/daterangepicker").Include(
                     "~/assets/plugins/bootstrap-daterangepicker-2.1.13/daterangepicker.js"
                     ));
         bundles.Add(new StyleBundle("~/css/daterangepicker").Include(
                     "~/assets/plugins/bootstrap-daterangepicker-2.1.13/daterangepicker.css"
                     ));

         // CSS			Fontello
         bundles.Add(new StyleBundle("~/css/fontello").Include(
                     "~/assets/plugins/fontello/css/fontello.css"
                     ));

         // jquery.validate
         bundles.Add(new ScriptBundle("~/js/jqueryval").Include(
                     "~/assets/plugins/jquery.validate-1.13.1/dist/jquery.validate.js",
                     "~/assets/plugins/jquery.validate-1.13.1/dist/localization/messages_zh.js",
                     "~/assets/plugins/jquery.validate.unobtrusive/jquery.validate.unobtrusive.js",
                     "~/assets/plugins/jquery.validate.bootstrap/jquery.validate.bootstrap.js"
                     ));

         // JS & CSS		Magnific-Popup 
         bundles.Add(new ScriptBundle("~/js/magnific-popup").Include(
                     "~/assets/plugins/Magnific-Popup-1.0.0/dist/jquery.magnific-popup.js"
                     ));
         bundles.Add(new StyleBundle("~/css/magnific-popup").Include(
                     "~/assets/plugins/Magnific-Popup-1.0.0/dist/magnific-popup.css"
                     ));

         // JS				Modernizr
         bundles.Add(new ScriptBundle("~/js/modernizr").Include(
                     "~/assets/plugins/modernizr-2.6.2/modernizr-*"));

         // JS & CSS		ThemePunch 
         bundles.Add(new ScriptBundle("~/js/themepunch").Include(
                     "~/assets/plugins/rs-plugin/js/jquery.themepunch.tools.min.js",
                     "~/assets/plugins/rs-plugin/js/jquery.themepunch.revolution.min.js"
                     ));
         bundles.Add(new StyleBundle("~/css/themepunch").Include(
                     "~/assets/plugins/rs-plugin/css/settings.css"
                     ));

         // JS				Isotope 
         bundles.Add(new ScriptBundle("~/js/isotope").Include(
                     "~/assets/plugins/isotope-2.2.1/dist/isotope.pkgd.min.js"
                     ));

         // JS				WayPoints 
         bundles.Add(new ScriptBundle("~/js/waypoints").Include(
                     "~/assets/plugins/waypoints-3.1.1/lib/jquery.waypoints.min.js"
                     ));

         // JS				CountTo 
         bundles.Add(new ScriptBundle("~/js/countTo").Include(
                     "~/assets/plugins/jquery-countTo-1.1.0/jquery.countTo.js"
                     ));

         // JS	& CSS		CountDown
         bundles.Add(new ScriptBundle("~/js/countDown").Include(
                     "~/assets/plugins/countdown-2.0.2/jquery.plugin.js",
                     "~/assets/plugins/countdown-2.0.2/jquery.countdown.js"
                     ));
         bundles.Add(new StyleBundle("~/css/countDown").Include(
                     "~/assets/plugins/countdown-2.0.2/jquery.countdown.css"
                     ));

         // JS				Parallax 
         bundles.Add(new ScriptBundle("~/js/parallax").Include(
                     "~/assets/plugins/parallax-2.1.3/deploy/jquery.parallax.min.js"
                     ));

         // JS				Morphext 
         bundles.Add(new ScriptBundle("~/js/morphext").Include(
                     "~/assets/plugins/Morphext-2.4.4/dist/morphext.min.js"
                     ));

         // JS				Vide
         bundles.Add(new ScriptBundle("~/js/vide").Include(
                     "~/assets/plugins/Vide-0.3.7/dist/jquery.vide.min.js"
                     ));

         // CSS			Animate
         bundles.Add(new StyleBundle("~/css/animate").Include(
                     "~/assets/plugins/animate.css-3.4.0/animate.custom.css"
                     ));

         // CSS			Hover
         bundles.Add(new StyleBundle("~/css/hover").Include(
                     "~/assets/plugins/Hover-2.0.2/hover-min.css"
                     ));

         // JS & CSS		Owl carousel
         bundles.Add(new ScriptBundle("~/js/owl-carousel").Include(
                     "~/assets/plugins/OwlCarousel-1.3.3/owl-carousel/owl.carousel.min.js"
                     ));
         bundles.Add(new StyleBundle("~/css/owl-carousel").Include(
                     "~/assets/plugins/OwlCarousel-1.3.3/owl-carousel/owl.carousel.css",
                     "~/assets/plugins/OwlCarousel-1.3.3/owl-carousel/owl.transitions.css"
                     ));

         // JS				jQuery-Browser
         bundles.Add(new ScriptBundle("~/js/jquery-browser").Include(
                     "~/assets/plugins/jquery-browser-plugin-0.0.8/dist/jquery.browser.min.js"
                     ));

         // JS				SmoothScroll
         bundles.Add(new ScriptBundle("~/js/smoothscroll").Include(
                     "~/assets/plugins/smoothscroll-1.2.1/SmoothScroll.js"
                     ));

         // JS				Chat Flot
         bundles.Add(new ScriptBundle("~/js/chart/flot").Include(
                     "~/assets/plugins/flot-0.8.3/jquery.flot.js",
                     "~/assets/plugins/flot-0.8.3/jquery.flot.time.js",
                     "~/assets/plugins/flot-0.8.3/jquery.flot.resize.js",
                     "~/assets/plugins/flot-0.8.3/jquery.flot.pie.js",
                     "~/assets/plugins/flot-0.8.3/jquery.flot.stack.js",
                     "~/assets/plugins/flot-0.8.3/jquery.flot.crosshair.js",
                     "~/assets/plugins/flot-0.8.3/jquery.flot.categories.js",
                     "~/assets/plugins/flot.tooltip-0.8.4/js/jquery.flot.tooltip.js"
                     ));

         // JS				Chat Sparkline
         bundles.Add(new ScriptBundle("~/js/chart/sparkline").Include(
                     "~/assets/plugins/jquery.sparkline-2.1.2/dist/jquery.sparkline.js"
                     ));

         // JS				Chat
         bundles.Add(new ScriptBundle("~/js/chart/chart").Include(
                     "~/assets/plugins/chart/chart.js"
                     ));

         // JS & CSS		SummerNote
         bundles.Add(new ScriptBundle("~/js/summernote").Include(
                     "~/assets/plugins/summernote-0.6.0/dist/summernote.js",
                     "~/assets/plugins/summernote-0.6.0/lang/summernote-zh-CN.js"
                     ));
         bundles.Add(new StyleBundle("~/css/summernote").Include(
                     "~/assets/plugins/summernote-0.6.0/dist/summernote.css"
                     ));

         // JS & CSS		Uploadify
         bundles.Add(new ScriptBundle("~/js/uploadify").Include(
                     "~/assets/plugins/uploadify-2.2/jquery.uploadify.js"
                     ));
         bundles.Add(new StyleBundle("~/css/uploadify").Include(
                     "~/assets/plugins/uploadify-2.2/uploadify.css"
                     ));

         // JS & CSS		jasny-bootstrap
         bundles.Add(new ScriptBundle("~/js/jasny").Include(
                     "~/assets/plugins/jasny-bootstrap-3.1.3/js/jasny-bootstrap.js"
                     ));
         bundles.Add(new StyleBundle("~/css/jasny").Include(
                     "~/assets/plugins/jasny-bootstrap-3.1.3/css/jasny-bootstrap.css"
                     ));

         // JS & CSS		ToastMessage
         bundles.Add(new ScriptBundle("~/js/toastmessage").Include(
                     "~/assets/plugins/jquery-toastmessage-0.2.0/js/jquery.toastmessage.js"
                     ));
         bundles.Add(new StyleBundle("~/css/toastmessage").Include(
                     "~/assets/plugins/jquery-toastmessage-0.2.0/css/jquery.toastmessage.css"
                     ));

         // JS          KnockOut
         bundles.Add(new ScriptBundle("~/js/knockout").Include(
                     "~/assets/plugins/knockout-3.2.0/knockout-3.2.0.js"
                     ));

         // JS	& CSS		Baidu WebUploader
         bundles.Add(new ScriptBundle("~/js/webuploader").Include(
                     "~/assets/plugins/webuploader-0.1.5/webuploader.js"
                     ));
         bundles.Add(new StyleBundle("~/css/webuploader").Include(
                     "~/assets/plugins/webuploader-0.1.5/webuploader.css"
                     ));

         // JS          Dropzone
         bundles.Add(new ScriptBundle("~/js/dropzone").Include(
							"~/assets/plugins/dropzone-4.3.0/js/dropzone.min.js"
							));

         // JS & CSS		ms-Dropdown
         bundles.Add(new ScriptBundle("~/js/ms-dropdown").Include(
                     "~/assets/plugins/ms-Dropdown-3.5.2/js/msdropdown/jquery.dd.js"
                     ));
         bundles.Add(new StyleBundle("~/css/ms-dropdown").Include(
                     "~/assets/plugins/ms-Dropdown-3.5.2/css/msdropdown/dd.css"
                     ));

         // JS & CSS		SweetAlert
         bundles.Add(new ScriptBundle("~/js/sweetalert").Include(
                     "~/assets/plugins/sweetalert-1.1.1/dist/sweetalert-dev.js"
                     ));
         bundles.Add(new StyleBundle("~/css/sweetalert").Include(
                     "~/assets/plugins/sweetalert-1.1.1/dist/sweetalert.css"
                     ));

         // JS & CSS    bootgrid
         bundles.Add(new ScriptBundle("~/js/bootgrid").Include(
                     "~/assets/plugins/jquery.bootgrid-1.1.4/jquery.bootgrid.js"));
         bundles.Add(new StyleBundle("~/css/bootgrid").Include(
                     "~/assets/plugins/jquery.bootgrid-1.1.4/jquery.bootgrid.css"));

         // JS & CSS    MediaElement
         bundles.Add(new ScriptBundle("~/js/mediaelement").Include(
                     "~/assets/plugins/MediaElement-2.18.2/build/mediaelement-and-player.js"));
         bundles.Add(new StyleBundle("~/css/mediaelement").Include(
                     "~/assets/plugins/MediaElement-2.18.2/build/mediaelementplayer.min.css"));

         // JS          Cookie
         bundles.Add(new ScriptBundle("~/js/cookie").Include(
                    "~/assets/plugins/jquery.cookie-1.3.1/jquery.cookie-1.3.1.js"));

         // JS & CSS    raty
         bundles.Add(new ScriptBundle("~/js/raty").Include(
                     "~/assets/plugins/jquery-raty-2.7.0/lib/jquery.raty.js"
                     ));
         bundles.Add(new StyleBundle("~/css/raty").Include(
                     "~/assets/plugins/jquery-raty-2.7.0/lib/jquery.raty.css"
                     ));
         
          // JS & CSS    js_composer
          bundles.Add(new ScriptBundle("~/js/composer").Include(
                     "~/assets/plugins/js_composer-4.3.5/js/js_composer_front.js",
                     "~/assets/plugins/js_composer-4.3.5/js/jquery-ui-tabs-rotate.js"
                     ));
         bundles.Add(new StyleBundle("~/css/composer").Include(
                     "~/assets/plugins/js_composer-4.3.5/css/js_composer.css"
                     ));

         // JS    JqueryUi
         bundles.Add(new ScriptBundle("~/js/jqueryui").Include(
                     "~/assets/plugins/jquery-ui-1.11.4.custom/jquery-ui.min.js"
                     ));

         // JS & CSS    layerslider
         bundles.Add(new ScriptBundle("~/js/layerslider").Include(
                    "~/assets/plugins/layersliderwp-v.5.0.2/static/js/layerslider.kreaturamedia.jquery.js",
                    "~/assets/plugins/layersliderwp-v.5.0.2/static/js/layerslider.transitions.js"
                    ));
         bundles.Add(new StyleBundle("~/css/layerslider").Include(
                     "~/assets/plugins/layersliderwp-v.5.0.2/static/css/layerslider.css"
                     ));



         // JS & CSS		This Admin App
         bundles.Add(new ScriptBundle("~/js/admin/app").Include(
                    "~/assets/js/admin/app.js",
                    "~/assets/js/admin/custom.js"
                     ));

         bundles.Add(new StyleBundle("~/css/admin/app").Include(
                     "~/assets/css/admin/app.css",
                     "~/assets/css/admin/charts-graphs.css",
                     "~/assets/css/admin/custom.css"
                     ));

         // JS & CSS		This App
         bundles.Add(new ScriptBundle("~/js/app").Include(
                     "~/assets/js/app/jquery-migrate.min.1.2.1.js",
                     "~/assets/js/app/greensock.1.11.8.js",
                     "~/assets/js/app/public.1.0.0.js",
                     "~/assets/js/app/publiccustominzer.1.0.0.js",
                     "~/assets/js/app/blockui.min.2.57.js",
                     "~/assets/js/custom.js"
                     ));

         bundles.Add(new ScriptBundle("~/js/app_foundation").Include(
                     "~/assets/js/app/foundation.js",
                     "~/assets/js/app/app.js"
                     ));

         bundles.Add(new StyleBundle("~/css/app").Include(
                     "~/assets/css/app/style4.3.1.css",
                     "~/assets/css/app/public.1.0.0.css",
                     "~/assets/css/app/options.css",
                     "~/assets/css/app/app.css",
                     "~/assets/css/app/foundation.css",
                     "~/assets/css/app/visual-composer.css",
                     "~/assets/css/app/optionsplus.css",
                     "~/assets/css/custom.css"
                     ));

      }
   }
}
