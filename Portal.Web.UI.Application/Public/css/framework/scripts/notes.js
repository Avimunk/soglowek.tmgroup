(function ($) {

	$.fn.notes = function () {
		return this.each(function () {
			var div = $(this);

			div.find("form").submit(function () {
				var form = this;
				$.post(this.action, $(form).serialize(), function (result) {
					var last=$("li:last", div);
					if(last.length==0){
						$("ol", div).html(result);
					}else{
						last.after(result);
					}
					$("li:last", div).effect("highlight");
					$("textarea", form).val("");
				});
				return false;
			});

			$("a.delete-note", div).live("click", function () {
				if (confirm(deleteMessage)) {
					var a = this;
					$.post(a.href, null, function () {
						$(a).parent().fadeOut("slow", function () {
							$(this).remove();
						});
					});
				}
				return false;
			});
		});
	};
})(jQuery);
