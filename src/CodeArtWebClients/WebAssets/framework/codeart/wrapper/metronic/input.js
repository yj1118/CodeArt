$$.createModule("Wrapper.metronic.input", function (api, module) {
    api.requireModules(["Component", "Component.input", "Wrapper.metronic"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component, $input = $component.input;
    var $select = $input.select, $single = $select.single, $multi = $select.multi;
    $$.wrapper.metronic.input = {};

    //#region radio单选组件

    var $radio = $$.wrapper.metronic.input.radio = function (painter, validator) {
        $o.inherit(this, $single, painter, validator);
    }


    $vir($radio, "initOption", function (o, p) {
        var my = this;
        $o.callvir(this, $single, "initOption", o, p);

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
            pt.iCheck(b ? 'check' : 'uncheck');
        }
    });

    $vir($radio, "execOptions", function (o, l) {
        var result = $o.callvir(this, $select, "execOptions", o, l);
        var my = this;
        if (l) {//设置
            //基方法已经生成了项，现在需要赋予事件
            o.getJquery().on("ifChecked", "input:radio", function (e) {
                o.execEvent("onchange", [o]);
            });
        } else {//得到
            return result;
        }
    });

    $vir($radio, "execDisabled", function (o, b) {
        o.getJquery().iCheck(b == false ? 'enable' : 'disable');
    });

    $radio.bindOption = function (o, d) {
        var j = o.getJquery(), p = $$(j.closest(".form-group")[0]), skin = p.para("skin");
        j.html(["<input type=\"radio\" class=\"icheck\" data-radio=\"iradio_", skin, "\" value=\"", d.value, "\" name=\"radio-", $$(j.parents('.form-group')[0]).name, "\" ", (d.disabled ? "disabled" : ""), " /> ", d.text].join(''));
        j.find('input').iCheck({ radioClass: "iradio_" + skin });
    }

    $radio.painter = function (methods) {
        $o.inherit(this, $single.painter, methods);
    }

    $$.wrapper.metronic.input.createRadio = function (painter) {
        if (!painter) painter = new $radio.painter();
        return new $radio(painter, new $select.validator());
    }

    //#endregion

    //#region checkbox多选组件

    var $checkbox = $$.wrapper.metronic.input.checkbox = function (painter, validator) {
        $o.inherit(this, $multi, painter, validator);
    }

    $vir($checkbox, "execOptions", function (o, l) {
        var result = $o.callvir(this, $multi, "execOptions", o, l);
        var my = this;
        if (l) {//设置
            //基方法已经生成了项，现在需要赋予事件
            o.getJquery().on("ifChanged", "input:checkbox", function (e) {
                o.execEvent("onchange", [o]);
            });
        } else {//得到
            return result;
        }
    });

    $vir($checkbox, "execDisabled", function (o, b) {
        o.getJquery().iCheck(b == false ? 'enable' : 'disable');
    });

    $vir($checkbox, "initOption", function (o, p) {
        var my = this;
        $o.callvir(this, $multi, "initOption", o, p);

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
            pt.iCheck(b ? 'check' : 'uncheck');
        }
    });

    $checkbox.painter = function (methods) {
        $o.inherit(this, $multi.painter, methods);
    }

    $checkbox.bindOption = function (o, d) {
        var j = o.getJquery(), p = $$(j.closest(".form-group")[0]), skin = p.para("skin");
        j.html(["<input type=\"checkbox\" class=\"icheck\" data-checkbox=\"icheckbox_", skin, "\" value=\"", d.value, "\" ", (d.disabled ? "disabled" : ""), " /> ", d.text].join(''));
        j.find('input').iCheck({ checkboxClass: "icheckbox_" + skin });
    }

    $$.wrapper.metronic.input.createCheckbox = function (painter) {
        if (!painter) painter = new $checkbox.painter();
        return new $checkbox(painter, new $select.validator());
    }

    //#endregion

    //#region dropdown组件

    var $dropdown = $select.dropdown = function (painter, validator) {
        $o.inherit(this, $select, painter, validator);

        this.execGet = function (o) {
            return o.dropdown.select2("val");
        }

        this.execDisabled = function (o, b) {
            o.dropdown.prop("disabled", b);
        }
    }

    $vir($dropdown, "execBrowseData", function (o) { return o.getText(); });

    $vir($dropdown, "give", function (o) {
        var my = this;
        o.dropdown = o.find("select");
        o.selectItem = function (i) {
            return my.execSelectItem(this, i);
        } //单选组件，可以根据序号选择
        o.getText = function () {
            return this.dropdown.find("option:selected").text();
        }
        $o.callvir(this, $select, "give", o);
    });

    $vir($dropdown, "execOptions", function (o, l) {
        var my = this;
        if (l) {//设置
            l = l.clone();
            if (!o.para("multiple")) {
                if (!o.para("group") && o.para("emptyItem") != false) {
                    l.unshift({ value: '', text: '' }); //不是多选时，填充第一项为空
                }
            }
            var data = o.para("group") ? { groups: l } : { items: l };
            my.find(o, "coreContainer").proxy().bind(data);
            o.para("items", l);
            var dropdown = o.dropdown;
            dropdown.select2("destroy");
            dropdown.select2({
                placeholder: o.para("placeholder"),
                allowClear: true
            });
            dropdown.off("change.dropdown");
            dropdown.on("change.dropdown", function () {
                o.validate(true);
                var v = o.get();
                o.execEvent("onchange", [o, v]);
                if (!v) o.execEvent("onunselect", [o]);
            });

            o.reset();
        } else {
            return my.find(o, "options").toArray();
        }
    });

    $vir($dropdown, "execSelectItem", function (o, i) {
        var my = this, ops = o.options();
        if (i >= ops.length) return;
        var p = ops[i];
        o.set(p.value);
    });

    $vir($dropdown, "execSet", function (o, v) {
        v = v + ""; //由于下拉选项不识别具体类型，所以一律按照字符串处理
        o.dropdown.select2("val", v);
        o.execEvent("onchange", [o, v]);
    });

    $vir($dropdown, "execReset", function (o) {
        o.dropdown.select2("val", null);
        o.execEvent("onchange", [o, null]);
    });

    $dropdown.painter = function (methods) {
        $o.inherit(this, $input.painter, methods);
        this.findOptions = function (o) {
            return o.find("*[data-name='option']");
        }
    }

    $$.wrapper.metronic.input.createDropdown = function (painter) {
        if (!painter) painter = new $dropdown.painter();
        return new $dropdown(painter, new $select.validator());
    }

    //#endregion

    //#region tags组件

    var $tags = $select.tags = function (painter, validator) {
        $o.inherit(this, $select, painter, validator);

        this.execGet = function (o) {
            if (o.para("multiple")) {
                var d = o.data, v = [];
                if (!d || d.length == 0) return v;
                var values = o.dropdown.select2("val");
                values.each(function (i, value) {
                    var t = d.first(function (j, ov) {
                        return ov.value == value;
                    });
                    if (t) v.push(t[0]);
                });
                return v;
            }
            else {
                var d = o.data, v = null;
                if (!d || d.length == 0) return v;
                var value = o.dropdown.select2("val");
                var t = d.first(function (j, ov) {
                    return ov.value == value;
                });
                return t ? t[0] : null;
            }
        }

        this.execDisabled = function (o, b) {
            o.dropdown.prop("disabled", b);
        }
    }

    $vir($tags, "give", function (o) {
        var my = this;
        o.dropdown = o.find("select");
        $o.callvir(this, $select, "give", o);
        o.add = function (v) {
            if (o.para("multiple")) {
                var d = this.get();
                d = d.concat(v);
                d = d.distinct(function (v0, v1) {
                    return v0.value == v1.value && v0.text == v1.text;
                });
                this.set(d);
            }
            else {
                this.set(v);
            }
        }

        o.getJquery().on("click.input-tags", "button[data-name='select']", function (e) {
            o.execEvent("onselect", [o]);
        });

        o.getJquery().on("click.input-tags", "button[data-name='reset']", function (e) {
            o.options([]);
        });

        //2016.3.30新加代码，修复多选模式下，文本框出现定值长度（例如1455PX等）的问题
        if (o.para("multiple")) {
            o.getJquery().find("input").css("width","100%");
        }
    });

    $vir($tags, "execBrowseData", function (o) {
        var d = o.data, l = [];
        if (!d || d.length == 0) return '';
        var values = o.dropdown.select2("val");
        if (o.para("multiple")) {
            values.each(function (i, value) {
                var t = d.first(function (j, ov) {
                    return ov.value == value;
                });
                if (t) l.push(t[0].text);
            });
            return l.join("，");
        }
        return values != null ? values.text : '';
    });

    var _tempCont = $("<div></div>");

    $vir($tags, "execOptions", function (o, l) {
        var my = this;
        if (l) {//设置
            l = l.clone();
            if (o.getJquery().find("option").length == 0) {
                o.getJquery().find("select").append(_tempCont.find("option").first());
            }

            var data = { items: l };
            my.find(o, "coreContainer").proxy().bind(data);
            o.para("items", l);
            if (l.length == 0) _tempCont.append(o.getJquery().find("option").first());

            var dropdown = o.dropdown;
            dropdown.select2("destroy");
            dropdown.select2({
                allowClear: true
            });
            dropdown.off("change.tags");
            dropdown.on("change.tags", function () {
                o.validate(true);
                var v = o.get();
                o.execEvent("onchange", [o, v]);
                if (!v) o.execEvent("onunselect", [o]);
            });

            o.data = l;
            o.reset();
        } else {
            return my.find(o, "options").toArray();
        }
    });

    $vir($tags, "execSet", function (o, v) { //v是{value,text}或集合
        if (o.para("multiple")) {
            o.options(v);
            o.dropdown.select2("val", getValues(v));
        } else {
            o.options([v]);
            o.dropdown.select2("val", v.value);
        }
    });

    $vir($tags, "execReset", function (o) {
        o.dropdown.select2("val", null);
    });

    $tags.painter = function (methods) {
        $o.inherit(this, $input.painter, methods);
        this.findOptions = function (o) {
            return o.find("*[data-name='option']");
        }
    }

    function getValues(l) {
        var vl = [];
        l.each(function (i, t) {
            vl.push(t.value);
        });
        return vl;
    }

    $$.wrapper.metronic.input.createTags = function (painter) {
        if (!painter) painter = new $tags.painter();
        return new $tags(painter, new $select.validator());
    }

    //#endregion


    //#region 多层级dropdown联动组件

    var $dropdownLevel = $select.dropdownLevel = function (painter, validator) {
        $o.inherit(this, $input.text, painter, validator);
        var my = this;
        my.give = function (o) {
            $o.callvir(this, $input.text, "give", o);
            //o.accept = function (d) { return my.execAccept(this, d); }
            o.loadOptions = function () { return my.execLoadOptions(this); }
            o.init = function (v, cb) {
                var o = this, p = {};
                if (o.para("xaml")) {
                    cb = v; //在xaml下可以接受回调函数 
                    p.callBack = function () {
                        var component = o.para("name");//组件名称
                        var viewName = o.para("view"); //组件相关的视图名称
                        var view = new $$view(o, viewName);
                        view.submit({ component: component, action: "Changed" }, { ignoreValidate: true });
                        if (cb) cb();
                    }
                } else {
                    if (type(v) == "function") {
                        p.callBack = v;
                    } else {
                        p.value = v;
                        p.callBack = cb;
                    }
                }
                return my.execInit(this, p);
            }; //初始化组件，先加载选项，然后用v来设置选项的值，最后触发用户的onchange事件
            o.getText = function () { return my.execGetText(this); }
            o.getUnits = function () { return _units; }

            o.scriptElement = function () { //获得脚本元素的元数据
                var my = this, v = my.get(), pv = my.parentValue || '';
                var p = new function () {
                    this.validate = function () {
                        return my.validate(true);
                    }
                }

                return {
                    id: my.getJquery().prop("id") || '',
                    metadata: { value: v, parentValue: pv },
                    eventProcessor: p
                };
            }


            init(o);
        }

        my.execBrowseData = function (o) {
            return o.getText().join(" > ");
        }

        this.execInit = function (o, p) {
            var v = p.value, cb = p.callBack;
            if (!empty(v)) {
                changePointer = function (o, u) {
                    changePointer = execChange; //还原事件指针
                    o.set(v);
                    if (cb) cb(o);
                }
            }
            else if (cb) {
                changePointer = function (o, u) {
                    changePointer = execChange; //还原事件指针
                    if (cb) cb(o);
                }
            }
            o.loadOptions();
        }

        var _units;
        function init(o) {
            _units = my.find(o, "units");
            var lastIndex = _units.length - 1;
            for (var i = 0; i < _units.length; i++) {
                var u = $$(_units[i]);
                u.index = i;
                u.owner = o;
                u.isLast = i == lastIndex;
                _units[i] = u;
                //u.getJquery().parent().show();
                u.on("onchange", unitChange);
                u.on("onunselect", unitUnselect);
            }
        }

        //触发onchange事件
        function execChange(o, u) {
            o.syncBrowse();//每次都要同步数据
            o.__tempValue = null; //清理临时value

            if (o.para("xaml")) {
                var component = o.para("name");//组件名称
                var viewName = o.para("view"); //组件相关的视图名称
                var view = new $$view(o, viewName);
                view.submit({ component: component, action: "Changed" }, { ignoreValidate: true });
            }
            else {
                o.execEvent("onchange", [o, u]);
            }
        }

        function clearOptions(u) {
            var o = u.owner;
            u.closeEvent();
            u.options([]);
            u.openEvent();
            if (o.para("autoHidden") != false)
                u.getJquery().parent().hide();
        }

        function unitUnselect(u) {
            var o = u.owner, ns = nexts(u);
            if (ns.length > 0) {
                for (var i = 0; i < ns.length; i++) {
                    clearOptions(ns[i]);
                }
            }
            changePointer(o, u); //清空值时，触发事件
        }

        var changePointer = execChange; //change事件指针

        function unitChange(u, v) {
            if (!v) return;
            var o = u.owner;
            var m = null;
            if (o.para("xaml")) { //此标记的意思是用xaml的解析机制来实现
                m = function load(key, callBack) {
                    o.parentValue = key; //该值会传递到服务器上
                    var component = o.para("name");//组件名称
                    var viewName = o.para("view"); //组件相关的视图名称
                    var view = new $$view(o, viewName);
                    view.success = function (r) {
                        callBack(r.items || []);
                    }
                    view.submit({ component: component, action: "LoadOptions" }, { ignoreValidate: true });//在xaml机制下,action是固定值
                };
            } else {
                m = o.para("loadOptions");
            }
            if (!m) return;
            m(v, function (items) {
                var ns = nexts(u);
                if (ns.length == 0) {
                    changePointer(o, u); //最后一个选项被赋值时，触发事件
                    return;
                }
                var f = ns[0];
                f.closeEvent();
                f.options(items);
                f.openEvent();
                var sv = _setValues.length > 0 ? _setValues.shift() : null;
                var first = items.length == 0 ? { value: null } : items[0];
                var acv = empty(sv) ? first.value : sv;
                if (items.length > 0) {
                    f.getJquery().parent().show();
                    f.set(acv); //如果是处于set方法下，那么就设置值
                }
                if (items.length == 0 || acv == '' || empty(acv)) {//没有下拉项，或者第一项没有选择
                    var startIndex = items.length == 0 ? 0 : 1;
                    for (var i = startIndex; i < ns.length; i++) {
                        clearOptions(ns[i]);
                    }
                    changePointer(o, u); //最后一个有效选项（有items）被赋值时，触发事件
                }
            }, u);
        }

        function nexts(u) {
            var i = u.index + 1;
            return _units.slice(i);
        }

        my.execLoadOptions = function (o) {
            var m = null;
            if (o.para("xaml")) { //此标记的意思是用xaml的解析机制来实现
                m = function load(key, callBack) {
                    var component = o.para("name");//组件名称
                    var viewName = o.para("view"); //组件相关的视图名称
                    var view = new $$view(o, viewName);
                    view.success = function (r) {
                        callBack(r.items || []);
                    }
                    view.submit({ component: component, action: "LoadOptions" }, { ignoreValidate:true});//在xaml机制下,action是固定值
                };
            } else {
                m = o.para("loadOptions");
            }

            if (!m) return;
            m(null, function (items) {
                var l = _units;
                if (l.length == 0) {
                    changePointer(o);
                    return;
                }
                var f = l[0];
                f.closeEvent();
                f.options(items);
                f.openEvent();
                var first = { value: '' };
                if (items.length > 0) {
                    f.getJquery().parent().show();
                    f.set(items[0].value);
                    first = items[0];
                }

                if (items.length == 0 || first.value == '' || empty(first.value)) {//没有下拉项，或者第一项没有选择
                    var startIndex = items.length == 0 ? 0 : 1;
                    for (var i = startIndex; i < l.length; i++) {
                        clearOptions(l[i])
                    }
                    changePointer(o, f);
                }
            }, null);
        }

        my.execGet = function (o) {
            if (o.__tempValue) return o.__tempValue; //回调期间使用
            var vl = [];
            for (var i = 0; i < _units.length; i++) {
                var u = _units[i], v = u.get();
                if (v == '' || empty(v)) break;
                vl.push(v);
            }
            return vl;
        }

        my.execGetText = function (o) {
            var tl = [];
            for (var i = 0; i < _units.length; i++) {
                var u = _units[i], t = u.getText();
                if (t == '' || empty(t)) break;
                tl.push(t);
            }
            return tl;
        }

        my.execSet = function (o, vl) {
            if (!vl || vl.length == 0 || _units.length == 0) return;
            _setValues = vl.clone(); //克隆
            o.__tempValue = vl.clone(); //该值是为了在回调期间使用，保持刚set，立即执行get方法也可以获取最新值
            var v = _setValues.shift(), u = _units[0], cv = u.get();
            if (v != cv) _units[0].set(v); //进入设置模式
            else unitChange(u, v); //无论设置的值是否等于当前值，都需要触发事件
        }

        var _setValues = [];
    }

    $dropdownLevel.painter = function (methods) {
        $o.inherit(this, $input.painter, methods);
        this.findUnits = function (o) {
            return o.find("*[data-name='unit']");
        }
    }

    $dropdownLevel.validator = function () {
        $o.inherit(this, $input.validator);
    }

    $$.wrapper.metronic.input.createDropdownLevel = function (painter) {
        if (!painter) painter = new $dropdownLevel.painter();
        return new $dropdownLevel(painter, new $select.validator());
    }
    //#endregion


    //#region 多文本组件

    var $texts = $$.wrapper.metronic.input.texts = function (painter, validator) {
        $o.inherit(this, $input.text, painter, validator);
        var my = this;
        my.give = function (o) {
            $o.callvir(this, $input.text, "give", o);
            o.items = function (l) { return my.execItems(this, l); }
            o.items(o.para("length") || o.para("min"));
        }

        my.onGive = function (o) { //give事件
            //取消基类的实现
        }

        my.execItems = function (o, le) {
            var c = my.find(o, "coreContainer"), items = [];
            for (var i = 0; i < le; i++) items.push({});
            $$(c).bind({ 'items': items });

            o.getJquery().find("input").each(function () {
                var jq = $(this);
                jq.off("blur.autoValidate");
                jq.on("blur.autoValidate", { obj: o }, function (e) { //追加自动验证事件
                    var o = e.data.obj;
                    o.validate(true);
                });
            });
        }

        my.execGet = function (o) {
            var c = my.find(o, "coreContainer"), ps = c.find("input");
            var vl = [];
            ps.each(function () {
                var v = $(this).val();
                if (v) vl.push(v);
            });
            return vl;
        }

        my.execSet = function (o, vl) {
            var c = my.find(o, "coreContainer"), ps = c.find("input");
            for (var i = 0; i < vl.length; i++) {
                var v = vl[i], p = ps[i];
                if (p) $(p).val(v);
            }
        }

        my.execReset = function (o, vl) {
            var c = my.find(o, "coreContainer"), ps = c.find("input");
            ps.each(function () {
                $(this).val('');
            });
        }

        my.execBrowseData = function (o) {
            return o.get().join('，');
        }
    }

    $texts.painter = function (methods) {
        $o.inherit(this, $input.painter, methods);
    }

    $texts.validator = function () {
        $o.inherit(this, $input.validator);
        this.addHandler(new $input.validateHandlers.required());
        this.addHandler(new $input.validateHandlers.length());
    }

    $$.wrapper.metronic.input.createTexts = function (painter) {
        if (!painter) painter = new $dropdownLevel.painter();
        return new $texts(painter, new $texts.validator());
    }
    //#endregion


});