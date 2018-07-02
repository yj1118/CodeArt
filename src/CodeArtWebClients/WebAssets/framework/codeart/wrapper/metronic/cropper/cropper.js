$$.createModule("Wrapper.metronic.cropper", function (api, module) {
    api.requireModules(["Wrapper.metronic"]);
    api.requireModules(["Component"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component, $request = $$.ajax.request;

    var $cropper = $$.wrapper.metronic.cropper = function (painter) {
        $o.inherit(this, $component.ui, painter);
        var my = this;
        this.give = function (o) {
            $o.callvir(this, $component.ui, "give", o);
            o.config = function (p) {
                //  如果基于图片的屏幕，那么选区截取的图片实际多大就是多大,画质较高
                var o = this, oj = o.getJquery(), g = oj.find(".cropper-img");
                o.__para = p;
                o.__getData = function () {
                    var p = this.__para, oj = o.getJquery(), g = oj.find(".cropper-img");
                    return g.cropper('getData');
                }

                o.__setData = function (data) {
                    var p = this.__para, oj = o.getJquery(), g = oj.find(".cropper-img");
                    return g.cropper('setData', data);
                }

                oj.find('.cropper-btn-range').attr("checked", false).parent('span').removeClass('checked');
                oj.find('.cropper-btn-image').attr("checked", false).parent('span').removeClass('checked');

                var options = {
                    aspectRatio: p.width / p.height,
                    //drapCrop: true,
                    //cropBoxResizable: true,
                    touchDragZoom:true,
                    mouseWheelZoom: false,
                    crop: function (data) {
                        data = o.__getData();
                        oj.find('.cropper-width').val(parseInt(data.width));
                        oj.find('.cropper-height').val(parseInt(data.height));
                        oj.find('.cropper-x').val(parseInt(data.x || data.left || 0));
                        oj.find('.cropper-y').val(parseInt(data.y || data.top || 0));
                    },
                    built: function () {
                    }
                };

                if (p.destroy || !o.__inited) {
                    g.cropper("destroy");
                    g.cropper(options);
                    o.__inited = true;
                } else {
                    var ip = g.data('cropper').options;
                    g.cropper("setAspectRatio", p.width / p.height);
                }
            }

            o.set = function (src) {
                var o = this, oj = o.getJquery(), g = oj.find(".cropper-img");
                g.cropper('reset').cropper('replace', src);
                g.data("fileType", getFileType(src));
                g.show();
            }

            o.upload = function () {
                var o = this;
                this.refreshTarget(function () {
                    var oj = o.getJquery(), g = oj.find(".cropper-img");
                    var data = o.__getData();
                    var canvas = (data.width == 0 || data.height == 0) ? null : g.cropper("getCroppedCanvas", data);
                    //生成名称
                    var name = new Date().format('yMdhms') + "." + getFileExtension(g.data("fileType"));
                    sendImage(o, g, name, canvas);
                });
            }

            o.refreshTarget = function (cb) {
                var my = this;
                var t = my.para("target");
                if (t) {
                    cb(my);
                    return;
                }
                t = my.para("getTarget");
                if (!t) throw new Error("没有设置target或getTarget参数，无法使用服务器端磁盘组件处理器");

                var req = new $$request();
                req.success = function (r) {
                    //此处还可以根据服务器端传递来的密匙到期时间，自动刷新访问路径
                    my.para("target", r.url);
                    cb(my);
                };
                req.getData = function (arg, paras) { return paras; };
                req.submit({ url: t });
            }

            init(o);
        }

        function getFileType(src) {
            var p = src.lastIndexOf('.'), e = 'png';
            if (p > -1) e = src.substr(p + 1);
            return 'image/' + e;
        }

        function getFileExtension(fileType) {
            var p = fileType.lastIndexOf('/'), e;
            if (p > -1) e = fileType.substr(p + 1);
            return e || "jpg";
        }

        function init(o) {
            var oj = o.getJquery(), g = oj.find(".cropper-img");

            oj.find('.cropper-btn-preview').on("click", { o: o, g: g }, function (e) {
                var o = e.data.o, g = e.data.g;
                var data = o.__getData();
                if (data.width == 0 || data.height == 0) {
                    bootbox.alert({
                        buttons: {
                            ok: {
                                label: '确定',
                            }
                        },
                        message: '请导入图片并划分选区',
                        title: "错误提示"
                    });
                    return;
                }
                var result = g.cropper("getCroppedCanvas", data);
                o.getJquery().find('.cropper-dialog-preview').modal({ backdrop: 'static' }).find('.modal-body').html(result);
            });

            oj.find('.cropper-btn-closePreview').on("click", { o: o, g: g }, function (e) {
                var o = e.data.o;
                o.getJquery().find('.cropper-dialog-preview').modal('toggle');
            });

            oj.find('.cropper-btn-zoomIn').on("click", { o: o, g: g }, function (e) {
                var o = e.data.o, g = e.data.g;
                g.cropper('zoom', 0.1);
            });

            oj.find('.cropper-btn-zoomOut').on("click", { o: o, g: g }, function (e) {
                var o = e.data.o, g = e.data.g;
                g.cropper('zoom', -0.1);
            });

            if (o.para("disk")) {

                var diskModal = oj.__modal = oj.find("div[data-name='disk-modal']");

                $$(oj.find("div[data-name='disk']")[0]).on("select", function (disk, d, item) {
                    o.set(d.source);
                    oj.__modal.modal('toggle');
                });

                oj.__modal.on("shown.bs.modal", function (e) {
                    if (o.__loadedDisk) return;
                    o.__loadedDisk = true;
                    $$(oj.__modal.find("div[data-name='disk']")[0]).load();
                });

                oj.find(".cropper-btn-disk").on('click', function (e) {
                    oj.__modal.modal('toggle');
                });

                oj.find('.btn-group-disk').show();
            }


            var ig = oj.find('.inputImage'),
            URL = window.URL || window.webkitURL,
            blobURL;

            var igId = "inputImage_" + util.getId();
            ig.attr("id", igId);
            oj.find('.cropper-btn-upload').attr("for", igId);

            if (URL) {
                ig.change(function () {
                    var files = this.files,
                        file;

                    if (!g.data('cropper')) {
                        return;
                    }

                    if (files && files.length) {
                        file = files[0];
                        if (/^image\/\w+$/.test(file.type)) {
                            blobURL = URL.createObjectURL(file);
                            g.one('built.cropper', function () {
                                URL.revokeObjectURL(blobURL); // Revoke when load complete
                            }).cropper('reset').cropper('replace', blobURL);
                            g.show();
                            g.data("fileType", file.type);
                            g.data("blobURL", blobURL);
                            ig.val('');
                        } else {
                            bootbox.alert({
                                buttons: {
                                    ok: {
                                        label: '确定',
                                    }
                                },
                                message: '请选择一个图片文件',
                                title: "错误提示"
                            });
                        }
                    }
                });
            } else {
                ig.parent().remove();
            }
        }

        function zoom(canvas, p, fileType, callBack) {
            var img = new Image(), scale = p.width / canvas.width;
            img = new Image();
            img.onload = function () {
                var g = this;
                var ctx = canvas.getContext("2d");
                // canvas清屏  
                ctx.clearRect(0, 0, canvas.width, canvas.height);
                // 重置canvas宽高
                //var zoomWidth = g.width * scale, zoomHeight = parseInt(g.height * scale;
                canvas.width = p.width;
                canvas.height = p.height;
                // 将图像绘制到canvas上
                ctx.drawImage(g, 0, 0, g.width, g.height, 0, 0, p.width, p.height);
                callBack(canvas);
            }
            img.src = canvas.toDataURL();
        }

        function sendImage(o, g, name, canvas) {
            var p = o.__para, fileType = g.data("fileType"), maxFileSize = (parseInt(o.para("maxfilesize")) * 1024);//maxfilesize是KB，需要转换成字节

            if (canvas == null) {
                bootbox.alert({
                    buttons: {
                        ok: {
                            label: '确定',
                        }
                    },
                    message: '请选择图片',
                    title: "错误提示"
                });
                return;
            }

            if (p.width > canvas.width || p.height > canvas.height) {
                bootbox.alert({
                    buttons: {
                        ok: {
                            label: '确定',
                        }
                    },
                    message: ' 裁剪后的图片宽度和高度不能小于 ' + p.width + 'x' + p.height,
                    title: "错误提示"
                });
                return;
            }

            zoom(canvas, p, fileType, function (result) {
                result.toBlob(function (blob) {
                    if (blob.size > maxFileSize) {
                        bootbox.alert({
                            buttons: {
                                ok: {
                                    label: '确定',
                                }
                            },
                            message: '文件体积超出指定大小，当前文件大小 ' + blob.size + ' 最大文件限制' + maxFileSize,
                            title: "错误提示"
                        });
                        return;
                    }
           
                    var fd = new FormData();
                    fd.append(name, blob);
                    fd.append("folderId", o.attr("data-diskRootId"));
                    var req = new $$request();
                    req.success = function (r) {
                        o.execEvent("onupload", [r.files[0]]);
                    };
                    req.formData({ url: o.para("target"), data: fd });
                }, fileType);
            });
        }

    }

    $cropper.painter = function (methods) {
        $o.inherit(this, $component.painter, methods);
        var my = this;
    }

    $cropper.create = function (painter) {
        if (!painter) painter = new $cropper.painter();
        return new $cropper(painter);
    }
});