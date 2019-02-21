;
(function ($, window, undefined) {
	'use strict';

	var $doc = $(document),
		 Modernizr = window.Modernizr;

	$(document).ready(function () {
		$.fn.foundationAlerts ? $doc.foundationAlerts() : null;
		$.fn.foundationButtons ? $doc.foundationButtons() : null;
		$.fn.foundationAccordion ? $doc.foundationAccordion() : null;
		$.fn.foundationNavigation ? $doc.foundationNavigation() : null;
		$.fn.foundationTopBar ? $doc.foundationTopBar() : null;
		$.fn.foundationCustomForms ? $doc.foundationCustomForms() : null;
		$.fn.foundationMediaQueryViewer ? $doc.foundationMediaQueryViewer() : null;
		$.fn.foundationTabs ? $doc.foundationTabs({ callback: $.foundation.customForms.appendCustomMarkup }) : null;
		$.fn.foundationTooltips ? $doc.foundationTooltips() : null;
		$.fn.foundationMagellan ? $doc.foundationMagellan() : null;
		$.fn.foundationClearing ? $doc.foundationClearing() : null;

		$.fn.placeholder ? $('input, textarea').placeholder() : null;
	});

	// UNCOMMENT THE LINE YOU WANT BELOW IF YOU WANT IE8 SUPPORT AND ARE USING .block-grids
	// $('.block-grid.two-up>li:nth-child(2n+1)').css({clear: 'both'});
	// $('.block-grid.three-up>li:nth-child(3n+1)').css({clear: 'both'});
	// $('.block-grid.four-up>li:nth-child(4n+1)').css({clear: 'both'});
	// $('.block-grid.five-up>li:nth-child(5n+1)').css({clear: 'both'});

	// Hide address bar on mobile devices (except if #hash present, so we don't mess up deep linking).
	if (Modernizr.touch && !window.location.hash) {
		$(window).load(function () {
			setTimeout(function () {
				window.scrollTo(0, 1);
			}, 0);
		});
	}


})(jQuery, this);


/*---------------------------------
 Correct OS & Browser Check
 -----------------------------------*/

var ua = navigator.userAgent,
    checker = {
    	os: {
    		iphone: ua.match(/iPhone/),
    		ipod: ua.match(/iPod/),
    		ipad: ua.match(/iPad/),
    		blackberry: ua.match(/BlackBerry/),
    		android: ua.match(/(Android|Linux armv6l|Linux armv7l)/),
    		linux: ua.match(/Linux/),
    		win: ua.match(/Windows/),
    		mac: ua.match(/Macintosh/)
    	},
    	ua: {
    		ie: ua.match(/MSIE/),
    		ie6: ua.match(/MSIE 6.0/),
    		ie7: ua.match(/MSIE 7.0/),
    		ie8: ua.match(/MSIE 8.0/),
    		ie9: ua.match(/MSIE 9.0/),
    		ie10: ua.match(/MSIE 10.0/),
    		opera: ua.match(/Opera/),
    		firefox: ua.match(/Firefox/),
    		chrome: ua.match(/Chrome/),
    		safari: ua.match(/(Safari|BlackBerry)/)
    	}
    };


/*---------------------------------
 Navigation dropdown
 -----------------------------------*/
(function ($) {

	var $top_menu = jQuery('.header-wrap');
	var $header = jQuery('#header');
	var $fixed_height = $('#header .fixed').height();
	var $logotype = $('#logo').find('img');
	if (!($header.data('not-fixed'))) {
		function crum_drop_menu() {
			var hH = $header.height();
			var sT = $(window).scrollTop();
			if (sT > (hH + 100)) {
				$header.css('height', hH);
				$top_menu.addClass('fixed');
				$logotype.css('height', $fixed_height)
			} else {
				$top_menu.removeClass('fixed');
				$header.css('height', 'auto');
			}

		}


		$(window).load(function () {
			if ($(window).width() >= 768) {
				crum_drop_menu();
			}
		});

		$(window).on("resize, scroll", function (e) {
			if ($(window).width() >= 768) {
				crum_drop_menu();
			}
		});
	}

})(jQuery);


