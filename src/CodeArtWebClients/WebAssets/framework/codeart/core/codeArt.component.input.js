$$.createModule("Component.input", function (api, module) {
    api.requireModules(["Component"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component, $input = $component.input;

    //文本组件
    (function () {
        var $text = $input.text = function (painter, validator) {
            $o.inherit(this, $input, painter, validator);
            var my = this;
            this.execReadOnly = function (o, b) {
                my.find(o, "input").prop("readOnly", b == false ? false : true);
            }
            this.execDisabled = function (o, b) {
                my.find(o, "input").prop("disabled", b == false ? false : true);
            }
            this.execSet = function (o, v) {
                my.find(o, "input").val(v);
            }
            this.execGet = function (o) {
                var v = my.find(o, "input").val();
                return v || "";
            }
            this.execReset = function (o) {
                o.set("", false);
            }

            this.onGive = function (o) { //give事件
                var p = my.find(o, "input");
                if (p) {
                    var jq = p.getJquery();
                    jq.on("blur.autoValidate", { obj: o }, function (e) { //追加自动验证事件
                        var o = e.data.obj;
                        o.validate(true);
                    });
                }
            }
        }

        $o.virtual($text, "give", function (o) {
            $o.callvir(this, $input, "give", o);
        });
        $o.virtual($text, "execBrowseData", function (o) { return o.get(); });

        //#region 验证器

        //#region 针对某一项逻辑的验证处理器

        var handlers = $input.validateHandlers = {};

        var requiredHandler = handlers.required = function (msg) { //必填处理器
            if (!msg) msg = "请输入{label}";
            this.exec = function (o, n, v) {
                if (o.para("required") && (empty(v) || v.length == 0)) throw new Error(msg.replace("{label}", n));
            }
        }

        var lengthHandler = handlers.length = function (msg) { //长度处理器
            if (!msg) msg = {};
            if (!msg.less) msg.less = "{label}的长度不能小于{min}";
            if (!msg.more) msg.more = "{label}的长度不能大于{max}";
            this.exec = function (o, n, v) {
                var t;
                if (ignoreValidate(o, v)) return;
                if ((t = o.para("min")) && v.length < t) {
                    throw new Error(msg.less.replace("{label}", n).replace("{min}", t));
                }
                else if ((t = o.para("max")) && v.length > t) {
                    throw new Error(msg.more.replace("{label}", n).replace("{max}", t));
                }
            }
        }

        function emailHandler() { //邮箱格式处理器
            this.exec = function (o, n, v) {
                if (ignoreValidate(o, v)) return;
                var r = /^[a-z0-9\u0391-\uFFE5]+([-|_|.]?[a-z0-9\u0391-\uFFE5]+)*@[a-z0-9\u0391-\uFFE5]+(-?[a-z0-9\u0391-\uFFE5]+)*\.[a-z\u0391-\uFFE5]{2,6}(\.[a-z\u0391-\uFFE5]{2,6})*$/i;
                if (!r.test(v)) throw new Error(n + "不是有效的邮箱地址格式");
            }
        }

        function urlHandler() { //url格式处理器
            this.exec = function (o, n, v) {
                if (ignoreValidate(o, v)) return;
                var s = "^((https|http|ftp|rtsp|mms)?://)?" // 
                + "(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" // ftp的user@ 
                + "(([0-9]{1,3}.){3}[0-9]{1,3}" // IP形式的URL- 199.194.52.184 
                + "|" // 允许IP和DOMAIN（域名） 
                + "([0-9a-z_!~*'()-]+.)*" // 域名- www. 
                + "([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]." // 二级域名 
                + "[a-z]{2,6})" // first level domain- .com or .museum 
                + "(:[0-9]{1,4})?" // 端口- :80 
                + "((/?)|" // a slash isn't required if there is no file name 
                + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";
                var r = new RegExp(s);
                if (!r.test(v)) throw new Error(n + "不是有效的url地址格式");
            }
        }

        function ignoreValidate(o, v) {
            if ((empty(v) || v.length == 0) && !o.para("required")) return true; //当没有输入，并且required不为true时，不用验证
            return false;
        }

        function passwordHandler() {//验证二次密码是否相同
            this.exec = function (o, n, v) {
                var p = o.para("target");
                if (!p) { return; }
                var _target = $$("#" + p);
                if (!_target) { throw new Error("没有找到二次密码的目标" + p); }
                if (v != _target.get()) {
                    throw new Error("两次输入的密码不同");
                }
            }
        }

        var numberMode = {
            "float": { reg: /^(-)?[0-9]+\.{0,1}[0-9]{0,2}$/, msg: "小数" },
            "int": { reg: /^(-)?\d+$/, msg: "整数" }
        };

        function numberHandler() {//数字验证器，可以指定整数、小数等
            this.exec = function (o, n, v) {
                var p = o.para("mode");
                var m = numberMode[p];
                if (!m) m = numberMode["float"]
                if (v) if (!m.reg.test(v)) { throw new Error("请输入" + m.msg); }
                validate();
                function validate() {//判断maxValue,minValue大小
                    var min = o.para("minValue"), max = o.para("maxValue"), pf = parseFloat;
                    if (!empty(min) && pf(v) < pf(min)) throw new Error(n + "不能小于" + min);
                    if (!empty(max) && pf(v) > pf(max)) throw new Error(n + "不能大于" + max);
                }
            }
        }

        function formatHandler() {//格式化验证器,format:规则，formatMessage:规则名称
            this.exec = function (o, n, v) {
                var p = o.para("format"), msg = o.para("formatMessage");
                if (p) {
                    if (!p.test(v)) throw new Error("请输入正确的" + (msg) ? msg : "规则");
                }
            }
        }

        //#endregion

        var $validator = $text.Validator = function () {
            $o.inherit(this, $input.validator);
            this.addHandler(new requiredHandler());
            this.addHandler(new lengthHandler());
        }

        $validator.text = function () {
            $o.inherit(this, $validator);
        }

        $validator.email = function () {
            $o.inherit(this, $validator);
            this.addHandler(new emailHandler());
        }

        $validator.url = function () {
            $o.inherit(this, $validator);
            this.addHandler(new urlHandler());
        }

        $validator.dateTime = function () {
            $o.inherit(this, $validator);
        }

        $validator.password = function () {
            $o.inherit(this, $validator);
            this.addHandler(new passwordHandler());
        }

        $validator.number = function () {
            $o.inherit(this, $validator);
            this.addHandler(new numberHandler());
        }
        //#endregion

        //#region 默认的绘画器

        $text.painter = function (methods) {
            $o.inherit(this, $input.painter, methods);
        }

        //#endregion

        //#region 工厂方法

        $input.createText = function (painter) {
            if (!painter) painter = new $text.painter();
            return new $text(painter, new $validator.text());
        }
        $input.createEmail = function (painter) {
            if (!painter) painter = new $text.painter();
            return new $text(painter, new $validator.email());
        }
        $input.createUrl = function (painter) {
            if (!painter) painter = new $text.painter();
            return new $text(painter, new $validator.url());
        }

        $input.createTextarea = function (painter) {
            if (!painter) painter = new $text.painter();
            return new $text(painter, new $validator.text());
        }

        $input.createPassword = function (painter) {
            if (!painter) painter = new $text.painter();
            return new $text(painter, new $validator.password());
        }

        $input.createNumber = function (painter) {
            if (!painter) painter = new $text.painter();
            return new $text(painter, new $validator.number());
        }

        $input.createVerifyCode = function (painter) {
            if (!painter) painter = new $text.painter();
            return new $text(painter, new $validator.text());
        }

        //#endregion


    })();

    //选择组件
    (function () {

        //#region 基类
        $select = $input.select = function (painter, validator) {
            $o.inherit(this, $input, painter, validator);
            var my = this;
            my.execReadOnly = function (o, b) {
                throw new Error("select组件没有实现readOnly方法");
            }
            my.execDisabled = function (o, b) {
                throw new Error("select组件没有实现disabled方法");
            }
        }

        $vir($select, "give", function (o) {
            $o.callvir(this, $input, "give", o); //调用父类方法
            var my = this;
            o.options = function (l) { return my.execOptions(this, l); } //作为选择控件，可以得到/设置选择项的集合
            var t = o.para("items");
            if (t) o.options(t); //解析完后，绑定选项
        });

        $vir($select, "execOptions", function (o, l) {
            var my = this;
            if (l) {//设置
                my.find(o, "coreContainer").proxy().bind({ items: l });
                o.para("items", l);
                var ops = my.find(o, "options").toArray();
                ops.each(function (i, e) {
                    my.initOption(o, $(e).proxy());
                });
                o.__options = ops;
                o.reset();

            } else {//得到
                return o.__options;
            }
        });

        $vir($select, "initOption", function (o, p) { p.input = o; }); //初始化选项，每个选项的input属性，指向组件自身

        //#region 验证器

        function requiredHandler() { //必填处理器
            this.exec = function (o, n, v) {
                if (!v) v = [];
                if (type(v) != 'array') v = [v];
                if (o.para("required") && v.length == 0) throw new Error("请选择" + n);
            }
        }

        function lengthHandler() { //选项个数处理器
            this.exec = function (o, n, v) {
                if (!v) v = [];
                if (type(v) != 'array') v = [v];
                var t;
                if (ignoreValidate(o, v)) return;
                if ((t = o.para("min")) && v.length < t) throw new Error(n + "的选项数不能少于" + t);
                else if ((t = o.para("max")) && v.length > t) throw new Error(n + "的选项数不能大于" + t);
            }
        }

        function ignoreValidate(o, v) {
            if (!o.para("required") && v.length == 0) return true; //当没有输入，并且required不为true时，不用验证
            return false;
        }

        $select.validator = function () {
            $o.inherit(this, $input.validator);
            this.addHandler(new requiredHandler());
            this.addHandler(new lengthHandler());
        }

        //#endregion

        //#region 单选基类
        var $single = $select.single = function (painter, validator) {
            $o.inherit(this, $select, painter, validator);

            this.execSelectItem = function (o, i) {
                if (empty(i)) { //获取
                    var c = null;
                    o.options().each(function (i, e) {
                        var t = $$(e);
                        if (t.selected()) { c = t; return false; }
                    });
                    return c;
                } else { //设置
                    var l = o.options();
                    if (i < 0 || i >= l.length) return;
                    var c = $$(l[i]);
                    c.selected(true);
                }
            }

            this.execGet = function (o) {
                var c = o.selectItem();
                return c ? c.value() : "";
            }
        }

        $vir($single, "execBrowseData", function (o) {
            var c = o.selectItem();
            return c ? c.getJquery().text() : "";
        });

        $vir($single, "give", function (o) {
            var my = this;
            o.selectItem = function (i) { return my.execSelectItem(this, i); } //单选组件，可以根据序号选择
            $o.callvir(this, $select, "give", o);
        });

        $vir($single, "execOptions", function (o, l) { return $o.callvir(this, $select, "execOptions", o, l); });

        $vir($single, "initOption", function (o, p) {
            $o.callvir(this, $select, "initOption", o, p);
            p.selectItem = function (b) {
                throw new Error("未实现selectItem方法");
            }
            p.value = function (v) {
                throw new Error("未实现value方法");
            }
            p.selected = function (b) {
                throw new Error("未实现selected方法");
            }
        });

        $vir($single, "execSet", function (o, v) {
            if (o.get() === v) return;
            var l = o.options(), c = o.selectItem(), n = l.first(function (i, e) { return util.equals($$(e).value(), v); });
            if (!n) return;
            else n = n.proxy();
            if (c) c.selected(false);
            n.selected(true);
            //o.execEvent("onchange", [o, v]);
        });

        $vir($single, "execReset", function (o) {
            var c = o.selectItem();
            if (c) c.selected(false);
            this.draw(o, "reset", c);//将旧项传给draw
            //o.execEvent("onchange", [o]);
        });

        //单选绘制器的基类
        $single.painter = function (methods) {
            $o.inherit(this, $input.painter, methods);
            this.findOptions = function (o) {
                return o.find("*[data-name='option']");
            }
        }
        //#endregion

        //#region 多选基类

        var $multi = $select.multi = function (painter, validator) {
            $o.inherit(this, $select, painter, validator);
            var my = this;

            /***如果有子类需要调用父类的方法，那么以下方法可以改写为虚方法***/

            this.execSelectAll = function (o, b) {
                if (b != false) b = true;
                var v = [];
                o.options().each(function (i, e) {
                    var p = $$(e);
                    p.selected(b);
                    if (b) v.push(p.value());
                });
                o.set(v);
            }

            this.execReverse = function (o) {
                var v = [];
                o.options().each(function (i, e) {
                    var p = $$(e);
                    p.selected(!p.selected());
                    if (p.selected()) v.push(p.value());
                });
                o.set(v);
            }

            this.execSelectItems = function (o, inds) {
                if (inds) {//设置
                    var vl = [], l = o.options(), sl = [];
                    for (var i = 0; i < inds.length; i++) {
                        var ind = inds[i];
                        if (ind < 0 || ind >= l.length) { alert("index范围非法!"); return; }
                        vl.push(l[ind].value);
                        sl.push(l[ind]);
                    }
                    o.set(vl);
                } else { //获取
                    var l = [];
                    o.options().each(function (i, e) {
                        var t = $$(e);
                        if (t.selected()) l.push(t);
                    });
                    return l;
                }
            }

            this.execSet = function (o, vl) {
                if (o.para("stringMode") || type(vl) == 'string') { //开启了字符串模式，值都以字符串的形式返回
                    o.para("stringMode", true);
                    var strValue = vl;
                    vl = strValue.split(',');
                }

                var l = o.options(), ol = o.selectItems(), nl = l.filterEx(function (k, e) {
                    var v = $$(e).value();
                    for (var i = 0; i < vl.length; i++) {
                        if (util.equals(vl[i], v)) return true;
                    }
                    return false;
                });

                var draw = my.draw;
                ol.each(function (i, e) {
                    var p = $$(e);
                    p.selected(false);
                    draw(o, "selectOption", p);
                });
                nl.each(function (i, e) {
                    var p = $$(e);
                    p.selected(true);
                    draw(o, "selectOption", p);
                });
                changeSelectItems(o);
            }

            this.execGet = function (o) {
                var vl = [];
                o.selectItems().each(function (i, e) {
                    vl.push($$(e).value());
                });

                if (o.para("stringMode")) { //开启了字符串模式，值都以字符串的形式返回
                    vl = vl.join(',');
                }
                return vl;
            }

            this.execReset = function (o) {
                var draw = my.draw;
                o.selectItems().each(function (i, e) {
                    var p = $$(e);
                    p.selected(false);
                    draw(o, "selectOption", p);
                });
                changeSelectItems(o);
            }
        }

        function changeSelectItems(o) { //o的选项被改变了
            o.execEvent("onchange", [o]);
        }

        $vir($multi, "execBrowseData", function (o) {
            var cc = o.selectItems(), l = [];
            cc.each(function () {
                var t = $(this);
                l.push(t.getJquery().text());
            });
            return l.join(" > ");
        });

        $vir($multi, "give", function (o) {
            var my = this;
            o.selectAll = function (b) { my.execSelectAll(this, b); };//全选
            o.reverse = function () { my.execReverse(this); };//反选
            o.selectItems = function (inds) { return my.execSelectItems(this, inds); }; //获取或设置被选择的项集合
            $o.callvir(this, $select, "give", o);
        });

        $vir($multi, "execOptions", function (o, l) { return $o.callvir(this, $select, "execOptions", o, l); });

        $vir($multi, "initOption", function (o, p) {
            var my = this;
            $o.callvir(this, $select, "initOption", o, p);

            p.selectItem = function (b) {
                var pt = this.getJquery().find("input");
                if (empty(b)) b = !pt.prop("checked");//没有强制设置选择状态，那么取反
                pt.prop("checked", b);
            }
            p.value = function (v) {
                var pt = this.getJquery().find("input");
                return pt.val();
            }
            p.selected = function (b) {
                var pt = this.getJquery().find("input");
                if (empty(b)) return pt.prop("checked");
                pt.prop("checked", b);
            }
        });

        //多选绘制器的基类
        $multi.painter = function (methods) {
            $o.inherit(this, $input.painter, methods);
            this.findOptions = function (o) {
                return o.find("*[data-name='option']");
            }
        }

        $vir($multi.painter, "drawBySelectOption", function (o, p) {
            //什么都不用做
        });

        //#endregion

        //#endregion

    })();
});
