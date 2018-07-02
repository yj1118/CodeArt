$$.createModule("Component.pageScroll", function (api, module) {
    api.requireModules(["Component"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component, $input = $component.input, $request = $$.ajax.request;

    var $pageScroll = $component.pageScroll = function (painter) {
        $o.inherit(this, $component.ui, painter);
        var my = this, scroll, prevPageIndex = -1, _cb;
        this.give = function (o) {
            $o.callvir(this, $component.ui, "give", o);
            o.load = function (cb) {
                _cb = cb;
                load();
            };
            o.clear = function () { clear(); };
            o.oversee = function () { //监视滚动条移动，异步加载数据
                $(document).ready(function () {
                    $(window).on("scroll.pageScroll", function () {
                        if (o.disabled || o.disabled == undefined) return;
                        o.disabled = true;
                        var offset = $(window).height() / 2; //浏览器可视高度
                        if ($$(document).scrollFull(offset)) {//当距离滚动条底部接近2分之一屏幕大小时，开始加载
                            load();
                        } else o.disabled = false;
                    });
                });
            }
            o.unoversee = function () {//不再监视，不加载数据
                $(window).off(".pageScroll");
            }

            o.oversee();//开始监视
            $component.util.initQuery(o);

            function load() {
                var newQuery = $.query.set("r", new Date().getTime());
                var newUrl = location.pathname;
                var url = o.para("url") || newUrl;
                var action = o.para("action");

                var req = new $$request();
                req.add("pageIndex", prevPageIndex + 1);
                var haveSize = false, paras = {};
                o.query().each(function (i, c) {
                    var n = c.name, v = c.get();
                    if (n == "pageSize") {
                        req.add(n, v);
                        haveSize = true;
                    } else paras[n] = v;
                });
                if (!haveSize) req.add("pageSize", o.para("pageSize") || 5); //如果没有从参数中得到pageSize,则使用手动设置的属性值
                req.add("paras", paras);
                req.success = function (r) {
                    if (r.rows.length == 0) {
                        if (_cb) _cb(prevPageIndex);
                        o.unoversee();
                        return;
                    }
                    else {
                        prevPageIndex++;
                        var cont = $$(my.find(o, "container"));
                        $(".container").css("display", "block");
                        cont.bind(r);
                        execOnload(o, cont, r);
                        //validateFull(o);
                    }
                }
                req.beforeSend = function () {
                    status("loading");
                }

                req.complete = function () {
                    o.disabled = false;
                    if (prevPageIndex < 0) status("empty", _cb); //当接受的最后一次数据为空后，不再监视滚动条事件，这代表整个组件不再提供后续数据的加载
                    else status("default");
                }

                req.post({ url: url, action: action });
            }

            function execOnload(o, cont, data) {
                var len = data.rows.length;
                if (o.getEvent("onload")) {
                    //有onload事件
                    var rows = $(cont).children(), newRows = [], offset = 1;
                    for (var i = 0; i < len; i++) {
                        newRows.unshift(rows[rows.length - 1 - offset - i]);
                    }
                    o.execEvent("onload", [newRows, data, o]);
                }
            }

            function status(n, fn) {
                var st = $(my.find(o, "status"));
                switch (n) {
                    case "loading":
                        {
                            st.text("加载中...");
                            st.css("display", "block");
                        }
                        break;
                    case "empty":
                        {
                            if (fn) {
                                fn(prevPageIndex);
                                return;
                            }
                            st.text("暂无信息，请查看其它...");
                            st.css("display", "block");
                        }
                        break;
                    case "default":
                        {
                            st.text("");
                            st.css("display", "none");
                        }
                        break;
                }
            }

            function clear() {
                var ds = $("div[class='pageRow row']");
                for (var i = 1; i < ds.length; i++) $(ds[i]).remove();
                prevPageIndex = -1;
            }

            //function validateFull(container) {
            //    var c = container;
            //    var cTop = $(c).offset().top;
            //    var cHeight = $(c).height();
            //    var cY = cTop + cHeight;
            //    var offset = $(window).height();
            //    if (cY < offset) load();
            //}

        }
    }

    $pageScroll.painter = function (methods) {
        $o.inherit(this, $$.component.painter, methods);
    }

    $pageScroll.create = function () {
        return new $component.pageScroll(new $pageScroll.painter());
    }


});