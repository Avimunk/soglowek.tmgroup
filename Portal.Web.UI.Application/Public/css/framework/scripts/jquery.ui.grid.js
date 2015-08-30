$.widget("ui.grid", {
	// default options
	options: {
		sort: true,
		zebra: true,
		url: window.location.href,
		page: 7,
		sortname: "",
		sortorder: "asc",
		pageSize: 15,
		total: 100,
		usePager: false,
		pagestat: 'Displaying {from} to {to} of {total} items'
	},
	pager: null,
	_create: function () {
		this._zebra();
		this._headers();

		if (this.usePager) {
			this._buildPager();
			this._buildButtons();
			this.pager = this.element.next();
		}
	},
	_buildButtons: function () {
		var ui = this;
		$(".icon-first", this.pager).click(function () {
			ui.changePage("fisrt");
			return false;
		});
	},
	loading: false,
	changePage: function (cmd) {
		if (this.loading) return true;

		switch (ctype) {
			case 'first': p.newp = 1; break;
			case 'prev': if (p.page > 1) p.newp = parseInt(p.page) - 1; break;
			case 'next': if (p.page < p.pages) p.newp = parseInt(p.page) + 1; break;
			case 'last': p.newp = p.pages; break;
			case 'input':
				var nv = parseInt($('.pcontrol input', this.pDiv).val());
				if (isNaN(nv)) nv = 1;
				if (nv < 1) nv = 1;
				else if (nv > p.pages) nv = p.pages;
				$('.pcontrol input', this.pDiv).val(nv);
				p.newp = nv;
				break;
		}

		if (p.newp == p.page) return false;

		if (p.onChangePage)
			p.onChangePage(p.newp);
		else
			this.load();
	},
	_zebra: function () {
		var index = this._sortIndex;
		$("tbody tr:odd", this.element).addClass("odd");

		if (index > 0) {
			$("tbody tr").each(function () {
				//$("td:nth-child(" + index + ")", this).css("background-color", "#f9f9f9");
			});
		}
	},
	_buildPager: function () {
		var options = this.options;
		var grid = this;

		$('.page-txt').val(options.page);
		$('.total-of').html("of " + Math.ceil(options.total / options.pageSize));

		var r1 = (options.page - 1) * options.pageSize + 1;
		var r2 = r1 + options.pageSize - 1;

		if (options.total < r2) r2 = options.total;

		var stat = options.pagestat;

		stat = stat.replace(/{from}/, r1);
		stat = stat.replace(/{to}/, r2);
		stat = stat.replace(/{total}/, options.total);

		$('.page-stat').html(stat);
	},
	_headers: function () {

		var grid = this;
		var options = this.options;

		$("th[rel]", grid.element).each(function () {
			var th = $(this);

			options.sortname = th.attr("rel");

			var div = $("<div/>").click(function () {

				if (options.sortname == th.attr('rel')) {
					if (options.sortorder == 'asc')
						options.sortorder = 'desc';
					else
						options.sortorder = 'asc';
				}
				grid._sortIndex = th.index() + 1;

				th.addClass('sorted').siblings().removeClass('sorted');
				$("div", th.parent()).removeClass('sort-asc');
				$("div", th.parent()).removeClass('sort-desc');
				$(this).addClass('sort-' + options.sortorder);
				options.sortname = th.attr('rel');

				grid.load();
			});

			th.wrapInner(div).addClass("grid-sort");
		});
	},
	_sortIndex: 0,
	load: function () {
		this.loading = true;

		var grid = this;
		var options = this.options;

		var data = {
			sortname: options.sortname,
			sortorder: options.sortorder,
			page: options.page,
			pageSize: options.pageSize
		};

		$("tbody", this.element).load(this.options.url, data, function () {
			grid.loading = false;
			grid._zebra();
		});
	},
	_doSomething: function () {
		// internal functions should be named with a leading underscore
		// manipulate the widget
	},
	value: function () {
		// calculate some value and return it
		return this._calculate();
	},
	length: function () {
		return this._someOtherValue();
	},
	destroy: function () {
		$.Widget.prototype.destroy.apply(this, arguments); // default destroy
		// now do other stuff particular to this widget
	}
});