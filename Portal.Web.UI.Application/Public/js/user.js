$(function () {
    $("li:first", "#nav").addClass("first");


    $('.menu-nav > li').bind('mouseover', openSubMenu);
    $('.menu-nav > li').bind('mouseout', closeSubMenu);

    $('.menu > li').bind('mouseover', openSubSubMenu);
    $('.menu > li').bind('mouseout', closeSubSubMenu);

    $('.sub-menu > li').bind('mouseover', openSubSubSubMenu);
    $('.sub-menu > li').bind('mouseout', closeSubSubSubMenu);


    $(".img_bg").mouseenter(function () { $(this).find(".tab_move").animate({ bottom: '0px' }, 500) });
    $(".img_bg").mouseleave(function () { $(this).find(".tab_move").animate({ bottom: '-165px' },100) });

   

    $("a.delete-message").click(function () {
        var a = this;
        if (confirm("Are you sure?")) {
            $.post(a.href, null, function( data ) {                
                if (data == "111") {
                    $("#" + $(a).attr("removeid")).remove();
                    alert("ההודעה נמחקה בהצלחה!");
                } else {
                    alert("מצטערים, אין לך הרשאה לביצוע פעולה זו, אנא פנה למנהל הפורום");
                }
            });
        }
        return false;
    });


    $.datepicker.regional['he'] = {
        closeText: 'סגור',
        prevText: '&#x3c;הקודם',
        nextText: 'הבא&#x3e;',
        currentText: 'היום',
        monthNames: ['ינואר', 'פברואר', 'מרץ', 'אפריל', 'מאי', 'יוני',
		'יולי', 'אוגוסט', 'ספטמבר', 'אוקטובר', 'נובמבר', 'דצמבר'],
        monthNamesShort: ['1', '2', '3', '4', '5', '6',
		'7', '8', '9', '10', '11', '12'],
        dayNames: ['ראשון', 'שני', 'שלישי', 'רביעי', 'חמישי', 'שישי', 'שבת'],
        dayNamesShort: ['א\'', 'ב\'', 'ג\'', 'ד\'', 'ה\'', 'ו\'', 'שבת'],
        dayNamesMin: ['א\'', 'ב\'', 'ג\'', 'ד\'', 'ה\'', 'ו\'', 'שבת'],
        weekHeader: 'Wk',
        dateFormat: 'dd/mm/yy',
        firstDay: 0,
        isRTL: true,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $.datepicker.setDefaults($.datepicker.regional['he']);
});


function openSubMenu() {
    $(this).find('ul.menu').css('display', 'block');
}

function closeSubMenu() {
    $(this).find('ul.menu').css('display', 'none');
    $(this).find('ul.sub-menu').css('display', 'none');
    $(this).find('ul.subsub-menu').css('display', 'none');
}


function openSubSubMenu() {

    $(this).find('ul.sub-menu').css('display', 'block');
}

function closeSubSubMenu() {
    $(this).find('ul.sub-menu').css('display', 'none');
    $(this).find('ul.subsub-menu').css('display', 'none');
}



function openSubSubSubMenu() {

    $(this).find('ul.subsub-menu').css('display', 'block');
}

function closeSubSubSubMenu() {
    $(this).find('ul.subsub-menu').css('display', 'none');
}


function add_like(e) {
   var forum_id=e.attr('name');

   if (e.attr('title') == 'UnLike') {
       $.post("/forums/counterUnLike", {
           id: forum_id
       }, function (item) {
           e.val(item);
           e.attr('title', 'Like');
           e.removeClass('btn_Unlike');
           e.addClass('btn_like');
       });

   }
   else {
       $.post("/forums/counter", {
           id: forum_id
       }, function (item) {
           e.val(item);
           e.attr('title', 'UnLike');
           e.removeClass('btn_like');
           e.addClass('btn_Unlike');
       });

   }
}


//cookies
(function ($) {
    $.cookie = function (key, value, options) {

        // key and at least value given, set cookie...
        if (arguments.length > 1 && (!/Object/.test(Object.prototype.toString.call(value)) || value === null || value === undefined)) {
            options = $.extend({}, $.cookie.defaults, options);

            if (value === null || value === undefined) {
                options.expires = -1;
            }

            if (typeof options.expires === 'number') {
                var days = options.expires, t = options.expires = new Date();
                t.setDate(t.getDate() + days);
            }

            value = String(value);

            return (document.cookie = [
                encodeURIComponent(key), '=', options.raw ? value : encodeURIComponent(value),
                options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
                options.path ? '; path=' + options.path : '',
                options.domain ? '; domain=' + options.domain : '',
                options.secure ? '; secure' : ''
            ].join(''));
        }

        // key and possibly options given, get cookie...
        options = value || $.cookie.defaults || {};
        var decode = options.raw ? function (s) { return s; } : decodeURIComponent;

        var cookies = document.cookie.split('; ');
        for (var i = 0, parts; (parts = cookies[i] && cookies[i].split('=')) ; i++) {
            if (decode(parts.shift()) === key) {
                return decode(parts.join('='));
            }
        }
        return null;
    };

    $.cookie.defaults = {};

})(jQuery);
