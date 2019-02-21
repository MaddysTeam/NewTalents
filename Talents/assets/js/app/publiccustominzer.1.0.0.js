
/*
 By Osvaldas Valutis, www.osvaldas.info
 Available for use under the MIT License
 */

;(function(e,t,n,r){e.fn.doubleTapToGo=function(r){if(!("ontouchstart"in t)&&!navigator.msMaxTouchPoints&&!navigator.userAgent.toLowerCase().match(/windows phone os 7/i))return false;this.each(function(){var t=false;e(this).on("click",function(n){var r=e(this);if(r[0]!=t[0]){n.preventDefault();t=r}});e(n).on("click touchstart MSPointerDown",function(n){var r=true,i=e(n.target).parents();for(var s=0;s<i.length;s++)if(i[s]==t[0])r=false;if(r)t=false})});return this}})(jQuery,window,document);

(function ($) {
    "use strict";
    $(function () {
        jQuery(".main-menu-item, .menu-item, .sub-menu-item").hover(function () {
            jQuery(this).addClass("hovered");
        }, function () {
            jQuery(this).removeClass("hovered");
        });

        jQuery('.metro-menu-item').each(function() {
            var $tile = jQuery(this).find('.menu-tile');
            if ($tile.data('inherit-color')){
                jQuery(this).find('.sub-menu li').css('background',$tile.data('inherit-color'));
            }
        });

        jQuery('li.has-submenu > a').doubleTapToGo();

    });
}(jQuery));