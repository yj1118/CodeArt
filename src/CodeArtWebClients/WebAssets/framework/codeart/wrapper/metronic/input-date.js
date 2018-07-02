/// <reference path="../../../metronic/theme/assets/global/plugins/bootstrap-datetimepicker/js/locales/bootstrap-datetimepicker.zh-CN.js" />
$$.createModule("Wrapper.metronic.input.date", function (api, module) {
    api.requireModules(["Component", "Component.input", "Wrapper.metronic", "Wrapper.metronic.input"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component, $input = $component.input;

    (function () {
        var $date = $input.date = function (painter, validator) {
            $o.inherit(this, $input, painter, validator);
            var my = this;
            my.execReadOnly = function (o, b) {
                throw new Error("date组件没有实现readOnly方法");
            }
            my.execDisabled = function (o, b) {
                throw new Error("date组件没有实现disabled方法");
            }

            my.execBrowseData = function (o) {
                var mode = o.para("mode"), v = o.get();
                if (!v) return '';
                if (mode == "day") {
                    return v.format("y年M月d日");
                } else if (mode == "month") {
                    return v.format("y年M月");
                } else if (mode == "minute") {
                    return v.format("y年M月d日 h点m分");
                }
            }

        }

        $vir($date, "give", function (o) {
            $o.callvir(this, $input, "give", o); //调用父类方法
            var my = this;
            init(o);
        });

        function init(o) {
            var oj = o.getJquery(), mode = o.para("mode"), core = oj.find("input");
            if (mode == "day") {
                core.datepicker({
                    rtl: Metronic.isRTL(),
                    orientation: "left",
                    autoclose: true,
                    language: 'zh-CN',
                    todayHighlight: true,
                    format: 'yyyy/mm/dd'
                });
            } else if (mode == "month") {
                core.datepicker({
                    rtl: Metronic.isRTL(),
                    orientation: "left",
                    autoclose: true,
                    language: 'zh-CN',
                    todayHighlight: true,
                    format: 'yyyy/mm'
                });
            } else if (mode == "minute") {
                core.datetimepicker({
                    rtl: Metronic.isRTL(),
                    autoclose: true,
                    language: 'zh-CN',
                    todayHighlight: true,
                    format: "yyyy/mm/dd hh:ii",
                    pickerPosition: (Metronic.isRTL() ? "bottom-right" : "bottom-left")
                });
            }
        }

        $vir($date, "execGet", function (o) {
            var oj = o.getJquery(), mode = o.para("mode"), core = oj.find("input");
            if (core.val() == '') return null;
            if (mode == "day" || mode == "month") {
                return core.data('datepicker').getDate();
            }
            else if (mode == "minute") {
                return core.data('datetimepicker').getDate();
            }
        });

        $vir($date, "execSet", function (o, v) {
            if (!v) return;
            if (type(v) == "string") v = new Date(v);
            var oj = o.getJquery(), mode = o.para("mode"), core = oj.find("input");
            if (mode == "day" || mode == "month") {
                return core.data('datepicker').setDate(v);
            }
            else if (mode == "minute") {
                return core.data('datetimepicker').setDate(v);
            }
        });

        $vir($date, "execReset", function (o) {
            var oj = o.getJquery(), mode = o.para("mode"), core = oj.find("input");
            if (mode == "day" || mode == "month") {
                return core.data('datepicker').setDate(null);
            } else if (mode == "minute") {
                core.val('');
            }
        });

        var handlers = $input.validateHandlers;

        $date.validator = function () {
            $o.inherit(this, $input.validator);
            this.addHandler(new handlers.required());
            //this.addHandler(new handlers.length({ less: "{label}的文件不能小于{min}个", more: "{label}的文件不能大于{max}个" }));
        }

        //单选绘制器的基类
        $date.painter = function (methods) {
            $o.inherit(this, $input.painter, methods);
        }

        $$.wrapper.metronic.input.createDate = function (painter) {
            if (!painter) painter = new $date.painter();
            return new $date(painter, new $date.validator());
        }
    })();

});