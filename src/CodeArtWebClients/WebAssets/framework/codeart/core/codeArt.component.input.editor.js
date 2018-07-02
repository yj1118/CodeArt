$$.createModule("Component.input.editor", function (api, module) {
    api.requireModules(["Component.input"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component, $input = $component.input;

    var $editor = $input.editor = function (painter, validator) {
        $o.inherit(this, $input.text, painter, validator);
        var my = this;
        my.give = function (o) {
            $o.callvir(this, $input.text, "give", o);
            o.mode = function (mode) { return my.execMode(this, mode); }
            o.command = function (n, args) { return my.execCommand(this, n, args); }
            o.createLink = function () { my.execCreateLink(this); }
            o.createTag = function () { my.execCreateTag(this); }
            o.getText = function () { return my.execGetText(this); }
            o.inited = function (v) {
                if (v) {
                    o.__tinymceInited = true;
                    //触发事件
                    if (o.para("xaml")) {
                        var cn = o.para("name");//组件名称
                        var vn = o.para("view"); //组件相关的视图名称
                        var view = new $$view(o, vn);
                        view.submit({ component: cn, action: "OnEditorInited" },{ ignoreValidate: true });
                    }
                }
                return o.__tinymceInited;
            } //标示文本编辑器组件已经初始化完毕，这个是第三方异步回调调用的
            init(o);
        }

        function init(o) {
            o.__tinymceInited = false;

            var id = o.attr("id"), oj = o.getJquery();
            o.tinymceId = "tinymce" + o.attr("id");
            eval("_editor_init_" + id + "();"); //执行初始化方法，控件会生成该方法

            var diskId = "editor-disk-" + id;
            if ($("#" + diskId).length > 0) {   //如果设置了文件空间，那么在窗口第一次打开时，加载文件
                var modal = "editor-disk-modal-" + id;
                $("#" + modal).on("shown.bs.modal", function (e) {
                    if (o.__loadedDisk) return;
                    o.__loadedDisk = true;
                    $$("#" + diskId).load();
                });
            }
        }

        my.execGet = function (o) {
            if (!o.inited()) return '';
            var id = o.tinymceId;
            return tinymce.get(id).getContent();
        }

        my.execGetText = function (o) {
            if (!o.inited()) return '';
            var id = o.tinymceId;
            return tinymce.get(id).getContent({ format: 'text' });
        }

        my.execSet = function (o, v) {
            if (!o.inited()) return;
            var id = o.tinymceId;
            tinymce.get(id).setContent(v);
        }

        my.execBrowseData = function (o) {
            var t = o.getText(), s = t.length > 100 ? "……" : "";
            return t.substr(0, 100) + s;
        }
    }

    $editor.painter = function (methods) {
        $o.inherit(this, $input.painter, methods);
    }

    $editor.validator = function () {
        $o.inherit(this, $input.validator);
    }

    $input.createEditor = function (painter) {
        if (!painter) painter = new $editor.painter();
        return new $editor(painter, new $editor.validator());
    }
});