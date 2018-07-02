var $$mobile;
$$.createModule("mobile", function (api, module) {
    api.requireModules(["Util"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, object = util.object;
    var mobile = {};

    var component = mobile.component = {};

    function onceExec(p, n) {
        var _exec = function (p) {
            if (p.execed) return true;
            if (p.satisfy()) {
                p.func();
                p.execed = true;
                return true;
            }
            return false;
        }

        if (!_exec(p) && p.resize) {
            var en = "resize.__mobile_onceExec_" + n;
            $(window).on(en, function () {
                if (_exec(p)) {
                    $(window).off(en);
                }
            });
        }
    }

    mobile.max = function (p) { //当屏幕最大宽度为width时，执行func,仅执行一次,resize:在函数没有执行的前提下，在页面大小发生改变时，再检测屏幕大小是否满足要求，如果满足则执行
        if (arguments.length > 1) { p = { width: arguments[0], func: arguments[1], resize: arguments[2] || false }; }
        p.satisfy = function () {
            return J(window).width() <= this.width;
        }
        onceExec(p, "max");
    }

    mobile.min = function (p) { //当屏幕宽度大于width时，执行func,仅执行一次
        if (arguments.length > 1) { p = { width: arguments[0], func: arguments[1], resize: arguments[2] || false }; }
        p.satisfy = function () {
            return J(window).width() > this.width;
        }
        onceExec(p, "min");
    }

    var tipMask = $("<div class='mobile-tip-mask'></div>")
    $(document.body).append(tipMask);
    mobile.tip = function (o) {
        if (!o) o = document.body;
        $(o).find(".mobile-tip").each(function () {
            var t = $(this);
            t.off("touchstart.mobile-tip");
            t.on("touchstart.mobile-tip", function (e) {
                var c = $(this), of = c.offset();
                tipMask.width(c.outerWidth());
                tipMask.height(c.outerHeight());
                tipMask.css({ left: of.left+"px", top: of.top+"px" });
                tipMask.show();
            });

            t.off("touchend.mobile-tip");
            t.on("touchend.mobile-tip", function (e) {
                var c = $(this);
                tipMask.hide();
            });

            t.off("touchcancel.mobile-tip");
            t.on("touchcancel.mobile-tip", function (e) {
                var c = $(this);
                tipMask.hide();
            });
        });
    }

    component.sidebar = function () { //快捷高效的侧边栏组件
        this.give = function (o) { //o是代理对象
            o.show = function () { o.getJquery().jqmShow(); }
            o.close = function () { o.getJquery().jqmHide(); }
            init(o);
        }
        function init(o) { o.getJquery().jqm(); }
    }

    J(document).ready(function () {
        mobile.tip();
    });

    $$mobile = $$.mobile = mobile;
});