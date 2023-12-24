$$.createModule("metronic.vis", function (api, module) {
    api.requireModules(["metronic"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;

    var $vis = $$.metronic.vis = function () {
        var my = this;

        my.give = function (o) {
            var my = this;
            o.get = function () {
                var v = this.getValue();
                return util.json.serialize(v);
            }

            o.set = function (code) {
                var v = util.json.deserialize(code);
                this.setValue(v);
            }

            o.setValue = function (v) {
                var o = this, oj = o.getJquery(), e = o._container;
                var ops = o._visOptions;
                var data = getData(v);
                o._data = data;
                if (!o._visNT)
                    o._visNT = new vis.Network(e, data, ops);
                else {
                    o._visNT.setData(data);
                }
                this.changed();
                return o._visNT;
            }

            o.getValue = function () { //将data转换为value
                var data = this._data;
                var v = {nodes:[],edges:[]};
                var t = data.nodes._data;
                for (var e in t) {
                    v.nodes.push(t[e]);
                }

                t = data.edges._data;
                for (var e in t) {
                    v.edges.push(t[e]);
                }

                return v;
            }

            o.updateEdge = function (edgeId, e) {
                var nt = this._visNT,data = this._data;
                nt.clustering.updateEdge(edgeId, e); //该方法不会同步数据
                var oe = data.edges._data; //将数据手工写入
                for (var key in oe) {
                    if (key == edgeId) {
                        var value = oe[key];
                        util.object.eachValue(e, function (n, v) {
                            value[n] = v;
                        });
                        break;
                    }
                }
            }

            o.getEdge = function (edgeId) {
                var nt = this._visNT, data = this._data;
                var oe = data.edges._data; //将数据手工写入
                for (var key in oe) {
                    if (key == edgeId) {
                        return oe[key];
                    }
                }
            }

            o.reset = function () {
                this.setValue({});
                this._addEdgeBtn.hide();
                this._removeBtn.hide();
                this._edgeTipBtn.hide();
            }

            o.refresh = function () {
                this._visNT.redraw();
            }

            o.visHeight = function (h) {
                var o = this, oj = o.getJquery();
                oj.height(h);
                o.refresh();
            }

            o.nodeTypes = function (types) { //设置节点类型，所有新增的节点只能使用指定的类型
                var modal = this._addNodeModal;
                if (!modal) return;
                var t = modal.nodeTypes;
                t.options(types);
            }

            o.addNode = function (n) {
                var v = this.getValue();
                if (v.nodes.length == 0) {
                    v.nodes.push(n);
                    this.setValue(v);
                } else {
                    this._visNT.body.data.nodes.add([n]);
                    this.changed();
                }
                this.refresh();
            }

            o.changed = function () {
                var v = this.getValue(), ns = v.nodes;
                var ade = this._addEdgeBtn;
                if (ade) {
                    if (ns.length > 1) ade.css("display", "inline-block");
                    else ade.hide();
                }
            }

            o.getNodeId = function () {
                var v = this.getValue();
                var ns = v.nodes;
                var l = ns.select((t) => t.id);
                return l.length == 0 ? 1 : l.max() + 1;
            }
           
            init(o);
        }

        function init(o) {
            var oj = o.getJquery(), disabled = o.attr("data-disabled").toLower() == "true";

            var ops = {
                interaction: { hover: true },
                manipulation: {
                    enabled: false
                },
                edges: {
                    arrows: {
                        to: { enabled: true, scaleFactor: 1, type: 'arrow' },
                    }
                },
                physics: {
                    enabled: true,
                    barnesHut: {
                        centralGravity:0.3,//将元素拉回中心
                        springLength: 200 //数值越长，连接的两个节点之间距离越大
                    },
                }
            };

            var _is = o.attr("data-initVis") || o.attr("initVis");
            if (_is) {
                var func;
                eval("func=" + _is+";");
                func(ops);
            }

            o._visOptions = ops;

            o._container = oj.find(".ca-m-vis")[0];
            var nt = o.setValue({});

            if (!disabled) {
                //新增节点
                var addNodeModalId = oj.attr("data-addNodesModalId");
                var addNodeModal = o._addNodeModal = $("#" + addNodeModalId).proxy();
                addNodeModal.nodeTypes = addNodeModal.find("[data-nodeTypes]").proxy();

                //添加节点
                oj.find("[data-addNodes]").on("click.vis", function () {
                    var t = addNodeModal.nodeTypes;
                    t.reset();
                    addNodeModal.open();
                });

                addNodeModal.find("[data-ok]").on("click.vis.submitAddNodes", function () {
                    var t = addNodeModal.nodeTypes;
                    var vs = t.get(true);
                    vs.each(function () {
                        var v = this;
                        o.addNode({ id: o.getNodeId(), label: v.text });
                    });
                    addNodeModal.close();
                });

                //连线批注
                var setEdgeModalId = oj.attr("data-setEdgeModalId");
                var setEdgeModal = o._setEdgeModal = $("#" + setEdgeModalId).proxy();
                setEdgeModal.content = setEdgeModal.find("[data-content]").proxy();

                var edgeTipBtn = oj.find("[data-edgeTip]");
                o._edgeTipBtn = edgeTipBtn;
                edgeTipBtn.on("click.vis", function () {
                    var t = setEdgeModal.content;
                    t.reset();

                    var es = edgeTipBtn.edges;
                    es.each(function () {
                        var edgeId = this;
                        var g = o.getEdge(edgeId);
                        if(g) t.set(g.label||'');
                    });
                    setEdgeModal.open();
                });

                setEdgeModal.find("[data-ok]").on("click.vis.submitEdgeTip", function () {
                    var t = setEdgeModal.content;
                    var cont = t.get();
                    var es = edgeTipBtn.edges;
                    es.each(function () {
                        var edgeId = this;
                        o.updateEdge(edgeId, { label: cont });
                    });
                    o.changed();
                    setEdgeModal.close();
                });


                //删除选中的对象
                var removeBtn = oj.find("[data-remove]");
                o._removeBtn = removeBtn;
                removeBtn.on("click.vis.remove", function () {
                    var nt = o._visNT;
                    nt.deleteSelected();
                    removeBtn.hide();
                    o.changed();
                });

                //选中节点
                nt.on("selectNode", function (params) {
                    tidyRemoveBtn(params, removeBtn);
                });
                nt.on("deselectNode", function (params) {
                    tidyRemoveBtn(params, removeBtn);
                });

                //选中连线
                nt.on("selectEdge", function (params) {
                    tidyRemoveBtn(params, removeBtn);
                    tidyEdgeTipBtn(params, edgeTipBtn);
                });
                nt.on("deselectEdge", function (params) {
                    tidyRemoveBtn(params, removeBtn);
                    tidyEdgeTipBtn(params, edgeTipBtn);
                });

                nt.on("dragStart", function (params) {
                    tidyRemoveBtn(params, removeBtn);
                    tidyEdgeTipBtn(params, edgeTipBtn);
                });

                //连接节点
                var addEdgeBtn = oj.find("[data-addEdge]");
                o._addEdgeBtn = addEdgeBtn;
                addEdgeBtn.on("click.vis.addEdge", function () {
                    var nt = o._visNT;
                    nt.addEdgeMode();
                });

            }
        }

        function tidyRemoveBtn(ps,btn) {
            if (ps.nodes.length > 0 || ps.edges.length > 0)
                btn.css("display", "inline-block");
            else
                btn.hide();
        }

        function tidyEdgeTipBtn(ps, btn) {
            var es = ps.edges;
            if (es.length > 0) {
                btn.css("display", "inline-block");
                btn.edges = es;
            }
            else {
                btn.hide();
                btn.edges = [];
            }
        }

    }

    function getData(v) {
        v.nodes = v.nodes || [];

        var nodes = new vis.DataSet(v.nodes);
        var edges = new vis.DataSet(v.edges || []);
        return { nodes: nodes, edges: edges };
    }

});