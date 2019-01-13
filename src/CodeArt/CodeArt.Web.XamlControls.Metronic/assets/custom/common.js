Dropzone.autoDiscover = false; //使用该语句禁止Dropzone的自动发现功能，我们需要自己初始化组件
Dropzone.confirm = function (question, accepted, rejected) {
    $$metronic.sweetAlert.confirm(question, '', accepted, rejected);
};
var $$page = $$.page = { context: {}};//代表页面级别的命名空间
(function () {
    $$.page.setUserBadge = function(n) {
        var b = $("#user-photo-badge");
        if (n > 0) {
            b.show();
            b.find("span").text(n);
        }
        else
            b.hide();
    }

    $$.page.closeMask = function (callback) {
        var mask = $(".body-mask");
        mask.fadeOut("slow", function () {
            $(".body-mask").remove();
            callback();
        });
    };

    var readies = [];
    $$.page.ready = function (callback) {
        if (_onready) callback();
        readies.push(callback);
    }

    var _onready = false;
    $$.page.onready = function () {
        _onready = true;
        readies.each(function () {
            this();
        });
    };

    $$.page.block = function (target) { //页面锁定，如果target所在modal中，那么锁定moadl,否则锁定整个页面
        if (target) {
            var modal = $$.metronic.modal.get(target);
            if (modal.length == 0) $$.page.block();
            else modal.proxy().block();
        }
        else {
            mApp.blockPage({
                overlayColor: '#000000',
                state: 'primary',
                size: 'lg'
                //message: $$strings.PleaseWaitWithDots 由于chrome升级后，文字无法居中，所以等metronic升级
            });

            $(".blockOverlay").css("z-index", 9500); //修正

        }
    }

    $$.page.unblock = function (target) {
        if (target) {
            var modal = $$.metronic.modal.get(target);
            if (modal.length == 0) $$.page.unblock();
            else modal.proxy().unblock();
        }
        else {
            mApp.unblockPage();
        }
    }

    $$.page.resize = function (func) {
        mUtil.addResizeHandler(func);
    }

    $$.page.notice = function (cont, p) {
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-top-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
        if (p) {
            $$.util.object.eachValue(p, function (n, v) {
                toastr.options[n] = v;
            });
        }
        if (!cont.type) cont.type = "info";
        if (cont.message)
            toastr[cont.type](cont.message,cont.title);
        else
            toastr[cont.type](cont.title);
    }

    mApp.scrollTo = function(target, offset) { //target可以为null
        el = $(target);
        var pos = (el && el.length > 0) ? el.offset().top : 0;
        pos = pos + (offset ? offset : 0);

        jQuery('html,body').animate({
            scrollTop: pos
        }, 'slow');
    }

    $$.page.scrollTo = function (target, offset) {
        mApp.scrollTo(target, offset);
    }

    $$.page.scrollTop = function () {
        mApp.scrollTo();
    }

})();

$$.page.ready(function () {
    $$.config.setAjaxEvent(new function () {
        var _times = 0, blockPage = false;
        this.beforeSend = function () {
            _times++;
            if (_times == 1) {
                var c;
                if (c = $$metronic.modal.current()) {
                    c.block();
                } else {
                    blockPage = true;
                    $$.page.block();
                }
            }
        }

        this.complete = function () {
            _times--;
            if (_times <= 0) {
                if (blockPage) {
                    $$.page.unblock();
                    blockPage = false;
                }
                var c;
                if (c = $$metronic.modal.current()) {
                    c.unblock();
                }
                _times = 0;
            }
        }

        this.error = function (msg) {
            swal($$strings.SystemAbnormal, msg, "error");
        }

    });
});

(function () {//菜单
    var menu = $$.page.menu = {
        init: function () {
            menu.view.show();
            shortcut.show();
            user.show();//与用户有关的菜单
            sameLevel.show();
        }
    };

    menu.setBadge = function(id, n) {
        if (n > 0) {
            var o = $("#" + id + " span.m-menu__link-text").mapNull() || $("#" + id + " span.m-nav__link-text").mapNull();
            if (o) o.after("<span class=\"m-nav__link-badge\"><span class=\"m-badge m-badge--success\">" + n + "</span></span>");
        }
    }

    menu.view = {};
    var mv = menu.view;
    mv.show = function () {
        if (!mv.data) return;
        var d = { childs: mv.data.childs || mv.data };
        if (mv.redirect) {
            _address = _pathname = mv.redirect.toLowerCase();
        }
        pretreatment(d);
        showByAuto(d);
        showNavigator();
    }

    //function showByManual(d, md) {
    //    var sf;//被选的一级菜单
    //    d.childs.each(function (i, f) {
    //        if (f.name == md.root) {
    //            f.selected = true;
    //            sf = f;
    //            return false;//已标记，退出循环
    //        }
    //    });

    //    $$("#m_ver_menu").bind(d);
    //    if (!sf) alert("没有找到名称为" + md.root + "的一级菜单");
    //}

    //manual
    function showByAuto(d) {
        markSelected(d);
        var m = $("#m_ver_menu").mapNull();
        if (m) {
            var p = m.proxy();
            if (p.bind) m.proxy().bind(d);
        }
    }

    var _navs = [];
    function showNavigator() {
        if (_navs.length == 0) return;
        var nav = _navs.first(function (t) {
            return this.selected;
        });
        if (!nav) return;
        var p = $("#pageNavigator");
        p.show();
        p.proxy().bind(nav.navigator);
    }

    function pretreatment(item) {
        if (containsTag(item, "navigator")) {
            var childs = item.childs;
            item.navigator = { childs: childs };
            item.childs = null;
            var codes = item.codes = childs.select((t) => t.code);
            if (codes.length > 0)
                item.code = codes[0];
            _navs.push(item); //加入到导航列表
            return;
        }

        var cs = item.childs;
        if (cs && cs.length > 0) {
            for (var i = 0, le = cs.length; i < le; i++) {
                pretreatment(cs[i]);
            }
        }
    }

    function markSelected(item) {
        if (sameAddress(item)) {
            item.selected = true;
            return true; //找到了， 返回true
        }
        var cs = item.childs;
        if (cs && cs.length > 0) {
            for (var i = 0, le = cs.length; i < le; i++) {
                if (markSelected(cs[i])) {
                    item.selected = true; //如果子项被选中，表示父亲也被选中
                    return true;
                }
            }
        }
        return false;
    }

    var _address = location.href.toLowerCase();
    var _pathname = location.pathname.toLowerCase();
    function sameAddress(item) {
        if (item.codes) {
            for (var i = 0; i < item.codes.length; i++) {
                var c = item.codes[i];
                if (sameAddress({ code: c })) return true;
            }
            return false;
        }
        else {
            var c = item.code || {}, u = c.url || '';
            u = u.trim().toLowerCase();
            var p = u.indexOf('?');
            if (p > -1) u = u.substr(0, p);
            return _pathname == u || _address == u;
        }
        
    }

    function haveChilds(d) {
        return !$$.util.empty(d.childs) && d.childs.length > 0;
    }

    function fillLink(j, d) {
        if (d.selected) {
            j.addClass("m-menu__item--active");
            //setSelect(j);
        }
        var a = j.find("a").first();
        bindLink(a, d);
        setIcon(a.find("i").first(), d, "flaticon-menu-2");
        a.find("i").last().remove();
        j.find("div").first().remove();
    }

    function setIcon(o,d,defIcon) {
        var icon = d.icon ? d.icon : defIcon;
        o.addClass(icon);
        var s = d.iconFontSize;
        if (s) {
            o.css("font-size",s);
        }
    }

    mv.onbind = function (o, d) {
        var j = o.getJquery(), hc = haveChilds(d);
        if (hc) {
            j.addClass("m-menu__item--submenu m-menu__item--expanded"); //追加了m-menu__item--expanded表示所有菜单可以同时展开，如果不加，那么同一时间只有一个节点可以展开
            j.attr("m-menu-submenu-toggle","hover");
            if (d.selected) {
                j.addClass("m-menu__item--open");
                //setSelect(j);
            } else if (containsTag(d,"opened")) {
                j.addClass("m-menu__item--open");
            }
            var a = j.find("a").first();
            a.addClass("m-menu__toggle");
            setIcon(a.find("i").first(), d, "flaticon-layers");
        }
        else {
            fillLink(j, d);
        }
    }

    mv.onThreeBind = function (o, d) {  //第3级菜单的绑定事件
        mv.onbind(o, d);
        var oj = o.getJquery();
        oj.off("mouseover.menu");
        oj.off("mouseout.menu");
        oj.on("mouseover.menu", function () {
            var menu = oj.find(".c-menu-float");
            if ($(".m-aside-left--minimize").length > 0) {
                menu.removeClass("c--menu-hover");
            }
            else {
                menu.addClass("c--menu-hover");
            }
            menu.show();
        });

        oj.on("mouseout.menu", function () {
            oj.find(".c-menu-float").hide();
        });

        oj.removeClass("m-menu__item--open");
    }

    mv.onLastBind = function (o, d) {  //第3级菜单的绑定事件
        var j = o.getJquery();
        var a = j.find("a").first();
        bindLink(a, d);
    }

    function containsTag(d, t) {
        return d.tags && d.tags.contains(function () {
            return this == t;
        });
    }

    //function setSelect(j) {
    //    var p = j.parent();
    //    while (p && !p.is("#m_ver_menu")) {
    //        if (p.is("li")) {
    //            p.proxy().data.selected = true;
    //            break;
    //        }
    //        p = p.parent();
    //    }
    //}

    function bindLink(o, d) {
        var code = d.code || {}, t;
        if (t = code["url"]) o.attr("href", t); else { o.attr("href", "#"); $(o).on("click", function (e) { e.preventDefault(); }); }
        if (t = code["target"]) o.attr("target", t);
    }

    //自动显示当前同级菜单至顶部
    var sameLevel = $$.page.menu.sameLevel = {};
    sameLevel.show = function () {
        var level = $("#sameLevel").mapNull();
        if (!level) return;
        var d = { childs: mv.data.childs || mv.data };
        var p = [];
        fillPath(p, d);
        if (p.length < 2) {
            level.remove(); //当前选中的菜单没有父亲，就无法快速选取同级菜单了
            return;
        }
        var parent = p[p.length - 2];
        level.proxy().bind(parent);
        setIcon($("#sameLevelIcon"), parent, "flaticon-layers");
    }

    function fillPath(l, d) { //获得被选择的数据的路径
        if (d.selected) l.push(d);
        $(d.childs).each(function () {
            fillPath(l, this);
        });
    }

    mv.onSameLevelBind = function (o, d) {
        var j = o.getJquery(), hc = haveChilds(d);
        if (hc) {
            j.addClass("m-menu__item--submenu");
            j.attr("m-menu-submenu-toggle", "hover");
            var a = j.find("a").first();
            a.addClass("m-menu__toggle");
            setIcon(a.find("i").first(), d, "flaticon-layers");
        }
        else {
            var a = j.find("a").first();
            bindLink(a, d);
            setIcon(a.find("i").first(), d, "flaticon-menu-2");
            a.find("i.m-menu__hor-arrow").remove();
            a.find("i.m-menu__ver-arrow").remove();
            j.find("div.m-menu__submenu").remove();
        }
    }

    //快捷方式
    var shortcut = $$.page.menu.shortcut = {};
    shortcut.show = function () {
        var j = $("#shortcut").mapNull();
        if (!j) return;
        var o = j.proxy();
        var d = { childs: mv.data.childs || mv.data };
        var t = []; //t就是快捷项的集合
        fillTagData(t, d,"shortcut");
        if (t.length == 0) {
            j.remove(); //没有快捷项，移除快捷方式的dom
            return;
        }
        t = t.group(5);
        var sd = { group: [] };
        t.each(function (i) {
            var n = $$strings.ShortcutGroup + (i + 1);
            sd.group.push({ name:n, items: this });
        });
        var c = $$("#shortcutContent"), cj = c.getJquery();
        cj.css("width", (sd.group.length * 250) + "px"); //每个分组占250px的宽
        c.bind(sd);
    }

    //用户有关的链接
    var user = $$.page.menu.user = {};
    user.show = function () {
        var cj = $("#userContent").mapNull();
        if (!cj) return;
        var c = cj.proxy();
        var d = { childs: mv.data.childs || mv.data };
        var l = [];
        fillTagData(l, d, "user");
        c.bind({ items: l });
    }


    function fillTagData(l, d,tag) {
        if (containsTag(d,tag)) {
            var c = { name: d.name, icon: d.icon||'',code:d.code||'' };
            l.push(c);
        }
        $(d.childs).each(function (){
            fillTagData(l, this, tag);
        });
    }

    mv.onTagItemBind = function (o, d) {
        var j = o.getJquery(), a = j.find("a").first(), i = j.find("i").first();
        bindLink(a, d);
        setIcon(i, d, "flaticon-menu-2");
    }

    //页面导航
    var nav = $$.page.menu.navigator = {};
    nav.onbind = function (o, d) {
        var j = o.getJquery();
        var a = j.find("a").first();
        bindLink(a, d);

        if (d.icon) {
            var l = ["<i></i>", d.name];
            a.html(l.join(''));
            setIcon(a.find("i").first(), d);
        }
        else {
            a.html(d.name);
        }

        if (sameAddress(d))
            a.addClass("active");
        else
            a.removeClass("active");
    }


})();

/*路径导航*/
(function () {
    var bar = $$.page.bar = {};
    bar.items = [];//附加项
    bar.init = function () {
        var j = $("#sitePath").mapNull();
        if (!j) return;
        var b = j.proxy();
        var items = [];
        var data = $$.page.menu.view.data || { childs: [] };
        fillItems(items, { childs: data.childs || data });
        if (items.length == 0) {
            j.hide();
            //当没有路径导航时，将右侧锁闭
            $("#m_aside_left_minimize_toggle").click();
            return;
        }
        var text = items.last()[0].text;
        items = items.concat(bar.items);
        items.last()[0].last = true;
        b.bind({ text: text,items: items });
    }

    bar.title = function (t) {sitePath
        var j = $("#sitePath").mapNull();
        if (!j) return;
        var title = j.children("h3").first();
        title.removeClass("m-subheader__title--separator"); //移除右边的|,这样当注入标题时就没有|后缀了
        title.text(t);
        j.children("ul").first().hide();
        j.show();
    }

    function fillItems(items, d) {
        if (!d.childs) return;
        d.childs.each(function (i, t) {
            if (t.selected) {
                var url = (t.code && t.code["url"]) || '';
                items.push({ text: t.name, url: url });
                if (t.childs) fillItems(items, t);
                return false;
            }
        });
    }

    $$.page.bar.onbind = function (o, d) {
        if (d.last) return;
        var j = o.getJquery();
        j.after("<li class=\"m-nav__separator\">&nbsp;-&nbsp;</li >");
    }

})();

