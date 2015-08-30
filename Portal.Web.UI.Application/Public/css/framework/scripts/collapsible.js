(function ($) {
	$(function () {
		$("fieldset.collapsible").each(function () {
			var f = this;
			var cookieEnabled=$.isFunction($.cookie);
			
			if(cookieEnabled){			
				if ($.cookie(f.id) == "true") {
					$(f).removeClass("collapsible-closed");
				} else {
					$(f).addClass("collapsible-closed");
				}
			}

			$("legend", f).click(function () {
				var has = $(f).hasClass("collapsible-closed");

				if (has) {
					$(f).removeClass("collapsible-closed")
				} else {
					$(f).addClass("collapsible-closed");
				}
				if(cookieEnabled){
					$.cookie(f.id, has);
				}
			});
		});
	});
})(jQuery);