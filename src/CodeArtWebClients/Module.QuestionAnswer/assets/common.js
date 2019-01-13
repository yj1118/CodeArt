var $$qa;
$$.createModule("qa", function (api, module) {
    var qa = $$qa = $$.qa = {};
    var util = api.util, empty = util.empty, type = util.type;
    qa.validator = {};

    var paper = qa.paper = function () {
        var my = this;

        my.give = function (o) {

            function initPaper(o) {
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

                    var pv = { answers: v, metadataId: this._metadata.get() };
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
                        var item = this.findItem(qid), type;
                        if (item) {
                            item = $$(item), type = item.attr("data-answer"), content = tv.Content;
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

                o.load = function (p, cb) {//加载试卷原型
                    if (type(p) == 'function') {
                        cb = p;
                        p = null;
                    }

                    var my = o;
                    if (!p) p = {};

                    if (!p.id) p.id = o.attr("data-metadataId") || null;
                    if (!p.markedCode) p.markedCode = o.attr("data-metadata") || null;

                    var view = new $$view({
                        id: '',
                        scriptElement: function () {
                            var metadataId, metadataMarkedCode;
                            if (type(p) == 'object') {
                                if (p.id) metadataId = p.id;
                                else metadataMarkedCode = p.markedCode;
                            }
                            else {
                                //默认编号
                                metadataId = p;
                            }
                            return metadataId ? { metadata: { metadataId: metadataId } } : { metadata: { metadataMarkedCode: metadataMarkedCode } };
                        }
                    }, 'self');

                    view.success = function (r) {
                        var code = r.code, to = $$tools.lazy.elements(code).first(), oj = my.getJquery();
                        oj.attr("data-metadataId", to.attr("data-metadataId")); //重置data-metadataId

                        var t = to.find(".qa--questions").mapNull();
                        var s = oj.find(".qa--questions");
                        s.html(t.html());
                        $$.init(s);
                        init(my); //重新初始化
                        if (cb) cb();
                    }

                    var n = o.getJquery().attr("name");
                    if (!n) throw new Error("必须为Paper组件指定name才能使用Load方法");

                    var arg = { component: n, action: "Load", scene: my };
                    if (p.url) arg.url = p.url;
                    view.submit(arg);
                }

                o.scriptElement = function () { //获得脚本元素的元数据
                    var my = this, v = my.get();
                    return { id: my.getJquery().prop("id") || '', metadata: { value: v } };
                }

                o.drawValid = function (e) {
                    //paper自身不需要绘制
                }

                o.metadata = function (v) {
                    var oj = this.getJquery();
                    if (empty(v)) return oj.attr("data-metadata");
                    else {
                        oj.attr("data-metadata", v);
                        oj.attr("data-metadataId", ''); //重置编号
                    }
                }

                o.metadataId = function (v) {
                    var oj = this.getJquery();
                    if (empty(v)) return oj.attr("data-metadataId");
                    else {
                        oj.attr("data-metadataId", v);
                        this._metadata.set(v);
                    }
                }

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

                var form = new $$.metronic.form();
                form.give(o);

                initPaper(o);
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

    qa.paperReader = function () {
        this.give = function (o) {
            o.load = function (p, cb) { //加载paper原型
                if (type(p) == 'function') {
                    cb = p;
                    p = null;
                }

                var my = o;
                if (!p) {
                    p = {};
                    p.id = o.attr("data-metadataId") || null;
                    p.markedCode = o.attr("data-metadata") || null;
                }

                var view = new $$view({
                    id: '',
                    scriptElement: function () {
                        var metadataId, metadataMarkedCode;
                        if (type(p) == 'object') {
                            if (p.id) metadataId = p.id;
                            else metadataMarkedCode = p.markedCode;
                        }
                        else {
                            //默认编号
                            metadataId = p;
                        }
                        return metadataId ? { metadata: { metadataId: metadataId } } : { metadata: { metadataMarkedCode: metadataMarkedCode } };
                    }
                }, 'self');

                view.success = function (r) {
                    var l = [], t = r.Questions;
                    if (t) {
                        t.each(function (i) {
                            var c = (i+1)+". " + this.Content;
                            var a = this.Options.join(' , ');
                            l.push({ id: this.Id, content: c, answer: a });
                        });
                    }
                    my.bind({ questions: l });
                    if (cb) cb();
                }

                var n = o.getJquery().attr("name");
                if (!n) throw new Error("必须为PaperReader组件指定name才能使用Load方法");

                view.submit({ component: n, action: "Load", scene: my });
            }

            o.set = function (d) {
                var oj = o.getJquery(), t = d.Answers;
                if (!t) return;
                t.each(function () {
                    var questionId = this.Question.Id;
                    var content = this.Content;
                    oj.find("[questionId='" + questionId + "']").html(content.html('c--p'));
                });
            }

            o.metadata = function (v) {
                var oj = this.getJquery();
                if (empty(v)) return oj.attr("data-metadata");
                else oj.attr("data-metadata", v);
            }

            o.metadataId = function (v) {
                var oj = this.getJquery();
                if (empty(v)) return oj.attr("data-metadataId");
                else oj.attr("data-metadataId", v);
            }

        }
    }
});