jQuery(document).ready(function () {


	/*---------------------------------
	 Lang drop-down
	 -----------------------------------*/

	jQuery(".lang-sel").hover(function () {

		jQuery(this).addClass("hovered");

	}, function () {

		jQuery(this).removeClass("hovered");

	});

	/* Mobile Devices Navigation Script */
	(function ($) {
		$('a.top-menu-button').bind('click', function () {
			if ($(this).hasClass('active')) {
				$('.tiled-menu').slideUp('fast');

				$(this).removeClass('active');
			} else {
				$('.tiled-menu').slideDown('fast');

				$(this).addClass('active');
			}
			return false;
		});
	})(jQuery);


	/*---------------------------------
	 Scroll To Top
	 -----------------------------------*/

	jQuery(".backtotop").addClass("hidden");
	jQuery(window).scroll(function () {
		if (jQuery(this).scrollTop() === 0) {
			jQuery(".backtotop").addClass("hidden")
		} else {
			jQuery(".backtotop").removeClass("hidden")
		}
	});

	jQuery('.backtotop').click(function () {
		jQuery('body,html').animate({
			scrollTop: 0
		}, 1200);
		return false;
	});




	jQuery(".menu-item").parents('.tiled-menu').addClass("no-customizer");

	/*---------------------------------
	 Letters animation
	 -----------------------------------*/

	var doAnimate = function () {
		jQuery('span').each(function () {
			var that = jQuery(this);
			setTimeout(function () {
				that.addClass('animated');
			}, that.index() * 15);
		});
	};

	var doAnimateFast = function () {
		jQuery('span').each(function () {
			var that = jQuery(this);
			setTimeout(function () {
				that.addClass('animated');
			}, that.index() * 10);
		});
	};

	jQuery('.folio-item').each(function () {

		var text = jQuery(this).find('.box-name > a').text();

		jQuery(this).mouseenter(function () {
			jQuery(this).find('.box-name > a').text('');
			for (i = 0; i < text.length; i++) {
				jQuery(this).find('.box-name > a').append('<span>' + text[i] + '</span>');
				if (i == text.length - 1) doAnimate();
			}

		}).mouseleave(function () {
			jQuery(this).find('.box-name > a').text(text);
		});
	});

	jQuery('.recent-posts-list h3.entry-title').each(function () {

		var text = jQuery(this).find('a').text();

		jQuery(this).mouseenter(function () {
			jQuery(this).find('a').text('');
			for (i = 0; i < text.length; i++) {
				jQuery(this).find('a').append('<span>' + text[i] + '</span>');
				if (i == text.length - 1) doAnimate();
			}

		}).mouseleave(function () {
			jQuery(this).find('a').text(text);
		});
	});


	/*---------------------------------
	 Scroll To Top
	 -----------------------------------*/

	var duration = 800;
	jQuery('.back-to-top').click(function (event) {
		event.preventDefault();
		jQuery('html, body').animate({ scrollTop: 0 }, duration);
		return false;
	});


	/*---------------------------------
	 Flipbox animation
	 -----------------------------------*/

	(function ($) {
		$('.flipbox').hover(function () {
			$(this).addClass('flip');
		}, function () {
			$(this).removeClass('flip');
		});

		$('.ie .flipbox').hover(function () {
			$('.back', this).animate({
				top: '0px'
			}, 250, function () {
				// Animation complete.
			});
		}, function () {
			$('.back', this).animate({
				top: '300px'
			}, 250, function () {
				// Animation complete.
			});

		});
	})(jQuery);


});


//Fix z-index youtube video embedding
/*
jQuery(document).ready(function (){
    jQuery('iframe').each(function() {
        var url = jQuery(this).attr("src");
        if (jQuery(this).attr("src").indexOf("?") > 0) {
            jQuery(this).attr({
                "src" : url + "&wmode=transparent&wmode=Opaque"
            });
        }
        else {
            jQuery(this).attr({
                "src" : url + "?wmode=transparent&wmode=Opaque"
            });
        }
    });

});
*/
/*---------------------------------
 Zoom images
 -----------------------------------*/

