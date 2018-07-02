$$.createModule("Wrapper.metronic.datatable", function (api, module) {
    api.requireModules(["Wrapper.metronic"]);
    api.requireModules(["Component"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component, $request = $$.ajax.request;

    function fillConfig(o, config) {
        var saveState = o.para("saveState");
        if (empty(saveState)) saveState = true;
        config["bStateSave"] = saveState;

        var cols = o.para("columns"), orderIndexs = [], openSort = false;
        var colConfigs = [];
        cols.each(function (i, c) {
            colConfigs.push({ "orderable": c.orderable, "searchable": c.searchable });
            if (c.defaultOrder) orderIndexs.push([i, c.defaultOrder]);
            if (c.orderable) {
                openSort = true;
            }
        });
        config["bSort"] = openSort;
        config["columns"] = colConfigs;
        if (openSort) config["order"] = orderIndexs;
        //config["destroy"] = true;
        //config["order"] = [
        //        [5, "asc"]
        //];
        if (o.para("maxHeight")) {
            config["scrollY"] = o.para("maxHeight");
            config["scrollCollapse"] = true;
        }
    }

    //#region 数据列表(不带翻页功能)
    $$.wrapper.metronic.datalist = function () {
        this.give = function (o) {
            $o.callvir(this, $component.ui, "give", o);
            var my = this;

            function isPageMode(o) { //是否开启了翻页模式
                var p = o.para("pageSize");
                return !empty(p) && p > 0;
            }

            o.isPageMode = isPageMode(o);

            o.load = function (p) {
                if (!p) p = {};
                var o = this, pageIndex, pageSize;
                var f = new $request();
                if (o.isPageMode) {
                    p = getPage(o, p);
                    f.add("pageIndex", p.index);
                    pageIndex = p.index;
                }
                //得到提交的查询参数
                var haveSize = false, paras = {};
                o.query().each(function (i, c) {
                    var n = c.getName(), v = c.get();
                    if (n == "pageSize" && o.isPageMode) {
                        f.add(n, v);
                        haveSize = true;
                        pageSize = v;
                    } else paras[n] = v;
                });
                if (!haveSize && o.isPageMode) {
                    pageSize = o.para("pageSize") || 20;
                    f.add("pageSize", pageSize); //如果没有从参数中得到pageSize,则使用手动设置的属性值
                }
                if (p.paras) { //额外参数
                    util.object.eachValue(p.paras, function (n, v) {
                        paras[n] = v;
                    });
                }
                f.add("paras", paras);
                f.success = function (r) {
                    o.data = r;
                    bindTable(o, r);
                    syncUrl(o, pageIndex, pageSize, paras);
                    if (r.page && (r.page.index > 0 && r.page.index >= r.page.pageCount)) { //超出最大页码，那么直接读取第1页(下标为0)
                        o.load({ index: 0 });
                    }
                    //else
                    //    o.execEvent("onload", [o, r]); onload事件统一由 绘制完table后触发
                }
                initAsyncEvent(o, f);
                var url = p.url || o.para("url") || window.location.href;
                f.submit({ url: url, action: o.para("action") });
            }

            o.bind = function (r, paras) {
                this.data = r;
                bindTable(this, r);
                syncUrl(o, null, null, paras);
            }

            o.empty = function () {
                emptyTable(this);
            }

            o.page = function () {
                return this.data ? this.data.page : null;
            }

            function getPage(o, p) { //得到page信息
                if (!p) p = o.page() || {};
                p.index = p.index || 0;
                if (p.index < 0) p.index = 0;
                if (o.page()) {//如果之前加载过数据，则根据上次计算结果，得到页面总数，并进行对比
                    var pc = o.page().count;//count值每次都会更新，因此无须担心页面最大值的真实性
                    if (p.index >= pc) p.index = (pc - 1) < 0 ? 0 : (pc - 1);
                }
                return p;
            }

            o.next = function () {
                var o = this, p = o.page();
                if (!p) { o.load(); return; }
                p.index++;
                o.load(p);
            };
            o.prev = function () {
                var o = this, p = o.page();
                if (!p) { o.load(); return; }
                var pi = p.index - 1;
                p.index = pi >= 0 ? pi : 0;
                o.load(p);
            };

            o.asyncEvent = function (e) { //设置或获取异步事件
                if (e) this.para("asyncEvent", e);
                else return this.para("asyncEvent");
            }

            function initAsyncEvent(o, $f) {
                var evts = o.asyncEvent();
                for (var e in evts)
                    $f[e] = evts[e];//赋予回调事件
            }

            function AsyncEvent(o) { //内置的异步事件
                var _o = o;
                this.beforeSend = function () {
                    Metronic.blockUI({
                        message: "数据加载中...",
                        target: _o.ent(),
                        overlayColor: 'none',
                        cenrerY: true,
                        boxed: true
                    });
                }
                this.complete = function () {
                    Metronic.unblockUI(_o.ent());
                }
                this.error = function (msg) {
                    bootbox.alert({
                        buttons: {
                            ok: {
                                label: '确定',
                            }
                        },
                        message: msg,
                        title: "错误提示"
                    });
                }
            }

            $component.util.initQuery(o);

            function syncUrl(o, pageIndex, pageSize, paras) {
                paras = paras || {};
                if (!o.para("syncUrl")) return; //没有开启同步url地址功能
                var view = o.para("_view");
                if (!view) {
                    view = new $.util.location.view("dataPage", function (state) {
                        o.execEvent("onrestore", [o, state]);
                    });
                    o.para("_view", view);
                }
                if (pageIndex) paras["pageIndex"] = pageIndex;
                if (pageSize) paras["pageSize"] = pageSize;
                view.update(paras);
            }

            initCommon(o);//初始化与数据相关行为
            init(o);

            function init(o) {
                var e = o.asyncEvent();
                if (!e) {
                    e = new AsyncEvent(o);
                    o.asyncEvent(e);
                }
            }

            function emptyTable(o) {
                bindTable(o, { rows: [], dataCount: 0 });
            }

            function bindTable(o, d) {
                if (o.table) { o.table.destroy(); } //一定要先破坏旧表格
                var tb = o.find("tbody[data-name='dataContent']"), ptb = $$(tb[0]);
                if (d.rows.length > 0 && ptb.__temp) {  //由于databind组件会为loops标签预留至少一项dom,以确保工作正常，但是这与datatable组件有冲突，无法显示没有数据的状态
                    var tcs = ptb.__temp.children();    //所以在绑定前，我们自己恢复预留项
                    if (tcs.length > 0) {
                        tb.append(tcs[0]);
                    }
                }
                ptb.bind(d);
                if (d.rows.length == 0) {              //在绑定后，自己移除预留项
                    if (!ptb.__temp) ptb.__temp = $("<div style='display:none;'></div>");
                    ptb.__temp.append(tb.children()[0]);
                }


                $$.wrapper.metronic.initAjax(o); //用于ajax请求后，文档结构发生变化，初始化新的内容
                var j = o.getJquery().find("table[data-name='dataTable']");
                //移除选中状态
                j.find("tr.active").removeClass("active");
                j.find("div.checker span.checked input[type='checkbox']").attr("checked", false);
                j.find("div.checker span.checked").removeClass("checked");

                var config = {
                    "lengthMenu": [
                        [5, 15, 30, -1],
                        [5, 15, 30, "全部"] // change per page values here
                    ],
                    // set the initial value
                    //"dom": "<'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>>t<'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>", // datatable layout
                    "dom": "<'table-scrollable't><'row'<'col-md-5 col-sm-12'li><'col-md-7 col-sm-12'p>>",
                    "pageLength": 15,
                    "autoWidth": false,
                    "pagingType": "bootstrap_full_number",
                    "language": {
                        "search": "本地搜索：",
                        "lengthMenu": "每页 _MENU_ 条",
                        "paginate": {
                            "previous": "上一页",
                            "next": "下一页",
                            "last": "末页",
                            "first": "首页"
                        },
                        "info": "<span class='seperator'>&nbsp;</span>显示第 _START_ 到 _END_ 条 共计 _TOTAL_ 项",
                        "emptyTable": "暂无数据",
                        "sInfoEmpty": "共计 0 项",
                        "infoFiltered": "(从 _MAX_ 项中过滤)",
                        "zeroRecords": "没有找到匹配的数据",
                    },
                    "drawCallback": function () {
                        o.execEvent("load", [o, o.data]);
                    }
                }

                if (o.para("page") === false) {
                    config.dom = "<'table-scrollable't><'row'>";
                    config.pageLength = -1;
                }

                fillConfig(o, config);
                o.table = j.DataTable(config);
                j.off('change', 'tbody tr .checkboxes');
                j.on('change', 'tbody tr .checkboxes', function () {
                    $(this).parents('tr').toggleClass("active");
                });

                j.find('.group-checkable').off('change');
                j.find('.group-checkable').on('change', { table: j }, function (e) {
                    var checked = jQuery(this).is(":checked");
                    var cks = e.data.table.find(".checkboxes");
                    cks.each(function () {
                        if (checked) {
                            $(this).attr("checked", true);
                            $(this).parents('tr').addClass("active");
                        } else {
                            $(this).attr("checked", false);
                            $(this).parents('tr').removeClass("active");
                        }
                    });
                    jQuery.uniform.update(cks);
                });
            }
        }
    }
    //#endregion

    //#region 数据翻页列表
    //包装
    var Datatable = function (p,sp) {
        var tableOptions; // main options
        var dataTable; // datatable object
        var table; // actual table jquery object
        var tableContainer; // actual table container object
        var tableWrapper; // actual table wrapper jquery object
        var tableInitialized = false;
        var ajaxParams = {}; // set filter mode
        var the;
        var _para = p || {};//自定义额外参数
        var _sp = sp || {}; //系统参数

        var countSelectedRecords = function () {
            var selected = $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).size();
            var text = tableOptions.dataTable.language.metronicGroupActions;
            if (selected > 0) {
                $('.table-group-actions > span', tableWrapper).text(text.replace("_TOTAL_", selected));
            } else {
                $('.table-group-actions > span', tableWrapper).text("");
            }
        };

        return {

            //main function to initiate the module
            init: function (options) {

                if (!$().dataTable) {
                    return;
                }

                the = this;

                // default settings
                options = $.extend(true, {
                    src: "", // actual table  
                    filterApplyAction: "filter",
                    filterCancelAction: "filter_cancel",
                    resetGroupActionInputOnSuccess: true,
                    loadingMessage: '数据加载中...',
                    dataTable: {
                        //"dom": "<'row'<'col-xs-12 col-sm-8'<'table-group-actions'>><'col-xs-12 col-sm-4'l>r><'table-scrollable't><'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>", // datatable layout
                        "dom": "<'table-scrollable't><'row'<'col-md-5 col-sm-12'li><'col-md-7 col-sm-12'p>>", // datatable layout
                        "pageLength": 15, // default records per page
                        "language": { // language settings
                            // metronic spesific
                            "metronicGroupActions": "_TOTAL_ records selected:  ",
                            "metronicAjaxRequestGeneralError": "无法完成请求，请检查您的网络连接。",

                            // data tables spesific
                            "lengthMenu": "每页 _MENU_ 条",
                            "info": "<span class='seperator'>&nbsp;</span>显示第 _START_ 到 _END_ 条 共计 _TOTAL_ 项",
                            "sInfoEmpty": "共计 0 项",
                            "emptyTable": "暂无数据",
                            "zeroRecords": "没有找到匹配的数据",
                            "paginate": {
                                "previous": "上一页",
                                "next": "下一页",
                                "last": "末页",
                                "first": "首页",
                                "page": "页码",
                                "pageOf": "/"
                            }
                        },
                        "orderCellsTop": true,
                        "pagingType": "bootstrap_full_number", // pagination type(bootstrap, bootstrap_full_number or bootstrap_extended)
                        "autoWidth": false, // disable fixed width and enable fluid table
                        "processing": false, // enable/disable display message box on record load
                        "serverSide": true, // enable/disable server side ajax loading
                        "ajax": { // define ajax settings
                            "url": "", // ajax URL
                            "type": "POST", // request type
                            "timeout": 10000,
                            "data": function (data) { // add request parameters before submit
                                var pageSize = data.length, pageIndex = parseInt(data.start / pageSize);
                                var prevData = $$(tableContainer).data;
                                //以下重定位，可以解决页尺寸改变时引出的页码BUG
                                if (prevData && prevData.page && prevData.page.size != pageSize) {
                                    if (prevData.page.size < pageSize) //新页尺码大于上次设定
                                    {
                                        var pageCount = parseInt(prevData.dataCount / pageSize);
                                        if (prevData.dataCount % pageSize > 0) pageCount++; //根据数据总数（这个一般不会变），计算本次请求最大页码数
                                        if (prevData.page.index >= pageCount) { //上一次定位的页码如果大于本次请求最大页码数，那么页码自动跳到本次请求最大页码数
                                            pageIndex = pageCount - 1;
                                        }
                                    }
                                }
                                var newData = {};
                                if (_sp.xaml) {
                                    //以xaml形式解析
                                    var master = _sp.master;
                                    var qp = master.queryParas = {}; //此处为组件设置了查询参数，在将组件作为视图元素提交时，会把该数据提交
                                    qp["pageSize"] = pageSize;
                                    qp["pageIndex"] = pageIndex;
                                    qp["paras"] = {};
                                    if (_para.paras) { //额外参数
                                        util.object.eachValue(_para.paras, function (n, v) {
                                            qp.paras[n] = v;
                                        });
                                    }
                                    //以下是根据xaml引擎的服务器端数据接口，对接的数据格式
                                    newData["component"] = _sp.component;//组件名称
                                    newData["action"] = _sp.action;//组件名称
                                    var view = new $$view(master, _sp.viewName);
                                    newData["argument"] = view.get();
                                }
                                else {
                                    newData["pageSize"] = pageSize;
                                    newData["pageIndex"] = pageIndex;

                                    //收集内部参数
                                    var paras = {};
                                    tableContainer.find("*[id^='filter.']").each(function () {
                                        var t = $(this), id = t.attr("id"), name = id.split('.')[1];
                                        paras[name] = t.proxy().get();
                                    });

                                    //收集外部参数
                                    $$(tableContainer).query().each(function (i, c) {
                                        var n = c.getName(), v = c.get();
                                        paras[n] = v;
                                    });

                                    if (_para.paras) { //额外参数
                                        util.object.eachValue(_para.paras, function (n, v) {
                                            paras[n] = v;
                                        });
                                    }

                                    newData["paras"] = paras;
                                }

                                //稍后扩展排序参数
                                Metronic.blockUI({
                                    message: tableOptions.loadingMessage,
                                    target: tableContainer,
                                    overlayColor: 'none',
                                    cenrerY: true,
                                    boxed: true
                                });
                                return $$.util.json.serialize(newData); //返回一个新的数据
                            },
                            "dataSrc": function (res) { // Manipulate the data returned from the server
                                if (res.customActionMessage) {
                                    Metronic.alert({
                                        type: (res.customActionStatus == 'OK' ? 'success' : 'danger'),
                                        icon: (res.customActionStatus == 'OK' ? 'check' : 'warning'),
                                        message: res.customActionMessage,
                                        container: tableWrapper,
                                        place: 'prepend'
                                    });
                                }

                                if (res.customActionStatus) {
                                    if (tableOptions.resetGroupActionInputOnSuccess) {
                                        $('.table-group-action-input', tableWrapper).val("");
                                    }
                                }

                                if ($('.group-checkable', table).size() === 1) {
                                    $('.group-checkable', table).attr("checked", false);
                                    $.uniform.update($('.group-checkable', table));
                                }

                                if (tableOptions.onSuccess) {
                                    tableOptions.onSuccess.call(undefined, the);
                                }

                                Metronic.unblockUI(tableContainer);
                                var po = $$(tableContainer);
                                po.data = res;
                                return po.getTempHtml(res);
                            },
                            "error": function (xhr) { // handle general connection errors
                                var error = $$.ajax.util.getErrorMessage(xhr);
                                if (tableOptions.onError) {
                                    tableOptions.onError.call(undefined, the);
                                }
                                Metronic.unblockUI(tableContainer);

                                $$bootbox.alert(error);

                                //Metronic.alert({
                                //    type: 'danger',
                                //    icon: 'warning',
                                //    message: error,
                                //    container: tableWrapper,
                                //    place: 'prepend'
                                //});
                            }
                        },

                        "drawCallback": function (oSettings) { // run some code on table redraw
                            if (tableInitialized === false) { // check if table has been initialized
                                tableInitialized = true; // set table initialized
                                table.show(); // display table
                            }
                            Metronic.initUniform($('input[type="checkbox"]', table)); // reinitialize uniform checkboxes on each table reload
                            countSelectedRecords(); // reset selected records indicator

                            // callback for ajax data load
                            if (tableOptions.onDataLoad) {
                                tableOptions.onDataLoad.call(undefined, the);
                            }
                        }
                    }
                }, options);

                tableOptions = options;

                // create table's jquery object
                table = $(options.src);

                tableContainer = table.parents(".table-container");

                // apply the special class that used to restyle the default datatable
                //var tmp = {
                //    sWrapper: $.fn.dataTableExt.oStdClasses.sWrapper,
                //    sFilterInput: $.fn.dataTableExt.oStdClasses.sFilterInput,
                //    sLengthSelect: $.fn.dataTableExt.oStdClasses.sLengthSelect
                //};
                //$.fn.dataTableExt.oStdClasses.sWrapper = $.fn.dataTableExt.oStdClasses.sWrapper + " dataTables_extended_wrapper";
                //$.fn.dataTableExt.oStdClasses.sFilterInput = "form-control input-small input-sm input-inline";
                //$.fn.dataTableExt.oStdClasses.sLengthSelect = "form-control input-xsmall input-sm input-inline";


                table.off('xhr.dt');
                table.on('xhr.dt', function (e, tb, json) {
                    json["recordsTotal"] = json["dataCount"];
                    json["recordsFiltered"] = json["dataCount"];
                });

                // initialize a datatable
                dataTable = table.DataTable(options.dataTable);

                //// revert back to default
                //$.fn.dataTableExt.oStdClasses.sWrapper = tmp.sWrapper;
                //$.fn.dataTableExt.oStdClasses.sFilterInput = tmp.sFilterInput;
                //$.fn.dataTableExt.oStdClasses.sLengthSelect = tmp.sLengthSelect;

                // get table wrapper
                tableWrapper = table.parents('.dataTables_wrapper');

                // build table group actions panel
                if ($('.table-actions-wrapper', tableContainer).size() === 1) {
                    $('.table-group-actions', tableWrapper).html($('.table-actions-wrapper', tableContainer).html()); // place the panel inside the wrapper
                    $('.table-actions-wrapper', tableContainer).remove(); // remove the template container
                }


                table.off('change', 'tbody tr .checkboxes');
                table.on('change', 'tbody tr .checkboxes', function () {
                    $(this).parents('tr').toggleClass("active");
                    countSelectedRecords();
                });

                table.find('.group-checkable').off('change');
                table.find('.group-checkable').on('change',{table: table}, function (e) {
                    var checked = jQuery(this).is(":checked");
                    var cks = e.data.table.find(".checkboxes");
                    cks.each(function () {
                        if (checked) {
                            $(this).attr("checked", true);
                            $(this).parents('tr').addClass("active");
                        } else {
                            $(this).attr("checked", false);
                            $(this).parents('tr').removeClass("active");
                        }
                    });
                    jQuery.uniform.update(cks);
                    countSelectedRecords();
                });

                return dataTable;
            },
            getSelectedRowsCount: function () {
                return $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).size();
            },

            getSelectedRows: function () {
                var rows = [];
                $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).each(function () {
                    rows.push($(this).val());
                });

                return rows;
            },
            getDataTable: function () {
                return dataTable;
            },

            getTableWrapper: function () {
                return tableWrapper;
            },

            gettableContainer: function () {
                return tableContainer;
            },

            getTable: function () {
                return table;
            }

        };

    };

    $$.wrapper.metronic.datagrid = function () {
        this.give = function (o) {
            $o.callvir(this, $component.ui, "give", o);
            var _table = o.getJquery().find("table[data-name='dataTable']"), rowTemplate = _table.find("tbody > tr").first();
            var _tempContainer = $("<div></div>");
            function outRowTemplate() { _tempContainer.append(rowTemplate); }
            function inRowTemplate() { _table.append(rowTemplate); }
            outRowTemplate();

            o.getTempHtml = function (r) { //获取html格式的临时数据
                if (r.rows.length == 0) return [];
                var tbody = $$(_table.children("tbody")[0]), tbj = tbody.getJquery();
                var list = [];
                r.rows.each(function () {
                    var t = [];
                    rowTemplate.children("td").each(function () {
                        t.push("&nbsp;");
                    });
                    list.push(t);
                });
                outRowTemplate();
                tbj.empty();
                return list;
            }

            $component.util.initQuery(o);

            o.empty = function () {
                //暂未实现
            }

            o.getQueryParas = function () { //该方法是xaml引擎下需要的
                return this.queryParas || {};
            }

            o.queryParas = null;

            o.load = function (p) {
                var o = this;
                if (o.table) { o.table.destroy(); } //一定要先破坏旧表格
                var ajaxUrl = (o.para("url") || location.pathname);
                var sp = {};
                if (o.para("xaml")) { //此标记的意思是用xaml的解析机制来实现
                    //xaml引擎加载机制
                    sp.xaml = true;
                    sp.component = o.para("name");//组件名称
                    sp.viewName = o.para("view"); //组件相关的视图名称
                    sp.master = o;
                    sp.action = "Load";//在xaml机制下,action是固定值
                } else {
                    //老版本加载机制
                    ajaxUrl += $.query.set("$PostAction", o.para("action"));
                }
                var grid = new Datatable(p, sp);
                var config = {
                    src: _table,
                    onSuccess: function (grid) {
                        // execute some code after table records loaded
                    },
                    onError: function (grid) {
                        // execute some code on network or other general error  
                    },
                    onDataLoad: function (grid) {
                        //由于是html拼接，所以需要重新赋予data值
                        if (o.data.rows.length == 0) return;
                        var tbody = $$(_table.children("tbody")[0]), tbj = tbody.getJquery();
                        tbj.empty();
                        inRowTemplate();//加入模板
                        tbody.bind(o.data);
                        $$.wrapper.metronic.initAjax(tbj); //用于ajax请求后，文档结构发生变化，初始化新的内容

                        //移除选中状态
                        tbj.find("tr.active").removeClass("active");
                        tbj.find("div.checker span.checked input[type='checkbox']").attr("checked", false);
                        tbj.find("div.checker span.checked").removeClass("checked");

                        o.execEvent("onload", [o, o.data]);
                    },
                    loadingMessage: '数据加载中...',
                    dataTable: { // here you can define a typical datatable settings from http://datatables.net/usage/options 

                        // Uncomment below line("dom" parameter) to fix the dropdown overflow issue in the datatable cells. The default datatable layout
                        // setup uses scrollable div(table-scrollable) with overflow:auto to enable vertical scroll(see: assets/global/scripts/datatable.js). 
                        // So when dropdowns used the scrollable div should be removed. 
                        //"dom": "<'row'<'col-md-8 col-sm-12'pli><'col-md-4 col-sm-12'<'table-group-actions pull-right'>>r>t<'row'<'col-md-8 col-sm-12'pli><'col-md-4 col-sm-12'>>",

                        "bStateSave": false, // save datatable state(pagination, sort, etc) in cookie.

                        "lengthMenu": [
                            [5, 15, 30],
                            [5, 15, 30] // change per page values here
                        ],
                        "pageLength": 15, // default record count per page
                        "ajax": {
                            "url": ajaxUrl, // ajax source
                        }
                    }
                };
                fillConfig(o, config.dataTable);
                o.table = grid.init(config);

                // handle group actionsubmit button click
                grid.getTableWrapper().on('click', '.table-group-action-submit', function (e) {
                    e.preventDefault();
                    var action = $(".table-group-action-input", grid.getTableWrapper());
                    if (action.val() != "" && grid.getSelectedRowsCount() > 0) {
                        grid.setAjaxParam("customActionType", "group_action");
                        grid.setAjaxParam("customActionName", action.val());
                        grid.setAjaxParam("id", grid.getSelectedRows());
                        grid.getDataTable().ajax.reload();
                        grid.clearAjaxParams();
                    } else if (action.val() == "") {
                        Metronic.alert({
                            type: 'danger',
                            icon: 'warning',
                            message: 'Please select an action',
                            container: grid.getTableWrapper(),
                            place: 'prepend'
                        });
                    } else if (grid.getSelectedRowsCount() === 0) {
                        Metronic.alert({
                            type: 'danger',
                            icon: 'warning',
                            message: 'No record selected',
                            container: grid.getTableWrapper(),
                            place: 'prepend'
                        });
                    }
                });
            }

            initCommon(o);            
        }
    }

    //#endregion

    function initCommon(o) { //datalist和datagrid共有的行为

        o.scriptElement = function () { //获得脚本元素的元数据
            var my = this, v = my.get(), d = my.getData();
            var p = new function () {
                this.validate = function () {
                    return my.validate();
                }
            }

            var md = {};
            md.selectedItems = v;
            md.data = d;
            md.query = my.getQueryParas ? my.getQueryParas() : {}; //获取查询参数

            return {
                id: my.getJquery().prop("id") || '',
                metadata: md,
                eventProcessor: p
            };
        }


        o.get = function () {
            var vl = [];
            this.getJquery().find('tbody tr .checkboxes').each(function () {
                var ck = $(this);
                if (ck.is(":checked")) {
                    var tr = ck.parents("tr")[0];
                    if (tr) {
                        var d = $$(tr).data;
                        if (d) vl.push(d);
                    }
                }
            });
            return vl;
        }

        o.getData = function () {
            var vl = [];
            this.getJquery().find('tbody tr').each(function () {
                var tr = $(this), trp = tr.proxy();
                var d = trp.data || {}; //获取绑定的数据

                var f = new $$.form();
                f.collect(tr[0]);
                var t = f.get();
                util.object.eachValue(t, function (n, v) {
                    d[n] = v;
                });

                vl.push(d);
            });
            return vl;
        }

        o.setData = function (vl) {
            this.getJquery().find('tbody tr').each(function (i) {
                var tr = $(this), f = new $$.form(), d = vl[i];
                f.collect(tr[0]);
                f.set(d);
            });
        }

        o.validate = function () {
            var result = new ValidateResult();
            this.getJquery().find('tbody tr').each(function () {
                var tr = $(this);
                var f = new $$.form();
                f.collect(tr[0]);
                var t = f.validate(true);
                result.add(t);
            });
            return result;
        }

        o.reset = function () {
            this.getJquery().find('tbody tr').each(function () {
                var tr = $(this);
                var f = new $$.form();
                f.reset();
            });
        }
    }

    function ValidateResult() { //class
        var _items = [];
        this.add = function (t) { _items.push(t); };
        this.status = function () {
            for (var i = 0; i < _items.length; i++) {
                var s = _items[i].status();
                if (s == "error") return s;
            }
            return "success";
        }
        this.message = function () {
            var t = [];
            for (var i = 0; i < _items.length; i++) {
                var m = _items[i].message();
                t.push(m);
            }
            return t.join(',');
        }
        this.satisfied = function () {
            for (var i = 0; i < _items.length; i++) {
                if (!_items[i].satisfied()) return false;
            }
            return true;
        }
    }

});