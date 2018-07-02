var $$request, $$storage;
var $$ = (function () {

    //#region 基础

    var OBJECT = "object", FUNCTION = "function", UNDEFINED = "undefined", J = jQuery;

    function isHostMethod(o, p) {
        var t = typeof o[p];
        return t == FUNCTION || (!!(t == OBJECT && o[p])) || t == "unknown";
    }

    function isHostObject(o, p) {
        return !!(typeof o[p] == OBJECT && o[p]);
    }

    function isHostProperty(o, p) {
        return typeof o[p] != UNDEFINED;
    }

    function createMultiplePropertyTest(testFunc) {
        return function (o, props) {
            var i = props.length;
            while (i--) {
                if (!testFunc(o, props[i])) {
                    return false;
                }
            }
            return true;
        };
    }

    var areHostMethods = createMultiplePropertyTest(isHostMethod);
    var areHostObjects = createMultiplePropertyTest(isHostObject);
    var areHostProperties = createMultiplePropertyTest(isHostProperty);

    function type(o) {
        var ty = typeof o;
        if (ty == 'object') {
            if (o == null) return "null";
            if (o.constructor == Array) return "array";
            if (o.constructor == Date) return "date";
            if (isEvent(o)) return "event";
            if (o.constructor == Blob) return "blob";
        }
        return ty;
    }

    function isEvent(o) {
        return isHostProperty(o, "srcElement")
                || isHostProperty(o, "fromElement")
                || isHostProperty(o, "shiftKey")
                || isHostProperty(o, "ctrlKey");
    }

    function empty(v) {//判断是否为空值
        return v == undefined || v == null;
    }

    function equals(v0, v1) {
        //如果遇到字符串和其他类型的数据对比，我们仅看值是否相等，而不考虑数据类型
        var ty0 = type(v0), ty1 = type(v1);
        if (ty0 == "string" || ty1 == "string") {
            v0 = v0 + "";
            v1 = v1 + "";
        }
        if (type(v0) != type(v1)) return false;
        if (v0.equals) return v0.equals(v1);
        return v0 == v1;
    }

    //#endregion

    var api = function (e) {
        if (empty(e)) return;
        if (e.isProxy) return e;
        var ty = type(e);
        if (ty == "object") { //直接传入对象，意味着需要创建该对象的代理
            if (isHostMethod(e, "getAttribute") || (isHostMethod(e, "ent") && e.ent() || e == document)) { //如果是dom对象，或者是代理对象
                return createProxy(e);
            }
        }
        else {
            var o = J(e);
            if (o.length == 1) return api(o[0]);
            throw new Error("没有找到" + e + "或类型不正确，无法生成代理对象");
        }
    };

    var modules = {};

    api.initialized = false;
    api.supported = true;
    api.util = {
        isHostMethod: isHostMethod,
        isHostObject: isHostObject,
        isHostProperty: isHostProperty,
        areHostMethods: areHostMethods,
        areHostObjects: areHostObjects,
        areHostProperties: areHostProperties,
        type: type,
        empty: empty,
        equals: equals
    };
    api.modules = modules;
    api.config = {
        alertOnFail: true,
        alertOnWarn: true
    };

    //#region 信息输出

    function consoleLog(msg) {
        if (isHostObject(window, "console") && isHostMethod(window.console, "log")) {
            window.console.log(msg);
        }
    }

    function alertOrLog(msg, shouldAlert) {
        if (shouldAlert) {
            window.alert(msg);
        } else {
            consoleLog(msg);
        }
    }

    function fail(reason) {
        api.initialized = true;
        api.supported = false;
        alertOrLog("codeArt框架不被您的浏览器兼容. 原因: " + reason, api.config.alertOnFail);
    }

    api.fail = fail;

    function warn(msg) {
        alertOrLog("警告: " + msg, api.config.alertOnWarn);
    }

    api.warn = warn;

    if ({}.hasOwnProperty) {
        api.util.extend = function (obj, props, deep) {
            var o, p;
            for (var i in props) {
                if (props.hasOwnProperty(i)) {
                    o = obj[i];
                    p = props[i];
                    if (deep && o !== null && typeof o == "object" && p !== null && typeof p == "object") {
                        api.util.extend(o, p, true);
                    }
                    obj[i] = p;
                }
            }
            return obj;
        };
    } else {
        fail("hasOwnProperty 不被支持");
    }

    //#endregion

    function initModules() {
        if (api.initialized) return;

        api.initialized = true;

        //初始化模块
        var module, errorMessage;
        for (var moduleName in modules) {
            if ((module = modules[moduleName]) instanceof Module) {
                module.init();
            }
        }
    }

    //#region 模块注入

    function Module(name, initializer) {
        this.name = name;
        this.initialized = false;
        this.supported = false;
        this.init = initializer;
    }

    Module.prototype = {
        fail: function (reason) {
            this.initialized = true;
            this.supported = false;
            alertOrLog("加载模块 '" + this.name + "' 失败: " + reason, api.config.alertOnFail);
        },
        warn: function (msg) {
            api.warn("模块 " + this.name + ": " + msg);
        },
        deprecationNotice: function (deprecated, replacement) {
            api.warn("过时: " + deprecated + " 在模块 " + this.name + "中已过时. 请使用 "
                + replacement + " 代替");
        },
        createError: function (msg) {
            alertOrLog("模块 " + this.name + " 发生错误: " + msg, api.config.alertOnFail);
        }
    };

    api.createModule = function (name, initFunc) {
        var module = new Module(name, function () {
            if (!module.initialized) {
                module.initialized = true;
                try {
                    initFunc(api, module);
                    module.supported = true;
                } catch (ex) {
                    var errorMessage = "加载模块 '" + name + "' 失败: " + getErrorDesc(ex);
                    //consoleLog(errorMessage);
                    throw new Error(errorMessage);
                }
            }
        });
        modules[name] = module;
    };

    function getErrorDesc(ex) {
        return ex.message || ex.description || String(ex);
    }

    api.requireModules = function (moduleNames) {
        for (var i = 0, len = moduleNames.length, module, moduleName; i < len; ++i) {
            moduleName = moduleNames[i];

            module = modules[moduleName];
            if (!module || !(module instanceof Module)) {
                throw new Error("没有找到模块 '" + moduleName + "'");
            }

            module.init();

            if (!module.supported) {
                throw new Error("不支持模块 '" + moduleName + "'");
            }
        }
    };

    var _gods = [];
    api.registerGod = function (gods) { //注册上帝（代理能力的给予者）^_^
        _gods.push(gods);
    }

    //#endregion

    //#region 处理代理对象

    function initProxy(o) { //初始化整个文档，或者某个区域，为其所有定义了代理属性的dom创建代理对象，并赋予相关能力
        if (!o) o = document.body;
        if (type(o) == "array") {
            J.each(o, function (i, c) {
                initProxy(c);//再初始化子节点
            });
        } else if (o.each) {
            o.each(function (i, c) {
                initProxy(c);
            });
        }
        else {
            var pro = createProxy(o, false); //尝试为自身创建代理
            if (pro) return;//如果已创建代理，那么子项不需要再初始化，因为在pro内部会初始化
            initProxy(J(o).children());//初始化子节点
        }
    }

    function getProxy(o) { //获取对象的代理信息
        o = J(o)[0];
        if (o.__type_proxy) return o; //o本身就为代理对象
        return o.__proxy ? o.__proxy : null;
    }

    function createProxy(o, force) { //force:强制创建代理对象，如果为false，那么只有定义了data-proxy属性，才会创建
        if (empty(force)) force = true;
        o = J(o);
        var pro = getProxy(o);
        if (pro) return pro;

        var s, ent = o[0];
        if (!(s = o.attr("data-proxy"))) {
            if (!force) return null;//没有代理属性，又不是强制的，因此不创建
            s = "{}";
            o.attr("data-proxy", s);
        }
        eval("pro=" + s + ";");
        ent.__proxy = pro;
        pro.__ent = ent;
        pro.__type_proxy = true;
        pro.isProxy = true;
        giveProxy(pro); //赋予代理的能力
        if (!pro.closeProxy) initProxy(o.children()); //初始化代理范围内的子节点
        giveCapability(pro);//最后赋予代理中手工指定的能力
        if (pro.id) o.attr("id", pro.id);

        var dt;
        if (dt = pro.ref) {//定义了引用，则从引用中导入属性
            for (var e in dt) { if (!pro[e]) pro[e] = dt[e]; } //空缺的属性，由引用导入
            delete pro.ref;
        }
        pro.initialized = true; //代理对象初始化完成
        return pro;
    }

    function clearProxy(e) {
        var l = J(e).children();
        l.each(function (i, t) { clearProxy(t); });
        if (e.__proxy) e.__proxy = null;
    }

    function giveCapability(o) {
        var g = o.give;
        if (g) {
            if (type(g) != "array") { g.give(o); return; }
            g.each(function (i, c) {
                c.give(o);
            });//每个能力对象会实现方法give
        }
    }

    function giveProxy(p) {
        J.each(_gods, function (i, g) {
            g.give(p);
        });
    }

    api.init = initProxy; //由外界决定，是否初始化代理

    api.registerGod(new function () {
        this.give = function (p) {
            p.ent = function () { return this.__ent; }
            p.getJquery = function () { return $(p.ent()); }
            p.attr = function (n, v) {
                if (type(n) == "object") {
                    for (var e in n) {
                        this.attr(e, n[e]);
                    }
                    return this;
                } else {
                    var ent = this.ent();
                    if (!ent) return this;
                    if (n.indexOf('@') > -1) {//自定义属性
                        n = n.substr(1);
                        var pro = createProxy(ent);
                        if (empty(v)) return pro[n];
                        else {
                            pro[n] = v;
                            if (n == "id") this.attr("id", v);//如果设置代理编号，为了提高检索性能，映射代理编号到dom编号上
                            return this;
                        }
                    } else {
                        if (n == "innerText") return innerText(ent, v);
                        if (empty(v)) {
                            var t = ent[n];
                            return empty(t) ? ent.getAttribute(n) : t;
                        }
                        else {
                            ent.setAttribute(n, v);
                            ent[n] = v;
                            if (n == "id") {//如果设置代理编号，为了提高检索性能，映射代理编号到dom编号上
                                var pro = createProxy(ent);
                                if (pro) pro.id = v;
                            }
                            return this;
                        }
                    }
                }
                function innerText(o, v) {
                    var t = o.innerText, n = "innerText";
                    if (empty(t)) n = "textContent";
                    if (!empty(v)) o[n] = v;
                    return o[n];
                }
            }
            p.removeAttr = function (n) {
                var ent = this[0];
                if (n.indexOf('@') > -1) {
                    n = n.substr(1);
                    var pro = createProxy(ent);
                    delete pro[n];
                } else {
                    var t = ent.removeAttribute;
                    if (t) t(n); else delete o[n];
                }
            }
        }
    });

    //$$ = function (o) {
    //    return $(o).proxy();
    //}

    //#endregion

    api.util.extend(api, J);
    api.util.getProxy = function (o) { //获取代理对象，并且，会自动创建设置了proxy属性的对象的代理对象
        var p = getProxy(o);
        if (!p) p = createProxy(o, false);
        return p;
    };
    api.util.clearProxy = clearProxy;
    var _currentId = 0;
    api.util.getId = function () {
        return _currentId++;
    };

    J(document).ready(function () { initModules(); });

    //#region 扩展getJquery对象
    J.fn.extend({
        ent: function () { return this[0]; },
        proxy: function () {
            var o = this[0];
            return createProxy(o);//如果没有代理对象，则创建，如果已存在，直接返回，不会重复创建代理对象
        },
        getJquery: function () { return this; }, //此处是为了与proxy对象保持统一的方法
        mapNull: function () {
            return empty(this[0]) ? null : this;
        }
    });
    //#endregion

    return api;
})();

