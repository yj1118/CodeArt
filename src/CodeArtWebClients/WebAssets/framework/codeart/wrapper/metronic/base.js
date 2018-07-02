var $$metronic, $$bootbox;
$$.createModule("Wrapper.metronic", function (api, module) {
    var metro = $$metronic = $$.wrapper.metronic = {};
    var util = api.util, empty = util.empty;

    //#region initAjax

    metro.initAjax = function (o) { //提出ajax请求后，初始化metronic的核心机制
        if (!o) Metronic.initAjax();
        else {
            handleUniform(o);
            handleiCheck(o);
            handleBootstrapSwitch(o);
            handleDropdownHover(o);
            handleScrollers(o);
            handleSelect2(o);
            handleFancybox(o);
            handleDropdowns(o);
            handleTooltips(o);
            handleAccordions(o);
            handleBootstrapConfirmation(o);
        }
    }

    function handleUniform(o) {
        if (!$().uniform) {
            return;
        }
        var test = o.find("input[type=checkbox]:not(.toggle, .make-switch, .icheck), input[type=radio]:not(.toggle, .star, .make-switch, .icheck)");
        if (test.size() > 0) {
            test.each(function () {
                if ($(this).parents(".checker").size() === 0) {
                    $(this).show();
                    $(this).uniform();
                }
            });
        }
    };

    function handleiCheck(o) {
        if (!$().iCheck) {
            return;
        }

        o.find('.icheck').each(function () {
            var checkboxClass = $(this).attr('data-checkbox') ? $(this).attr('data-checkbox') : 'icheckbox_minimal-grey';
            var radioClass = $(this).attr('data-radio') ? $(this).attr('data-radio') : 'iradio_minimal-grey';

            if (checkboxClass.indexOf('_line') > -1 || radioClass.indexOf('_line') > -1) {
                $(this).iCheck({
                    checkboxClass: checkboxClass,
                    radioClass: radioClass,
                    insert: '<div class="icheck_line-icon"></div>' + $(this).attr("data-label")
                });
            } else {
                $(this).iCheck({
                    checkboxClass: checkboxClass,
                    radioClass: radioClass
                });
            }
        });
    };

    function handleBootstrapSwitch(o) {
        if (!$().bootstrapSwitch) {
            return;
        }
        o.find('.make-switch').bootstrapSwitch();
    };

    function handleDropdownHover(o) {
        o.find('[data-hover="dropdown"]').not('.hover-initialized').each(function () {
            $(this).dropdownHover();
            $(this).addClass('hover-initialized');
        });
    };

    function handleScrollers(o) {
        Metronic.initSlimScroll(o.find('.scroller'));
    };

    function handleSelect2(o) {
        if ($().select2) {
            o.find('.select2me').select2({
                placeholder: "Select",
                allowClear: true
            });
        }
    };

    function handleFancybox(o) {
        if (!jQuery.fancybox) {
            return;
        }

        if (o.find(".fancybox-button").size() > 0) {
            o.find(".fancybox-button").fancybox({
                groupAttr: 'data-rel',
                prevEffect: 'none',
                nextEffect: 'none',
                closeBtn: true,
                helpers: {
                    title: {
                        type: 'inside'
                    }
                }
            });
        }
    };

    function handleDropdowns(o) {
        o.on('click', '.dropdown-menu.hold-on-click', function (e) {
            e.stopPropagation();
        });
    };

    function handleTooltips(o) {
        o.find('.tooltips').tooltip();

        o.find('.portlet > .portlet-title .fullscreen').tooltip({
            container: 'body',
            title: 'Fullscreen'
        });
        o.find('.portlet > .portlet-title > .tools > .reload').tooltip({
            container: 'body',
            title: 'Reload'
        });
        o.find('.portlet > .portlet-title > .tools > .remove').tooltip({
            container: 'body',
            title: 'Remove'
        });
        o.find('.portlet > .portlet-title > .tools > .config').tooltip({
            container: 'body',
            title: 'Settings'
        });
        o.find('.portlet > .portlet-title > .tools > .collapse, .portlet > .portlet-title > .tools > .expand').tooltip({
            container: 'body',
            title: 'Collapse/Expand'
        });
    };

    function handleAccordions(o) {
        o.on('shown.bs.collapse', '.accordion.scrollable', function (e) {
            Metronic.scrollTo($(e.target));
        });
    };

    function handleBootstrapConfirmation(o) {
        if (!$().confirmation) {
            return;
        }
        o.find('[data-toggle=confirmation]').confirmation({ container: 'body', btnOkClass: 'btn-xs btn-success', btnCancelClass: 'btn-xs btn-danger' });
    }

    //#endregion

    //#region pageBar
    var bar = metro.pageBar = function () { //pageBar
        this.give = function (o) {
            o.bind = function (d) {
                processData(d);
                var e = $$(o.find(".page-breadcrumb")[0]);
                e.bind(d);
                o.data = d;
                if (o.onbind) o.onbind(o, d);
            }

            o.last = function () { //最后一项数据
                var items = o.data.items;
                return items[items.length - 1];
            }

            o.get = function () {
                var t = this.last();
                if (t) return t.value;
                return null;
            }

            o.items = function () { return o.data.items; }

            o.push = function (item) {
                o.data.items.push(item);
                o.bind(o.data);
            }

            o.pop = function (d) {
                if (d.isEnd) return;
                var items = o.data.items, i = d.index + 1;
                items.splice(i, items.length - i);
                o.bind(o.data);
            }

            function processData(d) {
                var last = d.items.length - 1;
                d.items.each(function (i, t) {
                    t.index = i;
                    t.isEnd = t.index == last;
                });
            }
            o.data = { items: [] };
        }
    }
    bar.onbind = function (item, d) {
        item = item.getJquery();
        item.prev().remove();
        item.next().remove();
        if (d.index == 0) {
            item.before("<i class=\"fa fa-home\"></i>");
        }
        if (!d.isEnd) {
            item.after("<i class=\"fa fa-angle-right\"></i>");
        }
        if (!d.url) {
            item.attr("href", "javascript:;");
        }
        if (d.click) {
            item.off("click.bar");
            item.on("click.bar", { item: d }, d.click);
        }

    }

    var modal = metro.modal = function () {
        this.give = function (o) {
            o.title = function (v) {
                var j = o.getJquery();
                if (empty(v)) return j.find(".modal-title").first().text();
                j.find(".modal-title").first().text(v);
            }
        }
    }

    //#endregion

    //#region alert
    var alert = metro.alert = function () {
        this.give = function (o) {
            o.alert = function (d) {
                if (!d) d = {};
                var j = this.getJquery(), id = j.attr("id");
                Metronic.alert({
                    container: '#' + id,
                    place: 'append',
                    type: d.type || 'info',
                    message: d.message,
                    close: d.close == false ? false : true,
                    reset: d.reset == false ? false : true,
                    focus: d.focus == false ? false : true,
                    closeInSeconds: d.seconds || 0,
                    icon: d.cion || ''
                });
                if (j.find('.close').size() == 1) j.children().addClass('alert-dismissable');
                else j.children().removeClass('alert-dismissable');
            }
            o.close = function () {
                this.getJquery().empty();
            }
        }

    }
    //#endregion

    var progressbar = metro.progressbar = function () {
        this.give = function (o) {
            o.update = function (p) {
                if (util.type(p) == 'number') p = { value: p };
                var d = this.getJquery().find('div');
                if (p.color) {
                    d.css('background-color', p.color);
                }
                d.attr('aria-valuenow', p.value).css('width', p.value + '%');
            }
        }
    }

    $$bootbox = $$metronic.bootbox = {};
    $$bootbox.alert = function (msg) {
        bootbox.alert({
            buttons: {
                ok: {
                    label: '确定',
                }
            },
            message: msg
        });
    }

    var form = metro.form = function () {
        this.give = function (o) {
            var my = o;
            o.alert = function (d) {
                my.findAlert().alert(d);
            }

            o.findAlert = function () {
                var id = '#formAlert_' + my.id;
                return $$(id);
            }

            o.check = function (r) {
                var a = my.findAlert();
                if (!r.satisfied()) a.alert({ message: r.message(), type: 'danger', seconds: 0, close: true });
                else a.close();
            }

            o.closeAlert = function () {
                my.findAlert().close();
            }
        }
    }


    //#region portlet
    var portlet = metro.portlet = function () {
        this.give = function (o) {

        }
    }

    var tab = metro.tab = function () {
        this.give = function (o) {
            util.object.callvir(this, $$.component.ui, "give", o);
            o.showItem = function (i) {
                $(this.getJquery().find('.nav li:visible')[i]).find('a').tab('show');
            }
            o.hideItem = function (i) {
                $(this.getJquery().find('.nav li:visible')[i]).hide();
                $(this.getJquery().find('.tab-pane:visible')[i]).hide();
            }
            o.index = function () {
                var l = this.getJquery().find('.nav li:visible');
                for (var i = 0; i < l.length; i++) {
                    var t = $(l[i]);
                    if (t.hasClass('active')) return i;
                }
                return -1;
            }
            o.count = function () {
                return this.getJquery().find('.nav li:visible').length;
            }
            init(o);
        }

        function init(o) {
            if (o.getEvent("select")) {
                o.getJquery().find('a[data-toggle="tab"]').on('shown.bs.tab', { o: o }, function (e) {
                    var o = e.data.o, t = $(e.target);
                    o.execEvent("select", [o, t]);
                });
            }
        }

    }

    //#endregion

    //#region todoList
    var todoList = metro.todoList = function () {
        this.give = function (o) {
            util.object.callvir(this, $$.component.ui, "give", o);
            o.load = function (p) {
                if (!p) p = {};
                var _this = this, req = new $$request();
                req.success = function (r) {
                    _this.bind({ rows: r.rows });
                    var j = o.getJquery(), color = ['red', 'green', 'purple', 'blue', 'yellow'];
                    j.find('.todo-tasklist-item').each(function (i, e) {
                        var index = i % color.length;
                        $(this).addClass("todo-tasklist-item-border-" + color[index]);
                    });
                    _this.execEvent("onload", [_this, r]);
                }
                var url = p.url || _this.para("url") || window.location.href;
                if (p && p.paras) req.add("paras", p.paras);
                req.submit({ url: url, action: _this.para("action") });
            }
        }
    }


    //#endregion
});