jQuery('.entry-thumb').bind('touchstart', function () {
	jQuery(this).addClass('hovered');
}).bind('touchend', function () {
	jQuery(this).removeClass('hovered');
});

/*---------------------------------
 Zoom images
 -----------------------------------*/

//jQuery(document).ready(function () {


//	jQuery("a[href$='.jpg'], a[href$='.jpeg'], a[href$='.gif'], a[href$='.png']").prettyPhoto({
//		animationSpeed: 'normal', /* fast/slow/normal */
//		padding: 40, /* padding for each side of the picture */
//		opacity: 0.35, /* Value betwee 0 and 1 */
//		showTitle: true /* true/false */
//	});


//	jQuery("a[rel^='prettyPhoto']").prettyPhoto({ showTitle: true });
//	jQuery("a.zoom-link").prettyPhoto({ showTitle: true });
//	jQuery("a.thumbnail").prettyPhoto({ showTitle: true });
//});





/*---------------------------------
 Custom share buttons
 -----------------------------------*/

jQuery(document).ready(function () {

	var $share_container = jQuery('#social-share');

	if (jQuery($share_container).length > 0) {

		jQuery('#cr-twitter-share').sharrre({
			share: {
				twitter: true
			},
			template: '<a href="#" class="twitter"><i class="soc_icon-twitter-3"></i> <span class="total">{total}</span></a>',
			enableHover: false,
			urlCurl: $share_container.data('directory') + '/inc' + '/sharrre.php',
			click: function (api, options) {
				api.simulateClick();
				api.openPopup('twitter');
			}
		});

		jQuery('#cr-facebook-share').sharrre({
			share: {
				facebook: true
			},
			template: '<a href="#"><i class="soc_icon-facebook"></i> <span class="total">{total}</span></a>',
			enableHover: false,
			urlCurl: $share_container.data('directory') + '/inc' + '/sharrre.php',

			click: function (api, options) {
				api.simulateClick();
				api.openPopup('facebook');
			}
		});


		jQuery('#cr-googlep-share').sharrre({
			share: {
				googlePlus: true
			},
			template: '<a href="#"><i class="soc_icon-google__x2B_"></i> <span class="total">{total}</span></a>',
			enableHover: false,
			urlCurl: $share_container.data('directory') + '/inc' + '/sharrre.php',

			click: function (api, options) {
				api.simulateClick();
				api.openPopup('googlePlus');
			}
		});

		jQuery('#cr-pinterest-share').sharrre({
			share: {
				pinterest: true
			},
			buttons: {
				pinterest: {
					url: jQuery('#cr-pinterest-share').attr("data-url"),
					media: jQuery('#cr-pinterest-share').attr("data-media"),
					description: jQuery('#cr-pinterest-share').attr("data-description")
				}
			},
			template: '<a href="#"><i class="soc_icon-pinterest"></i> <span class="total">{total}</span></a>',
			enableHover: false,
			urlCurl: $share_container.data('directory') + '/inc' + '/sharrre.php',

			click: function (api, options) {
				api.simulateClick();
				api.openPopup('pinterest');
			}
		});

	}

	if (jQuery('.post-sharrre').length > 0) {
		jQuery('.post-sharrre').sharrre({
			share: {
				twitter: true,
				facebook: true,
				pinterest: true
			},
			template: '<div class="box"><div class="left"><i class="linecon-like"></i></div><div class="middle"><a href="#" class="facebook"><i class="soc_icon-facebook"></i></a><a href="#" class="twitter"><i class="soc_icon-twitter-2"></i></a><a href="#" class="pinterest"><i class="soc_icon-pinterest"></i></a></div><div class="right">{total}</div></div>',
			enableHover: false,
			enableTracking: true,
			render: function (api, options) {
				jQuery(api.element).on('click', '.twitter', function () {
					api.openPopup('twitter');
				});
				jQuery(api.element).on('click', '.facebook', function () {
					api.openPopup('facebook');
				});
				jQuery(api.element).on('click', '.pinterest', function () {
					api.openPopup('pinterest');
				});
				options.buttons.pinterest.media = jQuery(api.element).attr("data-media");
				options.buttons.pinterest.description = jQuery(api.element).attr("data-text");
				options.buttons.twitter.description = jQuery(api.element).attr("data-text");
			}


		});
	}



});