$$.createModule("Util", function (api, module) {
    var J = jQuery, util = api.util, type = util.type, empty = util.empty;

    //#region 字符串扩展
    var sp = String.prototype;
    sp.trim = function () { return this.replace(/(^\s*)|(\s*$)/g, "") };
    sp.toUpper = function (n) {
        if (!n) return this.toUpperCase();
        return this.substr(0, n).toUpperCase() + this.substr(n);
    };
    sp.toLower = function (n) {
        if (!n) return this.toLowerCase();
        return this.substr(0, n).toLowerCase() + this.substr(n);
    };
    sp.pad = function (l) {
        return function (n) {
            var num = this;
            return (0 >= (n = n - num.toString().length)) ? num : (l[n] || (l[n] = Array(n + 1).join(0))) + num;
        }
    }([]);

    sp.replaceAll = function (t0, t1, ignoreCase) {
        if (!RegExp.prototype.isPrototypeOf(t0)) {
            return this.replace(new RegExp(t0, (ignoreCase ? "gi" : "g")), t1);
        } else {
            return this.replace(t0, t1);
        }
    }

    if (!window.btoa) {
        var a = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        window.btoa = function (c) {
            var d = "";
            var m, k, h = "";
            var l, j, g, f = "";
            var e = 0;
            do {
                m = c.charCodeAt(e++);
                k = c.charCodeAt(e++);
                h = c.charCodeAt(e++);
                l = m >> 2;
                j = ((m & 3) << 4) | (k >> 4);
                g = ((k & 15) << 2) | (h >> 6);
                f = h & 63;
                if (isNaN(k)) {
                    g = f = 64
                } else {
                    if (isNaN(h)) {
                        f = 64
                    }
                }
                d = d + a.charAt(l) + a.charAt(j) + a.charAt(g) + a.charAt(f);
                m = k = h = "";
                l = j = g = f = ""
            } while (e < c.length);
            return d;
        };
        window.atob = function (c) {
            var d = "";
            var m, k, h = "";
            var l, j, g, f = "";
            var e = 0;
            do {
                l = a.indexOf(c.charAt(e++));
                if (l < 0) {
                    continue
                }
                j = a.indexOf(c.charAt(e++));
                if (j < 0) {
                    continue
                }
                g = a.indexOf(c.charAt(e++));
                if (g < 0) {
                    continue
                }
                f = a.indexOf(c.charAt(e++));
                if (f < 0) {
                    continue
                }
                m = (l << 2) | (j >> 4);
                k = ((j & 15) << 4) | (g >> 2);
                h = ((g & 3) << 6) | f;
                d += String.fromCharCode(m);
                if (g != 64) {
                    d += String.fromCharCode(k)
                }
                if (f != 64) {
                    d += String.fromCharCode(h)
                }
                m = k = h = "";
                l = j = g = f = ""
            } while (e < c.length);
            return d;
        }
    }

    sp.toBase64 = function () {
        return window.btoa(unescape(encodeURIComponent(this)))
    };

    sp.fromBase64 = function () {
        return decodeURIComponent(escape(window.atob(this)));
    };

    //#endregion

    //#region 集合扩展
    var ap = Array.prototype;
    ap.each = function (m) {
        var v;
        for (var i = 0; i < this.length; i++) {
            var c = this[i];
            if (m.apply(c, [i, c]) == false) break; //显示传递false,代表中断循环
        }
    }
    ap.filterEx = function (f, c) {
        var l = [];
        for (var i = 0; i < this.length; i++) {
            var o = this[i];
            if (f.apply(o, [i, o])) {
                l.push(o);
                if (c && l.length == c) break;
            }
        }
        return l;
    }
    ap.first = function (f) {
        if (f) return this.filterEx(f, 1).first();
        return this.length > 0 ? $(this[0]) : null;
    }
    ap.contains = function (f) {
        return !empty(this.first(f));
    }
    ap.distinct = function (f) { //返回非重复序列,f是比较函数，相等返回true
        if (!f) f = function (v0, v1) { return v0 == v1; }
        var l = [], s = this;
        s.each(function (i, v0) {
            var r = l.first(function (j, v1) { return f(v0, v1); });
            if (!r) l.push(v0);
        });
        return l;
    }
    ap.last = function () { return this.length > 0 ? $(this[this.length - 1]) : null; }
    ap.remove = function (p) {
        if (empty(p)) return;
        var l = this, le = l.length;
        if (type(p) == "function") {//设置了比较函数，p代表需要移除的值
            var ef = p, ags = [null], as = arguments;
            for (var i = 1; i < as.length; i++) { if (!empty(as[i])) ags.push(as[i]); }
            for (var i = 0; i < le; i++) {
                ags[0] = l[i];
                if (ef.apply(l[i], ags)) {
                    l.remove(i);
                    i--; //归位
                    le--;
                }
            }
            return l;
        } else {//没有设置比较函数，p代表位置
            if (p < 0 || p >= le) return;
            for (var i = p; i < le - 1; i++) l[i] = l[i + 1];
            return l.pop();
        }
    }
    ap.toArray = function () { return this; } //为了保持与jquery的扩展统一接口
    ap.equals = function (v) {
        if (this.length != v.length) return false;
        for (var i = 0; i < this.length; i++) {
            if (!util.equals(this[i], v[i])) return false;
        }
        return true;
    }
    ap.clone = function () {
        var len = this.length;
        return this.concat(this).splice(len);
    }
    //#endregion

    //#region 日期扩展
    Date.prototype.clone = function () { return createDate(this.get()); }
    Date.prototype.get = function () {
        return {
            year: this.getFullYear(), month: this.getMonth(), date: this.getDate(), hour: this.getHours(),
            minute: this.getMinutes(), second: this.getSeconds(), milliseconds: this.getMilliseconds()
        };
    }
    Date.prototype.equals = function (v) { return this.getTime() == v.getTime(); }
    function createDate(v) { return new Date(v.year, v.month, v.date, v.hour, v.minute, v.second); };
    Date.prototype.format = function (f) {
        var d = this.get(), l = [];
        for (var i = 0; i < f.length; i++) {
            var e = f[i];
            switch (e) {
                case "y": l.push(d.year); break;
                case "M": l.push((d.month + 1 + '').pad(2)); break;
                case "d": l.push((d.date + '').pad(2)); break;
                case "h": l.push((d.hour + '').pad(2)); break;
                case "m": l.push((d.minute + '').pad(2)); break;
                case "s": l.push((d.second + '').pad(2)); break;
                default: l.push(e);
            }
        }
        return l.join('');
    }

    //#endregion

    util.object = {
        clone: function (o) {
            var oc = o.constructor == Object ? new o.constructor() : new o.constructor(o.valueOf());
            for (var key in o) {
                var ocv = oc[key], tcv = o[key];
                if (ocv != tcv) {
                    oc[key] = typeof (tcv) == 'object' ? util.object.clone(tcv) : tcv;
                }
            }
            oc.toString = o.toString;
            oc.valueOf = o.valueOf;
            return oc;
        },
        inherit: function (o, base) { //模拟继承，不论是虚方法，还是this方法，都可以被重写，但是虚方法可以在子类被调用，this方法不行
            var args = [], as = arguments;
            for (var i = 2; i < as.length; i++) { args.push(as[i]); }
            base.apply(o, args);
            var p = base.prototype;
            for (var e in p) {
                o[e] = p[e];
            }
            p = o.constructor.prototype; //自身的原型
            for (var e in p) {
                o[e] = p[e]; //重置自身原型提供的方法，这些方法是不被base重写的，同名方法覆盖父方法
            }
        },
        virtual: function (base, fn, func) { //为base类型声明一个虚方法
            var p = base.prototype;
            p[fn] = func || function () { throw new Error("方法" + fn + "是抽象的"); }
        },
        callvir: function (o, base, fn) { //调用虚方法,fn:基类的方法名称
            var args = [], as = arguments;
            for (var i = 3; i < as.length; i++) { args.push(as[i]); }
            var f = base.prototype[fn];
            if (!f) throw new Error("无法执行方法" + fn + ",必须定义在原型链上！");
            return f.apply(o, args);
        },
        eachValue: function (o, f) {
            for (var n in o) {
                var v = o[n];
                if (util.type(v) == "function") continue;
                f.apply(o, [n, v]);
            }
        }
    };

    util.json = {};
    util.json.serialize = function (obj) {
        var v = empty(obj) ? '' : JSON.stringify(obj).replace(/\\n/g, '\n');
        return v;
    }
    util.json.deserialize = function (str) {//请保证str是json格式的{....}
        if (empty(str) || str.length == 0) return {};
        var o;
        eval("o=" + str + ";");
        return o;
    }
    var loc = util.location = {};
    loc.addUrl = function (url, state) {
        window.history.pushState(state, '', url);
    };
    loc.replaceUrl = function (url, state) {
        window.history.replaceState(state, '', url);
    };
    loc.updateQuery = function (state, replace) { //更新参数
        var q = $.query, t = q;
        util.object.eachValue(state, function (n, v) {
            if (!empty(v)) t = t.set(n, v);
        });
        var url = location.pathname + t.toString();
        if (replace) util.location.replaceUrl(url, state); else util.location.addUrl(url, state);
    };
    /*
    本地视图代表一种模式：由url参数组成视图的状态
    第一次进入视图时，更新url地址栏（替换）
    第二次以后进入视图，则用追加的形式更新地址栏，当浏览器后退了
    那么会恢复视图，并且触发恢复视图的事件
    恢复视图不会更新url地址
    这个模式适用于ajax和url地址同步改变
    */
    loc.view = function (name, onrestore) { //
        var my = this;
        this.name = name;
        this.restoring = false; //正在恢复中
        this.firstWork = true; //第一次工作
        this.update = function (state) { //state是用来描述视图的状态
            if (my.restoring) { //正在恢复中，代表恢复已完毕
                my.restoring = false;
                return;
            }
            var replace = this.firstWork;//第一次工作，那么需要替换当前url,否则以add形式追加url
            $.util.location.updateQuery(state, replace); //将state作为参数替换url,并且将state作为状态存储到历史记录中
            this.firstWork = false;
        }

        $(window).on("popstate." + name, function (e) {
            my.restoring = true;
            onrestore(e.originalEvent.state);
        });
    }
    loc.back = function () { window.history.back(); } //简单的封装，以后可以扩展
    $location = loc;

    var $b = util.browser = {};

    (function () {
        var u = navigator.userAgent.toLowerCase(), s;
        (s = u.match(/msie ([\d.]+)/)) ? $b.ie = s[1] :
                (s = u.match(/firefox\/([\d.]+)/)) ? $b.firefox = s[1] :
                (s = u.match(/chrome\/([\d.]+)/)) ? $b.chrome = s[1] :
                (s = u.match(/opera.([\d.]+)/)) ? $b.opera = s[1] :
                (s = u.match(/version\/([\d.]+).*safari/)) ? $b.safari = s[1] : 0;

    })();

    api.registerGod(new function () {
        this.give = function (p) {
            p.clone = function (onlyNode) { //代理中的节点克隆 
                var e = this.ent(), t = e.cloneNode(true);
                util.clearProxy(t);//防止克隆了__proxy属性
                if (onlyNode == true) return t;//仅克隆节点结构，并不执行初始化操作,返回dom节点
                api.init(t);
                return $$(t);//代理的克隆方法，返回代理
            }
            p.find = function (exp) { return this.getJquery().find(exp); }
            var scrollCurrentTop;
            p.tag = function () {
                var t = this.ent().tagName;
                return t ? t.toLowerCase() : "";
            }
            p.scrollHeight = function () {
                var ent = this.ent();
                return scroll(ent, "height");
            }
            p.scrollWidth = function () {
                var ent = this.ent();
                return scroll(ent, "width");
            }
            p.scrollTopTo = function (target) {//target:滚动条需要定位的对象
                var t = J(target);
                if (this.scrollHeight() > 0) this.scrollTop(t.offset().top - this.offset().top);
            }
            p.scrollHidden = function () {
                var o = this.ent();
                if (o == document) o = document.documentElement;
                if ($b.firefox) {
                    var t = J(document).scrollTop();
                    o.style.overflow = "hidden";
                    J(document).scrollTop(t);//防止滚动条自动滑动
                    scrollCurrentTop = t;
                } else
                    o.style.overflow = "hidden";
            }
            p.scrollResume = function () {
                var o = this.ent();
                if (o == document) o = document.documentElement;
                if ($b.firefox) {
                    o.style.overflow = "";
                    J(document).scrollTop(scrollCurrentTop);//防止滚动条自动滑动
                } else
                    o.style.overflow = "";
            }
            p.existScroll = function () { //存在滚动条
                var o = this.getJquery(), rawTop = o.scrollTop();
                if (rawTop > 0) return true;
                var top = 10, r;
                o.scrollTop(10);
                r = o.scrollTop() > 0;
                o.scrollTop(0);
                return r;
            }
            p.scrollFull = function (offset) {  //滚动条是否达到底部
                offset = offset || 0;//修正，离底部的距离
                var t = this.getJquery(), pageHeight = t.height(), lookHeight = $(window).height();//页面高度，浏览器可视高度
                return (t.scrollTop() + offset) >= (pageHeight - lookHeight);
            }
            p.floating = function (p) {
                var o = this.getJquery(), enId = api.util.getId();

                function exec(e) {
                    var target = e.data.target, para = e.data.para, flag;
                    if (target.css("display") == "none") return;
                    var targetHeight = target.outerHeight(true), offset = para.offset, innerOffset = para.innerOffset || 0;
                    var start = offset.top(), end = $(document).height() - offset.bottom();
                    var scrollTop = $(document).scrollTop();
                    if (scrollTop < start) {
                        target.css({ "position": "inherit", "top": "auto" });
                        flag = "stayTop";
                    }
                    else {
                        target.css({ "position": "fixed", "top": innerOffset + "px" });
                        flag = "floating";
                    }
                    var targetBottom = target.offset().top + targetHeight;
                    if (targetBottom > end) {
                        target.css({ "position": "fixed", "top": (end - targetBottom) + "px" });
                        flag = "stayBottom";
                    }
                    if (para.handler) para.handler(flag);
                }

                $(window).off("scroll.floating_" + enId);
                $(window).on("scroll.floating_" + enId, { target: o, para: p }, function (e) { exec(e); });
                $(window).off("resize.floating_" + enId);
                $(window).on("resize.floating_" + enId, { target: o, para: p }, function (e) { exec(e); });
            }

            function scroll(o, n) {
                if (o == document) {
                    o = o.documentElement;
                }
                var pn = "scroll" + n.charAt(0).toUpperCase() + n.substr(1);
                return o[pn];
            }

        }
    });
});

