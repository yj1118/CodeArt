$$.createModule("Component.input.thumbnails", function (api, module) {
    api.requireModules(["Component.input"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component, $input = $component.input;

    var $thumbnails = $input.thumbnails = function (painter, validator) {
        $o.inherit(this, $input, painter, validator);
        var my = this;
        my.give = function (o) {
            $o.callvir(this, $input, "give", o);
            o.setConfig = function (items) {
                var t = my.find(o, "thumbnails");
                $$(t).bind({ "items": items });
                var gs = t.get(0).getElementsByTagName("img");//一定要用原生方法，否则holder不识别
                Holder.run({
                    images: gs
                });
                tidyWdith(gs);
                o.__config = { "items": items };
            }
            init(o);
        }

        function init(o) {
            initCropper(o);
            initUpload(o);
        }

        function initCropper(o) {
            var modal = getModal(o, "cropper"), cropper = $$(modal.find('.cropper')), btnOk = modal.find(".thumbnails-cropper-btn-ok");
            btnOk.off("click");
            btnOk.on("click", function () {
                cropper.upload();
            });
        }

        function initUpload(o) {
            var modal = getModal(o, "upload"), btnOk = modal.find(".thumbnails-upload-btn-ok");
            btnOk.off("click");
            btnOk.on("click", function () {
                var p = $$(modal.find(".thumbnails-input-cover")), v = p.get();
                if (v.length == 0) {
                    $$("#thumbnailsAlert_upload_" + o.attr("id")).alert({ message: '请上传一个封面图片。', type: 'danger', seconds: 10, close: false });
                }
                else {
                    var c = modal.data("current");
                    setImage(c.find("img"), v[0]);
                    modal.modal('toggle');
                }
            });
        }


        my.execGet = function (o) {
            var t = my.find(o, "thumbnails");
            var vl = [];
            t.find("img").each(function () {
                var d = $$(this).data;
                if (d) vl.push(d);
            });
            return vl;
        }

        my.execSet = function (o, vl) {
            var t = my.find(o, "thumbnails");
            if (type(vl) == "array") {
                //重置
                t.find("img").each(function () {
                    resetImage(this);
                });

                for (var i = 0; i < vl.length; i++) {
                    var v = vl[i];
                    var g = t.find("img[data-resolution='" + formatResolution(v.resolution) + "']").first().mapNull();
                    if (g) setImage(g, v);
                }
            }
            else { //设置单个分辨率的图片
                var g = t.find("img[data-resolution='" + formatResolution(vl.resolution) + "']").first().mapNull();
                if (g) setImage(g, vl);
            }
        }

        my.execReset = function (o) {
            o.set([]);
        }

        my.execBrowseData = function (o) {
            return '';
        }
    }

    function resetImage(g) {
        emptyImage(g);
        initBtnRemove(g);
    }

    function setImage(g, d) {
        g = $(g);
        g.attr("src", d.url);
        d.resolution = parseResolution(g.attr("data-resolution"));
        $$(g).data = d;

        var p = g.closest('.thumbnail');
        p.find(".thumbnails-btn-remove").show();
    }

    function initBtnRemove(g) {
        var p = $(g).closest('.thumbnail');
        var removeBtn = p.find(".thumbnails-btn-remove");
        removeBtn.show();
        removeBtn.off("click");
        removeBtn.on("click", function () {
            var p = $(this).closest('.thumbnail');
            emptyImage(p.find('img')[0]);
            $(this).hide();
        });
        removeBtn.hide();
    }

    function emptyImage(g) {
        g = $(g);
        var dr = g.attr("data-resolution");
        if (empty(dr)) return;
        var src = "holder.js/" + dr;
        g.attr("src", src);
        g.attr("data-src", src);
        $$(g).data = null;
        var gs = [g[0]];
        Holder.run({
            images: gs
        });
        tidyWdith(gs);
    }

    function formatResolution(r) {
        return [r.width, 'x', r.height].join('');
    }

    function parseResolution(r) {
        var t = r.split('x');
        return { width: t[0], height: t[1] };
    }

    function getModal(o, t) {
        var modalId = "#selectCoverModal_" + t + "_" + o.attr("id");
        return $(modalId);
    }

    $thumbnails.itemBind = function (item, d) {
        var g = $(item.find("img"));
        var rf = formatResolution(d.resolution);
        g.attr("data-resolution", rf);
        g.attr("src", "holder.js/" + rf);
        g.attr("data-src", "holder.js/" + rf);

        var cropperBtn = item.find(".thumbnails-btn-cropper");
        if (cropperBtn.length > 0) {
            $$(cropperBtn).data = d;
            cropperBtn.off("click");
            cropperBtn.on("click", function () {
                var o = $(this).closest(".thumbnails");
                var modal = getModal(o, "cropper"), cropper = $$(modal.find('.cropper')), r = $$(this).data.resolution;
                $$(modal).title("上传封面图片文件 - " + formatResolution(r));
                cropper.config({
                    width: r.width,
                    height: r.height
                });
                cropper.on("upload", function (file) {
                    if (!file) {
                        $$("#thumbnailsAlert_cropper_" + o.attr("id")).alert({ message: '请上传一个封面图片。', type: 'danger', seconds: 10, close: false });
                    }
                    else {
                        var c = modal.data("current");
                        setImage(c.find("img"), { url: file.source, resolution: r, id: file.id });
                        modal.modal('hide');
                    }
                });

                modal.modal({ backdrop: 'static' });
                modal.data("current", $(this).closest(".thumbnail"));
            });
        }


        var uploadBtn = item.find(".thumbnails-btn-upload");
        if (uploadBtn.length > 0) {
            $$(uploadBtn).data = d;
            uploadBtn.off("click");
            uploadBtn.on("click", function () {
                var o = $(this).closest(".thumbnails");
                var modal = getModal(o, "upload");
                $$(modal).title("上传封面图片文件 - " + formatResolution($$(this).data.resolution));
                $$(modal.find(".thumbnails-form")).reset();
                modal.modal('toggle');
                modal.data("current", $(this).closest(".thumbnail"));
            });
        }

        initBtnRemove(g);
    }



    $thumbnails.painter = function (methods) {
        $o.inherit(this, $input.painter, methods);
    }

    var requiredHandler = function (msg) { //必填处理器
        if (!msg) msg = "请上传完整的{label}";
        this.exec = function (o, n, v) {
            if (o.para("required")) {
                if (v.length != o.__config.items.length) {
                    throw new Error(msg.replace("{label}", n));
                }
            }
        }
    }

    function tidyWdith(gs) {
        $(gs).each(function () {
            $(this).css({
                width: "auto",
                height: "auto",
                maxWidth: "100%"
            });
        });
    }

    $thumbnails.validator = function () {
        $o.inherit(this, $input.validator);
        this.addHandler(new requiredHandler());
    }

    $input.createThumbnails = function (painter) {
        if (!painter) painter = new $thumbnails.painter();
        return new $thumbnails(painter, new $thumbnails.validator());
    }
});