var $$qa;
$$.createModule("qa", function (api, module) {
    var qa = $$qa = $$.qa = {};
    var util = api.util, empty = util.empty, type = util.type;
    qa.validator = {};


    var paper = qa.paper = function () {
        var my = this;

        my.give = function (o) {

            var form = new $$.metronic.form();
            form.give(o);

            o.items = function () {
                return this._items;
            }

            o.findItem = function (questionId) {
                for (var i = 0; i < this._items.length; i++) {
                    var item = this._items[i];
                    if ($$(item).attr("data-field") == questionId) return item;
                }
                return null;
            }

            o.get = function () {
                var v = [];
                this.items().each(function (i, e) {
                    var item = $$(this), questionId = item.attr("data-field"), content = item.get();
                    var answerId = item.answerId, type = item.attr("data-answer");
                    if (type == "2") {//多选
                        content = content.join(',');
                    }
                    var t = { questionId: questionId, content: content };
                    if (answerId) t.id = answerId;
                    v.push(t);
                });

                var pv = { answers : v, metadataId : this._metadata.get() };
                var paperId = this._paper.get();
                if (paperId) pv.id = paperId;

                return pv;
            }

            o.reset = function () {
                this.items().each(function (i, e) {
                    $$(this).reset();
                });
                onchange(o, "reset");
            }

            o.set = function (pv) {
                if (!pv) return;
                var v = pv.Answers;
                for (var i = 0; i < v.length; i++) {
                    var tv = v[i], qid = tv.Question.Id;
                    var item = this.findItem(qid);
                    if (item) {
                        var item = $$(item), type = item.attr("data-answer"), content = tv.Content;
                        if (type == "2") {//多选
                            content = content.split(',');
                        }
                        item.answerId = tv.Id;
                        item.set(content);
                    }
                }

                this._paper.set(pv.Id);
                onchange(this, "set");
            }

            o.disable = function (b) {
                this.items().each(function (i, e) {
                    $$(this).disable(b);
                });
            }

            o.load = function (metadataId,cb) {//加载试卷原型
                var view = new $$view({
                    id:'',
                    scriptElement: function () {
                        return { metadata: { metadataId: metadataId } }
                    }
                }, 'self');

                var my = o;
                view.success = function (r) {
                    var code = r.code, to = $(code), oj = my.getJquery();
                    oj.attr("data-metadataId", to.attr("data-metadataId")); //重置data-metadataId

                    var t = to.find(".qa--quesetions").mapNull();
                    var s = oj.find(".qa--quesetions")
                    s.html(t.html());
                    $$.init(s);
                    init(my); //重新初始化
                    if (cb) cb();
                }

                var n = o.getJquery().attr("name");
                if (!n) throw new Error("必须为Paper组件指定name才能使用Load方法");
                view.submit({ component: n, action: "Load" });
            }

            o.scriptElement = function () { //获得脚本元素的元数据
                var my = this, v = my.get();
                return { id: my.getJquery().prop("id") || '', metadata: { value: v } };
            }

            o.drawValid = function (e) {
                //paper自身不需要绘制
            }

            function init(o) {
                o._items = o.getJquery().find("[data-answer]");
                o._items.each(function () {
                    $$(this);
                });

                o._metadata = $$(o.find("*[data-field='metadataId']"));
                var metadataId = o.attr("data-metadataId");
                o._metadata.set(metadataId);

                o._paper = $$(o.find("*[data-field='paperId']"));
            }

            init(o);

            onchange(o, "init");//分析组件会执行该方法
        }

        function onchange(o, cmd, t) {
            if (o.onchange)
                o.onchange(cmd, t);
        }

    }

    $$.metronic.validator.addMethod("paperValidate", function (value, element, rules) {
        var o = element.proxy();
        var r = o.validate();
        return r.satisfied();
    });

});
