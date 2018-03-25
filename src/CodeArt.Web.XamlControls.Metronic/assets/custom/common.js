var $$page = $$.page = {};//代表页面级别的命名空间
(function () {
    $$.page.closeMask = function (callback) {
        var mask = $(".body-mask");
        mask.fadeOut("slow", function () {
            $(".body-mask").remove();
            callback();
        });
    };

    var readies = [];
    $$.page.ready = function (callback) {
        readies.push(callback);
    }

    $$.page.onready = function () {
        readies.each(function () {
            this();
        });
    };

    $$.page.block = function (target) { //页面锁定，如果target所在modal中，那么锁定moadl,否则锁定整个页面
        if (target) {
            var modal = target.getJquery().closest("[data-modal]");
            if (modal.length == 0) $$.page.block();
            else modal.proxy().block();
        }
        else {
            mApp.blockPage({
                overlayColor: '#000000',
                state: 'primary',
                size: 'lg',
                zIndex: 9500,
                message: $$strings.PleaseWaitWithDots
            });
        }
    }

    $$.page.unblock = function (target) {
        if (target) {
            var modal = target.getJquery().closest("[data-modal]");
            if (modal.length == 0) $$.page.unblock();
            else modal.proxy().unblock();
        }
        else {
            mApp.unblockPage();
        }
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
    var menu = $$.page.menu;
    menu.view = {};
    var mv = menu.view;
    mv.show = function () {
        var d = { childs: mv.data.childs || mv.data };
        if (mv.redirect) {
            _address = _pathname = mv.redirect.toLowerCase();
        }
        showByAuto(d);
    }

    function showByManual(d, md) {
        var sf;//被选的一级菜单
        d.childs.each(function (i, f) {
            if (f.name == md.root) {
                f.selected = true;
                sf = f;
                return false;//已标记，退出循环
            }
        });

        $$("#m_ver_menu").bind(d);
        if (!sf) alert("没有找到名称为" + md.root + "的一级菜单");
    }

    //manual
    function showByAuto(d) {
        markSelected(d);
        $$("#m_ver_menu").bind(d);
    }

    function markSelected(item) {
        if (sameAddress(item)) {
            item.selected = true;
            return true; //找到了， 返回true
        }
        var cs = item.childs;
        if (cs && cs.length > 0) {
            for (var i = 0, le = cs.length; i < le; i++) {
                if (markSelected(cs[i])) return true;
            }
        }
        return false;
    }

    var _address = location.href.toLowerCase();
    var _pathname = location.pathname.toLowerCase() + location.search.toLowerCase();
    function sameAddress(item) {
        var c = item.code || {}, u = c.url || '';
        u = u.trim().toLowerCase();
        return _pathname == u || _address == u;
    }

    function haveChilds(d) {
        return !$$.util.empty(d.childs) && d.childs.length > 0;
    }

    function fillLink(j, d) {
        if (d.selected) {
            j.addClass("m-menu__item--active");
            setSelect(j);
        }
        var a = j.find("a").first();
        bindLink(a, d);
        setIcon(a.find("i").first(), d, "flaticon-menu");
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
            j.attr("data-menu-submenu-toggle","hover");
            if (d.selected) {
                j.addClass("m-menu__item--open");
                setSelect(j);
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

    function containsTag(d, t) {
        return d.tags && d.tags.contains(function () {
            return this == t;
        });
    }

    function setSelect(j) {
        var p = j.parent();
        while (p && !p.is("#m_ver_menu")) {
            if (p.is("li")) {
                p.proxy().data.selected = true;
                break;
            }
            p = p.parent();
        }
    }

    function bindLink(o, d) {
        var code = d.code || {}, t;
        if (t = code["url"]) o.attr("href", t); else { o.attr("href", "#"); $(o).on("click", function (e) { e.preventDefault(); }); }
        if (t = code["target"]) o.attr("target", t);
    }

    //自动显示当前同级菜单至顶部
    var sameLevel = $$.page.menu.sameLevel = {};
    sameLevel.show = function () {
        var d = { childs: mv.data.childs || mv.data };
        var p = [];
        fillPath(p, d);
        if (p.length < 2) {
            $$("#sameLevel").getJquery().remove(); //当前选中的菜单没有父亲，就无法快速选取同级菜单了
            return;
        }
        var parent = p[p.length - 2];
        $$("#sameLevel").bind(parent);
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
            j.attr("data-menu-submenu-toggle", "hover");
            var a = j.find("a").first();
            a.addClass("m-menu__toggle");
            setIcon(a.find("i").first(), d, "flaticon-layers");
        }
        else {
            var a = j.find("a").first();
            bindLink(a, d);
            setIcon(a.find("i").first(), d, "flaticon-menu");
            a.find("i.m-menu__hor-arrow").remove();
            a.find("i.m-menu__ver-arrow").remove();
            j.find("div.m-menu__submenu").remove();
        }
    }

    //快捷方式
    var shortcut = $$.page.menu.shortcut = {};
    shortcut.show = function () {
        var o = $$("#shortcut"), j = o.getJquery();
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
        var d = { childs: mv.data.childs || mv.data };
        var l = [];
        fillTagData(l, d, "user");
        var c = $$("#userContent"), cj = c.getJquery();
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
        setIcon(i, d, "flaticon-menu");
    }

})();

/*路径导航*/
(function () {
    var bar = $$.page.bar = {};
    bar.items = [];//附加项
    bar.init = function () {
        var b = $$("#sitePath"); items = [];
        var data = $$.page.menu.view.data || { childs: [] };
        fillItems(items, { childs: data.childs || data });
        if (items.length == 0) return;
        var text = items.last()[0].text;
        items = items.concat(bar.items);
        items.last()[0].last = true;
        b.bind({ text: text,items: items });
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
(function () {
    var util = $$.page.util = {};
    function image() {
        this.onBind = function (dn, url, w, h, c) {//图片加载路径，宽度，高度，裁剪类型

            if (!c) c = 1;
            return function (o, d) {
                var oj = o.getJquery();
                var s = url.indexOf('?') < 0 ? '?' : '&';
                var org = url + s + "key=" + d[dn] + "&w=" + w + "&h=" + h + "&c=" + c;
                o.attr("src", "/framework/codeart/wrapper/metronic/images/disk/ico_dark/jpg.jpg");
                o.attr("data-original", org);
                oj.css({ width: w, height: h });
                oj.lazyload({
                    effect: "fadeIn"
                });


            }
        }
    }
    util.image = new image();
})();


var $$metronic;
$$.createModule("metronic", function (api, module) {
    var metro = $$metronic = $$.metronic = {};
    var util = api.util, empty = util.empty, copyEvent = $$.ajax.util.copyEvent;
    var type = util.type;

    (function () {
        var sa = metro.sweetAlert = {};
        sa.alert = function (arg) {
            swal(arg);
        }
        sa.error = function (title,text) {
            swal(title, text, "error");
        }
        sa.warning = function (title, text) {
            swal(title, text, "warning");
        }
        sa.confirm = function (title, text,ok,cancel) {
            swal({
                title: title,
                text: text,
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: '确定',
                cancelButtonText: '取消',
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
                    //data-core-container是组件核心的第一个父亲
                    var c = p.find("[data-core-container]").mapNull();
                    if (!c) return;
                    var help = c.children(".m-form__help").mapNull();
                    if (!help) c.append(e);
                    else e.insertBefore(help);
                }
            }

            function resetValid(p) {
                var op = p.proxy();
                if (op.drawValid) {
                    op.drawValid(null);
                    return;
                }
                p.removeClass("has-danger");
                var c = p.find("[data-core-container]").first();
                c.children(".form-control-feedback").remove();
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
                var p = ele.proxy(), name = p.name;
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

    })();


    //表单组件
    (function () {
        var form = metro.form = function () {
            var self=this, _formId, _formName;
            var _inputs = [], _customValidate;

            self.give = function (o) {
                var oj = o.getJquery();
                var t;//表单名称
                if (t = o.name) _formName = t;
                if (t = oj.attr("name")) _formName = t;
                if (t = oj.attr("id")) _formId = t;
                if (t = o.validate) _customValidate = t;

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
                    initValidator(my);
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
                    if (rp.length > 0) {
                        initValidator(my);
                    }
                    return rp;//返回被移除的对象
                }
                collect(o, o.ent()); //收集inputs数据

                //inputs end

                o.set = o.accept = function (data, b) {//接收数据,b:是否验证
                    var my = this;
                    my.reset();
                    for (var e in data) {
                        var p = my.input(e);
                        if (p) {
                            p.proxy().set(data[e], b);
                        }
                    }
                }

                o.get = function (n) {
                    var v = {};
                    _inputs.each(function (i, e) {
                        exec($$(e), "get", function (p) {
                            if (!p.needSubmit || p.needSubmit()) {
                                v[p.name] = p.get();
                            }
                        });
                    });
                    return v;
                }

                o.disabled = function (b) {
                    _inputs.each(function (i, e) {
                        exec($$(e), "disabled", function (p) { p.disabled(b); });
                    });
                }

                o.alert = function (d) {//{type:'success|danger',message:'...'}
                    var a = this.findAlert();
                    if (!a) return;
                    a.show(d);
                    mApp.scrollTo(a.getJquery(), -200);
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
                    }

                    return { id: _formId || '', metadata: { data: v }, eventProcessor: p };
                }

                o.validate = function (ele) {
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
                    _validator.element(p.getJquery()); // validate element
                }

                o.submit = function (tg, vl) {//vl:是额外需要提交的值,键值对
                    if (!tg) throw new Error("没有设置目标，无法提交表单");
                    var my = this;
                    if (!my.validate()) return;
                    var req = new $$.ajax.request();
                    copyEvent(self, my, true);
                    copyEvent(my, req);
                    _inputs.each(function (i, e) {
                        exec($$(e), "get", function (p) {
                            if (!p.needSubmit || p.needSubmit()) {
                                req.add(p.name, p.get());
                            }
                        });
                    });
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
                    _validator.reset();
                }
            }

            var _validator;

            function initValidator(o) {
                _validator = createValidator(o);
            }

            function getName(p) {
                return p.attr("data-field") || p.attr("name");
            }

            function createValidator(o) {
                var rules = {}, messages = {};
                //收集控件提供的规则
                var ps = _inputs;
                ps.each(function (i, e) {
                    var p = $$(e), n = getName(p);
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
                initValidator(o);
            }

            function append(o,l) {//追加输入组件
                var my = o;
                if (type(l) != "array") l = [l];
                var ps = _inputs;

                l.each(function (i, p) {
                    p = $(p).proxy();
                    var n = getName(p), op = my.input(n);
                    if (empty(n)) {
                        throw Error("组件没有定义name，无法加入form" + p.getJquery().prop("outerHTML")); return;
                    }
                    if (!op) {
                        giveInput(p);
                        ps.push(p);
                        p.formOwner = my;
                    }
                    else if (p != op.proxy()) { throw Error("名称为" + n + "的输入组件已存在！"); return; }
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
                    else if (fn == '' || fn == n) append(o, $$(e));
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
                    if (fn == n) append(o, $$(e));
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

    //modal
    (function () {
        var sp = metro.modal = function () {
            this.give = function (o) {
                var oj = o.getJquery();
                oj.on('show.bs.modal', function () { //在调用 show 方法后触发。
                    push($$(this)); //记录当前打开的窗口
                });
                oj.on('shown.bs.modal', function () { //窗口完全显示（动画效果完毕）
                    
                });

                oj.on('hide.bs.modal', function () {//在调用 hide 方法后触发。
                    pop();
                })

                oj.on('hidden.bs.modal', function () { //窗口完全退出（动画效果完毕）
                    
                })

                o.open = function () {
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


    })();

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
                    return s.selectpicker('val');
                }

                o.modalMode = function () {
                    return o.attr("data-select-modal") ? true : false;
                }

                o.set = function (vl) {
                    if (this.modalMode()) {
                        var items = vl; //modal模式下赋予选项的完整信息，包括value和text
                        this.options(items);
                        vl = items.select((t) => { return t.value || t; }); //支持传递对象数组进来，会自动转换
                    }
                    var s = this.find("select");
                    s.selectpicker('val', vl);
                    selectChanged.apply(s);
                }

                o.selectedItems = function () { //返回{value,text}形式的被选项
                    var items = this.options(), values = this.get();
                    return items.filter(function (item) { return values.contains(item.value); });
                }

                o.reset = function () {
                    var s = this.find("select");
                    s.val([]);
                    s.selectpicker('refresh');
                }

                o.disabled = function (b) {
                    var s = this.find("select");
                    s.prop('disabled', b);
                    s.selectpicker('refresh');
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
            }

            function initOptions(o) {
                var ops = o.attr("data-options");
                if (ops) eval("ops=" + ops + ";");
                else ops = [];
                o.options(ops);
            }

            function selectChanged() {
                var s = $(this), o = s.closest("[data-select]").proxy();
                var form = o.formOwner;
                if (form) form.validate(o);
                if (o.changed) o.changed();
            }

            function tryInitModal(o) { //如果开启了modal模式，那么初始化
                var modalId = o.attr("data-select-modal");
                if (!modalId) return;
                var modal = $$("#" + modalId), core = o.find("[data-core]");
                core.on("click", function (event) {
                    var b = $(this);
                    var table = modal.find("[data-datatable='true']").proxy();
                    if (table.inited) {
                        table.selectedItems(o.selectedItems());
                        modal.open();
                    } else {
                        $$page.block(b);
                        table.load(function () {
                            $$page.unblock(b);
                            table.selectedItems(o.selectedItems());
                            modal.open();
                        });
                    }
                    event.stopPropagation();
                });

                var okBtn = modal.find("[data-select-ok]");
                if (okBtn.length == 0) return;
                var md = okBtn.attr("data-select-ok");
                eval("md=" + md); //得到描述获取项数据的元数据，格式：{value:'aa',text:'bbb'}
                md.value = md.value.toLower();
                md.text = md.text.toLower();
                okBtn.on("click", function () {
                    var b = $(this);
                    var table = modal.find("[data-datatable='true']").proxy();
                    updateSelect(o, table);
                    modal.close();
                });
            }

            function updateSelect(o, table) {
                //更新选项
                var items = table.selectedItems().clone();
                o.set(items); //modal模式下，将选项信息作为值传递
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
                init(o);
            }

            function init(o) {
                o.find("input").on("keyup.textBox", function () {
                    var f = o.formOwner;
                    if (f) f.validateElement(o);
                });
            }

            function getCore(o) {
                var oj = o.getJquery();
                var p = oj.find("input").first();
                if (p.length > 0) return p;
                return oj.find("textarea").first();
            }
        }
    })();

    //radio和checkbox
    (function () {
        var sp = metro.radio = function () {
            this.give = function (o) {
                o.options = function (l) {
                    var o=this,s = getCore(o);
                    if (l) {
                        s.proxy().bind({ items: l });
                        var ps = s.find("input[type=\"radio\"]");
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
                        if (p.attr("data-empty")) return;
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
                        if (p.attr("data-empty")) return;
                        if (p.val().equalsIgnoreCase(v))
                            p.prop("checked", "checked");
                        else
                            p.removeAttr("checked");
                    });
                    selectChanged.apply(this);
                }

                o.reset = function () {
                    var c = getCore(this), l = c.find("input[type='radio']");
                    l.removeAttr("checked");
                    l.last().prop("checked", "checked"); //不知道为什么removeAttr("checked")无效，所以要追加一个空选项来模拟重置
                }

                o.disabled = function (b) {
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

            var core = getCore(o), n = o.attr("data-field") || '';
            //在core里追加空项,不知道为什么removeAttr("checked")无效，所以要追加一个空选项来模拟重置
            core.append($("<input type='radio' data-empty='true' name='" + n + "' style='display:none' />"));
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
            var arg = {
                layout: {
                    scroll: false,
                    footer: false
                },
                sortable: true,
                pagination: true,
                toolbar: {
                    items: {
                        pagination: {
                            pageSizeSelect: [10, 20, 30, 50, 100],
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
            this.give = function (o) {
                o.rows = [];
                o.load = function (callback) {
                    if (callback) this.loadCallback = callback; //本次加载完毕后，回调方法callback
                    if (init(this)) return; //初始化的时候会自动加载一次数据
                    var oj = this.getJquery(), dt = this.mDatatable;
                    updateQuery(oj, dt);
                    resetPagination(oj, dt); //每次加载都从第一页开始，因为如果翻页到第5页，这时候更改了查询条件，就必须从第一页开始
                    dt.reload();
                }

                o.valueField = arg.columns[0].field;//我们取第一列的值作为关键值提交
                var textColumn = arg.columns.first(function () { return this.textField; }) || arg.columns[0];
                o.textField = textColumn.field;

                o.reload = function (deletedValues) { //不更新查询条件也不重置页到第一页，仅刷新列表
                    if (deletedValues) {
                        removeSelectedItems(this, deletedValues);//这里要同步删除selectedValues,否则长时间操作可能会数据过大
                    }
                    this.mDatatable.reload();
                }

                o.selectedItemsImpl = [];

                o.get = function () { //仅提取值
                    var items = o.selectedItemsImpl;
                    var vl = [];
                    items.each(function () {
                        vl.push(this.value); 
                    });
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
                    this.set([]);
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
                    pageSize: 10,
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
                    if (o.onload) o.onload();
                });
                oj.on("m-datatable--on-ajax-fail", function (e, xhr) {
                    var msg = $$.ajax.util.getErrorMessage(xhr);
                    swal($$strings.SystemAbnormal, msg, "error");
                });

                oj.on("m-datatable--on-layout-updated", function (e, args) {
                    drawSelected(o);
                });

                oj.on("m-datatable--on-check", function (e, args) {
                    addSelectedItems(o, args);
                    drawSelectedBottom(o);
                });

                oj.on("m-datatable--on-uncheck", function (e, args) {
                    removeSelectedItems(o, args);
                    drawSelectedBottom(o);
                });

                o.mDatatable = oj.find('.m_datatable').mDatatable(arg);
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
                    var p = t.proxy();
                    query[field] = p.get();
                });
            }

            function resetPagination(oj,dt,page) {
                var p = dt.getDataSourceParam("pagination");
                p.page = page || 1;
                dt.setDataSourceParam("pagination", p);
            }

            function initRow(row, data, index) {
                $$.init(row);
                var d = util.object.values(data);
                row.proxy().data = d;
                var l = row.find("[data-proxy]");
                l.each(function () {
                    var p = $(this).proxy();
                    if (p.invoke) { //具有远程调用的能力
                        p.metadata = { data: d };
                    }
                });
            }
        }

        function addSelectedItems(o, values) {
            var items = [], rows = o.rows, vf = o.valueField, tf = o.textField;
            rows.each(function () {
                var row = this, v = row[vf];
                if (values.contains(v)) {
                    var text = row[tf];
                    items.push({ value: v, text: text });
                }
            });
            o.selectedItemsImpl = o.selectedItemsImpl.concat(items).distinct(function (v0, v1) { return v0.value == v1.value; });
        }

        function removeSelectedItems(o, values) {
            o.selectedItemsImpl.remove(function (t) { return values.contains(t.value); });
        }

        function drawSelected(o) { //绘制表格的checkbox是否被勾选和底部已选择区域
            var items = o.selectedItemsImpl;
            var rows = o.rows, field = o.valueField;
            for (var i = 0; i < rows.length; i++) {
                var row = rows[i], v = row[field];
                if (items.contains(function () { return this.value == v; })) {
                    o.mDatatable.setActive(v);
                } else o.mDatatable.setInactive(v);
            }
            drawSelectedBottom(o);
        }

        function drawSelectedBottom(o) { //绘制底部已选择区域
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
    })();
    
});

$$.createModule("metronic.list", function (api, module) {
    api.requireModules(["metronic"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $form = $$metronic.form;

    var $list = $$.metronic.list = function (painter, validator) {
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
            
            o.addItem = function () {
                var o = this, max = arg.max;
                if (max && o.items().length >= max) return;
                var t = my.container.addItem();
                onchange(this, "addItem", t);
                return t;
            }
            o.removeItem = function (t) {
                var o = this;
                if (o.items().length <= arg.min) return;
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

            o.drawValid = function (e) {
                //错误是，组件不添加样式名 has-danger
                if (e) {
                    e.addClass("m--font-danger");
                    var c = this.find("[data-core-container]").first();
                    var help = c.children(".m-form__help").mapNull();
                    if (!help) c.append(e);
                    else e.insertBefore(help);
                }
                else {
                    var c = this.find("[data-core-container]").first();
                    c.children(".form-control-feedback").remove();
                }
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
            if (o.onchange)
                o.onchange(cmd, t);
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

            item.on("click.list", "a[data-name='addItem']", function (e) { _o.addItem(this);});
            item.on("click.list", "a[data-name='prevItem']", function (e) { _o.moveUp(this); });
            item.on("click.list", "a[data-name='nextItem']", function (e) { _o.moveDown(this); });
            item.on("click.list", "a[data-name='resetItem']", function (e) { _o.resetItem(this); });
            item.on("click.list", "a[data-name='removeItem']", function (e) { _o.removeItem(this); });
            return item;
        }

        this.addItem = function () {
            var item = getFromCache();
            if (!item) item = createNew();
            _resetItem(item);
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

    function deepEmpty(v) {
        var ty = type(v);
        if (ty == "array") {
            if (v.length == 0) return true;
            var e = true;
            v.each(function () {
                var b = deepEmpty(this);
                if (!b) {
                    e = false;
                    return false;
                }
            });
            return e;
        }

        if (ty == "object") {
            for (var e in v) {
                var t = v[e];
                return deepEmpty(t);
            }
        }

        if (ty == "function" || ty=="null") return true;
        return (v == '' || empty(v));
    }

    $$.metronic.validator.addMethod("listItemsValidate", function (value, element,rules) {
        var o = element.proxy();
        var l = o.items(), result = true;
        l.each(function (i, e) {
            var item = $$(this);
            var r = item.validate();
            result = r.satisfied() && result;
        });
        return result;
    });

    //#region 验证器

    //function itemsHandler() { //每个选项验证处理器
    //    this.exec = function (o, n, v) {
    //        var es = [], l = o.items();
    //        l.each(function (i, e) {
    //            var f = new $form();
    //            f.collect(e);
    //            var r = f.validate(true);
    //            if (!r.satisfied()) {
    //                es.push("[ 第" + (i + 1) + "项：" + r.message() + " ]");
    //            }
    //        });
    //        if (es.length > 0)
    //            throw new Error(n + "输入错误，" + es.join(''));
    //    }
    //}

    //function ignoreValidate(o, v) {
    //    if (!o.para("required") && v.length == 0) return true; //当没有输入，并且required不为true时，不用验证
    //    return false;
    //}

    //$list.validator = function () {
    //    $o.inherit(this, $input.select.validator);
    //    this.addHandler(new itemsHandler());
    //}

    //#endregion

    //$list.painter = function (methods) {
    //    $o.inherit(this, $input.painter, methods);
    //    var my = this;
    //    this.findItem = function (o) {
    //        return o.find("*[data-name='listItem']").first();
    //    }
    //    this.findMessage = function (o) {
    //        return null;//对于集合组件，不需要提示消息，每一项内部会有消息提示
    //    }


    //    var _setMsg = function (o, msg) {
    //        var h = my.find(o, "help");
    //        if (h) h.text(msg ? msg : "");
    //        my.find(o, "label").removeClass("list-has-success").removeClass("list-has-error");
    //        my.find(o, "help").removeClass("list-has-success").removeClass("list-has-error");
    //    }
    //    this.onError = function (o, msg) {
    //        _setMsg(o, msg);
    //        my.find(o, "label").addClass("list-has-error");
    //        my.find(o, "help").addClass("list-has-error");
    //    }
    //    this.onSuccess = function (o, msg) {
    //        _setMsg(o, msg);
    //        my.find(o, "label").addClass("list-has-success");
    //        my.find(o, "help").addClass("list-has-success");
    //    }
    //    this.onResume = function (o, msg, finder) {
    //        _setMsg(o, msg);
    //    }

    //    this.drawByBrowse = function (o, b, d) {

    //    }

    //}

    //$$.wrapper.metronic.input.createList = function (painter) {
    //    if (!painter) painter = new $list.painter();
    //    return new $list(painter, new $list.validator());
    //}
});