/*层级结构的列表*/
(function () {
    var level = $$.page.level = {};
    level.init = function (g) {
        var bar = $$(g.bar), list = $$(g.list);
        bar.onbind = function () { list.load(); }
        bar.getLevel = function () {
            //获取bar的层级数，这个层级数不包括菜单项
            var items = this.items(), i = 0;
            items.each(function (j, t) {
                if (!$$.util.empty(t.value)) i++;
            });
            return i + 1;
        }

        var listLoaded = list.getEvent("load");
        list.on("load", function (o, d) {
            o = o.getJquery();
            var len = bar.getLevel(), max = g.maxLevel;
            if (len < max) {
                o.find("*[data-name='next']").show();
                o.off("click.next", "*[data-name='next']");
                o.on("click.next", "*[data-name='next']", function (e) {
                    var o = $$(this), d = o.data;
                    bar.push({
                        text: d.name, value: d.id, click: function (e) {
                            bar.pop(e.data.item);
                        }
                    });
                });
            }
            else {
                o.find("*[data-name='next']").hide();
            }
            if (listLoaded) listLoaded(o, d);
        });
        if (g.rootName) {//兼容老写法
            var r = g.root || {};
            g.root = r;
            g.root.name = g.rootName;
        }
        if (g.root) { //绑定根结点
            bar.push({ text: g.root.name, value: g.root.value, click: function (e) { bar.pop(e.data.item); } });
        }
    }
})();


//暂时 用它做图片呈现
//(function () {
//    var util = $$.page.util = {};
//    function image() {
//        this.onBind = function (dn, url, w, h, c) {//图片加载路径，宽度，高度，裁剪类型

//            if (!c) c = 1;
//            return function (o, d) {
//                var oj = o.getJquery();
//                var s = url.indexOf('?') < 0 ? '?' : '&';
//                var org = url + s + "key=" + d[dn] + "&w=" + w + "&h=" + h + "&c=" + c;
//                o.attr("src", "/framework/codeart/wrapper/metronic/images/disk/ico_dark/jpg.jpg");
//                o.attr("data-original", org);
//                oj.css({ width: w, height: h });
//                oj.lazyload({
//                    effect: "fadeIn"
//                });


//            }
//        }
//    }
//    util.image = new image();
//})();


