$$.createModule("Component", function (api, module) {
    api.requireModules(["Util", "Ajax"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy, getProxyDeclare = util.getProxyDeclare;
    var $o = util.object, $vir = $o.virtual;
    $$.component = {};

    //绑定组件
    (function () {
        $$.databind = $$.component.databind = function () {
            this.give = function (o) { //o是代理对象
                o.bind = function (data) {
                    exec(this.ent(), data, false, true);
                    this.data = data;
                    if (this.onbind) this.onbind(this, data);
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
                    if (n == "_self") { //值自身
                        v = d;
                    }
                    else eval("v=d." + n + ";"); //不要写成v=d[n],因为这样就不支持v=d.data.title了
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
            var l = ["loops", "object", "binds"], cg, po, pod = getProxyDeclare(o); //要获取定义，不能先初始化，因为初始化o可能会创建额外的dom
            if (pod) {
                //解析绑定标签
                l.each(function (i, n) {
                    var v = pod[n];
                    if (v) {
                        if (!cg) {
                            cg = new config(o);
                            po = getProxy(o); //一定要在cg创建之后再获取proxy,因为getProxy会初始化o，初始化o可能会创建额外的dom
                        }
                        cg[n] = v;
                        if (po.__bind_cg) return;//解析过，就不再解析
                        if (n == "loops") {
                            po.getJquery().css({ display: "none" });
                        }
                    }
                });
                if (cg) po.__bind_cg = cg;
            }
            if (pod) $$.init(o);
            l = J(o).children();
            l.each(function (i, e) { parse(e); });
        }

        function config(e) {
            var my = this;
            my.binds = null;
            my.loops = null;
            my.object = null;

            my.elem = $(e)[0].cloneNode(true); //克隆的结果是dom
            //util.clearProxy(my.elem);//防止克隆了__proxy属性

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
    //表单组件，该组件仅用于提交数据，不含验证功能，需要这方面的功能，请用外部组件
    (function () {
        $$.form = $$.component.form = function (formName) {
            var my = this, _inputs = [], _formName = formName;

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

            function giveInput(p) {//给予 get、set
                if (!p.get) p.get = function () {
                    return this.getJquery().val();
                }
                if (!p.set) p.set = function (v) {
                    this.getJquery().val(v);
                }
                if (empty(p.name))
                    p.name = p.getJquery().prop("name");
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
                return { id: _formId || '', metadata: { data: v } };
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

});