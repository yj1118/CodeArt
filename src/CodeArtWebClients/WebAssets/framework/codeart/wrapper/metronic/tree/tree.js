$$.createModule("Wrapper.metronic.tree", function (api, module) {
    api.requireModules(["Wrapper.metronic"]);
    api.requireModules(["Component"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component, $request = $$.ajax.request;

    var $tree = $$.wrapper.metronic.tree = function (painter) {
        $o.inherit(this, $component.ui, painter);
        var my = this;
        this.give = function (o) {
            $o.callvir(this, $component.ui, "give", o);
            //$component.util.initQuery(o);
            var _o = o;
            o.load = function (cb) {
                load(this, cb)
            };
            o.scriptElement = function () { //获得脚本元素的元数据
                var my = this, v = my.get();
                return {
                    id: my.getJquery().prop("id") || '',
                    metadata: { value: v }
                };
            }
            o.get = function () {
                var oj = this.getJquery();
                return oj.data("tree-value") || {};
            }
            o.open = function () {
                var oj = this.getJquery();
                oj.jstree("open_all");
            }
            o.select = function (ids) {
                var oj = this.getJquery();
                oj.jstree("select_node", ids);
            }
            o.selectIndex = function (i) {
                var oj = this.getJquery();
                var data = oj.jstree(true).settings.core.data;
                if (i == 0) {
                    this.select(data.id);
                }
                else {
                    var c = data.children, d = c[i];
                    if (d) {
                        this.select(d.id);
                    }
                }
            }
            init(o);

            function init(o) {
                var oj=o.getJquery(), setting = {};
                //setting["plugins"] = ["wholerow", "checkbox", "types"];
                setting["plugins"] = ["wholerow", "types"];
                setting["core"] = {
                    "multiple": false,
                    "themes": { "responsive": false },
                    "types": {
                        "default": { "icon": "fa fa-folder icon-state-warning icon-lg" },
                        "file": { "icon": "fa fa-file icon-state-warning icon-lg" }
                    }
                };
                oj.jstree(setting);
                oj.bind("activate_node.jstree", function (obj, e) {
                    var data = e.node.original;
                    oj.data("tree-value", data);
                    var component = o.para("name");//组件名称
                    var viewName = o.para("view"); //组件相关的视图名称
                    var view = new $$view(o, viewName);
                    view.submit({ component: component, action: "ChangedValue" }, { ignoreValidate: true });//在xaml机制下,action是固定值
                });
                oj.bind("refresh.jstree", function (event, data) {
                    o.open();
                    var t;
                    if (t = o.jsTreeCallback) t();
                });
            }

            //o.add = function (parentNode,data) {
            //    this.tree.addNodes(parentNode, data);
            //}
            //o.select = function (node) {
            //    this.tree.selectNode(node);
            //}
            //o.update = function (node) {
            //    this.tree.updateNode(node);
            //}
            //o.remove = function (node) {
            //    this.tree.removeNode(node);
            //}
            //o.get = function () {
            //    var nodes = this.tree.getNodes();
            //    return toData(nodes[0]);
            //}

            function load(o, cb) {
                //o.parentValue = key; //该值会传递到服务器上
                o.jsTreeCallback = cb;
                var component = o.para("name");//组件名称
                var viewName = o.para("view"); //组件相关的视图名称
                var view = new $$view(o, viewName);
                view.success = function (r) {
                    process(o, r);
                }
                view.submit({ component: component, action: "LoadData" }, { ignoreValidate: true });//在xaml机制下,action是固定值
            }

            function process(o, r) {
                var oj = o.getJquery(), setting = oj.jstree(true).settings;
                setting.core["data"] = r;
                oj.jstree("refresh");
            }

            function status(n) {
                //var loading = my.find(o, "loading"), core = my.find(o, "core");
                //switch (n) {
                //    case "loading": {
                //        loading.css("display", "block");
                //        core.css("display", "none");
                //    }
                //        break;
                //    case "empty": {
                //        loading.css("display", "none");
                //        core.css("display", "none");
                //    }
                //        break;
                //    case "core": {
                //        loading.css("display", "none");
                //        core.css("display", "block");
                //    }
                //        break;
                //}
            }

        }
    }

    $tree.painter = function (methods) {
        $o.inherit(this, $$.component.painter, methods);
    }

    $tree.create = function () {
        return new $tree(new $tree.painter());
    }
});