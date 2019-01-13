var $$view;
$$.createModule("Xaml", function (api, module) {
    api.requireModules(["Util"]);
    api.requireModules(["Ajax"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, object = util.object;
    var copyEvent = $$.ajax.util.copyEvent;

    $$.ajax.request.getData = function (arg, paras) {
        return arg.type === "POST" ? { component: arg.component || '', action: arg.action, argument: paras } : paras;
    }
    $$.xaml = {};
});

$$.createModule("Xaml.Component", function (api, module) {
    api.requireModules(["Util", "Ajax"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual;

    $$.xaml.component = {};

    var copyEvent = $$.ajax.util.copyEvent;
    //脚本视图
    (function () {
        $$view = $$.xaml.component.view = function (sender, name) {
            if (type(sender) == 'string' && !name) {
                name = sender;
                sender = null;
            }
            var my = this, _elements = [], _viewName=mapViewName(name), _sender = sender;

            function onlySelf() { return _viewName == "_self" || _viewName == "self"; } //仅提交自身

            my.elements = function () { return _elements; }

            my.element = function (id) {
                return _elements.first(function (i, e) { return $$(e).attr("id") == id; });
            }

            my.append = function (l) {//追加视图元素
                if (onlySelf()) return;
                if (type(l) != "array") l = [l];
                var eles = _elements;

                l.each(function (i, e) {
                    e = $(e).proxy();
                    var id = e.attr("id");
                    if (!id) { //没有指定编号的脚本元素
                        var oe = _elements.first(function (i, t) { return $$(t) === e; });
                        if (!oe) {
                            giveElement(e);
                            eles.push(e);
                        }
                    }
                    else {
                        var oe = my.element(id);
                        if (!oe) {
                            giveElement(e);
                            eles.push(e);
                        }
                        else if (e != oe.proxy()) { throw Error("编号为" + id + "的视图元素已存在！"); }
                    }
                });
            }

            function mapViewName(n) { //将n为true的值，替换成''
                if (n == true || n == "true") n = '';
                n = n || '';
                if (type(n) == 'string' && n.indexOf(',') > -1) {
                    var t = n.split(',');
                    for (var i = 0; i < t.length; i++) {
                        var c = t[i];
                        if (c == true || c == "true") {
                            c = '';
                            t[i] = c;
                        }
                    }
                    n = t.join(',');
                }
                return n;
            }

            function giveElement(e) {//给予默认的 scriptElement 方法实现
                if (!e.scriptElement)
                    e.scriptElement = function () {
                        var p = this, d = {};
                        if (p.ent) {
                            var ent = p.ent(), ej = $(ent);
                            var id = ej.prop("id");
                            if (id) d.id = id;
                        }
                    
                        if (p.metadata) d.metadata = p.metadata;
                        else {
                            var md = {};
                            if (p.data) md.value = p.data; //如果元素自带数据，那么将数据作为值提交
                            if (p.get) md.value = p.get();
                            d.metadata = md;
                        }
                        return d;
                        //待实现
                    }
                if (empty(e.id) && e.getJquery)
                    e.id = e.getJquery().prop("id") || '';
            }

            my.remove = function (l) {//移除视图元素，l为元素编号或dom对象的集合
                if (type(l) != "array") l = [l];
                var es = _elements, re = [];//被移除的元素
                l.each(function (i, t) {
                    var ele = util.type(t) == 'string' ? my.element(t) : $$(t);
                    if (ele) {
                        es.remove(function (e, t) {
                            return $$(e).attr("id") == t || $$(e).ent() == t;
                        }, t);
                        re.push(ele);
                    }
                });
                return re;//返回被移除的对象
            }

            my.submit = function (tg, option) { //我们要保证submit的幂等性，内部不能改变当前view的状态
                if (!tg) throw new Error("没有设置目标，无法提交视图");
                if (!option) option = {};
                var t;
                if (t = option.getElements) { //指定了获取元素的方法，那么使用
                    var es = t(_sender);
                    this.append(es);
                }

                //提交视图之前，记录本次提交的环境
                $$view.context = { view: this, target: tg };

                //获取事件处理器
                var customProcessor;
                if (option.ignoreCustomValidate) { //有选项指定不执行自定义验证
                    customProcessor = new function () { };
                } else {
                    customProcessor = $$view.getEventProcessor(tg.action) || new function () { };
                }
                var req = new $$.ajax.request();
                var _success = my.success;
                var events = { //防止重复使用一个view,导致success重复执行
                    success: function (r) {
                        r = $$view.exec(r) || r;
                        if (_success) _success(r);
                    }
                };
                //截获事件
                copyEvent(events, req);
                copyEvent(my, req); //如果视图指定了事件，那么使用
                copyEvent(customProcessor, req); //如果事件处理器指定了事件，那么使用事件处理器的事件

                var eles = [], ps = [];
                _elements.each(function (i, e) {
                    exec($$(e), "scriptElement", function (p) {
                        var e = p.scriptElement();
                        if (e.eventProcessor) {
                            ps.push(e.eventProcessor);
                            delete e.eventProcessor;
                        }
                        eles.push(e);
                    });
                });

                if (!option.ignoreValidate) {
                    var vr = validate(ps, customProcessor);
                    if (!vr.satisfied()) return;
                }

                ps.each(function (i, p) {
                    copyEvent(p, req); //查看脚本元素的事件处理器是否定义了ajax事件的处理
                });

                req.add("elements", eles);//提交视图元素

                //将视图发送者也提交
                if (_sender) {
                    var s = _sender;
                    giveElement(s);
                    var e = s.scriptElement();
                    req.add("sender", e);//提交视图元素
                }
                req.add("session",$$view.session.get()); //提交视图会话
                req.submit(tg);
            }

            my.get = function () { //获取视图的数据
                var data = {};
                var eles = [];
                _elements.each(function (i, e) {
                    exec($$(e), "scriptElement", function (p) {
                        var e = p.scriptElement();
                        eles.push(e);
                    });
                });

                data["elements"] = eles;

                //将视图发送者也提交
                if (_sender) {
                    var s = _sender;
                    giveElement(s);
                    var e = s.scriptElement();
                    data["sender"] = e;
                }

                data["session"] = $$view.session.get(); //视图会话

                return data;
            }

            my.collect = function (target) { //将target区域的视图元素信息，收集到视图中
                _elements = [];
                if (onlySelf()) return;
                target = target || document.body;
                var ns = _viewName.indexOf(',') > -1 ? _viewName.split(',') : [_viewName];
                my.append(internal(target, ns));
            }

            function internal(c, ns, es) {//ns:视图名称集合,由于视图名称可以指定多个，因此ns为数组
                es = es || [];
                var l = J(c).children();
                if (l) {
                    l.each(function (i, e) {
                        var pe = getProxy(e);
                        if (pe) {
                            var vn = pe.view || pe.attr("data-view");
                            var exist = false;
                            if (!empty(vn)) {
                                vn = mapViewName(vn);
                                if (vn === "*") {
                                    exist = true;
                                }
                                else if (type(vn) == 'string' && vn.indexOf(',') > -1) {
                                    //指定了多个视图名称
                                    var vns = vn.split(',');
                                    exist = vns.contains(function (i, v) {
                                        return ns.contains(function (k, n) {
                                            return v == n;
                                        });
                                    });
                                }
                                else {
                                    exist = ns.contains(function (k, n) {
                                        return vn == n;
                                    });
                                }
                            }
                            if (exist) {
                                my.append(pe);
                                internal(e, ns, es); //继续分析后续
                            }
                        }
                        internal(e, ns, es);
                    });
                }
                return es;
            }

            function validate(ps,customProcessor) {
                var r = new result();
                ps.each(function (i, p) {
                    exec(p, "validate", function (p) {
                        r.add(p.validate());
                    });
                });
                if (!r.satisfied()) return r;//如果组件验证失败,那么报错,不用继续执行自定义验证
                var _validate = customProcessor.validate; //视图自定义验证器
                if (_validate) {
                    var e = my;
                    try {
                        var t = _validate.apply(e, [e]);//自定义验证
                        if (t!=true && !empty(t)) {
                            var m = type(t) == "string" ? t : "自定义验证出错";
                            r.add(new ValidateResult("error", m, false));
                        }
                    } catch (ex) {
                        r.add(new ValidateResult("error", ex.message, false));
                    }
                }
                return r;
            }

            my.collect();//创建对象后自动收集脚本元素
        }

        function ValidateResult(s, m, st) { //class
            var _s = s, _m = m, _st = st;
            this.status = function () { return _s; };
            this.message = function () { return _m; };
            this.satisfied = function () { return _st; };
        }

        $$view.ValidateResult = ValidateResult;

        $$view.exec = function (d) { //执行脚本视图数据
            var t = d.process;
            if (!t) return;
            t = t.fromBase64();
            var f;
            eval(["f = function(){", t, "};"].join('')); //这是字符串格式，需要转换为函数执行
            return f();
        }

        $$view.callback = function (d) {//回调一个视图数据，默认情况下用jquery的ready方法注册回调函数
            $(document).ready(function () {
                $$view.exec(d);
            });
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
    })();

    var _globalEventProcessors = {}; //全局事件处理器

    $$view.registerEventProcessor = function (name, processor) { //注册全局视图事件处理器
        var t = _globalEventProcessors;
        if (t[name]) throw new Error("重复注册全局视图事件处理器" + name);
        t[name] = processor;
    }

    $$view.getEventProcessor = function (name) { return _globalEventProcessors[name] || null; } //获取全局视图事件处理器

    $$view.submit = function (p) { //提交视图
        var ctx = $$view.context; //这是视图最后一次提交的环境
        if (!ctx) return;
        ctx.view.submit(ctx.target, p);
    }

    api.registerGod(new function () {
        this.give = function (p) {
            var k = p.invoke;//invoke:{events:[{client:'click',server:'save',option:{...}}]}
            if (k) {//如果元素指定了远程调用参数，那么赋予远程调用的能力
                var pj = p.getJquery();
                var evts = k.events;
                J.each(evts, function (i, evt) {
                    var c = [evt.client, ".invoke"].join(''); //客户端事件标示
                    pj.on(c, { source: p, evt: evt, k: k }, function (e) {
                        var s = e.data.source, p = s.getJquery(), n = p.attr("name") || '';
                        if (!n) {
                            //尝试查找舞台组件，如果对象在舞台组件内部，那么使用舞台组件的名称作为提交的目标
                            var cm = p.closest("[data-stage-component]").mapNull();
                            if (cm) {
                                n = cm.attr("data-stage-component");  //以舞台组件的设置值为名称
                                if (n == "true") n = cm.attr("name") || ''; //如果名称为true，那么使用舞台组件的名称
                            }
                        }
                        var viewName = p.attr("data-view") || p.attr("view") || null; //如果没有设置视图名称，那么视图名称为null

                        var evt = e.data.evt;
                        var view = new $$view(s, viewName);
                        view.submit({ component: n, action: evt.server,scene:s }, evt.option);
                    });
                });
            }
        }
    });

    $$view.session = new function () {//基于视图的会话，该会话在单个页面访问、回调期间有效，每次提交视图都会提交全局视图会话数据
        var my = this, _data = {};
        this.get = function (n) {
            if (empty(n)) return _data;
            return _data[n];
        }
        this.set = function (n,v) {
            _data[n] = v;
        }
    }; 
});