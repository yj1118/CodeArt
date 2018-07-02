$$.createModule("Wrapper.metronic.input.dynamic", function (api, module) {
    api.requireModules(["Component", "Component.input", "Wrapper.metronic", "Wrapper.metronic.input"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component, $input = $component.input, $form = $component.form;

    var $dynamic = $$.wrapper.metronic.input.dynamic = function (painter, validator) {
        $o.inherit(this, $input.text, painter, validator);
        var my = this;
        my.give = function (o) {
            $o.callvir(this, $input.text, "give", o);
            o.accept = function (d, cb) { return my.execAccept(this, d, cb); }
        }

        function isEmpty(v) {
            if (v.length == 0) return true;
            for (var i = 0; i < v.length; i++) {
                var t = v[i];
                if (!empty(t.value) && t.value != '') return false;
            }
            return true;
        }

        my.execAccept = function (o, d, cb) {
            if (o.para("xaml")) {
                var gv = o.get();
                //先记录上次的值
                var v = isEmpty(gv) ? o.currentValue : o.get();
                var code = d;
                o.getJquery().html(code);
                $$.init(o);
                _form = newForm(o);
                if (v) o.set(v);
            }
            else {
                var req = new $$request();
                req.add("config", d);
                req.success = function (r) {
                    o.getJquery().html(r);
                    $$.init(o);
                    _form = newForm(o);
                    if (cb) cb(o, d);//执行回调函数
                };
                var url = o.para("url") || "/input/dynamic.htm";
                req.submit({
                    url: url,
                    action: "Load",
                    dataFilter: function (rawData, dataType) { return rawData; }
                });
            }
        }

        my.execClear = function (o) {
            o.getJquery().html('');
        }

        my.execGet = function (o) {
            var vl = [];
            if (_form) {
                var ov = _form.get();
                $o.eachValue(ov, function (n, v) {
                    vl.push({ name: n, value: v });
                });
            }
            return vl;
        }

        my.execSet = function (o, vl) {
            //在组件中记录值
            o.currentValue = vl;

            if (!_form) return;
            var ov = {};
            vl.each(function (i, c) {
                ov[c.name] = c.value;
            });
            _form.accept(ov);
        }

        my.execReset = function () {
            if (!_form) return;
            _form.reset();
            //o.currentValue = null;
        }

        my.validate = function () {
            if (!_form) return { status: function () { return "success"; }, message: function () { return ''; }, satisfied: function () { return true; } };
            var r = _form.validate(true);
            //return { status: r.satisfied() ? "success" : "error", message: r.message() };
            //return { status: function () { r.satisfied() ? "success" : "error" }, message: function () { return r.message(); } };
            return r;
        }


        //内置form
        var _form;
        function newForm(o) {
            var name = o.para("name");
            var f = new $form("dynamic_" + name || '');
            f.collect(o.ent());
            return f;
        }
    }

    $dynamic.painter = function (methods) {
        $o.inherit(this, $input.painter, methods);
        var my = this;
        my.drawByValidate = function (o, result) {
            //什么也不用绘制
        }
    }

    $$.wrapper.metronic.input.createDynamic = function (painter) {
        if (!painter) painter = new $dynamic.painter();
        return new $dynamic(painter);
    }

});