/*!
 * Lettering.JS 0.6.1
 *
 * Copyright 2010, Dave Rupert http://daverupert.com
 * Released under the WTFPL license
 * http://sam.zoy.org/wtfpl/
 *
 * Thanks to Paul Irish - http://paulirish.com - for the feedback.
 *
 * Date: Mon Sep 20 17:14:00 2010 -0600
 */

(function (b) {
	function c(a, e, c, d) {
		e = a.text().split(e);
		var f = "";
		e.length && (b(e).each(function (a, b) {
			f += '<span class="' + c + (a + 1) + '">' + b + "</span>" + d
		}), a.empty().append(f))
	}

	var d = {
		init: function () {
			return this.each(function () {
				c(b(this), "", "char", "")
			})
		}, words: function () {
			return this.each(function () {
				c(b(this), " ", "word", " ")
			})
		}, lines: function () {
			return this.each(function () {
				c(b(this).children("br").replaceWith("eefec303079ad17405c889e092e105b0").end(), "eefec303079ad17405c889e092e105b0", "line", "")
			})
		}
	};
	b.fn.lettering =
		 function (a) {
		 	if (a && d[a]) return d[a].apply(this, [].slice.call(arguments, 1));
		 	if ("letters" === a || !a) return d.init.apply(this, [].slice.call(arguments, 0));
		 	b.error("Method " + a + " does not exist on jQuery.lettering");
		 	return this
		 }
})(jQuery);

/*
 * textillate.js
 * http://jschr.github.com/textillate
 * MIT licensed
 *
 * Copyright (C) 2012-2013 Jordan Schroter
 */

