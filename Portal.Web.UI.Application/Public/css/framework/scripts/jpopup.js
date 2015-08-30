jQuery.extend({
	popup: function (options, callback) {
		var btns = {};

		var defaults = {
			cancelText: "Cancel",
			submitText: "Submit",
			isAjax: true
		};

		var options = $.extend(defaults, options);

		
		btns[options.submitText] = function () {
			var f = $("form", this);
			var d = $(this);

			if(options.isAjax){
				$.post(options.url, f.serialize(), function (result) {
					if (result.split("|")[0] == "ok") {
						if ($.isFunction(callback)) {
							d.dialog("close");
							callback(d);
						} else {
							if (result.split("|").length == 1) {
								window.location.href = window.location.href;
							} else {
								window.location.href = result.split("|")[1];
							}
						}
					} else {
						d.html(result);
						checkresult(d);
					}
				});
			}else{
				f.submit();
			}
		};


		btns[options.cancelText] = function () {
			$(this).dialog("close");
		};

		if (options.buttons) {
			for (var i = 0; i < options.buttons; i++) {
				btns[i.name] = i.fun;
			}
		}

		var d = $("<div/>");

		d.dialog({
			title: options.title,
			modal: true,
			autoOpen: false,
			width: 450,
			buttons: btns,
			maxHeight: 500,
			close: function (event, ui) {
				$("div.opened-dialog").remove();
			},
			open: function (event, ui) {
				$(this).css({ 'max-height': 500, 'overflow-y': 'auto' });
			}
		});

		d.addClass("opened-dialog").load(options.url, function (result) {
			checkresult(d);
			d.dialog("open");
		});

		function checkresult(obj) {
			if (obj.find("form").length == 0) {
				obj.wrapInner("<form action='" + options.url + "' method='post'><fieldset></fieldset></form>");
			}

			obj.find("form").submit(function () {
				return !options.isAjax;
			});
		}
	}
});