$$.createModule("Ajax", function (api, module) {
    api.requireModules(["Util"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, object = util.object;

    var serializer = new Serializer();

    (function () {

        function Request() {
            var my = this, _ser = serializer;

            this.post = this.submit = function (arg) {
                arg = parseArg(arg);
                var p = {
                    cache: false,
                    type: "POST",
                    url: arg.url,
                    async: arg.async,
                    data: _ser.serialize(my.getData(arg,my.paras)),
                    beforeSend: function (xhr) {
                        xhr.responseType = "text";
                        xhr.setRequestHeader("PostAction", arg.action ? arg.action : '');
                        execEvent(my, "beforeSend", [my, xhr]);
                    },
                    dataFilter: arg.dataFilter || function (rawData, dataType) { return _ser.deserialize(rawData); }
                };
                initEvent(my, p);
                copyEvent(my, p);
                J.ajax(p);
            }

            my.paras = {};
            my.add = function (n, v) {
                if (type(n) == "object") {
                    var o = n;
                    object.eachValue(o, function (on, ov) {
                        my.add(on, ov);
                    });
                }
                this.paras[n] = v;
            }; //增加参数
            my.clear = function () { this.paras = {}; }; //清空参数


            this.get = function (arg) {
                arg = parseArg(arg);
                var p = {
                    cache: false,
                    type: "GET",
                    url: arg.url,
                    async: arg.async,
                    data: _ser.serialize(my.getData(arg,my.paras)),
                    beforeSend: function (xhr) {
                        xhr.responseType = "text";
                        xhr.onprogress = function (e) { execEvent(my, "progress", [e]); }
                        execEvent(my, "beforeSend", [my, xhr]);
                    },
                    dataFilter: function (rawData, dataType) { return rawData; }
                };
                initEvent(my, p);
                copyEvent(my, p);
                J.ajax(p);
            }

            this.formData = function (arg) {
                arg = parseArg(arg);
                var p = {
                    cache: false,
                    type: "POST",
                    url: arg.url,
                    async: arg.async,
                    data: arg.data, //formData
                    contentType: false,//必须false才会自动加上正确的Content-Type 
                    processData: false,//必须false才会避开jQuery对 formdata 的默认处理, XMLHttpRequest会对 formdata 进行正确的处理 
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("PostAction", arg.action ? arg.action : '');
                        execEvent(my, "beforeSend", [my, xhr]);
                    },
                    dataFilter: arg.dataFilter || function (rawData, dataType) { return _ser.deserialize(rawData); }
                };
                initEvent(my, p);
                copyEvent(my, p);
                J.ajax(p);
            }

            this.getData = function (arg, paras) { //可以实例化重写
                return Request.getData(arg, paras);
            }

        }

        Request.getData = function (arg,paras) { return paras; } //外界可以重写

        $$.ajax = {};
        $$.ajax.request = Request;
    })();

    function Serializer() {
        this.deserialize = function (text) {
            return util.json.deserialize(text);
        }
        this.serialize = function (obj) { return util.json.serialize(obj); }
    }

    function parseArg(arg) { //分析参数
        if (empty(arg)) return {};
        if (type(arg) == "string") { arg = { url: arg } };
        if (!arg.url) arg.url = window.location.href;
        if (empty(arg.async)) arg.async = true;
        return arg;
    }

    function initEvent(obj, para) {
        fillDefaultEvent(obj);
        para.success = obj.success;
        para.complete = obj.complete;
        para.progress = obj.progress;
        para.error = obj.error ? function (xhr, msg, o) { obj.error(getErrorMessage(xhr)); } : processError;


        function processError(xhr, msg, obj) { //或d
            bootbox.alert({
                buttons: {
                    ok: {
                        label: '确定',
                    }
                },
                message: getErrorMessage(xhr),
                title: "错误提示"
            });
        }
    }

    function getErrorMessage(xhr) {
        var txt;
        if (xhr.status == "error" && xhr.message) txt = xhr.message;
        else try { txt = xhr.responseText; } catch (e) { } //读取xhr.responseText有可能报错
        if (txt) return txt;
        else {
            return [
                xhr.response ? xhr.response : '',
                xhr.statusText ? xhr.statusText : '',
                xhr.status ? xhr.status : '',
            ].join(',')
        }
    }

    function execEvent(obj, n, args) {
        var t;
        if (t = obj[n]) {
            return t.apply(obj, args);
        }
    }

    var _events = ["beforeSend", "dataFilter", "error", "success", "complete", "progress"];
    function copyEvent(source, target, force) { //拷贝source的ajax事件到target ,force:是否强制赋予目标对象事件
        if (empty(source) || empty(target)) return;
        var t;
        _events.each(function (i, n) {
            if (t = source[n]) {
                if (force) target[n] = t;
                else {
                    if (!target[n]) target[n] = t;
                }
            }//target没有对应的事件才拷贝
        });
    }

    $$.ajax.util = {
        copyEvent: copyEvent,
        getErrorMessage: getErrorMessage
    }

    function fillDefaultEvent(target) { //用系统的默认事件，填充没有设置的事件
        copyEvent(_defaultEvent, target);
    }

    var _defaultEvent = new function () { //默认事件
        var my = this;
        _events.each(function (i, n) {
            my[n] = function () { }
        });
    }

    $$.config.setAjaxEvent = function (evt) {
        _defaultEvent = evt;
    }
    $$.wrapper = {};//提供扩展用
    $$request = $$.ajax.request;
});


$$.createModule("storage", function (api, module) {
    api.requireModules(["Util"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, object = util.object;

    var storage = function (n, v, p) {
        var isRead = arguments.length == 1;
        if (arguments.length == 2) {
            if (type(v) != 'string') {
                p = v;
                v = null;
                isRead = true;
            }
        }

        if (!p) p = {};
        if (!p.mode) p.mode = 'local';
        if (p.mode == 'cookie' || !window.localStorage || !window.sessionStorage) return isRead ? $.cookie(n) : $.cookie(n, v, p);
        var h = p.mode == 'local' ? window.localStorage : window.sessionStorage;
        return isRead ? h.getItem(n) : h.setItem(n, v);
    }

    storage.remove = function (n,p) {
        if (!p) p = {};
        if (!p.mode) p.mode = 'local';
        if (p.mode == 'cookie' || !window.localStorage || !window.sessionStorage) $.cookie(n, null);
        else {
            var h = p.mode == 'local' ? window.localStorage : window.sessionStorage;
            h.removeItem(n);
        }
    }

    $$storage = $$.storage = storage;
});