(function ($) {
	function isInEffect(effect) {
		return /In/.test(effect) || $.inArray(effect, $.fn.textillate.defaults.inEffects) >= 0
	}

	function isOutEffect(effect) {
		return /Out/.test(effect) || $.inArray(effect, $.fn.textillate.defaults.outEffects) >= 0
	}

	function getData(node) {
		var attrs = node.attributes || [], data = {};
		if (!attrs.length) return data;
		$.each(attrs, function (i, attr) {
			if (/^data-in-*/.test(attr.nodeName)) {
				data["in"] = data["in"] || {};
				data["in"][attr.nodeName.replace(/data-in-/, "")] = attr.nodeValue
			} else if (/^data-out-*/.test(attr.nodeName)) {
				data.out =
					 data.out || {};
				data.out[attr.nodeName.replace(/data-out-/, "")] = attr.nodeValue
			} else if (/^data-*/.test(attr.nodeName)) data[attr.nodeName] = attr.nodeValue
		});
		return data
	}

	function shuffle(o) {
		for (var j, x, i = o.length; i; j = parseInt(Math.random() * i), x = o[--i], o[i] = o[j], o[j] = x);
		return o
	}

	function animate($c, effect, cb) {
		$c.addClass("animated " + effect).css("visibility", "visible").show();
		$c.one("animationend webkitAnimationEnd oAnimationEnd", function () {
			$c.removeClass("animated " + effect);
			cb && cb()
		})
	}

	function animateChars($chars, options, cb) {
		var that = this, count = $chars.length;
		if (!count) {
			cb && cb();
			return
		}
		if (options.shuffle) shuffle($chars);
		$chars.each(function (i) {
			var $this = $(this);

			function complete() {
				if (isInEffect(options.effect)) $this.css("visibility", "visible"); else if (isOutEffect(options.effect)) $this.css("visibility", "hidden");
				count -= 1;
				if (!count && cb) cb()
			}

			var delay = options.sync ? options.delay : options.delay * i * options.delayScale;
			$this.text() ? setTimeout(function () {
				animate($this, options.effect, complete)
			}, delay) : complete()
		})
	}

	var Textillate =
		 function (element, options) {
		 	var base = this, $element = $(element);
		 	base.init = function () {
		 		base.$texts = $element.find(options.selector);
		 		if (!base.$texts.length) {
		 			base.$texts = $('<ul class="texts"><li>' + $element.html() + "</li></ul>");
		 			$element.html(base.$texts)
		 		}
		 		base.$texts.hide();
		 		base.$current = $("<span>").text(base.$texts.find(":first-child").html()).prependTo($element);
		 		if (isInEffect(options.effect)) base.$current.css("visibility", "hidden"); else if (isOutEffect(options.effect)) base.$current.css("visibility", "visible");
		 		base.setOptions(options);
		 		setTimeout(function () {
		 			base.options.autoStart && base.start()
		 		}, base.options.initialDelay)
		 	};
		 	base.setOptions = function (options) {
		 		base.options = options
		 	};
		 	base.start = function (index) {
		 		var $next = base.$texts.find(":nth-child(" + (index || 1) + ")");
		 		(function run($elem) {
		 			var options = $.extend({}, base.options, getData($elem));
		 			base.$current.text($elem.html()).lettering("words");
		 			base.$current.find('[class^="word"]').css({
		 				"display": "inline-block", "-webkit-transform": "translate3d(0,0,0)", "-moz-transform": "translate3d(0,0,0)",
		 				"-o-transform": "translate3d(0,0,0)", "transform": "translate3d(0,0,0)"
		 			}).each(function () {
		 				$(this).lettering()
		 			});
		 			var $chars = base.$current.find('[class^="char"]').css("display", "inline-block");
		 			if (isInEffect(options["in"].effect)) $chars.css("visibility", "hidden"); else if (isOutEffect(options["in"].effect)) $chars.css("visibility", "visible");
		 			animateChars($chars, options["in"], function () {
		 				setTimeout(function () {
		 					var options = $.extend({}, base.options, getData($elem));
		 					var $next = $elem.next();
		 					if (base.options.loop && !$next.length) $next =
								 base.$texts.find(":first-child");
		 					if (!$next.length) return;
		 					animateChars($chars, options.out, function () {
		 						run($next)
		 					})
		 				}, base.options.minDisplayTime)
		 			})
		 		})($next)
		 	};
		 	base.init()
		 };
	$.fn.textillate = function (settings, args) {
		return this.each(function () {
			var $this = $(this), data = $this.data("textillate"), options = $.extend(true, {}, $.fn.textillate.defaults, getData(this), typeof settings == "object" && settings);
			if (!data) $this.data("textillate", data = new Textillate(this, options)); else if (typeof settings == "string") data[settings].apply(data,
				 [].concat(args)); else data.setOptions.call(data, options)
		})
	};
	$.fn.textillate.defaults = { selector: ".texts", loop: false, minDisplayTime: 2E3, initialDelay: 0, "in": { effect: "fadeInLeftBig", delayScale: 1.5, delay: 50, sync: false, shuffle: false }, out: { effect: "hinge", delayScale: 1.5, delay: 50, sync: false, shuffle: false }, autoStart: true, inEffects: [], outEffects: ["hinge"] }
})(jQuery);

/*!
 *  FluidVids.js v1.2.0
 *  Responsive and fluid YouTube/Vimeo video embeds.
 *  Project: https://github.com/toddmotto/fluidvids
 *  by Todd Motto: http://toddmotto.com
 *
 *  Copyright 2013 Todd Motto. MIT licensed.
 */
