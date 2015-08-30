$(function () {
	$("h4.toggle").each(function () {
		var h = this;
		$(h).click(function () {
			$(h).toggleClass("toggle-open").next().slideToggle("slow", function () {
				$.cookie(h.id, $(h).hasClass("toggle-open"));
			});
		});

		if ($.cookie(h.id) == "true") {
			$(h).addClass("toggle-open").next().show();
		} else {
			$(h).next().hide();
		}
	});
});