$$.createModule("Wrapper.metronic.input.list", function (api, module) {
    api.requireModules(["Wrapper.metronic.input"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component, $input = $component.input;
    var $form = $component.form;

    var $list = $$.wrapper.metronic.input.list = function (painter, validator) {
        var my = this;
        $o.inherit(this, $input, painter, validator);

        my.give = function (o) {
            $o.callvir(this, $input, "give", o);
            var my = this;
            o.addItem = function () { var t = my.execAddItem(this); onchange(this, "addItem", t); return t; } //作为集合控件，可以添加项
            o.removeItem = function (t) { my.execRemoveItem(this, t); onchange(this, "removeItem", t); } //作为集合控件，可以移除项
            o.resetItem = function (t) { my.execResetItem(this, t); onchange(this, "resetItem", t); } //作为集合控件，可以重置项
            o.moveUp = function (t) { my.execMoveUp(this, t); onchange(this, "moveUp", t); }
            o.moveDown = function (t) { my.execMoveDown(this, t); onchange(this, "moveDown", t); }
            o.findItem = function (t) { return my.container.findItem(t).proxy(); }
            o.getItemData = function (t) {
                var e = this.findItem(t), f = new $form();
                f.collect(e.ent());
                return f.get();
            }
            o.items = function (l) { return my.execItems(this, l); }
            o.browse = function (b) {
                my.container.items().each(function (i, e) {
                    var f = new $form();
                    f.collect(e);
                    f.browse(b);
                });
            }

            my.obj = o;
            my.container = new itemContainer(o, my.painter);
            o.getJquery().on("click.input-list", "button[data-name='addItem']", function (e) { o.addItem(this); });

            initItems(o);
            onchange(o, "init");//分析组件会执行该方法
        }

        function onchange(o, cmd, t) { o.execEvent("onchange", [o, cmd, t]); }

        my.execAddItem = function (o) {
            var max = o.para("max");
            if (max && o.items().length >= max) return;
            return my.container.addItem();
        }

        my.execRemoveItem = function (o, t) {//t可以是项也可以是项中的子节点
            var min = o.para("min");
            if (o.items().length <= min) return;
            my.container.removeItem(t);
        }

        my.execResetItem = function (o, t) { my.container.resetItem(t); }
        my.execMoveUp = function (o, t) { my.container.moveItem(t, "up"); }
        my.execMoveDown = function (o, t) { my.container.moveItem(t, "down"); }

        my.execGet = function (o) {
            var my = this, v = [];
            my.container.items().each(function (i, e) {
                var f = new $form();
                f.collect(e);
                v.push(f.get());
            });
            return v;
        }

        my.execBrowseData = function (o) {
            return '';
        }

        my.execSet = function (o, v) {
            removeAll();
            for (var i = 0; i < v.length; i++) {
                var e = o.addItem().ent();
                var f = new $form();
                f.collect(e);
                f.accept(v[i]);
                o.execEvent("onset", [$(e), v[i], o]);
            }

            if (v.length == 0) initItems(o);

            onchange(o, "set");
        }

        my.execReset = function (o) {
            removeAll();
            initItems(o);
            onchange(o, "reset");
        }

        my.execItems = function (o, le) {
            if (!le) return my.container.items(); //读
            removeAll();
            for (var i = 0; i < le; i++) o.addItem();
        }


        function removeAll() {
            var cont = my.container;
            cont.items().each(function (i, e) {
                cont.removeItem(e);
            });
        }

        function initItems(o) {
            var le = o.para("length") || o.para("min");
            if (le) o.items(le);
        }

    }

    function itemContainer(o, painter) { //私有类
        var my = this, _o = o, rawItem = painter.find(o, "item"), _template = rawItem.clone(), _cache = $("<div></div>"), _cont = rawItem.parent();
        rawItem.remove();
        var moveToCache = function (item) {
            if (inCache(item)) return;
            _cache.append(item);
        }

        var inCache = function (item) {
            var cs = _cache.children();
            return cs.index(item) > -1;
        }
        var getFromCache = function () {
            var list = _cache.children();
            if (list.length <= 1) return null; //第0项留给template,该项不移动
            var item = $(list[1]);
            _resetItem(item);
            return item;
        }
        var createNew = function () {
            var item = $$(_template.clone());
            item.attr("data-name", "listItem"); //为新项打上标记
            item = item.getJquery();
            item.show();//防止默认项隐藏导致的bug
            $$.wrapper.metronic.initAjax(item);

            item.on("click.input-list", "button[data-name='prevItem']", function (e) { _o.moveUp(this); });
            item.on("click.input-list", "button[data-name='nextItem']", function (e) { _o.moveDown(this); });
            item.on("click.input-list", "button[data-name='resetItem']", function (e) { _o.resetItem(this); });
            item.on("click.input-list", "button[data-name='removeItem']", function (e) { _o.removeItem(this); });
            return item;
        }

        this.addItem = function () {
            var item = getFromCache();
            if (!item) item = createNew();
            _resetItem(item);
            _cont.append(item.getJquery());
            _o.execEvent("oncreate", [_o, item]);
            return item;
        }
        this.removeItem = function (t) {
            moveToCache(_findItemBy(t));
        }
        this.resetItem = function (t) { _resetItem(_findItemBy(t)); }
        this.moveItem = function (t, m) {
            var item = _findItemBy(t), tar = m == "up" ? item.prev() : item.next();
            if (!tar) return;
            var mn = m == "up" ? "before" : "after";
            tar[mn](item.ent());
        }
        this.findItem = function (t) {
            return _findItemBy(t);
        }
        var _findItemBy = function (t) {
            t = $(t);
            return t.attr("data-name") == "listItem" ? t : t.parents("*[data-name='listItem']").first().mapNull();
        }

        var _resetItem = function (item) {
            var f = new $form();
            f.collect(item);
            f.reset();
        }

        this.items = function () {
            var l = _cont.children("*[data-name='listItem']");
            if (l.length == 1 && l.first().css("display") == "none") return [];
            return l;
        }

        //moveToCache(_template); //将初始化的项，移到容器中
    }

    //#region 验证器

    function itemsHandler() { //每个选项验证处理器
        this.exec = function (o, n, v) {
            var es = [], l = o.items();
            l.each(function (i, e) {
                var f = new $form();
                f.collect(e);
                var r = f.validate(true);
                if (!r.satisfied()) {
                    es.push("[ 第" + (i + 1) + "项：" + r.message() + " ]");
                }
            });
            if (es.length > 0)
                throw new Error(n + "输入错误，" + es.join(''));
        }
    }

    function ignoreValidate(o, v) {
        if (!o.para("required") && v.length == 0) return true; //当没有输入，并且required不为true时，不用验证
        return false;
    }

    $list.validator = function () {
        $o.inherit(this, $input.select.validator);
        this.addHandler(new itemsHandler());
    }

    //#endregion

    $list.painter = function (methods) {
        $o.inherit(this, $input.painter, methods);
        var my = this;
        this.findItem = function (o) {
            return o.find("*[data-name='listItem']").first();
        }
        this.findMessage = function (o) {
            return null;//对于集合组件，不需要提示消息，每一项内部会有消息提示
        }


        var _setMsg = function (o, msg) {
            var h = my.find(o, "help");
            if (h) h.text(msg ? msg : "");
            my.find(o, "label").removeClass("list-has-success").removeClass("list-has-error");
            my.find(o, "help").removeClass("list-has-success").removeClass("list-has-error");
        }
        this.onError = function (o, msg) {
            _setMsg(o, msg);
            my.find(o, "label").addClass("list-has-error");
            my.find(o, "help").addClass("list-has-error");
        }
        this.onSuccess = function (o, msg) {
            _setMsg(o, msg);
            my.find(o, "label").addClass("list-has-success");
            my.find(o, "help").addClass("list-has-success");
        }
        this.onResume = function (o, msg, finder) {
            _setMsg(o, msg);
        }

        this.drawByBrowse = function (o, b, d) {
            
        }

    }

    $$.wrapper.metronic.input.createList = function (painter) {
        if (!painter) painter = new $list.painter();
        return new $list(painter, new $list.validator());
    }
});