window.fluidvids = (function (window, document, undefined) {

	'use strict';

	/*
	 * Constructor function
	 */
	var Fluidvids = function (elem) {
		this.elem = elem;
	};

	/*
	 * Prototypal setup
	 */
	Fluidvids.prototype = {

		init: function () {

			var videoRatio = (this.elem.height / this.elem.width) * 100;
			this.elem.style.position = 'absolute';
			this.elem.style.top = '0';
			this.elem.style.left = '0';
			this.elem.width = '100%';
			this.elem.height = '100%';

			var wrap = document.createElement('div');
			wrap.className = 'fluidvids';
			wrap.style.width = '100%';
			wrap.style.position = 'relative';
			wrap.style.paddingTop = videoRatio + '%';

			var thisParent = this.elem.parentNode;
			thisParent.insertBefore(wrap, this.elem);
			wrap.appendChild(this.elem);

		}

	};

	/*
	 * Initiate the plugin
	 */


	var iframes = document.getElementsByTagName('iframe');


	for (var i = 0; i < iframes.length; i++) {

		var players = /www.youtube.com|player.vimeo.com/;

		if (iframes[i].src.search(players) > 0) {

			var domElement = jQuery(iframes[i]).get(0);

			var parent_div = jQuery(domElement).parents('div:first');

			var className = jQuery(parent_div).attr('class');

			if (!(/ls/i.test(className))) {
				new Fluidvids(iframes[i]).init();
			}

		}

	}

})(window, document);

jQuery(document).ready(function ($) {

	// Cache the Window object
	$window = $(window);

	// Cache the Y offset and the speed of each sprite
	$('[data-type]').each(function () {
		$(this).data('offsetY', parseInt($(this).attr('data-offsetY')));
		$(this).data('Xposition', $(this).attr('data-Xposition'));
		$(this).data('speed', $(this).attr('data-speed'));
	});

	// For each element that has a data-type attribute
	$('section[data-type="background"]').each(function () {


		// Store some variables based on where we are
		var $self = $(this),
			 offsetCoords = $self.offset(),
			 topOffset = offsetCoords.top;

		// When the window is scrolled...
		$(window).scroll(function () {

			// If this section is in view
			if (($window.scrollTop() + $window.height()) > (topOffset) &&
				 ((topOffset + $self.height()) > $window.scrollTop())) {

				// Scroll the background at var speed
				// the yPos is a negative value because we're scrolling it UP!
				var yPos = -($window.scrollTop() / $self.data('speed'));

				// If this element has a Y offset then add it on
				if ($self.data('offsetY')) {
					yPos += $self.data('offsetY');
				}

				// Put together our final background position
				var coords = '50% ' + yPos + 'px';

				// Move the background
				$self.css({ backgroundPosition: coords });

				// Check for other sprites in this section
				$('[data-type="sprite"]', $self).each(function () {

					// Cache the sprite
					var $sprite = $(this);

					// Use the same calculation to work out how far to scroll the sprite
					var yPos = -($window.scrollTop() / $sprite.data('speed'));
					var coords = $sprite.data('Xposition') + ' ' + (yPos + $sprite.data('offsetY')) + 'px';

					$sprite.css({ backgroundPosition: coords });

				}); // sprites

				// Check for any Videos that need scrolling
				$('[data-type="video"]', $self).each(function () {

					// Cache the video
					var $video = $(this);

					// There's some repetition going on here, so
					// feel free to tidy this section up.
					var yPos = -($window.scrollTop() / $video.data('speed'));
					var coords = (yPos + $video.data('offsetY')) + 'px';

					$video.css({ top: coords });

				}); // video

			}
			; // in view

		}); // window scroll

	});	// each data-type

}); // document ready

/*
 By Osvaldas Valutis, www.osvaldas.info
 Available for use under the MIT License
 */

; (function (e, t, n, r) { e.fn.doubleTapToGo = function (r) { if (!("ontouchstart" in t) && !navigator.msMaxTouchPoints && !navigator.userAgent.toLowerCase().match(/windows phone os 7/i)) return false; this.each(function () { var t = false; e(this).on("click", function (n) { var r = e(this); if (r[0] != t[0]) { n.preventDefault(); t = r } }); e(n).on("click touchstart MSPointerDown", function (n) { var r = true, i = e(n.target).parents(); for (var s = 0; s < i.length; s++) if (i[s] == t[0]) r = false; if (r) t = false }) }); return this } })(jQuery, window, document);