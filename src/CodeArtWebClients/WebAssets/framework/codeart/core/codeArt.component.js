$$.createModule("Component", function (api, module) {
    api.requireModules(["Util", "Ajax"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual;
    $$.component = {};

    //绑定组件
    (function () {
        $$.databind = $$.component.databind = function () {
            this.give = function (o) { //o是代理对象
                o.bind = function (data) {
                    exec(this.ent(), data, false, true);
                    this.data = data;
                    if (this.onbind) this.onbind();
                }
                o.clear = function () {
                    exec(this.ent(), {}, false, true);
                    this.data = null;
                    if (this.onclear) this.onclear();
                }
                parse(o.ent());
            }

            //#region 执行绑定

            function exec(o, data, noloops, force) { //o是dom对象
                var po = getProxy(o);
                if (!force && po && po.bind) return; //防止两个binging组件，嵌套后受到影响
                var into = true, cg;
                if (po && (cg = po.__bind_cg)) {
                    var t;
                    if ((t = cg.loops) && !noloops) { loops(o, data[t], cg); into = false; }
                    else if (t = cg.object) { object(o, data[t], cg); into = false; }
                    else if (t = cg.binds) binds(o, data, cg);
                }
                if (into) {
                    var l = J(o).children(), clps = "";
                    l.each(function (i, c) {
                        var co = getProxy(c), ccg, isBind, copy;
                        if (co) {
                            ccg = co.__bind_cg;
                            isBind = co.bind;
                        }
                        if (ccg && ccg.loops) {//如果是循环节点，则只解析第一个
                            if (ccg.loops == clps) copy = true;
                            else clps = ccg.loops;
                        }
                        if (!copy && !isBind) exec(c, data);
                    });
                }
            }

            function loops(o, d, cg) {
                if (!d) d = [];
                var last = ts = $(o), loopsName = $$(ts[0]).loops;
                while ((ts = ts.next()) && ts.length > 0 && ($$(ts[0]).loops == loopsName)) {
                    last = ts;
                    ts = last;
                }
                last = last.proxy().ent();

                var ms, po = $$(o);
                if (po.increment) {//increment:自增模式
                    var ds = d.length, ms = [];
                    for (var k = 0; k < ds; k++) {
                        last = cg.newMember(last);
                        ms.push(last);
                    }
                }
                else {
                    var ds = d.length - cg.getMembers(o).length;
                    for (var k = 0; k < ds; k++) {
                        last = cg.newMember(last);
                    }
                    if (ds < 0) { while (ds++) cg.removeMember(o); } //删除多余的
                    ms = cg.getMembers(o);
                }

                var ctx = { count: d.length };

                for (var i = 0, len = d.length; i < len; i++) {
                    var m = ms[i], jm = $(m), v = d[i], mo = jm.proxy(), dis = mo.display;
                    jm.css({ display: empty(dis) ? "block" : dis });
                    mo.data = v;
                    exec(m, v, true);
                    ctx.index = i;
                    if (mo.onbind) mo.onbind.apply(mo, [mo, v, ctx]); //绑定结束时触发的事件，注意，由于是绑定结束时触发，所以如果有内嵌的loops，则是内嵌的loops全部执行完毕后才触发
                }
            }

            function object(o, d, cg) {
                if (cg.binds) binds(o, d, cg);
                var l = J(o).children();
                l.each(function (i, e) { exec(e, d); });
            }

            function binds(o, d, cg) {
                var st = cg.binds, po = $(o).proxy(), ext = { html: "innerHTML", text: "innerText" }, t;
                for (var e in st) {
                    var n = st[e], v;
                    if (type(n) != "string") continue;
                    eval("v=d." + n + ";"); //不要写成v=d[n],因为这样就不支持v=d.data.title了
                    bindValue(po, e, v);
                }
                po.data = d;
                if (po.format) {
                    var field = st["text"] || st["html"], fd = d[field];
                    po.attr("innerText", fd ? fd.format(po.format) : '');
                }
                if (po.onbind) po.onbind.apply(po, [po, d]); //绑定结束时触发的事件
            }

            function bindValue(po, n, v) {
                if (n == "text") po.attr("innerText", v);
                else if (n == "html") po.attr("innerHTML", v);
                else if (n == "display") po.css("display", v ? "block" : "none");
                else if (n == "visibility") po.css("visibility", v ? "visible" : "hidden");
                else po.attr(n, v); //如果写了@就是自定义属性的绑定
            }

            //#endregion
        }

        function parse(o) {//元素所在的父亲或者父亲以上节点，是否为loops
            var l = ["loops", "object", "binds"], cg, po = getProxy(o);
            if (po && !po.__bind_cg) {//解析过，就不再解析
                //解析绑定标签
                l.each(function (i, n) {
                    var v = po[n];
                    if (v) {
                        if (!cg) cg = new config(o);
                        cg[n] = v;
                        if (n == "loops") {
                            po.getJquery().css({ display: "none" });
                        }
                    }
                });
                if (cg) po.__bind_cg = cg;
            }
            l = J(o).children();
            l.each(function (i, e) { parse(e); });
        }

        function config(e) {
            var my = this;
            my.binds = null;
            my.loops = null;
            my.object = null;

            my.elem = $(e).proxy().clone(true); //克隆的结果是dom，因为参数为true

            my.temp = document.createElement("div");
            my.getMembers = function (e) {//获得当前有效的成员
                var f = J(e).parent(), l = J(f).children(), ol = [];
                l.each(function (i, c) {
                    var co = getProxy(c), cg = co ? co.__bind_cg : null;
                    if (cg && cg.loops == my.loops) ol.push(c);
                });
                return ol;
            }
            my.newMember = function (last) {
                var l = J(my.temp).children();
                if (l.length > 0) {
                    var t = l[0];
                    $(last).after(t)
                    return t;
                }
                else {
                    var t = my.elem.cloneNode(true);
                    $(last).after(t);//先追加到dom中
                    $(t); //再初始化t,避免bug
                    parse(t);
                    return t;
                }
            }
            my.removeMember = function (e) {
                var l = my.getMembers(e), ind = l.length - 1;
                if (ind == 0) l[0].style.display = "none"; //最后一个元素不移除，只隐藏，保证数据为0时，还有dom可供分析
                else J(my.temp).append(l[ind]);
            }
        }
    })();

    var copyEvent = $$.ajax.util.copyEvent;
    //表单组件
    (function () {
        $$.form = $$.component.form = function (formName, validate) {
            var my = this, _inputs = [], _formName = formName, _validate = validate;

            my.setValidate = function (validate) {
                _validate = validate;
            }

            my.inputs = function () { return _inputs; }

            my.input = function (n) {
                return _inputs.first(function (i, e) { return $$(e).name == n; });
            }

            my.append = function (l) {//追加输入组件
                if (type(l) != "array") l = [l];
                var ps = _inputs;

                l.each(function (i, p) {
                    p = $(p).proxy();
                    var n = p.name || p.getJquery().prop("name"), op = my.input(n);
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

            function giveInput(p) {//给予 get、set,isBrowsed 方法
                if (!p.get) p.get = function () {
                    return this.getJquery().val();
                }
                if (!p.set) p.set = function (v) {
                    this.getJquery().val(v);
                }
                if (!p.isBrowsed) p.isBrowsed = function () {
                    return false;
                }
                if (empty(p.name))
                    p.name = p.getJquery().prop("name");
            }

            my.remove = function (l) {//移除输入元素，l为元素名称或dom对象的集合
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

            my.validate = function (b) {//b:是否重绘控件,t:是否alert错误
                if (type(b) == 'array') {
                    var l = b;
                    l.each(function (i, d) {
                        var n = d.name;
                        _inputs.each(function (i, e) {
                            var p = $$(e);
                            if (p.name == n) {
                                p.validate(d);
                                return false;
                            }
                        });
                    });
                }
                else {
                    var r = new result();
                    _inputs.each(function (i, e) {
                        exec($$(e), "validate", function (p) {
                            r.add(p.validate(b));
                        });
                    });
                    if (!r.satisfied()) {
                        execCheck(r);
                        return r;//如果组件验证失败,那么报错,不用继续执行自定义验证
                    }
                    if (_validate) {
                        var e = my._entProxy || my;//my._entProxy是dom模式下的对象,该对象是该dom的代理对象
                        try {
                            var t = _validate.apply(e, [e]);//自定义验证
                            if (t != true && !empty(t)) {
                                var m = type(t) == "string" ? t : "自定义验证出错";
                                r.add(new ValidateResult("error", m, false));
                            }
                        } catch (ex) {
                            r.add(new ValidateResult("error", ex.message, false));
                        }
                    }
                    execCheck(r);
                    return r;
                }
            }

            function execCheck(vr) {
                var check = (my._entProxy && my._entProxy.check) || my.check; //触发验证事件
                if (check) check(vr);
            }

            function ValidateResult(s,m,st) { //class
                var _s = s, _m = m, _st = st;
                this.status = function () { return _s; };
                this.message = function () { return _m; };
                this.satisfied = function () { return _st; }
            }

            my.submit = function (tg, vl) {//vl:是额外需要提交的值,键值对
                if (!tg) throw new Error("没有设置目标，无法提交表单");
                var vr = my.validate(true);
                if (!vr.satisfied()) return;
                var req = new $$.ajax.request();
                copyEvent(my._entProxy, my, true);
                var success = my.success;
                if (success) {
                    //截获事件
                    my.success = function (r) {
                        _inputs.each(function (i, e) {
                            exec($$(e), "clean", function (p) {
                                p.clean(true);//设置对象为干净的
                            });
                        });
                        success(r);
                    }
                }
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

            my.set = my.accept = function (data, b) {//接收数据,b:是否验证
                for (var e in data) {
                    var p = my.input(e);
                    if (p) {
                        p.proxy().set(data[e], b);
                        p.proxy().clean(true);//设置对象为干净的
                    }
                }
                this.data = data;
            }

            my.browse = function (b) {
                _inputs.each(function (i, e) {
                    exec($$(e), "browse", function (p) { p.browse(b); });
                });
            }

            my.reset = function () {
                _inputs.each(function (i, e) {
                    exec($$(e), "reset", function (p) { p.reset(); });
                });
            }

            my.get = function (n) {
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

            my.disabled = function (b) {
                _inputs.each(function (i, e) {
                    exec($$(e), "disabled", function (p) { p.disabled(b); });
                });
            }

            my.readonly = function (b) {
                _inputs.each(function (e) {
                    exec($$(e), "readonly", function (p) { p.readonly(b); });
                });
            }

            //#region 实现give接口
            var _formId;

            my.give = function (o) {
                var oj = o.getJquery();
                _formMethods.each(function (i, n) {
                    o[n] = my[n];
                });
                var t;//表单名称
                if (t = o.name) _formName = t;
                if (t = oj.attr("name")) _formName = t;
                if (t = oj.attr("id")) _formId = t;
                if (o.uiParas && (t = o.uiParas["validate"])) _validate = t;
                my.collect(o.ent());
                my._entProxy = o; //form关联的实体对象的代理
            }

            my.collect = function (target) { //将target区域的组件信息，收集到form中
                my.append(internal(target, _formName));
                if (!empty(_formName)) my.append(external(target, document.body, _formName)); //定义了表单名称，还要收集外部控件
            }

            function internal(c, n, ps) {//n:表单名称
                ps = ps || [];
                var l = J(c).children();
                l.each(function (i, e) {
                    var pe = getProxy(e);
                    if (pe) {
                        var fn = pe.form;
                        if (fn == 'none') return;//显示指明form="none"，意为不参与提交
                        if (empty(fn)) internal(e, n, ps);
                        else if (fn == '' || fn == n) my.append(pe);
                    }
                    else internal(e, n, ps);
                });
                return ps;
            }

            function external(fe, c, n, ps) {//f:表单实体,n:表单名称
                ps = ps || [];
                if (fe == c) return ps;
                var l = J(c).children();
                l.each(function (i, e) {
                    var pe = getProxy(e);
                    if (pe && pe.form == n) my.append(pe);
                    else external(fe, e, n, ps);
                });
                return ps;
            }

            //#endregion

            //#region 实现 接口 （支持脚本视图）

            my.scriptElement = function () { //获得脚本元素的元数据
                var my = this, v = my.get();
                var p = new function () {
                    this.validate = function () {
                        return my.validate(true);
                    }
                }

                return { id: _formId || '', metadata: { value: v }, eventProcessor: p };
            }

            //#endregion
        }

        function exec(p, n, f) { //验证组件p是否实现了方法n,如果实现了，则执行函数f
            if (p[n]) { f(p); return }
            //throw new Error("组件" + p.name + "没有实现方法" + n);
        }

        function result() { //验证结果
            var _satisfied = true, _infos = [];
            this.success = this.satisfied = function () { return _satisfied; };
            this.add = function (info) {
                _infos.push(info);
                if (info.status() == "error") {
                    _satisfied = false;
                    _status = "error";
                }
            }
            var _status = "success";
            this.status = function () {
                return _status;
            }
            this.info = function () { return _infos; }
            this.errorText = function () {
                var l = [];
                _infos.each(function (i, t) {
                    if (t.status() == "error") {
                        if (l.length > 0) l.push('、');
                        l.push(t.message());
                    }
                });
                return l.join('');
            }
            this.message = function () {
                return this.errorText();
            }
        }

        var _formMethods = ["inputs", "input", "append", "remove", "validate", "submit", "set", "accept", "reset", "get", "disabled", "setValidate", "browse", "scriptElement"];
    })();

    //组件基类
    (function () {
        $$.component.painter = function (methods) { //methods自定义方法
            var my = this;
            this.methods = methods || {};
            my.draw = function (o, type) {
                var fn = "drawBy" + type.toUpper(1), f = this.methods[fn] || this[fn];
                if (!f) throw new Error("没有找到绘制器的方法" + fn);
                var args = [o];
                for (var i = 2, le = arguments.length; i < le; i++) { args.push(arguments[i]); }
                f.apply(this, args);
            }
        }

        $vir($$.component.painter, "find", function (o, n) {
            if (!n) return null;
            var fn = "find" + n.toUpper(1), f = this.methods[fn] || this[fn];
            if (f) return f.apply(this, [o]);
            return o.find("*[data-name='" + n + "']").first().mapNull();
        });

        var $ui = $$.component.ui = function (painter) {
            var my = this;
            my.painter = painter;
            this.find = function (o, n) { return my.painter.find(o, n); }
            this.draw = function (o) {
                var l = [];
                for (var i = 0, len = arguments.length; i < len; i++) { l.push(arguments[i]); }
                my.painter.draw.apply(my.painter, l);//动态参数
            }
        }

        $vir($ui, "give", function (o) {
            var my = this;
            o.para = function (n, v) {
                if (empty(v)) return this.uiParas[n];
                this.uiParas[n] = v;
            };
            o.execEvent = function (n, ags) { //执行组件事件
                if (!this.initialized) return; //只有组件初始化完成了，才会触发事件
                if (this.__closeEvent) return; //手工关闭了事件触发，那么不触发事件
                var f = this.getEvent(n);
                if (f) {
                    if (!ags) ags = [];
                    return f.apply(this, ags);
                }
            };
            o.getEvent = function (n) {
                if (n.length > 2 && n.substr(0, 2) != 'on') n = ['on', n].join('');
                return this.para(n);
            }
            o.setEvent = function (n, f) {
                if (n.length > 2 && n.substr(0, 2) != 'on') n = ['on', n].join('');
                this.para(n, f);
            }
            o.__closeEvent = false;
            o.closeEvent = function () { //关闭事件触发
                this.__closeEvent = true;
            }
            o.openEvent = function () { //开启事件触发
                this.__closeEvent = false;
            }
            o.on = function (n, f) { return this.setEvent(n, f); }
        });
    })();

    //所有参与form提交的input的基类
    (function () {
        var $input = $$.component.input = function (painter, validator) { //painter 绘制控件、查找元素
            $o.inherit(this, $$.component.ui, painter);
            var my = this;
            my.validator = validator;
            this.validate = function (o) { return my.validator.validate(o); }
        }

        //#region input组件的虚方法
        $vir($input, "give", function (o) {
            $o.callvir(this, $$.component.ui, "give", o);
            var my = this;
            o.label = function () { return $$(this).para("call") || this.find("label").first().text(); };
            o.readOnly = function (b) { my.execReadOnly(this, b); };//作为输入控件，可以设置属性是否只读
            o.disabled = function (b) { my.execDisabled(this, b); };//作为输入控件，可以设置是否禁用
            o.set = function (v, b) {
                if (this.clean()) this.cleanValue(this.get()); //如果组件是干净的，那么在赋值之前记录组件的干净值
                my.execSet(this, v);
                if (b != false) this.validate(true);
                this.syncBrowse();//调用该方法同步数据
            };//作为输入控件，可以赋值
            o.get = function () { return my.execGet(this); };//作为输入控件，可以取值
            o.reset = function () {
                my.execReset(this);
                my.draw(this, "reset");//重绘控件
            };
            o.validate = function (b) { //b:是否重绘控件
                if (type(b) == 'object') {//主动设置模式
                    var result = b;
                    my.draw(this, "validate", result);//更新视图
                    return result;
                }
                else {
                    if (this.para("validate") == false) return;
                    var result = my.validate(this);
                    if (b) my.draw(this, "validate", result);//更新视图
                    return result;
                }
            };//可以自我验证
            o.validator = my.validator;//外界可以查看验证器
            o.browse = function (b) {
                var bc = my.find(this, "browseContainer");
                if (!bc) return;
                if (b == false) {
                    if ($$(bc).only) return; //设置了只读，因此不切换
                }
                else if (empty(b)) {
                    b = this.isBrowsed();
                }

                if (b) {
                    //设置为浏览模式，如果组件为不干净的，那么重新还原干净值
                    if (!this.clean()) {
                        var cv = this.cleanValue();
                        if (!empty(cv)) this.set(cv);
                        this.clean(true);
                    }
                }
                this.isBrowsed(b); //记录状态
                this.syncBrowse(b);
            }//重绘控件
            o.syncBrowse = function (b) { //当b为不设置时，仅同步数据，不切换状态
                var d = my.execBrowseData(this);
                my.draw(this, "browse", b, d);
            }
            o.isBrowsed = function (b) {
                if (empty(b)) {
                    var v = this.para("browseStatus");
                    if (empty(v)) {
                        var bc = my.find(this, "browseContainer");
                        v = bc ? true : false; //当有browse节点时，默认开启browse模式
                        this.para("browseStatus", v);
                    }
                    return this.para("browseStatus");
                }
                this.para("browseStatus", b);
            }
            o.needSubmit = function () {  //在浏览模式是否提交数据
                var s = this.isBrowsed();
                if (!s) return true; //沒有設置瀏覽模式，那麼會提交
                var bc = my.find(this, "browseContainer");
                return $$(bc).submit || false; //設置了瀏覽模式,則要判斷submit是否為true
            }
            o.clean = function (b) { //获取或设置组件的干净状态
                if (b == true) this.cleanValue(this.get());
                else {
                    var cv = this.cleanValue();
                    if (empty(cv)) return true;//组件默认为干净的
                    return util.equals(cv, this.get());
                }
            }
            o.cleanValue = function (v) {
                if (empty(v)) return this.__cleanValue;
                this.__cleanValue = v;
            }

            o.scriptElement = function () { //获得脚本元素的元数据
                var my = this, v = my.get();
                var p = new function () {
                    this.validate = function () {
                        return my.validate(true);
                    }
                }

                return {
                    id: my.getJquery().prop("id") || '',
                    metadata: { value: v },
                    eventProcessor: p
                };
            }

            if (my.onGive) { my.onGive(o); }
        });

        $vir($input, "execReadOnly");
        $vir($input, "execDisabled");
        $vir($input, "execSet");
        $vir($input, "execGet");
        $vir($input, "execValidate");
        $vir($input, "execReset");
        $vir($input, "execBrowseData");
        //#endregion

        //#region 验证器的基类

        $input.validator = function () {
            var _handlers = [];
            this.addHandler = function (h) { _handlers.push(h); } //增加验证处理器

            this.validate = function (o) {
                var lab = o.label(), v = o.get();
                try {
                    _handlers.each(function (i, h) {
                        h.exec(o, lab, v);
                    });
                    customValidate(o, lab, v);
                }
                catch (e) {
                    var r = new function () {
                        this.status = function () { return "error"; }
                        var _msg;
                        this.message = function (v) { if (v) _msg = v; else return _msg; }
                        this.satisfied = function () { return false; }
                    }
                    r.message(e.message);
                    return r;
                }
                return new function (e) {
                    this.status = function () { return "success"; }
                    this.satisfied = function () { return true; }
                    this.message = function () { return ''; }
                }
            }

            function customValidate(o, n, v) { //自定义验证
                var val = o.para("validate");
                if (val) val.apply(o, [o, n, v, my]);
            }
        }

        //#endregion

        //#region 绘画器的基类

        $input.painter = function (methods) { //methods自定义方法
            $o.inherit(this, $$.component.painter, methods);
            var my = this;
            my.findWarn = function (o) {
                return o.find("*[name='warn']").first().mapNull() || this.find(o, "help");
            }

            //#region 根据验证状态绘制

            my.drawByValidate = function (o, result) {
                if (!result) return;
                var s = result.status(), msg = result.message();
                saveResume(o);
                if (s == "success") {//验证状态是成功的，但是需要进一步确认，是否需要还原组件为初始状态
                    if (my.isResume(o)) s = "resume";
                }
                if (s == "resume" || s == "success") msg = o.__resumeMsg;
                my["on" + s.toUpper(1)](o, msg);
            }

            function saveResume(o) { //记录正常情况下的状态
                if (empty(o.__resumeMsg)) o.__resumeMsg = my.getResumeMsg(o);
            }

            my.isResume = function (o) {
                var v = o.get();
                return empty(v) || v.length == 0;
            }
            my.getResumeMsg = function (o) {
                var m = my.find(o, "help");
                return m ? m.text() : "";
            }

            var _setMsg = function (o, msg) {
                var h = my.find(o, "help")
                if (h) h.text(msg ? msg : "");
                var j = o.getJquery();
                j.removeClass("has-success");
                j.removeClass("has-error");
            }
            my.onError = function (o, msg) {
                _setMsg(o, msg);
                o.getJquery().addClass("has-error");
                if (o.para("pulsate")) {
                    o.getJquery().pulsate({
                        color: "#bf1c56",
                        repeat: false
                    });
                }
            }
            my.onSuccess = function (o, msg) {
                _setMsg(o, msg);
                o.getJquery().addClass("has-success");
            }
            my.onResume = function (o, msg, finder) {
                _setMsg(o, msg);
            }

            my.drawByBrowse = function (o, b, d) {
                var bc = my.find(o, "browseContainer"), cc = my.find(o, "coreContainer");
                if (bc) {
                    bc.html(d);  //无论如何都会同步数据
                    if (o.isBrowsed()) my.drawByReset(o);
                    else o.validate(true);
                    if (empty(b)) return;//不执行切换
                    if (b) {
                        bc.show();
                        cc.hide();
                    }
                    else {
                        bc.hide();
                        cc.show();
                    }
                }
            }
        }

        $vir($input.painter, "drawByReset", function (o) {
            this.drawByValidate(o, {
                status: function () { return "resume"; },
                message: function () { return ''; },
                satisfied: function () { return true; }
            });
        });

        //#endregion

    })();

    //通用的翻页组件
    (function () {
        $$.pageTurn = $$.component.pageTurn = function (p) {
            var my = this, _s = new status(p.size), _distance = p.distance;
            var _changed = p.changed;
            this.size = function () {
                return _s.size();
            }
            this.count = function (v) { //页总数
                return _s.count();
            }
            this.index = function () {
                return _s.index();
            }

            var _ent = $('<ul class="pagination"></ul>');
            this.ent = function () { return _ent; }

            this.update = function (dataCount) {
                var pageIndex = _s.index(), pageSize = _s.size();
                var pageCount = parseInt(dataCount / pageSize);
                if (dataCount % pageSize > 0) pageCount++;
                _s.count(pageCount); //更新页面总数

                var code = [];
                code.push('<li><a data-name="page-prev" href="javascript:;"><i class="fa fa-angle-left"></i></a></li>');
                for (var i = pageIndex - _distance; i < pageIndex; i++) {
                    if (i >= 0) {
                        code.push('<li><a data-name="page-index" href="javascript:;">' + (i + 1) + '</a></li>');
                    }
                }
                code.push('<li class="active"><a href="javascript:;">' + (pageIndex + 1) + '</a></li>');
                for (var i = pageIndex + 1; i < (pageIndex + 1 + _distance) ; i++) {
                    if (i < pageCount) {
                        code.push('<li><a data-name="page-index" href="javascript:;">' + (i + 1) + '</a></li>');
                    }
                }
                code.push('<li><a data-name="page-next" href="javascript:;"><i class="fa fa-angle-right"></i></a></li>');
                _ent.html(code.join(''));

                _ent.off('click', "a[data-name='page-prev']");
                _ent.on('click', "a[data-name='page-prev']", { o: my }, function (e) {
                    if (_s.prev()) _changed(my);
                });

                _ent.off('click', "a[data-name='page-next']");
                _ent.on('click', "a[data-name='page-next']", { o: my }, function (e) {
                    if (_s.next()) _changed(my);
                });

                _ent.off('click', "a[data-name='page-index']");
                _ent.on('click', "a[data-name='page-index']", { o: my }, function (e) {
                    var pi = parseInt($(this).text()) - 1;
                    if (_s.index(pi)) _changed(my);
                });
            }
        }

        function status(size) {
            var my = this, _index = 0, _size = size, _count = 0;
            this.size = function () {
                return _size;
            }
            this.count = function (v) { //页总数
                if (!v) return _count;
                _count = v;
            }

            this.index = function (v) {
                if (empty(v)) return _index;
                if (v < 0 || v >= _count) return false;
                _index = v;
                return true;
            }
            this.prev = function () {
                if (_index <= 0) return false;
                _index--;
                return true;
            }
            this.next = function () {
                if (_index >= (_count - 1)) return false;
                _index++;
                return true;
            }
            this.clear = function () {
                _index = 0;
            }
        }
    })();


    $$.component.util = {};
    $$.component.util.initQuery = function (o) { //这是一个工具方法，用于解析查询中的query参数
        var q = o.para("paras");
        var paras = parse(q);
        o.para("paras", paras);
        o.query = function (p) { //查询参数
            if (p) this.para("paras", p);
            else return this.para("paras");
        }
        o.addQuery = function (p) { //增加查询参数
            var ps = this.query();
            var newParas = parse([p]);
            newParas.each(function (i, e) {
                ps.push(e);
            });
            this.query(ps);
        }

        function parse(q) {
            var paras = [];
            q.each(function (i, e) {
                if (e.id) {
                    var t = $(["#", e.id].join('')).mapNull();
                    if (!t) throw new Error("没有找到编号为" + e.id + "的DOM元素");
                    t = t.proxy();
                    if (!t.get) {
                        t.get = function () {
                            if (!empty(this.value)) return this.value;
                            if (!empty(this.attr("value"))) return this.attr("value");
                            if (!empty(this.text())) return this.text();
                        }
                    }
                    if (!t.getName) t.getName = function () {
                        if (!empty(this.name)) return this.name;
                        if (!empty(this.attr("name"))) return this.attr("name");
                    }
                    paras.push(t);
                } else if (e.provider) {
                    var pro = e.provider;

                    function Provider(o, pro) {
                        var _obj = o, _pro = pro;
                        this.getName = _pro.getName ? function () {
                            return _pro.getName.apply(this, [_obj]);
                        } : function () {
                            if (!empty(_pro.name)) return _pro.name;
                        }
                        this.get = _pro.get ? function () {
                            return _pro.get.apply(this, [_obj]);
                        } : function () {
                            if (!empty(_pro.value)) return _pro.value;
                        }
                    }
                    var t = new Provider(o, pro);
                    paras.push(t);
                }
            });
            return paras;
        }

    }
});