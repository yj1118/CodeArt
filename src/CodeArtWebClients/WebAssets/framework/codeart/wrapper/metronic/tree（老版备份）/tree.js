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
            $component.util.initQuery(o);
            var _o = o;
            o.load = function () { load() };
            o.add = function (parentNode,data) {
                this.tree.addNodes(parentNode, data);
            }
            o.select = function (node) {
                this.tree.selectNode(node);
            }
            o.update = function (node) {
                this.tree.updateNode(node);
            }
            o.remove = function (node) {
                this.tree.removeNode(node);
            }
            o.get = function () {
                var nodes = this.tree.getNodes();
                return toData(nodes[0]);
            }

            function toData(n) {
                var d = { id: n.id, name: n.name };
                var len = n.children ? n.children.length : 0;
                if (len > 0) {
                    var c = d.children = [];
                    for (var i = 0; i < len; i++) {
                        var cd = toData(n.children[i]);
                        c.push(cd);
                    }
                }
                return d;
            }

            function load() {
                var url = o.para("url") || location.pathname;
                var action = o.para("action");

                var req = new $$request(), paras = {};
                o.query().each(function (i, c) {
                    var n = c.name, v = c.get();
                    paras[n] = v;
                });
                req.add("paras", paras);

                req.success = function (r) {
                    status("core");
                    process(r);
                }
                req.beforeSend = function () {
                    status("loading");
                }

                req.post({ url: url, action: action });
            }

            function process(r) {
                var core = my.find(o, "core");
                var setting = {
                    treeId:'',
	                view: {
	                    selectedMulti: false
	                },
	                edit: {
	                    showRemoveBtn: false,
	                    showRenameBtn: false
	                },
	                callback:{}
                };

                if (o.para("add")) {
                    var at = o.para("add");
                    setting.view.addHoverDom = function (treeId, treeNode) {
                        var sObj = $("#" + treeNode.tId + "_span");
                        if (treeNode.editNameFlag || $("#addBtn_" + treeNode.tId).length > 0) return;
                        var addStr = "<span class='button add' id='addBtn_" + treeNode.tId
                            + "' title='"+ at.text +"' onfocus='this.blur();'></span>";
                        sObj.after(addStr);
                        var btn = $("#addBtn_" + treeNode.tId);
                        btn.data("obj", _o);
                        btn.data("action", at.before);
                        if (btn) {
                            btn.on("click", function (e) {
                                var o = $(this).data("obj");
                                var action = $(this).data("action");
                                var data = action(treeNode);
                                if(data) o.add(treeNode, data);
                                return false;
                            });
                        }
                    };
                    setting.view.removeHoverDom = function (treeId, treeNode) {
                        $("#addBtn_" + treeNode.tId).unbind().remove();
                    };
                }

                if (o.para("update")) {
                    var ut = o.para("update");
                    setting.edit.enable = true;
                    setting.edit.showRenameBtn = true;
                    setting.edit.renameTitle = ut.text;
                    setting.callback.beforeEditName = function(treeId, treeNode) {
                        //o.select(treeNode);
                        var node = ut.before(treeNode);
                        if (node) o.update(node);
                        return false;
                    }
                }

                if (o.para("delete")) {
                    var dt = o.para("delete");
                    setting.edit.enable = true;
                    setting.edit.showRemoveBtn = true;
                    setting.edit.removeTitle = dt.text;
                    setting.callback.beforeRemove = function (treeId, treeNode) {
                        return dt.before(treeNode);
                    }
                }

                var root = mapData(r);
                o.tree = $.fn.zTree.init(core.getJquery(), setting, [root]);
            }

            function mapData(r) {
                if (empty(r.open)) r.open = true;
                if (!r.children) return r;
                var len = r.children.length;
                for (var i = 0; i < len; i++) {
                    var c = r.children[i];
                    mapData(c);
                }
                return r;
            }

            function status(n) {
                var loading = my.find(o, "loading"), core = my.find(o, "core");
                switch (n) {
                    case "loading": {
                        loading.css("display", "block");
                        core.css("display", "none");
                    }
                        break;
                    case "empty": {
                        loading.css("display", "none");
                        core.css("display", "none");
                    }
                        break;
                    case "core": {
                        loading.css("display", "none");
                        core.css("display", "block");
                    }
                        break;
                }
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