var $$metronic;
$$.createModule("metronic", function (api, module) {
    var metro = $$metronic = $$.metronic = {};
    var util = api.util, empty = util.empty, copyEvent = $$.ajax.util.copyEvent;
    var type = util.type;

    swal.setDefaults({
        heightAuto: true //解决滚动条置顶的问题
    });

    (function () {
        var sa = metro.sweetAlert = {};
        sa.alert = function (arg) {
            swal(arg);
        }
        sa.error = function (title, text, ok) {
            swal(title, text, "error", ok);
        }
        sa.warning = function (title, text, ok) {
            swal({
                title: title,
                text: text,
                type: 'warning',
                allowOutsideClick: false
            }).then(function (result) {
                if (result.value) {
                    if (ok) ok();
                }
            });
        }
        sa.success = function (title, text, ok) {
            swal(title, text, "success", ok);
        }
        sa.confirm = function (title, text, ok, cancel) {
            swal({
                title: title,
                text: text,
                type: 'warning',
                allowOutsideClick: false,
                showCancelButton: true,
                confirmButtonText: $$strings.OK,
                cancelButtonText: $$strings.Cancel,
                reverseButtons: true
            }).then(function (result) {
                if (result.value) {
                    if (ok) ok();
                } else if (result.dismiss === 'cancel') {
                    if (cancel) cancel();
                }
            });
        }

    })();

    (function () {
        var alert = metro.alert = function () {
            this.give = function (o) {
                o.show = function (d) {
                    var oj = this.getJquery(), my = this;
                    oj.find(".m-alert__text").text(d.message);
                    if (d.icon) {
                        var i = oj.find(".m-alert__icon").find("i");
                        if (i.length == 0) {
                            createIcon(oj);
                            i = oj.find(".m-alert__icon").find("i");
                        }
                        i.attr("class", d.icon);
                    } else if (d.icon === '') {
                        //显示指定了为空，那么移除icon
                        removeIcon(oj);
                    }
                    if (d.color) {
                        var old = my._color;
                        if (old) oj.removeClass("alert-" + old);
                        oj.addClass("alert-" + d.color);
                        this._color = d.color;
                    } else if (d.color === '') {
                        var old = this._color;
                        if (old) oj.removeClass("alert-" + old);
                        my._color = '';
                    }
                    oj.removeClass('m--hide').show();
                    if (d.closeTime > 0) {
                        setTimeout(function () {
                            my.hide();
                        }, d.closeTime); 
                    }
                }

                o.hide = function () {
                    this.getJquery().hide();
                }
            }

            function createIcon(oj) {
                oj.prepend('<div class="m-alert__icon"><i class="{TemplateBinding Icon}"></i></div>');
                oj.addClass("m-alert--icon");
            }

            function removeIcon(oj) {
                oj.find(".m-alert__icon").remove();
                oj.removeClass("m-alert--icon");
            }

        }
    })();

    function isNestInput(e) { //组件是否嵌套组件
        e = e.getJquery();
        return e.attr("formAppend") || e.attr("data-formAppend");
    }

    function setChanged(o) {
        var oj = o.getJquery(), changed = oj.attr("changed") || oj.attr("data-changed");
        eval("o.changed =" + changed + ";");
    }

    (function () {
        var vtor = metro.validator = function (form, arg) { //由于jquery的验证器必须使用在form下，而一个form又不能嵌套子form,导致list组件无法正常工作，所以我们自己实现validator
            var _form = form, _arg = arg;
            var _errors = [];
            this.invalidHandler = arg.invalidHandler || function () { };
            this.drawValid = function (error, input) {
                var p = input.getJquery();
                resetValid(p);
                if (error) {
                    var op = p.proxy();
                    var e = $("<div class=\"form-control-feedback\">" + error + "</div>");
                    if (op.drawValid) {
                        op.drawValid(e);
                        return;
                    }
                    p.addClass("has-danger");

                    p.find(".form-control-feedback").remove();
                    var tip = p.find("[data-tip]").last().mapNull(); //有可能append或者pre嵌套组件，所以使用的find().last()
                    if (tip) {
                        var help = tip.children(".m-form__help").mapNull();
                        if (!help) tip.append(e);
                        else e.insertBefore(help);
                    }
                    else {
                        //data-core-container是组件核心的第一个父亲
                        var c = p.children("[data-core-container]").mapNull();
                        if (!c) return;
                        var help = c.children(".m-form__help").last().mapNull();
                        if (!help) c.append(e);
                        else e.insertBefore(help);
                    }
                }
            }

            function resetValid(p) {
                var op = p.proxy();
                if (op.drawValid) {
                    op.drawValid(null);
                    return;
                }
                p.removeClass("has-danger");
                var c = p.children("[data-core-container]").first();
                p.find(".form-control-feedback").last().remove();
            }

            this.valid = function () {
                var es = [], rs = _arg.rules;
                for (var name in rs) {
                    var p = _form.input(name);
                    var value = p.get();
                    var rules = rs[name]; //name是控件名称,rules是控件要满足的规则
                    var error = null;
                    for (var ruleName in rules) {
                        var b = execValidMethod(ruleName, value, p, rules);
                        if (!b) {
                            error = message(name, ruleName, value, p, rules);
                            es.push(error);
                            break;
                        }
                    }
                    this.drawValid(error, p);
                }

                _errors = es;
                if (es.length > 0) this.invalidHandler(this);
                return _errors.length == 0;
            }

            this.element = function (ele) {
                var p = ele.proxy(), name = getName(p);
                var rules = _arg.rules[name]; //name是控件名称,rules是控件要满足的规则
                var error = null, value = p.get();
                for (var ruleName in rules) {
                    var b = execValidMethod(ruleName, value, p, rules);
                    if (!b) {
                        error = message(name, ruleName, value, p, rules);
                        _errors.push(error);
                        break;
                    }
                }
                this.drawValid(error, p);

                if (isNestInput(p)) { //如果有子组件，那么继续检查子组件
                    var my = this;
                    p.find("[data-field]").each(function () {
                        my.element($(this));
                    });
                }
            }

            this.reset = function () {
                var my = this, ps = _form.inputs();
                ps.each(function () {
                    my.drawValid(null, this.getJquery());
                });
                _errors = [];
            }

            function message(name, ruleName, value, element, rules) {
                var msg = _arg.messages[name][ruleName];
                if (empty(msg)) {
                    //从预定义中得到消息提示
                    return getMessage(ruleName, value, element, rules);
                }
                if (type(msg) == "function") return msg(value, element, rules);
                return msg;
            }

            this.errors = function () {
                return _errors;
            }

            this.getErrorsMessage = function() {
                return _errors.join(';');
            }
        }

        var _ruleMethods = {};
        vtor.addMethod = function (ruleName, valid, msg) {
            ruleName = ruleName.toLower();
            _ruleMethods[ruleName] = {
                valid: valid,
                message: msg
            };
        }

        function execValidMethod(ruleName, value, element, rules) { //执行验证
            ruleName = ruleName.toLower();
            var method = _ruleMethods[ruleName].valid;
            return method(value, element, rules);
        }

        function getMessage(ruleName, value, element, rules) { //执行验证
            ruleName = ruleName.toLower();
            var msg = _ruleMethods[ruleName].message;
            if (type(msg) == "function") return msg(value, element, rules);
            return msg;
        }

        vtor.addMethod("required", function (value, element, rules) {
            if (!rules.required) return true;
            if (empty(value)) return false;
            return getLength(value) > 0;
        });

        function getLength(value) {
            if (empty(value)) return 0;
            var ty = type(value);
            var v = ty == "array" ? value : value.toString();
            return v.length;
        }

        vtor.addMethod("minlength", function (value, element, rules) {
            if (rules.minlength <= 0) return true;
            var le = getLength(value);
            if (le == 0) return true; //不为空的验证交由required
            return le >= rules.minlength;
        });


        vtor.addMethod("maxlength", function (value, element, rules) {
            if (rules.maxlength <= 0) return true;
            var le = getLength(value);
            if (le == 0) return true; //不为空的验证交由required
            return le <= rules.maxlength;
        });

        vtor.addMethod("rangelength", function (value, element, rules) {
            var le = getLength(value);
            if (le == 0) return true; //不为空的验证交由required
            var min = rules.rangelength[0];
            var max = rules.rangelength[1];
            return le >= min && le <= max;
        });

        vtor.addMethod("email", function (value, element, rules) {
            var le = getLength(value);
            if (le == 0) return true; //不为空的验证交由required
            return /^[\w\-\.]+@[\w\-\.]+(\.\w+)+$/.test(value);
        });

        vtor.addMethod("equalToPrev", function (value, element, rules) {
            var o = element.getJquery();
            var p = o.prev().mapNull();
            if (!p) return false;
            return p.proxy().get() == value;
        });

        function getName(p) {
            return p.attr("data-field") || p.attr("name");
        }

        vtor._getName = getName;

    })();

    //表单组件
    (function () {
        function getName(p) {
            return metro.validator._getName(p);
        }

        var form = metro.form = function () {
            var self = this, _formId, _formName, _openRecord; //是否开启记录模式，也就是修改提交的时候，没有变的值不提交
            var _inputs = [], _customValidate;

            self.give = function (o) {
                var oj = o.getJquery();
                var t;//表单名称
                if (t = o.name) _formName = t;
                if (t = oj.attr("name")) _formName = t;
                if (t = oj.attr("id")) _formId = t;
                if (t = o.validate) _customValidate = t;
                if (t = (oj.attr("data-record") || oj.attr("record"))) _openRecord = true;

                //inputs start
                o.inputs = function () { return _inputs; }
                o.input = function (n) {
                    n = n.toLower();
                    return _inputs.first(function (i, e) {
                        return $$(e).name.toLower() == n;
                    });
                }
                o.append = function (l) {//追加输入组件
                    append(this, l);
                }
                o.remove = function (l) {//移除输入元素，l为元素名称或dom对象的集合
                    var my = this;
                    if (type(l) != "array") l = [l];
                    var ps = _inputs, rp = [];//被移除的元素
                    l.each(function (i, n) {
                        var p = util.type(n) == 'string' ? my.input(n) : $$(n);
                        if (p) {
                            p.formOwner = null;
                            ps.remove(function (e, n) {
                                return $$(e).name == n || $$(e).ent() == n;
                            }, n);
                            rp.push(p);
                        }
                    });
                    return rp;//返回被移除的对象
                }
                collect(o, o.ent()); //收集inputs数据

                //inputs end
                var _record = null;
                function equalsRecord(p, v) {
                    if (!_openRecord) return false;
                    if (!_record) return false;
                    if (p.getJquery().attr("ignoreRecord")) return; //显示指定了忽略记录的功能，那么就不需要对比
                    var n = p.name;
                    var rv = _record[n.toLower()];
                    return util.json.serialize(v) === util.json.serialize(rv);
                }

                function setRecord(n, v) {
                    if (!_record) _record = {};
                    _record[n.toLower()] = v;
                }

                o.updateRecord = function () {//更新记录值
                    _inputs.each(function (i, e) {
                        exec($$(e), "get", function (p) {
                            setRecord(p.name, p.get());
                        });
                    });
                }

                o.set = function (data, b) {//接收数据,b:是否验证
                    var my = this; //set方法不重置数据，针对性的改数据
                    for (var e in data) {
                        var p = my.input(e);
                        if (p) {
                            var po = p.proxy();
                            po.set(data[e], b);
                            if (_openRecord)
                                setRecord(e, po.get());//为了保证数据一致，我们获取一次值
                        }
                    }
                }

                o.accept = function (data, b) { //接受数据，该方法会先重置表单，再赋值数据
                    this.reset();
                    this.set(data, b);
                }

                o.get = function (n) {
                    var v = {};
                    _inputs.each(function (i, e) {
                        exec($$(e), "get", function (p) {
                            if (p.ignoreForm) return; //忽略form，那么不参表单行为
                            var pv = p.get();
                            if (pv == null) return; //如果返回的值显示指明了为null，那么不提交该值
                            if (equalsRecord(p, pv)) return; //如果值等于记录中的数据，那么不提交，这表示值没有改变过，不用提交修改
                            v[p.name] = pv;
                        });
                    });
                    return v;
                }

                o.disable = function (b) {
                    _inputs.each(function (i, e) {
                        exec($$(e), "disable", function (p) {
                            if (p.ignoreForm) return; //忽略form，那么不参与表单行为
                            p.disable(b);
                        });
                    });
                }

                o.alert = function (d) {//{type:'success|danger',message:'...'}
                    var a = this.findAlert();
                    if (!a) return;
                    a.show(d);
                    if ($$.metronic.modal.get(this)) return; //如果在窗口里面的表单，那么不移动滚动条
                    $$.page.scrollTo(a.getJquery(), -200);
                }

                o.closeAlert = function () {
                    var a = this.findAlert();
                    if (!a) return;
                    a.hide();
                }

                o.alertSuccess = function (msg) {
                    this.alert({ icon: "la la-check", color: "success", message: msg, closeTime: 2000 });
                }

                o.alertDanger = function (msg) {
                    this.alert({ icon: "la la-warning", color: "danger", message: msg });
                }

                o.findAlert = function () {
                    var a = oj.find(".alert").mapNull();
                    return a && a.proxy();
                }

                o.scriptElement = function () { //获得脚本元素的元数据
                    var my = this, v = my.get();
                    var p = new function () {
                        this.validate = function () {
                            return my.validate();
                        }

                        if (_openRecord) {
                            this.override = function (tg) {
                                var _s = tg.success;
                                if (_s) {
                                    tg.success = function (r) {
                                        _s(r);
                                        my.updateRecord();
                                    }
                                }
                                else {
                                    tg.success = function (r) {
                                        my.updateRecord();
                                    }
                                }
                            }
                        }
                    }

                    return { id: _formId || '', metadata: { data: v }, eventProcessor: p };
                }

                o.validate = function (ele) {
                    initValidator(this);
                    if (ele) {
                        //验证某个项
                        var success = _validator.element(ele.getJquery());
                        return new $$view.ValidateResult(success ? "success" : "error", _validator.getErrorsMessage(), success);
                    } else {
                        this.closeAlert();
                        var form = this.getJquery();
                        var success = _validator.valid();
                        return new $$view.ValidateResult(success ? "success" : "error", _validator.getErrorsMessage(), success);
                    }
                }

                o.validateElement = function (p) { //验证某个元素
                    return this.validate(p);
                }

                o.submit = function (tg, vl) {//vl:是额外需要提交的值,键值对
                    if (!tg) throw new Error("没有设置目标，无法提交表单");
                    var my = this;
                    if (!my.validate()) return;
                    var req = new $$.ajax.request();
                    copyEvent(self, my, true);
                    copyEvent(my, req);
                    var data = this.get();
                    util.object.eachValue(data, function (n, v) {
                        req.add(n, v);
                    });
                    //_inputs.each(function (i, e) {
                    //    exec($$(e), "get", function (p) {
                    //        if (!p.needSubmit || p.needSubmit()) {
                    //            req.add(p.name, p.get());
                    //        }
                    //    });
                    //});
                    if (vl) {
                        util.object.eachValue(vl, function (n, v) {
                            req.add(n, v);
                        });
                    }
                    req.submit(tg);
                }

                o.reset = function () {
                    this.closeAlert();
                    _inputs.each(function (i, e) {
                        exec($$(e), "reset", function (p) {
                            p.reset();
                        });
                    });
                    if (_validator)
                        _validator.reset();
                    _record = null;
                }

                //o.rebuild = function () {
                //    //目前重建表单仅做了重新建立规则
                //    initValidator(this);
                //}

            }

            var _validator;

            function initValidator(o) {
                _validator = createValidator(o);
            }

            function createValidator(o) {
                var rules = {}, messages = {};
                //收集控件提供的规则
                var ps = _inputs;
                ps.each(function (i, e) {
                    var p = $$(e);
                    if (p.ignoreForm) return; //忽略form，那么不参与表单行为
                    var n = getName(p);
                    var r = typeof p.rule === 'function' ? p.rule() : p.rule;
                    rules[n] = r.rules;
                    messages[n] = r.messages;
                });
                var validator = new metro.validator(o,{
                    rules: rules,
                    messages: messages,
                    invalidHandler: function (validator) {
                        var msg = validator.getErrorsMessage();
                        o.alertDanger(msg);
                    }
                });
                return validator;
            }

            function collect(o, target) { //将target区域的组件信息，收集到form中
                append(o, internal(o, target, _formName));
                if (!empty(_formName)) append(o, external(o, target, document.body, _formName)); //定义了表单名称，还要收集外部控件
            }

            function append(o,l) {//追加输入组件
                var my = o;
                if (type(l) != "array") l = [l];
                var ps = _inputs;

                l.each(function (i, p) {
                    p = $(p).proxy();
                    var n = getName(p), op = my.input(n);
                    if (empty(n)) {
                        throw Error("组件没有定义data-field，无法加入form" + p.getJquery().prop("outerHTML")); return;
                    }
                    if (!op) {
                        giveInput(p);
                        ps.push(p);
                        p.formOwner = my;
                    }
                    else if (p != op.proxy()) { throw Error("data-field为" + n + "的输入组件已存在！"); return; }
                });
            }

            function internal(o,c, n, ps) {//n:表单名称
                ps = ps || [];
                var l = $(c).children();
                l.each(function () {
                    var e = this, ej = $(e);
                    var fn = ej.attr("data-form");
                    if (fn == 'none') return;//显示指明form="none"，意为不参与提交
                    if (empty(fn)) internal(o, e, n, ps);
                    else if (fn == '' || fn == n) {
                        append(o, $$(e));
                        if (isNestInput(ej)) { //表示组件内部也有需要加入到表单的组件
                            internal(o, e, n, ps);
                        }
                    }
                });
                return ps;
            }

            function external(o,fe, c, n, ps) {//f:表单实体,n:表单名称
                ps = ps || [];
                if (fe == c) return ps;
                var l = $(c).children();
                l.each(function () {
                    var e = this, ej = $(e);
                    var fn = ej.attr("data-form");
                    if (fn == n) {
                        append(o, $$(e));
                        if (isNestInput(ej)) { //表示组件内部也有需要加入到表单的组件
                            external(o, fe, e, n, ps);
                        }
                    }
                    else external(o,fe, e, n, ps);
                });
                return ps;
            }

            function exec(p, n, f) { //验证组件p是否实现了方法n,如果实现了，则执行函数f
                if (p[n]) { f(p); return }
            }

            function giveInput(p) {//给予 get、set等方法
                if (!p.get) p.get = function () {
                    return this.getJquery().val();
                }
                if (!p.set) p.set = function (v) {
                    this.getJquery().val(v);
                }
                if (!p.rule) {
                    p.rule = getRule(p);
                }
                if (empty(p.name))
                    p.name = getName(p);
            }           
        }

        function getRule(o) {
            var rc = o.attr("data-rule");
            if (!rc) return { rules: {}, messages: {} };
            var r = {};
            if (rc) eval("r=" + rc);
            return r;
        }
    })();

    //portlet
    (function () {
        var portlet = metro.portlet = function () {
            this.give = function (o) {
                o.title = function (t, st) { //主标题、副标题
                    var area = this.getJquery().find("[data-portlet-title='true']").first();
                    if (st) {
                        area.html([t, "<small>", st, "</small>"].join(''));
                    }
                    else
                        area.text(t);
                }

                o.description = function (t) {
                    var area = this.getJquery().find("[data-portlet-description='true']").first();
                    if (area) {
                        area.html(t);
                    }
                }

                o.content = function (es) {
                    var oj = this.getJquery();
                    var body = $(oj.children(".m-portlet__body").first());
                    body.empty();
                    es.each(function () {
                        body.append(this);
                    });
                }

                o.full = function () {
                    var mp = this.mPortlet();
                    mp.fullscreen();
                }

                o.mPortlet = function () {
                    var id = this.attr("id");
                    if (!id) throw new Error("要使用mPortlet必须指定id");
                    return new mPortlet(this.attr("id"));
                    //return this.getJquery().mPortlet();
                }

                o.cover = function (b) { //将整个屏幕盖住，这是指对象原本是隐藏的，后来遮盖了整个屏幕，当缩小后，对象又隐藏了
                    var oj = this.getJquery();
                    var p = this.mPortlet();
                    if (b === false) {
                        p.unFullscreen();
                    }
                    else {
                        oj.show();
                        p.fullscreen();
                        p.one("afterFullscreenOff", function () {
                            oj.hide();
                        });
                    }
                }

            }
        }
    })();

    //modal
    (function () {
        var sp = metro.modal = function () {
            this.give = function (o) {
                var oj = o.getJquery();
                oj.on('show.bs.modal', function () { //在调用 show 方法后触发。
                    push($$(this)); //记录当前打开的窗口
                    if (o.preopen) o.preopen();
                });

                o._modalCallback = [];

                oj.on('shown.bs.modal', function () { //窗口完全显示（动画效果完毕）
                    var cbs = o._modalCallback;
                    cbs.each(function () {
                        this();
                    });
                    o._modalCallback = [];
                    if (o.opened) o.opened();
                });

                oj.on('hide.bs.modal', function () {//在调用 hide 方法后触发。
                    pop();
                })

                oj.on('hidden.bs.modal', function () { //窗口完全退出（动画效果完毕）
                    
                })

                o.open = function (cb) {
                    if (cb) {
                        this._modalCallback.push(cb);
                    }
                    this.getJquery().modal('show');
                }
                o.close = function () {
                    this.getJquery().modal('hide');
                }
                o.title = function (v) {
                    this.getJquery().find(".modal-title").first().text(v);
                }

                o.block = function () {
                    mApp.block(this.find('.modal-content'), {
                        overlayColor: '#000000',
                        state: 'primary',
                        size: 'lg',
                        message: $$strings.PleaseWaitWithDots
                    });
                }

                o.unblock = function () {
                    mApp.unblock(this.find('.modal-content'));
                }
            }
        }

        var _modals = [];//维护所有打开的modals
        sp.current = function () {
            return _modals.length == 0 ? null : _modals.last()[0];
        }
        function push(o) {
            if (_modals.length == 0) {
                _modals.push(o);
                return;
            }
            //更改z-index
            _modals.push(o);
            var zIndex = o.getJquery().css("z-index");
            updateZIndex(zIndex);
        }

        function pop() {
            var last = _modals.pop();
            if (_modals.length == 0) return;
            var zIndex = last.getJquery().css("z-index");
            updateZIndex(zIndex);
        }

        function updateZIndex(maxZIndex) { //最后一个弹出层zindex最大，其余的等比追加值
            var len = _modals.length, value = maxZIndex / len, lastIndex = len - 1;
            var backs = $(".modal-backdrop");
            _modals.each(function (i) {
                var j = this.getJquery();
                if (i == lastIndex) {
                    j.css("z-index", maxZIndex);
                    return;
                }
                var v = (i + 1) * value;
                j.css("z-index", v + 10);
                $(backs[i]).css("z-index", v);
            });
        }

        sp.get = function (t) { //获得t所在的窗口
            return t.getJquery().closest("[data-modal]");
        }
    })();


    function _help(o,v) {
        var oj = o.getJquery(), h = oj.find(".m-form__help").mapNull();
        if (!empty(v)) {
            if (h) h.html(v);
            else {
                var c = "<span class=\"m-form__help\">" + v + "</span>";
                oj.find("[data-tip='true']").prepend(c);
            }
        }
        else {
            return h ? h.html() : '';
        }
    }

    //select
    (function () {
        var select = metro.select = function () {
            this.give = function (o) {
                o.options = function (l) {
                    var j = this.getJquery(), s = j.find("select");
                    if (l) {
                        s.proxy().bind({ items: l });
                        s.selectpicker("refresh");
                    }
                    else {
                        return s.proxy().data.items;
                    }
                }

                o.get = function () {
                    var s = this.find("select");
                    var v = s.selectpicker('val');
                    if (this.multiple) {
                        if (type(v) != 'array') {
                            v = [v]; //多选数据，提交数组
                        }

                        if (v.length == 1 && v[0] == '') v = [];
                    }
                    if (v === '' || empty(v)) return null; //返回null表示不提交这项数据的值，因为没有填
                    v = toType(this, v);
                    return v; //如果是单选模式，只会提交单值数据
                }

                function toType(o, v) {
                    var vt = o.attr("data-valueType");
                    if (vt === "number") {
                        if (o.multiple) {
                            for (var i = 0; i < v.length; i++) {
                                v[i] = Number(v[i]);
                            }
                        }
                        else {
                            v = Number(v);
                        }
                    }
                    return v;
                }


                o.modalMode = function () {
                    return o.attr("data-select-modal") ? true : false;
                }

                o.set = function (vl, ignoreChanged) {
                    if (type(vl)!='array') {
                        vl = [vl]; //传递进来的值不是数组，所以要转换一下
                    }
                    if (this.modalMode() || this.options().length==0) {
                        var items = vl; //modal模式下赋予选项的完整信息，包括value和text
                        this.options(items);
                        vl = items.select((t) => { return t.value || t; }); //支持传递对象数组进来，会自动转换
                    }
                    if (ignoreChanged) this.ignoreChanged = true;
                    var s = this.find("select");
                    s.selectpicker('val', vl);
                    if (ignoreChanged) this.ignoreChanged = false;
                    //selectChanged.apply(s); 不用手动触发，因为s.on('changed.bs.select', selectChanged);已经绑定了事件
                }

                o.help = function (v) {
                    _help(this, v);
                }

                o.selectedItems = function () { //返回{value,text}形式的被选项
                    var items = this.options(), values = this.get();
                    if (this.multiple) {
                        return items.filter(function (item) { return values.contains(item.value); });
                    }
                    var v = values;//单选
                    return items.filter(function (item) { return item.value == v; });
                }

                o.selectedItem = function () {
                    var items = this.options(), values = this.get();
                    var v = values;//单选
                    return items.first(function (i) { return this.value == v; });
                }

                o.reset = function () {
                    if (this.modalMode()) {
                        this.options([]);
                    }
                    var s = this.find("select");
                    s.selectpicker('val', []);
                }

                o.disable = function (b) {
                    var s = this.find("select");
                    s.prop('disabled', b);
                    s.selectpicker('refresh');
                }

                o.isDisabled = function () {
                    var s = this.find("select");
                    return s.prop('disabled');
                }

                o.more = function (modal,cb) {
                    var o = this;
                    var table = modal.find("[data-datatable='true']").proxy();
                    if (table.inited) {
                        table.selectedItems(o.selectedItems());
                        modal.open();
                    } else {
                        $$page.block();
                        table.load(function () {
                            $$page.unblock();
                            table.selectedItems(o.selectedItems());
                            modal.open();
                        });
                    }

                    var okBtn = modal.find("[data-select-ok]");
                    if (okBtn.length == 0) return;
                    var md = okBtn.attr("data-select-ok");
                    eval("md=" + md); //得到描述获取项数据的元数据，格式：{value:'aa',text:'bbb'}
                    md.value = md.value.toLower();
                    md.text = md.text.toLower();
                    okBtn.off("click");
                    okBtn.on("click", function () {
                        var b = $(this);
                        var table = modal.find("[data-datatable='true']").proxy();
                        var items = table.selectedItems().clone();
                        if (cb) {
                            cb(items);
                        }
                        modal.close();
                    });
                }


                //事件
                o.changed = null;

                init(o);
            }

            function init(o) {
                var oj = o.getJquery();
                var s = oj.find("select");
                var multiple = s.attr("data-multiple");
                if (multiple && multiple.toLower() == "true") {
                    s.prop("multiple", true);
                    o.multiple = true;
                }
                s.removeAttr("data-multiple");
                if (s.attr("data-max-options") == "0") s.removeAttr("data-max-options");
                s.selectpicker({
                    noneSelectedText: "",
                    selectAllText: $$strings.SelectAll,
                    deselectAllText: $$strings.DeselectAll,
                    maxOptionsText: function (m) {
                        var s = String.format($$strings.OnlySelectItems, m);
                        return [s, s];//有两种文本，所以返回数组
                    },

                    actionsBox: true
                });

                initOptions(o);

                tryInitModal(o);

                s.on('changed.bs.select', selectChanged);

                var ds = o.attr("data-disabled").toLowerCase() == "true";
                o.disable(ds);

                o.scriptElement = function () { //获得脚本元素的元数据
                    var my = this, v = my.get();
                    return { id: my.getJquery().prop("id") || '', metadata: { selectedItems: this.selectedItems() } };
                }

                setChanged(o);
            }

            function initOptions(o) {
                var ops = o.attr("data-options");
                if (ops) eval("ops=" + ops + ";");
                else ops = [];
                o.options(ops);
            }

            function selectChanged() {
                var s = $(this), o = s.closest("[data-select]").proxy();
                if (o.ignoreChanged) return;
                var form = o.formOwner;
                if (form) form.validate(o);
                if (o.changed) o.changed();
            }

            function tryInitModal(o) { //如果开启了modal模式，那么初始化
                var modalId = o.attr("data-select-modal");
                if (!modalId) return;
                var modal = $$("#" + modalId), core = o.find("[data-core]");
                core.on("click", { source: o }, function (event) {
                    var source = event.data.source;
                    if (source.isDisabled()) return;
                    source.more(modal, (items) => {
                        source.set(items);
                    });
                    event.stopPropagation();
                });
            }
        }
    })();

    //textBox
    (function () {
        var sp = metro.textBox = function () {
            this.give = function (o) {
                o.get = function () {
                    return getCore(this).val();
                }

                o.set = function (v) {
                    getCore(this).val(v);
                }

                o.reset = function () {
                    getCore(this).val('');
                }

                o.disable = function (b) {
                    var c = getCore(this);
                    if (b) c.prop("disabled", "disabled");
                    else c.removeAttr("disabled");
                }

                o.placeholder = function (v) {
                    var c = getCore(this);
                    if (!empty(v)) {
                        c.attr("placeholder",v);
                    }
                    return c.attr("placeholder");
                }

                o.help = function (v) {
                    _help(this, v);
                }

                o.ignore = function (b) { //忽略项，这个方法会将组件隐藏，并且不参与form行为
                    var o = this, oj = this.getJquery();
                    if (b) {
                        o.ignoreForm = true;
                        oj.hide();
                    } else {
                        o.ignoreForm = false;
                        oj.show();
                    }
                }

                init(o);
            }

            function init(o) {

                o.scriptElement = function () { //获得脚本元素的元数据
                    var my = this, v = my.get();
                    return { id: my.getJquery().prop("id") || '', metadata: { value: v } };
                }

                var c = getCore(o);
                c.on("keyup.textBox", function () {
                    contentChanged.apply(o);
                });
                var ds = o.attr("data-disabled").toLower() == "true";
                o.disable(ds);

                setChanged(o);

                var da = o.attr("autosize") || o.attr("data-autosize") || '';
                if (da.toLower() != "false") {
                    autosize(c);
                }
            }

            function getCore(o) {
                var oj = o.getJquery();
                var p = oj.find("input").first();
                if (p.length > 0) return p;
                return oj.find("textarea").first();
            }

            function contentChanged() {
                var o = this;
                var f = o.formOwner;
                if (f) f.validateElement(o);
                if (o.changed) o.changed();
            }
        }
    })();

    //inputMask
    (function () {
        var mask = metro.inputMask = function () {
            this.give = function (o) {
                o.get = function () {
                    var v = getCore(this).val();
                    var t = this._type;
                    if (t == "number") {
                        return parseInt(v);
                    }
                    if (t == "float") {
                        return parseFloat(v);
                    }
                    return v;
                }

                o.set = function (v) {
                    getCore(this).val(v);
                }

                o.reset = function () {
                    getCore(this).val('');
                }

                o.disable = function (b) {
                    var c = getCore(this);
                    if (b) c.prop("disabled", "disabled");
                    else c.removeAttr("disabled");
                }

                init(o);
            }

            function init(o) {
                getCore(o).on("keyup.inputMask", function () {
                    contentChanged.apply(o);
                });
                setMask(o);
                setChanged(o);
            }

            function getCore(o) {
                var oj = o.getJquery();
                return oj.find("input");
            }

            function setMask(o) {
                var c = getCore(o), t = o.attr("data-type");
                if (t == "number") {
                    c.inputmask("-{0,1}9{0,15}");
                }
                else if (t == "float") {
                    c.inputmask("-{0,1}9{0,15}.{0,1}9{0,15}");
                }
                o._type = t;
            }

            function contentChanged() {
                var o = this;
                var f = o.formOwner;
                if (f) f.validateElement(o);
                if (o.changed) o.changed();
            }

        }
    })();

    //datetime
    (function () {
        var dt = metro.datetime = function () {
            this.give = function (o) {
                o.get = function () {
                    var core = getCore(this);
                    var v = core.val();
                    if (v) return getCore(this).data('datetimepicker').getDate();
                    return null;
                }

                o.set = function (v) {
                    if (!v) return;
                    if (type(v) == "string") v = new Date(v);
                    getCore(this).data('datetimepicker').setDate(v);
                }

                o.reset = function () {
                    getCore(this).val('');
                }

                o.disable = function (b) {
                    var c = getCore(this);
                    if (b) c.prop("disabled", "disabled");
                    else c.removeAttr("disabled");
                }

                init(o);
            }

            function init(o) {
                var format = o.attr("data-format") || "yyyy/mm/dd hh:ii"
                var c = getCore(o);
                c.datetimepicker({
                    todayHighlight: true,
                    autoclose: true,
                    orientation: 'bottom left',
                    format: format,
                    clearBtn: true,
                    language: $$language
                });
                c.on("changed.datetime", function () {
                    var f = o.formOwner;
                    if (f) f.validateElement(o);
                });
            }

            function getCore(o) {
                var oj = o.getJquery();
                return oj.find("input");
            }
        }

    })();

    (function () {
        var s = $$strings;
        $.fn.datepicker.dates[$$language] = {
            days: [s.Sunday, s.Monday, s.Tuesday, s.Wednesday, s.Thursday, s.Friday, s.Saturday],
            daysShort: [s.Sun, s.Mon, s.Tue, s.Wed, s.Thu, s.Fri, s.Sat],
            daysMin: [s.Su, s.Mo, s.Tu, s.We, s.Th, s.Fr, s.Sa],
            months: [s.January, s.February, s.March, s.April, s.May, s.June, s.July, s.August, s.September, s.October, s.November, s.December],
            monthsShort: [s.Jan, s.Feb, s.Mar, s.Apr, s.May, s.Jun, s.Jul, s.Aug, s.Sep, s.Oct, s.Nov, s.Dec],
            today: s.Today,
            monthsTitle: s.SelectMonth,
            clear: s.Clear,
            format: "yyyy-mm-dd",
            titleFormat: s.DateTitleFormat,
            weekStart: 1
        };
    })();

    //date
    (function () {
        var date = metro.date = function () {
            this.give = function (o) {
                o.get = function () {
                    return getCore(this).data('datepicker').getDate();
                }

                o.set = function (v) {
                    getCore(this).data('datepicker').setDate(v);
                }

                o.reset = function () {
                    getCore(this).val('');
                }

                o.disable = function (b) {
                    var c = getCore(this);
                    if (b) c.prop("disabled", "disabled");
                    else c.removeAttr("disabled");
                }

                init(o);
            }

            function init(o) {
                var format = o.attr("data-format") || "yyyy/mm/dd"
                var c = getCore(o);
                c.datepicker({
                    todayHighlight: true,
                    autoclose: true,
                    orientation: 'bottom left',
                    format: format,
                    clearBtn: true,
                    language: $$language
                });
                c.on("changed.date", function () {
                    var f = o.formOwner;
                    if (f) f.validateElement(o);
                });
            }

            function getCore(o) {
                var oj = o.getJquery();
                return oj.find("input");
            }
        }
    })();

    //daterange
    (function () {
        var dt = metro.daterange = function () {
            this.give = function (o) {
                o.get = function () {
                    var core = getCore(this);
                    var v = core.val();
                    if (v) {
                        var p = core.data('daterangepicker');
                        if(this.attr("data-string"))
                            return { start: p.startDate._d.format("y/M/d h:m"), end: p.endDate._d.format("y/M/d h:m") };
                        else
                            return { start: p.startDate._d, end: p.endDate._d };
                    }
                    return null;
                }

                o.set = function (v) {
                    if (!v) return;
                    var core = getCore(this);
                    var p = core.data('daterangepicker');
                    p.setStartDate(v.start);
                    p.setEndDate(v.end);
                }

                o.reset = function () {
                    getCore(this).val('');
                }

                o.disable = function (b) {
                    var c = getCore(this);
                    if (b) c.prop("disabled", "disabled");
                    else c.removeAttr("disabled");
                }

                init(o);
            }

            function init(o) {
                var format = o.attr("data-format") || "YYYY/MM/DD"
                var c = getCore(o);
                var s = $$strings, rs = {};
                rs[s.Today] = [moment(), moment()];
                rs[s.Yesterday] = [moment().subtract(1, 'days'), moment().subtract(1, 'days')];
                rs[s.Last7Days] = [moment().subtract(6, 'days'), moment()];
                rs[s.Last30Days] = [moment().subtract(29, 'days'), moment()];
                rs[s.ThisMonth] = [moment().startOf('month'), moment().endOf('month')];
                rs[s.LastMonth] = [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')];
                c.daterangepicker({
                    buttonClasses: 'm-btn btn',
                    applyClass: 'btn-primary',
                    cancelClass: 'btn-secondary',
                    ranges: rs,
                    locale: {
                        format: format,
                        applyLabel: s.OK,
                        cancelLabel: s.Cancel,
                        daysOfWeek: [s.Su, s.Mo, s.Tu, s.We, s.Th, s.Fr, s.Sa],
                        monthNames: [s.Jan, s.Feb, s.Mar, s.Apr, s.May, s.Jun, s.Jul, s.Aug, s.Sep, s.Oct, s.Nov, s.Dec],
                        customRangeLabel: s.CustomRange
                    }
                });

                c.on("changed.daterange", function () {
                    var f = o.formOwner;
                    if (f) f.validateElement(o);
                });
            }

            function getCore(o) {
                var oj = o.getJquery();
                return oj.find("input");
            }
        }

    })();


    //dropzone
    (function () {
        var dropzone = metro.dropzone = function () {
            this.give = function (o) {

                o.set = function (files) {
                    var o = this;
                    this.reset();
                    l = type(files) == "array" ? files : [files];
                    l.each(function () {
                        addFile(o, this);
                    });
                }

                o.get = function () {
                    var files = this._dz.getAcceptedFiles();
                    var l = files.select((t) => {
                        return t.info || t;
                    });
                    if (l.length == 0) {
                        return this._max == 1 ? null : [];
                    }
                    return this._max == 1 ? l[0] : l;
                }

                o.reset = function () {
                    this._dz.removeAllFiles();
                }

                o.disable = function (b) {
                    var core = this.find("[data-core-container]");
                    var dz = this.find(".m-dropzone");
                    
                    if (b) {
                        var clone = $(dz.clone());
                        dz.hide();
                        clone.addClass("dz-disabled");
                        clone.find(".dz-remove").hide();
                        core.append(clone);
                    }
                    else {
                        core.find(".dz-disabled").remove();
                        dz.show();
                    }
                }

                function addFile(o, f) {
                    var dz = o._dz, url = o._loadThumbnailUrl;
                    var info = {
                        id: f.Id || f.id
                        , name: f.Name || f.name
                        , size: f.Size || f.size
                        , storeKey: f.StoreKey || f.storeKey
                    };

                    info.accepted = true;
                    dz.files.push(info);
                    dz.emit("addedfile", info);
                    if (isImage(info.storeKey)) {
                        var s = url.indexOf('?') < 0 ? '?' : '&';
                        var g = url + s + "key=" + info.storeKey + "&w=120&h=120&c=2";
                        dz.emit("thumbnail", info, g);
                    }
                    dz.emit("complete", info);
                }

                function isImage(storeKey) {
                    var p = storeKey.lastIndexOf('.');
                    if (p > -1) {
                        var e = storeKey.substr(p + 1).toLowerCase();
                        return _images.contains(e);
                    }
                    return false;
                }


                init(o);
            }

            var _images = ["png", "jpg", "jpeg", "bmp", "gif"];

            function init(o) {
                var c = getCore(o);
                var max = parseInt(o.attr("data-max"));
                if (max == 0) max = null;

                var size = parseInt(o.attr("data-size"));
                if (size == 0) size = null;

                var acceptedFiles = o.attr("data-acceptedFiles") || null;
                var uploadUrl = o.attr("data-uploadUrl") || location.href;

                var headers = o.attr("data-headers") || "{}";
                eval("headers=" + headers + ";");

                var dz = new Dropzone(c[0],{
                    url: uploadUrl,
                    paramName: "file", // The name that will be used to transfer the file
                    maxFiles: max,
                    maxFilesize: size, // MB
                    addRemoveLinks: true,
                    headers: headers,
                    acceptedFiles: acceptedFiles,
                    accept: function (file, done) {
                        done();
                    },
                    dictMaxFilesExceeded: String.format($$strings.DictMaxFilesExceeded, max),
                    dictResponseError: $$strings.DictResponseError,
                    dictInvalidFileType: String.format($$strings.DictInvalidFileType, acceptedFiles),
                    dictFallbackMessage: $$strings.DictFallbackMessage,
                    dictFileTooBig: String.format($$strings.DictFileTooBig, size),
                    dictCancelUpload: $$strings.DictCancelUpload,
                    dictRemoveFile: $$strings.DictRemoveFile,
                    dictCancelUploadConfirmation: $$strings.DictCancelUploadConfirmation,
                    init: function () {
                        this.on("addedfile", function (file) {
                            //上传文件时触发的事件
                        });
                        this.on("complete", function (file) {
                            //上传完成后触发的方法
                            if (file.xhr) { //如果没有xhr，那么就是set的数据，不需要做处理
                                var info = file.xhr.responseText;
                                if (info) { //如果是取消上传，info为空
                                    eval("info=" + info + ";");
                                    file.info = info;
                                }
                                else {
                                    dz.removeFile(file);
                                }
                            }

                            validate(o);
                        });
                        this.on("removedfile", function (file) {
                            //删除文件时触发的方法
                            validate(o);
                        });
                    }
                });
                o._dz = dz;
                o._max = max;
                o._loadThumbnailUrl = o.attr("data-loadThumbnailUrl");
            }

            function getCore(o) {
                return o.getJquery().find(".dropzone");
            }

            function validate(o) {
                var f = o.formOwner;
                if (f) f.validateElement(o);
            }

        }

    })();

    (function () {
        var s = $$strings;
        $.fn.datetimepicker.dates[$$language] = {
            days: [s.Sunday, s.Monday, s.Tuesday, s.Wednesday, s.Thursday, s.Friday, s.Saturday, s.Sunday],
            daysShort: [s.Sun, s.Mon, s.Tue, s.Wed, s.Thu, s.Fri, s.Sat, s.Sun],
            daysMin: [s.Su, s.Mo, s.Tu, s.We, s.Th, s.Fr, s.Sa, s.Su],
            months: [s.January, s.February, s.March, s.April, s.May, s.June, s.July, s.August, s.September, s.October, s.November, s.December],
            monthsShort: [s.Jan, s.Feb, s.Mar, s.Apr, s.May, s.Jun, s.Jul, s.Aug, s.Sep, s.Oct, s.Nov, s.Dec],
            meridiem: [s.am, s.pm],
            suffix: ['st', 'nd', 'rd', 'th'],
            today: s.Today,
            clear: s.Clear
        };
    })();

    //radio
    (function () {
        var sp = metro.radio = function () {
            this.give = function (o) {
                o.groupName = sp.getGroup();

                o.options = function (l) {
                    var o = this, s = getCore(o), gn = this.groupName;
                    if (l) {
                        s.proxy().bind({ items: l });
                        var ps = s.find("input[type=\"radio\"]");
                        ps.each(function () {
                            $(this).prop("name", gn);
                        });

                        ps.off("change.radio");
                        ps.on("change.radio", function () {
                            selectChanged.apply(o);
                        });
                    }
                    else {
                        return s.proxy().data.items;
                    }
                }

                o.get = function () {
                    var c = getCore(this), v;
                    c.find("input[type='radio']").each(function () {
                        var p = $(this);
                        if (p.prop("checked")) {
                            v = p.val();
                            return false;
                        }
                    });
                    return v;
                }

                o.set = function (v) {
                    this.reset();
                    v = v.toString();
                    var c = getCore(this);
                    c.find("input[type='radio']").each(function () {
                        var p = $(this);
                        if (p.val().equalsIgnoreCase(v))
                            p.prop("checked", "checked");
                        else {
                            p.prop("checked", false);
                            p.removeAttr("checked");
                        }
                    });
                    selectChanged.apply(this);
                }

                o.reset = function () {
                    var c = getCore(this), l = c.find("input[type='radio']");
                    l.prop("checked", false);
                    l.removeAttr("checked");
                }

                o.disable = function (b) {
                    var c = getCore(this);
                    if (b) {
                        c.find("label").addClass("m-radio--disabled");
                        c.find("input").prop("disabled", "disabled");
                    }
                    else {
                        c.find("label").removeClass("m-radio--disabled");
                        c.find("input").removeAttr("disabled");
                    }
                }

                //事件
                o.changed = null;
                init(o);
            }
        }

        function init(o) {
            var ops = o.attr("data-options");
            if (ops) eval("ops=" + ops + ";");
            o.options(ops);

            var ds = o.attr("data-disabled").toLowerCase() == "true";
            o.disable(ds);
        }

        function getCore(o) {
            return o.find("[data-core]");
        }

        function selectChanged() {
            var o = this;
            var form = o.formOwner;
            if (form) form.validate(o);
            if (o.changed) o.changed();
        }

        sp.groupId = 0;
        sp.getGroup = function () {
            var i = sp.groupId++;
            return "radioGroup" + i;
        }

    })();

    //checkbox
    (function () {
        var cb = metro.checkbox = function () {
            this.give = function (o) {
                o.options = function (l) {
                    var o = this, s = getCore(o);
                    if (l) {
                        s.proxy().bind({ items: l });
                        var ps = s.find("input[type=\"checkbox\"]");
                        ps.off("change.checkbox");
                        ps.on("change.checkbox", function () {
                            selectChanged.apply(o);
                        });
                    }
                    else {
                        return s.proxy().data.items;
                    }
                }

                o.get = function (b) { //b:true表示获取值和文本
                    var c = getCore(this), vl = [];
                    c.find("input[type='checkbox']").each(function () {
                        var p = $(this);
                        if (p.prop("checked")) {
                            v = p.val();
                            vl.push(v);
                        }
                    });

                    if (b) {
                        var items = this.options();
                        return items.where((t) => vl.contains(t.value));
                    }

                    return vl;
                }


                o.set = function (v) {
                    this.reset();
                    var c = getCore(this);
                    c.find("input[type='checkbox']").each(function () {
                        var p = $(this);
                        var pv = p.val();
                        if (v.contains(pv))
                            p.prop("checked", "checked");
                        else {
                            p.prop("checked", false);
                            p.removeAttr("checked");
                        }
                    });
                    selectChanged.apply(this);
                }

                o.reset = function () {
                    var c = getCore(this), l = c.find("input[type='checkbox']");
                    l.prop("checked", false);
                    l.removeAttr("checked");
                }

                o.disable = function (b) {
                    var c = getCore(this);
                    if (b) {
                        c.find("label").addClass("m-checkbox--disabled");
                        c.find("input").prop("disabled", "disabled");
                    }
                    else {
                        c.find("label").removeClass("m-checkbox--disabled");
                        c.find("input").removeAttr("disabled");
                    }
                }

                //事件
                o.changed = null;
                init(o);
            }
        }

        function init(o) {
            var ops = o.attr("data-options");
            if (ops) eval("ops=" + ops + ";");
            o.options(ops);

            var ds = o.attr("data-disabled").toLowerCase() == "true";
            o.disable(ds);
        }

        function getCore(o) {
            return o.find("[data-core]");
        }

        function selectChanged() {
            var o = this;
            var form = o.formOwner;
            if (form) form.validate(o);
            if (o.changed) o.changed();
        }
    })();

    //datatable
    (function () {
        var dt = metro.datatable = function (p) {
            var config = p;
            tidyColumns(p);
            var arg = {
                layout: {
                    scroll: false,
                    footer: false
                },
                sortable: true,
                pagination: p.pagination,
                toolbar: {
                    items: {
                        pagination: {
                            pageSizeSelect: [10, 20, 30, 50, 100]
                        },
                    },
                },
                columns: p.columns,
                rows: {
                    afterTemplate: initRow
                },
                translate: {
                    records: {
                        processing: $$strings.PleaseWaitWithDots,
                        noRecords: $$strings.NoRecords
                    },
                    toolbar: {
                        pagination: {
                            items: {
                                default: {
                                    first: $$strings.FirstPage,
                                    prev: $$strings.PreviousPage,
                                    next: $$strings.NextPage,
                                    last: $$strings.LastPage,
                                    more: $$strings.MorePages,
                                    input: $$strings.PageNumber,
                                    select: $$strings.SelectPageSize
                                },
                                info: $$strings.DisplayingRecords
                            }
                        }
                    }
                }
            };

            setDetailColumn(arg, p);

            function setDetailColumn(arg, p) {
                var detailColumn = p.columns.first(function () { return this.detail; });
                if (detailColumn == null) return;
                detailColumn.field = "__detailField";//详细列必须有field，否则组件显示不正确
                var info = detailColumn.detail;
                info.accept = setDetailData;
                arg.detail = {
                    title: info.title,
                    content: function (e) {
                        var cell = $(e.detailCell), row = $(e.parentRow);
                        var expanded = row.hasClass("m-datatable__row--subtable-expanded");
                        if (expanded) {
                            loadDetail(row, cell, info); //展开，以后可以追加展开事件
                        } else {
                            //emptyDetail(row, cell, info); //合拢，以后可以追加合拢事件（不清理资源）
                        }
                    }
                }
            }

            //function emptyDetail(row, cell, info) {
            //    cell.empty(); //清理资源
            //}

            function loadDetail(row, cell, info, force,callback) {
                var data = row.data().obj, cs = cell.children();
                if (cs.length > 0 && !force) return; //如果已加载过，并且不是强制加载，那么保留数据

                if (!info) {
                    var detailColumn = config.columns.first(function () { return this.detail; });
                    info = detailColumn.detail;
                    info.accept = setDetailData;
                }
               
                var obj = null;
                if (cs.length == 0) {
                    var code = info.template(), oj = row.closest("[data-datatable]"), o = $$(oj);
                    var detailLoad = oj.attr("data-detailLoad") || oj.attr("detailLoad");
                    if (detailLoad)
                        code = ["<div class=\"c--datatable-detail\" data-proxy=\"{give:new $$.databind(),onbind:", detailLoad, "}\">", code, "</div>"].join('');
                    else
                        code = ["<div class=\"c--datatable-detail\" data-proxy=\"{give:new $$.databind()}\">", code, "</div>"].join('');

                    obj = $(code).proxy();
                    if (o.detailLoad) obj.onbind = o.detailLoad; //直接赋予函数的形式
                    cell.empty().append(obj.getJquery()); //先加入，再绑定数据
                } else obj = $$(cs[0]);
                
                info.accept(obj, data, callback);
            }

            function setDetailData(obj, localData,callback) {
                if (this.action) {
                    //从服务器获取数据
                    var sender = {
                        id: "row",
                        scriptElement: function () { //获得脚本元素的元数据
                            return { metadata: { data: localData } };
                        }
                    }

                    var view = new $$view(sender);
                    view.success = function (r) {
                        r.rowData = localData; //提供行数据，以供跨级绑定
                        obj.bind(r);
                        if (callback) callback();
                    }

                    var oj = obj.getJquery().closest("[data-datatable]"), component = oj.attr("name") || oj.attr("id") || '';
                    view.submit({ component: component, action: this.action });
                }
                else {
                    obj.bind(localData);
                    if (callback) callback();
                }
            }

            function getValueColumn(arg) {
                return arg.columns.first(function () { return this.valueField; }) || arg.columns.first(function () { return this.selector; });
            }

            this.give = function (o) {
                o.rows = [];
                o.load = function (callback) {
                    if (this.loading) return;
                    this.loading = true;
                    if (callback) this.loadCallback = callback; //本次加载完毕后，回调方法callback
                    if (init(this)) return; //初始化的时候会自动加载一次数据
                    var oj = this.getJquery(), dt = this.mDatatable;
                    updateQuery(oj, dt);
                    resetPagination(oj, dt); //每次加载都从第一页开始，因为如果翻页到第5页，这时候更改了查询条件，就必须从第一页开始
                    _reload(o);
                }

                var valueColumn = getValueColumn(arg);
                o.valueField = valueColumn ? valueColumn.field : (o.attr("valueField") || o.attr("data-valueField"));
                var textColumn = arg.columns.first(function () { return this.textField; }) || arg.columns[0];
                o.textField = textColumn.field;
                o.selectorColumn = arg.columns.first(function () { return this.selector; });

                o.reload = function (deletedValues) { //不更新查询条件也不重置页到第一页，仅刷新列表
                    if (deletedValues) {
                        removeSelectedItems(this, deletedValues);//这里要同步删除selectedValues,否则长时间操作可能会数据过大
                    }
                    _reload(this);
                }

                o.selectedItemsImpl = [];

                o.get = function () { //仅提取值
                    var items = o.selectedItemsImpl;
                    var vl = [], fs = this.valueFields;
                    if (fs.length == 0) {
                        items.each(function () {
                            vl.push(this.value);
                        });
                    }
                    else {
                        items.each(function () {
                            var value = {}, item = this;
                            fs.each(function () {
                                var field = this;
                                value[field] = item.data[field];
                            });
                            vl.push(value);
                        });
                    }
                    return vl;
                }
                o.selectedItems = function (items) { 
                    if (items) {
                        this.selectedItemsImpl = items; //为table赋予一组值，当列表加载或者翻页时，含有该值就会被勾选
                        drawSelected(this);
                    }
                    else return this.selectedItemsImpl; //提取选择行的所有数据，如果想获取当前表格的数据，请用.data属性
                }

                o.clear = function () {
                    this.selectedItems([]);
                }

                o.expandAll = function (value) {  //展开对应value的行的详细信息
                    var trs = this.findRows();
                    for (var i = 0; i < trs.length; i++) {
                        var tr = $(trs[i]);
                        expand(tr);
                    }
                }

                function expand(row) {
                    var expanded = row.hasClass("m-datatable__row--subtable-expanded");
                    if (expanded) return; //如果已经展开那么不展开
                    row.find(".m-datatable__toggle-subtable:first").trigger("click");
                }

                o.expand = function (value) {  //展开对应value的行的详细信息
                    var tr = this.findRow(value);
                    if (!tr) return;
                    expand(tr);
                }

                o.reloadDetail = function (value,cb) {  //重新加载对应value的行的详细信息
                    var tr = this.findRow(value);
                    if (!tr) return;
                    var detail = tr.next();
                    var cell = detail.find(".m-datatable__subtable:first");
                    loadDetail(tr, cell, null, true, cb);
                }

                function transformData(data) {
                    var cols = config.columns;
                    //处理data，主要是时间需要转换
                    for (var i = 0; i < cols.length; i++) {
                        var col = cols[i], f = col.field;
                        if (col.type === "date") {
                            var v = data[f];
                            if (!v) break;
                            data[f] = v.format("y/M/d");
                        } else if (col.type === "datetime") {
                            var v = data[f];
                            if (!v) break;
                            data[f] = v.format("y/M/d h:m");
                        }
                    }
                }

                o.reloadRow = function (data, reloadDetail,cb) { //reloadDetail是否重新加载详细信息
                    var value = data[this.valueField];
                    var target = this.findRow(value);
                    if (!target) return;
                    transformData(data);
                    var row = generateRow(this, data);
                    var newTds = row.children("td");
                    var tds = target.children("td");
                    var cols = config.columns;
                    for (var i = 0; i < cols.length; i++) {
                        var col = cols[i];
                        if (col.detail || col.selector) continue; //非内容列，不用更新
                        var span = $(tds[i]).find("span"), newSpan = $(newTds[i]).find("span");
                        span.html(newSpan.html());
                    }
                    initRow(target, data, Number(target.attr("data-row")));
                    if (!reloadDetail) {
                        if (cb) cb();
                        return;
                    }
                    //判断是否有详细内容，如果有那么重新加载
                    var detailColumn = cols.first(function () { return this.detail; });
                    if (detailColumn != null) {
                        //var expanded = target.hasClass("m-datatable__row--subtable-expanded");
                        this.reloadDetail(value, cb);
                    }
                }

                function generateRow(o,data) {
                    var p = config, arg = {
                        columns: p.columns,
                        rows: {
                            afterTemplate: initRow
                        },
                        data: {
                            type: 'local',
                            source: [data],
                            pageSize: 10
                        }
                    };

                    var oj = o.getJquery(), c;
                    if (o.tempContainer) {
                        c = o.tempContainer;
                        c.table._mt.destroy();
                        c.empty();
                    } else {
                        c = $("<div style=\"display:none;\"></div>");
                        c.insertAfter(oj.find('.m_datatable:first'));
                        o.tempContainer = c;
                    }

                    var table = $("<div class=\"m_datatable\"></div>");
                    c.append(table);
                    table._mt = table.mDatatable(arg);
                    c.table = table;

                    return table.find("tbody.m-datatable__body > tr:first");

                }

                o.findRows = function () {
                    var oj = this.getJquery();
                    return oj.find("tbody.m-datatable__body:first").children("tr");
                }

                o.closestRow = function (e) {
                    var ej = $$(e).getJquery();
                    if (ej.hasClass(".m-datatable__row-subtable") || ej.hasClass(".m-datatable__row")) return ej;
                    return ej.closest(".m-datatable__row-subtable").mapNull() || ej.closest(".m-datatable__row").mapNull();
                }

                o.findRow = function (value) { //根据行的标示值，找到具体行
                    var trs = this.findRows();
                    var valueField = this.valueField;
                    for (var i = 0; i < trs.length; i++) {
                        var tr = trs[i], data = $$(tr).data;
                        if (data && data[valueField] == value) {
                            return $(tr);
                        }
                    }
                    return null;
                }

                o.findDetail = function (value) {  //重新加载对应value的行的详细信息
                    var tr = this.findRow(value);
                    if (!tr) return;
                    var detail = tr.next();
                    return detail;
                }

                o.scriptElement = function () { //获得脚本元素的元数据
                    var my = this, v = my.get();
                    return { id: my.getJquery().prop("id") || '', metadata: { selectedValues: v } };
                }
                //事件
                o.onload = null;
            }

            function init(o) {
                if (o.inited) return false;
                o.inited = true;
                var oj = o.getJquery(), id = oj.attr("id");

                //判定是否明确指定了提交的字段
                var valueFields = oj.attr("valueFields") || oj.attr("data-valueFields");
                if (valueFields) eval("o.valueFields=" + valueFields + ";");
                else o.valueFields = [];

                //找出是时间类型的列，这些列要特殊处理才能使用
                var datetimes = arg.columns.where((c) => { return c.type == "datetime"; }).select((c) => c.field).join(",");
                var dates = arg.columns.where((c) => { return c.type == "date"; }).select((c) => c.field).join(",");
                arg.data = {
                    type: 'remote',
                    source: {
                        read: {
                            method: 'POST',
                            url: p.url || window.location.href,
                            params: {
                                info: {
                                    component: id,
                                    action: 'Load',
                                    senderId: id,
                                    senderName: oj.attr("name"),
                                    datetimes: datetimes,
                                    dates: dates
                                },
                                query: {
                                }
                            },
                            map: function (raw) {
                                // sample data mapping
                                var dataSet = raw;
                                if (typeof raw.data !== 'undefined') {
                                    dataSet = raw.data;
                                }
                                return dataSet;
                            },
                        },
                    },
                    pageSize: config.pageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                };
                fillQuery(oj, arg.data.source.read.params.query); //用参数填充下
                oj.on("m-datatable--on-ajax-done", function (event, data) {
                    //加载完毕
                    if (data.length == 0) {
                        var dt = o.mDatatable, p = dt.getDataSourceParam("pagination");
                        if (p.page > 1) {
                            //当数据返回为空，数据的页码又大于1时，很大程度是因为服务器端删除了数据导致的，我们要重新查询小一个页码
                            resetPagination(oj, dt, p.page - 1);
                            o.reload();
                            return;
                        }
                    }
                    o.rows = data;
                    if (o.loadCallback) {
                        o.loadCallback();
                        o.loadCallback = null; //加载的回调方法只用执行一次
                    }
                    _finishLoad(o);
                    if (o.onload) o.onload();
                });
                oj.on("m-datatable--on-ajax-fail", function (e, xhr) {
                    var msg = $$.ajax.util.getErrorMessage(xhr);
                    swal($$strings.SystemAbnormal, msg, "error");
                    _finishLoad(o);
                });

                oj.on("m-datatable--on-layout-updated", function (e, args) {
                    drawSelected(o);
                    fixedWidth(o);
                });

                oj.on("m-datatable--on-check", function (e, args) {
                    addSelectedItems(o, args);
                    drawSelectedBottom(o);
                });

                oj.on("m-datatable--on-uncheck", function (e, args) {
                    removeSelectedItems(o, args);
                    drawSelectedBottom(o);
                });

                o.mDatatable = oj.find('.m_datatable:first').mDatatable(arg);
                return true;
            }

            function updateQuery(oj,dt) {
                var query = dt.getDataSourceParam("query");
                fillQuery(oj, query);
                dt.setDataSourceParam("query", query);
            }

            function fillQuery(oj, query) {
                var l = oj.find("[query-field]");
                l.each(function () {
                    var t = $(this), field = t.attr("query-field");
                    var p = t.proxy(), v = p.get();
                    if (!empty(v)) query[field] = v;
                    else delete query[field];
                });
            }

            function resetPagination(oj,dt,page) {
                var p = dt.getDataSourceParam("pagination");
                p.page = page || 1;
                dt.setDataSourceParam("pagination", p);
            }

            function setRemoteInvoke(o,d) {//赋予具有远程调用的能力
                var l = row.find("[data-proxy]");
                l.each(function () {
                    var p = $(this).proxy();
                    if (p.invoke) { //具有远程调用的能力
                        p.metadata = { data: d };
                    }
                });
            }

            function initRow(row, data, index) {
                $$.init(row);

                var d = util.object.values(data);
                var oj = row.closest("[data-datatable]"), vf = oj.proxy().valueField;
                var l = row.find("[data-proxy]");
                l.each(function () {
                    var p = $(this).proxy();
                    if (p.invoke && !p.metadata) { //具有远程调用的能力，但是没有绑定任何值
                        var temp;
                        if (vf) {
                            temp = {};
                            temp[vf] = d[vf];
                        } else temp = d;
                        p.metadata = { data: temp };
                    }
                });

                var db = new $$.databind();
                db.give($$(row));
                row.proxy().bind(d);

                var af = oj.attr("afterTemplate") || oj.attr("data-afterTemplate");
                if (af) {
                    var func;
                    eval("func=" + af + ";");
                    func(row, data,index);
                }

                //调整宽度
                fixedRowWidth(row.children("td"));
            }

            function fixedRowWidth(tds,isHeader) {
                var cols = arg.columns;
                for (var i = 0; i < cols.length; i++) {
                    var col = cols[i], td = $(tds[i]);
                    if (isHeader && col.detail) {
                        td.css("width", "40px"); //因为显示详细的按钮中有padding，所以标题栏要多加20PX
                    }
                    else td.css("width", col.width);
                }
            }

            function fixedWidth(o) {
                var oj = o.getJquery();
                var row = oj.find(".m-datatable__table > .m-datatable__head > .m-datatable__row");
                fixedRowWidth(row.children("th"),true);
            }

            function _reload(o) {
                var oj = o.getJquery();
                o.mDatatable.reload();
            }

            function _finishLoad(o) {
                var oj = o.getJquery();
                o.loading = false;
            }
        }

        function addSelectedItems(o, values) {
            var items = [], rows = o.rows, vf = o.valueField, tf = o.textField;
            rows.each(function () {
                var row = this, v = row[vf];
                if (values.contains(v)) {
                    var text = row[tf];
                    items.push({ value: v, text: text, data: row });
                }
            });
            o.selectedItemsImpl = o.selectedItemsImpl.concat(items).distinct(function (v0, v1) { return v0.value == v1.value; });
        }

        function removeSelectedItems(o, values) {
            o.selectedItemsImpl.remove(function (t) { return values.contains(t.value); });
        }

        function drawSelected(o) { //绘制表格的checkbox是否被勾选和底部已选择区域
            if (!o.selectorColumn) return;
            var items = o.selectedItemsImpl;
            var rows = o.rows, field = o.valueField;
            for (var i = 0; i < rows.length; i++) {
                var row = rows[i], v = row[field].toString(); //o.mDatatable.setActive不识别非字符串，所以得转换
                if (items.contains(function () { return this.value == v; })) {
                    o.mDatatable.setActive(v);
                } else o.mDatatable.setInactive(v);
            }
            drawSelectedBottom(o);
        }

        function drawSelectedBottom(o) { //绘制底部已选择区域
            if (!o.selectorColumn) return;
            var items = o.selectedItemsImpl;
            var area = o.find("[data-datatable-selectedarea]").first();
            var ap = area.proxy();

            var old = ap.data;
            if (old && old.items) {
                if (old.items.length == items.length) {
                    var same = true;
                    old.items.each(function () {
                        var item = this;
                        same = items.contains(function () { return this.value == item.value && this.text == item.text; });
                        if (!same) return false;
                    });
                    if (same) return;  //集合相同，不必刷新
                }
            }
            else {
                if (items.length == 0) return;
            }

            ap.bind({ items: items.clone() });
            if (items.length == 0) area.hide();
            else area.show();
        }

        dt.selectedItemBind = function (o, d) {
            var a = o.find("a"), owner = o.getJquery().closest("[data-datatable]").proxy();
            a.off("click.selected");
            a.on("click.selected", function () {
                var li = $(this).closest("li");
                var data = li.proxy().data;
                removeSelectedItems(owner,[data.value]);
                drawSelected(owner);
            });
        }

        function tidyColumns(p) {
            p.columns.each(function () {
                var w = this.width;
                if (w) {
                    if (!isNaN(w)) {
                        this.width = w + "px"; //纯数字要加上单位
                    }
                }
            });
        }

    })();

    //wizard
    (function () {
        var wz = metro.wizard = function () {
            this.give = function (o) {
                o.goNext = function () {
                    this._wz.goNext();
                }

                init(o);
            }
        }

        function init(o) {
            var oj = o.getJquery(), step = parseInt(oj.attr("data-step"));
            var id = oj.attr("id");
            wizard = new mWizard(id,{
                startStep: step
            });

            //== Validation before going to next page
            wizard.on('beforeNext', function (wizard) {
                //if (validator.form() !== true) {
                //    return false;  // don't go to the next step
                //}
            });

            //== Change event
            wizard.on('change', function (wizard) {
                $$.page.scrollTop();
            });

            oj.find(".m-wizard__step-number").off("click");

            o._wz = wizard;
        }

    })();


    //tags
    (function () {
        var sp = metro.tags = function () {
            var my = this;

            my.give = function (o) {
                var my = this;
                var arg = {
                    max: o.attr("data-maxlength") || 99,
                    min: o.attr("data-minlength") || 1,
                    length: o.attr("data-length") || 1
                };
                arg.max = Number(arg.max);
                arg.min = Number(arg.min);
                arg.length = Number(arg.length);

                o.addItem = function (row) {
                    var o = this, max = arg.max;
                    if (max && o.items().length >= max) return;
                    var t = my.container.addItem(row);
                    onchange(this, "addItem", t);
                    return t;
                }

                o.removeItem = function (t) {
                    var o = this, m = arg.min || arg.length;
                    if (o.items().length <= m) return;
                    my.container.removeItem(t);
                    onchange(this, "removeItem", t);
                }
                o.resetItem = function (t) {
                    my.container.resetItem(t);
                    onchange(this, "resetItem", t);
                }
                o.moveUp = function (t) {
                    my.container.moveItem(t, "up");
                    onchange(this, "moveUp", t);
                }
                o.moveDown = function (t) {
                    my.container.moveItem(t, "down");
                    onchange(this, "moveDown", t);
                }
                o.findItem = function (t) { return my.container.findItem(t).proxy(); }

                o.items = function (le) {
                    if (!le) return my.container.items(); //读
                    var o = this;
                    removeAll();
                    for (var i = 0; i < le; i++) o.addItem();
                }

                o.get = function () {
                    var v = [];
                    this.items().each(function (i, e) {
                        var t = getTextBox(this).get();
                        if (t) v.push(t);
                    });
                    return v;
                }

                o.reset = function () {
                    var o = this;
                    removeAll();
                    initItems(o);
                    onchange(o, "reset");
                }

                o.set = function (v) {
                    var o = this;
                    removeAll();
                    for (var i = 0; i < v.length; i++) {
                        var e = o.addItem();
                        getTextBox(e).set(v[i]);
                    }

                    if (v.length == 0) initItems(o);
                    onchange(o, "set");
                }

                o.disable = function (b) {
                    this.items().each(function (i, e) {
                        getTextBox(this).disable(b);
                    });
                    if (b) {
                        this.find("[data-actions]").hide();
                    }
                    else
                        this.find("[data-actions]").show();
                }

                o.drawValid = function (e) {
                    //错误是，组件不添加样式名 has-danger
                    var oj = this.getJquery();
                    if (e) {
                        e.addClass("m--font-danger");
                        var c = oj.children("[data-core-container]").first();
                        var help = c.children(".m-form__help").mapNull();
                        if (!help) c.append(e);
                        else e.insertBefore(help);
                    }
                    else {
                        var c = oj.children("[data-core-container]").first();
                        c.children(".form-control-feedback").remove();
                    }
                }

                o.disable = function (b) {
                    var items = this.items(), oj = this.getJquery();
                    items.each(function () {
                        getTextBox(this).disable(b);
                    });
                    if (b) {
                        oj.find("[data-add]").hide();
                        oj.find("[data-remove]").hide();
                    }
                    else {
                        oj.find("[data-add]").show();
                        oj.find("[data-remove]").show();
                    }
                }

                o.scriptElement = function () { //获得脚本元素的元数据
                    var my = this, v = my.get();
                    return { id: my.getJquery().prop("id") || '', metadata: { value: v } };
                }


                function initItems(o) {
                    var le = arg.length || arg.min;
                    if (le) o.items(le);
                }

                function init(o) {
                    var oj = o.getJquery();
                    oj.find("[data-add]").on("click.tags", function (e) { o.addItem(); });
                    oj.find("[data-remove]").on("click.tags", function (e) { o.removeItem(); });
                }

                my.obj = o;
                my.container = new itemContainer(o);

                init(o);
                initItems(o);
                o._disabled = o.attr("data-disabled").toLowerCase() == "true";
                if (o._disabled) {
                    o.disable(true);
                }
                onchange(o, "init");//分析组件会执行该方法
            }

            function onchange(o, cmd, t) {
                var m = o.changed || o.onchange;
                if (m)
                    m.apply(o, [cmd, t]);
            }

            function removeAll() {
                var cont = my.container;
                cont.items().each(function (i, e) {
                    cont.removeItem(e);
                });
            }

        }

        function itemContainer(o) { //私有类
            var my = this, _o = o, rawItem = o.find("[data-tags-item]").first(), _template = rawItem.clone(), _cache = $("<div></div>"), _cont = rawItem.parent();
            rawItem.remove();
            var moveToCache = function (item) {
                if (inCache(item)) return;
                _cache.append(item);
            }

            var inCache = function (item) {
                var cs = _cache.children();
                return cs.index(item) > -1;
            }
            var getFromCache = function () {
                var list = _cache.children();
                if (list.length <= 1) return null; //第0项留给template,该项不移动
                var item = $(list[1]);
                _resetItem(item);
                return item;
            }
            var createNew = function () {
                var item = $$(_template.clone());
                item.attr("data-tags-item", "true"); //为新项打上标记
                item = item.getJquery();
                item.show();//防止默认项隐藏导致的bug
                var form = new $$metronic.form();
                form.give(item.proxy());
                return item;
            }

            this.addItem = function (t) {
                var item = getFromCache();
                if (!item) item = createNew();
                _resetItem(item);
                if (_o._disabled) {
                    getTextBox(item).disable(true);
                }

                if (!t) {
                    t = _cont.find("[data-tags-item]").last();
                }
                if (t.length == 0) {
                    _cont.append(item);
                }
                else {
                    var row = _findItemBy(t);
                    item.insertAfter(row);
                }

                execEvent(_o, "oncreate", [_o, item]);
                return item;
            }

            this.removeItem = function (t) {
                var item = t ? _findItemBy(t) : _cont.find("[data-tags-item]").last();
                moveToCache(item);
            }
            this.resetItem = function (t) { _resetItem(_findItemBy(t)); }
            this.moveItem = function (t, m) {
                var item = _findItemBy(t), tar = m == "up" ? item.prev() : item.next();
                if (!tar) return;
                var mn = m == "up" ? "before" : "after";
                tar[mn](item.ent());
            }
            this.findItem = function (t) {
                return _findItemBy(t);
            }
            var _findItemBy = function (t) {
                t = $(t);
                return t.attr("data-tags-item") == "true" ? t : t.closest("[data-tags-item]").mapNull();
            }

            var _resetItem = function (item) {
                item.getJquery().find("[data-textbox]").proxy().reset();
            }

            this.items = function () {
                var l = _cont.children("[data-tags-item]");
                if (l.length == 1 && l.first().css("display") == "none") return [];
                return l;
            }
        }

        function execEvent(o, en, args) {
            var f = o[en];
            if (f) {
                if (!ags) ags = [];
                return f.apply(o, ags);
            }
        }

        function getTextBox(item) {
            return $(item).find("[data-textbox]").proxy();
        }

        $$.metronic.validator.addMethod("tagsValidate", function (value, element, rules) {
            var o = element.proxy();
            var l = o.items(), result = true;
            l.each(function (i, e) {
                var item = $$(this);
                var v = getTextBox(this).get();
                if (v) { //仅对填写了值的行验证
                    var r = item.validate();
                    result = r.satisfied() && result;
                }
            });
            return result;
        });

    })();

    //switch
    (function () {
        var ms = metro._switch = function () {
            this.give = function (o) {

                o.get = function () {
                    var c = getCore(this);
                    return c.prop("checked") ? 1 : 0;
                }

                o.set = function (v) {
                    var c = getCore(this);
                    if (v)
                        c.prop("checked", "checked");
                    else {
                        c.prop("checked", false);
                        c.removeAttr("checked");
                    }
                    switchChanged(o);
                }

                o.reset = function () {
                    var c = getCore(this);
                    c.prop("checked", false);
                    c.removeAttr("checked");
                    switchChanged(o);
                }

                o.disable = function (b) {
                    var c = getCore(this);
                    if (b) {
                        c.prop("disabled", "disabled");
                    }
                    else {
                        c.removeAttr("disabled");
                    }
                }

                init(o);

                //事件
                o.changed = null;
            }
        }

        function init(o) {
            var c = getCore(o);
            c.change(function () {
                switchChanged(o);
            });
        }

        function getCore(o) {
            return o.find("input");
        }

        function switchChanged(o) {
            var form = o.formOwner;
            if (form) form.validate(o);
            if (o.changed) o.changed();
        }
    })();

    //rangeSlider
    (function () {
        var rs = metro.rangeSlider = function () {
            this.give = function (o) {
                
                o.get = function () {
                    var c = getCore(this);
                    return this.type == "single" ? c.prop("value") : { from: c.data("from"), to:c.data("to") };
                }

                o.set = function (v) {
                    var c = getCore(this);
                    var slider = c.data("ionRangeSlider");
                    if (this.type == "single") {
                        slider.update({
                            from:v
                        });
                    } else {
                        slider.update({
                            from: v.from,
                            to: v.to
                        });
                    }
                }

                o.reset = function () {
                    var c = getCore(this);
                    var slider = c.data("ionRangeSlider");
                    slider.reset();
                }

                o.disable = function (b) {
                    var c = getCore(this);
                    var slider = c.data("ionRangeSlider");
                    slider.update({
                        disable: b
                    });
                }
                init(o);
            }
        }

        function init(o) {
            var min = o.attr("data-min") || 0;
            var max = o.attr("data-max") || 100;
            var step = o.attr("data-step") || 1;
            var type = o.attr("data-type") || "single";
            o.type = type;
            var prefix = o.attr("data-prefix") || "";
            var postfix = o.attr("data-postfix") || "";
            var disabled = o.attr("data-disabled") || false;

            var core = getCore(o);
            core.ionRangeSlider({
                type: type,
                min: Number(min),
                max: Number(max),
                prefix: prefix,
                postfix: postfix,
                step: Number(step),
                disable: util.toBool(disabled)
            });
        }

        function getCore(o) {
            return o.find("input");
        }

    })();

    //progress
    (function () {
        metro.progress = function () {
            this.give = function (o) {

                o.get = function () {
                    var c = getCore(this);
                    return c.attr("aria-valuenow");
                }

                o.set = function (v) {
                    var c = getCore(this);
                    c.attr("aria-valuenow", v);
                    c.css("width", v + "%");
                }

                o.reset = function () {
                    this.set(0);
                }
            }
        }

        function getCore(o) {
            return o.find(".progress-bar");
        }

    })();

    //listView
    (function () {
        metro.listView = function (g) {
            this.give = function (o) {
                o.listViewArg= g;
                o.load = function (callback) {
                    if (this.loading) return;
                    this.loading = true;
                    if (callback) this.loadCallback = callback; //本次加载完毕后，回调方法callback
                    resetPagination(this); //每次加载都从第一页开始，因为如果翻页到第5页，这时候更改了查询条件，就必须从第一页开始
                    _reload(this);
                }

                o.reload = function () {
                    if (this.loading) return;
                    this.loading = true;
                    _reload(this);
                }

                o.reloadRow = function () {
                    //todo
                }

                o.reloadItem = function (t, data) { //重新加载某一行中的某一项，需要ListViewItem组件才行
                    if (type(t) == "string") t = $("#" + t);
                    var tj = t.getJquery(), item;
                    if (tj.hasClass(".c--listView-item")) {
                        item = tj.proxy();
                    } else {
                        item = $$(tj.closest(".c--listView-item").mapNull());
                    }

                    var d = item.data;
                    if (data) {
                        bindItem(item, d, data);
                    }
                    else {
                        metro.listView.onDetailBind(item, d);
                    }
                }

                o.reloadRow = function (t, data) { //重新加载某一行
                    if (type(t) == "string") t = $("#" + t);
                    var tj = t.getJquery(), row = $$(tj.closest("tbody").mapNull());
                    var table = row.getJquery().closest("table").proxy();
                    table.update(row, data);
                }

                o.pagination = { pageIndex: 1, pageSize: g.pageSize, enable: g.pagination };
            }

            

        }

        function resetDetail(oj) {
            oj.find(".c--listView-detail-container").each(function () {
                var cs = $(this).children();
                $(cs[0]).show();
                $(cs[1]).hide();
            });
        }

        function _reload(o) {
            var oj = o.getJquery(), g = o.listViewArg, p = o.pagination;
            var data = { pageIndex: p.pageIndex, pageSize: p.pageSize};
            fillQuery(oj, data);

            //从服务器获取数据
            var sender = {
                metadata: data
            }

            var view = new $$view(sender);

            //屏蔽了empty代码，是因为防抖动，真要做好，就要empty区域以透明层的方式覆盖现有的数据区域，这样就不会让
            //用户有抖动的感觉，目前没时间做这么复杂，就用整体loading效果
            //view.beforeSend = function () {
            //    showLoading(o);
            //}

            view.success = function (r) {
                callback(o, r);
            }

            var component = oj.attr("name") || oj.attr("id") || '';
            view.submit({ component: component, action: "Load", scene: oj });
        }

        //function showLoading(o) {
        //    var oj = o.getJquery();
        //    oj.find(".c--listView-content").hide();
        //    oj.find(".c--listView-empty").hide();
        //    oj.find(".c--listView-loading").show();
        //}

        function showEmpty(o) {
            var oj = o.getJquery();
            oj.find(".c--listView-content").hide();
            oj.find(".c--listView-empty").show();
            //oj.find(".c--listView-loading").hide();
        }

        function showContent(o) {
            var oj = o.getJquery();
            oj.find(".c--listView-content").show();
            oj.find(".c--listView-empty").hide();
            //oj.find(".c--listView-loading").hide();
        }

        function fillQuery(oj, data) {
            var l = oj.find("[query-field]");
            var paras = {};
            l.each(function () {
                var t = $(this), field = t.attr("query-field");
                var p = t.proxy(), v = p.get();
                if (!empty(v)) paras[field] = v;
                else delete paras[field];
            });
            data.paras = paras;
        }

        function resetPagination(o, pageIndex) {
            var p = o.pagination;
            p.pageIndex = pageIndex || 1;
        }

        function callback(o, r) {
            var g = o.listViewArg;
            showContent(o);

            resetDetail(o.getJquery());

            var table = $$(o.find("table.fc-list-table").first().mapNull());
            table.bind(r);

            var p = o.pagination;
            var ele = o.find(".c--listView-page").mapNull();

            //加载完毕
            if (r.dataCount == 0) {
                if (p.enable && p.pageIndex > 1) {
                    //当数据返回为空，数据的页码又大于1时，很大程度是因为服务器端删除了数据导致的，我们要重新查询小一个页码
                    resetPagination(o, p.pageIndex - 1);
                    o.reload();
                    return;
                }
                else {
                    showEmpty(o);
                }
            }

            if (p.enable) {
                layui.use('laypage', function () {
                    var laypage = layui.laypage;
                    laypage.render({
                        elem: ele, //注意，这里的 test1 是 ID，不用加 # 号
                        layout: g.pageLayout,
                        prev: $$strings.PreviousPage,
                        next: $$strings.NextPage,
                        last: $$strings.LastPage,
                        first: $$strings.FirstPage,
                        limit: p.pageSize,
                        curr: p.pageIndex,
                        limits: [10, 20, 30, 50, 100],
                        count: r.dataCount, //数据总数，从服务端得到
                        theme: "#716aca",
                        jump: function (obj, first) {
                            if (first) return;  //首次不执行
                            var p = o.pagination;
                            p.pageIndex = obj.curr;
                            p.pageSize = obj.limit;
                            o.reload();
                        }
                    });
                });

                if (r.pageCount == 1) {
                    ele.hide();
                }
                else
                    ele.show();
            }
            else {
                ele.hide();
            }

            o.rows = r.rows;
            if (o.loadCallback) {
                o.loadCallback();
                o.loadCallback = null; //加载的回调方法只用执行一次
            }
            o.loading = false;
            if (o.onload) o.onload();
        }

        metro.listView.onDetailBind = function (item, d) {
            var tj = item.getJquery();
            var action = tj.attr("data-action");

            //从服务器获取数据
            var sender = { metadata: { data: d } };

            var view = new $$view(sender);
            view.success = function (r) {
                bindItem(item, d, r);
            }

            view.submit({ action: action });
        }

        function bindItem(item,rowData,itemData) {
            var tj = item.getJquery(), obj = $$(tj.find(".c--listView-detail")[0]);
            itemData.rowData = rowData;
            if (obj.bind)
                obj.bind(itemData);
            else {
                var table = tj.closest("table").proxy();
                table.update(obj, itemData);
            }
        }


    })();

    //listSlim
    (function () {
        metro.listSlim = function (g) {
            this.give = function (o) {
                o._listSlimConfig = g;
                init(o);

                function init(o) {
                    var g = o._listSlimConfig, oj = o.getJquery();
                    drawHeader(oj, g);
                }

                function drawHeader(oj,g) {
                    var l = [];
                    g.columns.each(function () {
                        var col = this;
                        l.push("<span class=\"m-widget6__caption");
                        if (col.textAlign) {
                            l.push(" m--align-");
                            l.push(col.textAlign);
                        }
                        l.push("\"");
                        var w = empty(col.width) ? "auto" : col.width;
                        l.push("style=\"width:" + w + "\"");
                        l.push(">");
                        l.push(col.title);
                        l.push("</span>");
                    });
                    oj.find("._header").html(l.join(''));
                }
            }
        }

        metro.listSlim.onRowBind = function (o, d) {
            var list = $$(o.getJquery().closest(".c--listSlim").mapNull());
            var g = list._listSlimConfig, oj = o.getJquery();
            drawRow(list, oj, g, d);
            $$.init(oj.children());
        }

        function drawRow(list,oj, g, d) {
            var l = [];
            g.columns.each(function (i) {
                var col = this;
                l.push("<span class=\"m-widget6__text");
                if (col.textAlign) {
                    l.push(" m--align-");
                    l.push(col.textAlign);
                }
                l.push("\"");
                var w = empty(col.width) ? "auto" : col.width;
                l.push("style=\"width:" + w + "\"");
                l.push(">");

                var code;
                if (col.template) {
                    code = col.template(d, i, list);
                } else {
                    code = col.field ? d[col.field] : '';
                }
                l.push(code);
                l.push("</span>");
            });
            oj.html(l.join(''));
        }
    })();

    //popover
    (function () {
        metro.popover = function () {
            this.give = function (o) {
                var oj = o.getJquery();
                if (!oj.attr("data-placement")) oj.attr("data-placement", "bottom");
                if (!oj.attr("data-html")) oj.attr("data-html", "true");
                mApp.initPopover(oj);
            }
        }
    })();


    metro.scrollable = function () {
        this.give = function (o) {
            o.tryScroll = function () { //当内容超出范围时，滚动条有效，否则无效
                var t = this.getJquery();
                if (this._scrollerInited) {
                    mUtil.scrollerDestroy(t.ent());
                }
                mUtil.scrollerInit(t.ent(), {
                    disableForMobile: true,
                    handleWindowResize: true,
                    height: function () {
                        return mUtil.isInResponsiveRange("tablet-and-mobile") && t.data("mobile-height") ? t.data("mobile-height") : t.data("height");
                    }
                });
                this._scrollerInited = true;
                if (!this.hasY()) {
                    mUtil.scrollerDestroy(t.ent());
                    this._scrollerInited = false;
                }
            }

            o.scrollerUpdate = function () {
                mUtil.scrollerUpdate(this.getJquery().ent());
            }

            o.hasY = function () {
                var t = this.getJquery();
                var y = t.find(".ps__thumb-y");
                return parseInt(y.css("height")) > 0;
            }
        }
    }

    metro.tryScroll = function (o) { //将o内部的所有滚动条组件尝试激活
        function exec(o) {
            o.find(".m-scrollable").each(function () {
                var t = $$(this);
                if (!t.tryScroll) {
                    new metro.scrollable().give(t);
                }
                t.tryScroll();
            });
        }
        exec(o);

        $$.page.resize(function () { //防止窗口大小发生变化，导致滚动条行为错误
            var timer;
            timer = setTimeout(function () {
                exec(o);
                clearTimeout(timer);
            }, 200);
            
        });
    }
    

});

$$.createModule("metronic.list", function (api, module) {
    api.requireModules(["metronic"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, deepEmpty = util.deepEmpty, getProxy = util.getProxy;
    var $form = $$metronic.form;

    var $list = $$.metronic.list = function () {
        var my = this;

        my.give = function (o) {
            var my = this;
            var arg = {
                max: o.attr("data-maxlength") || 99,
                min: o.attr("data-minlength") || 1,
                length: o.attr("data-length") || 1
            };
            arg.max = Number(arg.max);
            arg.min = Number(arg.min);
            arg.length = Number(arg.length);

            o.addItem = function (row) {
                var o = this, max = arg.max;
                if (max && o.items().length >= max) return;
                var t = my.container.addItem(row);
                onchange(this, "addItem", t);
                return t;
            }

            o.removeItem = function (t) {
                var o = this, m = arg.min || arg.length;
                if (o.items().length <= m) return;
                my.container.removeItem(t);
                onchange(this, "removeItem", t);
            }
            o.resetItem = function (t) {
                my.container.resetItem(t);
                onchange(this, "resetItem", t);
            }
            o.moveUp = function (t) {
                my.container.moveItem(t, "up");
                onchange(this, "moveUp", t);
            }
            o.moveDown = function (t) {
                my.container.moveItem(t, "down");
                onchange(this, "moveDown", t);
            }
            o.findItem = function (t) { return my.container.findItem(t).proxy(); }
            o.getItemData = function (t) {
                var e = this.findItem(t), f = new $form();
                f.collect(e.ent());
                return f.get();
            }
            o.items = function (le) {
                if (!le) return my.container.items(); //读
                var o = this;
                removeAll();
                for (var i = 0; i < le; i++) o.addItem();
            }

            o.get = function () {
                var v = [];
                this.items().each(function (i, e) {
                    var t = $$(this).get();
                    if (!deepEmpty(t))
                        v.push(t);
                });
                return v;
            }

            o.reset = function () {
                var o = this;
                removeAll();
                initItems(o);
                onchange(o, "reset");
            }

            o.set = function (v) {
                var o = this;
                removeAll();
                for (var i = 0; i < v.length; i++) {
                    var e = o.addItem();
                    e.proxy().set(v[i]);
                }

                if (v.length == 0) initItems(o);
                onchange(o, "set");
            }

            o.order = function (field,type) {
                var vl = this.get().order(function (t) {
                    return t[field];
                },type);
                this.set(vl);
            }

            o.disable = function (b) {
                this.items().each(function (i, e) {
                    $$(this).disable(b);
                });
                if (b) {
                    this.find("[data-actions]").hide();
                }
                else
                    this.find("[data-actions]").show();
            }

            o.drawValid = function (e) {
                //错误是，组件不添加样式名 has-danger
                var oj = this.getJquery();
                if (e) {
                    e.addClass("m--font-danger");
                    var c = oj.children("[data-core-container]").first();
                    var help = c.children(".m-form__help").mapNull();
                    if (!help) c.append(e);
                    else e.insertBefore(help);
                }
                else {
                    var c = oj.children("[data-core-container]").first();
                    c.children(".form-control-feedback").remove();
                }
            }

            o.scriptElement = function () { //获得脚本元素的元数据
                var my = this, v = my.get();
                return { id: my.getJquery().prop("id") || '', metadata: { value: v } };
            }


            function initItems(o) {
                var le = arg.length || arg.min;
                if (le) o.items(le);
            }

            my.obj = o;
            my.container = new itemContainer(o);

            initItems(o);
            onchange(o, "init");//分析组件会执行该方法
        }

        function onchange(o, cmd, t) {
            var m = o.changed || o.onchange;
            if (m)
                m.apply(o,[cmd, t]);
        }

        function removeAll() {
            var cont = my.container;
            cont.items().each(function (i, e) {
                cont.removeItem(e);
            });
        }

    }

    function execEvent(o, en, args) {
        var f = o[en];
        if (f) {
            if (!ags) ags = [];
            return f.apply(o, ags);
        }
    }

    function itemContainer(o) { //私有类
        var my = this, _o = o, rawItem = o.find("tr[data-name='listItem']").first(), _template = rawItem.clone(), _cache = $("<div></div>"), _cont = rawItem.parent();
        rawItem.remove();
        var moveToCache = function (item) {
            if (inCache(item)) return;
            _cache.append(item);
        }

        var inCache = function (item) {
            var cs = _cache.children();
            return cs.index(item) > -1;
        }
        var getFromCache = function () {
            var list = _cache.children();
            if (list.length <= 1) return null; //第0项留给template,该项不移动
            var item = $(list[1]);
            _resetItem(item);
            return item;
        }
        var createNew = function () {
            var item = $$(_template.clone());
            item.attr("data-name", "listItem"); //为新项打上标记
            item = item.getJquery();
            item.show();//防止默认项隐藏导致的bug
            var form = new $form();
            form.give($(item).proxy());

            item.on("click.list", "a[data-name='addItem']", function (e) {_o.addItem(e.target);});
            item.on("click.list", "a[data-name='prevItem']", function (e) { _o.moveUp(e.target); });
            item.on("click.list", "a[data-name='nextItem']", function (e) { _o.moveDown(e.target); });
            item.on("click.list", "a[data-name='resetItem']", function (e) { _o.resetItem(e.target); });
            item.on("click.list", "a[data-name='removeItem']", function (e) { _o.removeItem(e.target); });
            return item;
        }

        this.addItem = function (t) {
            var item = getFromCache();
            if (!item) item = createNew();
            _resetItem(item);
            if (t) {
                var row = _findItemBy(t);
                item.insertAfter(row);
            }
            else
                _cont.append(item);
            tidyIndex();
            execEvent(_o, "oncreate", [_o, item]);
            return item;
        }

        this.removeItem = function (t) {
            moveToCache(_findItemBy(t));
            tidyIndex();
        }
        this.resetItem = function (t) { _resetItem(_findItemBy(t)); }
        this.moveItem = function (t, m) {
            var item = _findItemBy(t), tar = m == "up" ? item.prev() : item.next();
            if (!tar) return;
            var mn = m == "up" ? "before" : "after";
            tar[mn](item.ent());
            tidyIndex();
        }
        this.findItem = function (t) {
            return _findItemBy(t);
        }
        var _findItemBy = function (t) {
            t = $(t);
            return t.attr("data-name") == "listItem" ? t : t.closest("[data-name='listItem']").mapNull();
        }

        var _resetItem = function (item) {
            item.proxy().reset();
        }

        function tidyIndex() {
            var body = _o.find("tbody").first();
            body.find("th[scope='row']").each(function (i) {
                $(this).text(i+1);
            });
        }

        this.items = function () {
            var l = _cont.children("[data-name='listItem']");
            if (l.length == 1 && l.first().css("display") == "none") return [];
            return l;
        }
    }

    $$.metronic.validator.addMethod("listItemsValidate", function (value, element,rules) {
        var o = element.proxy();
        var l = o.items(), result = true;
        l.each(function (i, e) {
            var item = $$(this);
            var v = item.get();
            if (!deepEmpty(v)) { //仅对填写了值的行验证
                var r = item.validate();
                result = r.satisfied() && result;
            }
        });
        return result;
    });
});

var $$tools;
(function () {
    var tools = $$tools = {};
    tools.syncHeight = function (p) {
        var items;
        if (p.length) {
            items = p;
        }
        else
            items = $(p.items);
        if (p.unity) { //统一一个高度
            var height = 0;
            items.each(function () {
                var item = $(this);
                item.css("height", '');
                var h = item.height();
                height = height > h ? height : h;
            });

            items.each(function () {
                var item = $(this);
                item.height(height);
            });
        }
        else {
            for (var i = 0; i < items.length; i += 2) {
                if (i >= items.length - 1) break;
                sh($(items[i]), $(items[i + 1]));
            }
        }

        function sh(b0, b1) {
            b0.css("height", '');//先要移除已设置的高度，让内容自动伸展，这样才能重新判断
            b1.css("height", '');
            var h0 = b0.height(), h1 = b1.height();
            if (h0 > h1) {
                b1.height(h0);
            }
            else {
                b0.height(h1);
            }
        }
    }

    tools.syncHeightInit = function (p) {
        if ($$.type(p) == "string") {
            var items = p;
            p = { items: items };
        }

        if ($$.util.empty(p.lazy)) p.lazy = true; //表示不立即执行

        $$.page.resize(function () {
            tools.syncHeight(p);
        });

        $$.page.ready(function () {
            if (p.dynamic) {
                var l;
                if ($$.util.type(p.dynamic) == "array") l = p.dynamic;
                else l = [p.dynamic];

                l.each(function () {
                    var con = $$(this.toString());
                    var func = con.onbind;
                    if (func) {
                        con.onbind = function (o, d) {
                            func(o, d);
                            tools.syncHeight(p);
                        }
                    }
                    else {
                        con.onbind = function (o, d) {
                            tools.syncHeight(p);
                        }
                    }
                });
            }

            tools.syncHeight(p);
        });

        if (!p.lazy) {
            tools.syncHeight(p);
        }

    }

    var _progressCode = "<div class=\"progress m-progress--sm\"><div class=\"progress-bar m--bg-{color}\" role=\"progressbar\" style=\"width: {value}%;\" aria-valuenow=\"{value}\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div></div>";

    tools.getProgressCode = function (v, col) { //该方法常用于列表组件显示进度
        if (!col) col = "success";
        return _progressCode.replaceAll("{value}", v).replaceAll("{color}", col);
    }


    tools.list = {};
    tools.list.syncHeight = function (tb, itemsClass, itemClass) { //tb:表编号,itemsClass：列表详细内容的子区域的样式，itemClass:子区域中，需要同步高度的项的样式
        var my = this, osClass = "#" + tb + " ." + itemsClass;
        itemClass = "." + itemClass;
        itemsClass = "." + itemsClass;
        var _table;

        function table() {
            if (!_table) _table = $$("#" + tb);
            return _table;
        }

        this.tidy = function (o,tss,ts) {
            if (o) {//调整指定行的
                tidyItemsHeight(o,tss,ts);
            }
            else {
                var os = $(osClass); //调整所有的
                os.each(function () {
                    tidyItemsHeight($(this),tss,ts);
                });
            }
        }

        function tidyItemsHeight(t, tss, ts) { //调整某一个区域的
            var row = table().closestRow(t);
            if (!row) return;
            row = $(row);
            if (!row.is(":visible")) return;
            //统一高度

            row.find(tss || itemsClass).each(function () {
                var b = $(this);
                var l = b.find(ts || itemClass);
                $$tools.syncHeight(l);
            });
        }


        $$.page.ready(function () {
            var func = table().detailLoad;
            if (func) {
                table().detailLoad = function (o, d) {
                    func(o,d);
                    sizeTidy(o, itemsClass, itemClass);
                }
            }
            else {
                table().detailLoad = function (o, d) {
                    sizeTidy(o, itemsClass, itemClass);
                }
            }
        });

        function sizeTidy(o, itemsClass, itemClass) {
            o.find(itemsClass).off("resize.tidy").on("resize.tidy", function () {
                my.tidy($$(this), itemsClass, itemClass);
            });

            my.tidy(o, itemsClass,itemClass);
        }

        //$(window).resize(function () {
        //    my.tidy();
        //});
    }

    tools.lazy = function () {

        var _cache = {};
        var _contextLoaded = {};

        this.load = function (p) {
            if ($$.util.empty(p.cache)) p.cache = true; //如果没有定义缓存选项，那么默认缓存
            if (p.cache) {
                var es = _cache[p.url];
                if (es) {
                    success(p, es); //从缓存中处理
                    return;
                }
            }

            //网络获取
            var req = new $$.ajax.request();
            req.success = function (code) {
                loaded(p, code);
            };

            var loadContent = _contextLoaded[p.url]; //如果加载过环境，不用重复加载，那么就意味着仅加载内容
            var url = p.url;
            if (loadContent) url = url.setUrlParam("loadContent", 1); 
            req.get({
                url: url
            });
        }

        function loaded(p, c) {
            var e = $(["<div>", c, "</div>"].join(''));
            var ready = parse(e);
            var cont = e.find("._lazyContent").children();

            if (!_contextLoaded[p.url]) { //环境是不需要重复加载的，不论是否开启了缓存
                _contextLoaded[p.url] = true;

                var ctx = e.find("._lazyContext").children();
                ctx.each(function () { //先进行环境设置
                    var t = $(this);
                    if (t.is("script") || t.is("link") || t.is("style")) {
                        //经过测试，我们发现这种方式加入link是异步的（在chrome浏览器下），不过样式表的异步不会影响后续JS的执行
                        $$.util.addExternal(this); //经过测试，我们发现这种方式加入script是同步的（在chrome浏览器下），所以不必担心异步加载问题
                    }
                });

                var tl = [];
                ctx.each(function () {
                    var t = $(this);
                    if (!t.is("script") && !t.is("link") && !t.is("style")) {
                        $('body').append(t);
                        tl.push(t);
                    }
                });

                tl.each(function () {
                    $$.init(this);
                });
            }

            cont.each(function () {
                $$.init(this);
            });

            ready.each(function () {
                $$.util.addExternal(this);
            });

            if (p.elements) {  //elements只会触发一次
                p.elements(cont);
                success(p, cont);
            }
            if (p.cache) {
                _cache[p.url] = cont;
            }
        }


        function success(p, es) {
            if (p.success) {
                p.success(es);
            }
        }

        function parse(e) {
            var ready = [];
            var ctx = e.find("._lazyContext");
            e.children().each(function () {
                var t = $(this);
                if (t.is("script")) {
                    //经过测试，我们发现这种方式加入script是同步的（在chrome浏览器下），所以不必担心异步加载问题
                    var code = t.prop("innerHTML");
                    if (code.indexOf("$(document).ready(") > -1) {
                        ready.push(this);
                    }
                    else {
                        $$.util.addExternal(this);
                    }
                }
                else if (t.is("link") || t.is("style")) {//经过测试，我们发现这种方式加入link是异步的（在chrome浏览器下），不过样式表的异步不会影响后续JS的执行
                    $$.util.addExternal(this);
                }
                else {
                    if (!t.hasClass("_lazyContent") && !t.hasClass("_lazyContext")) {
                        //需要额外处理的，加入到环境中
                        ctx.append(t);
                    }
                }
            });

            return ready;
        }

    }

    tools.lazy.elements = function(code) { //将代码转换成可以识别的元素,移除link style script等非dom元素
        var e = $(["<div>", code, "</div>"].join(''));
        e.find("script").remove();
        e.find("style").remove();
        e.find("link").remove();
        return e.children();
    }

})();