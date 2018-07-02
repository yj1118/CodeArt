$$.createModule("Wrapper.metronic.input.upload", function (api, module) {
    api.requireModules(["Component", "Component.input", "Wrapper.metronic", "Wrapper.metronic.input"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component, $input = $component.input;

    //#region upload组件

    (function () {
        var $upload = $input.upload = function (painter, validator) {
            $o.inherit(this, $input, painter, validator);
            var my = this;
            my.execReadOnly = function (o, b) {
                throw new Error("upload组件没有实现readOnly方法");
            }
            my.execDisabled = function (o, b) {
                throw new Error("upload组件没有实现disabled方法");
            }
            my.execBrowseData = function (o) {
                return ["<form class=\"browse-files\">", my.find(o, "edit-files").find("table").prop("outerHTML"), "</form>"].join('');
            }
        }

        $vir($upload, "give", function (o) {
            $o.callvir(this, $input, "give", o); //调用父类方法
            var my = this;
            o.add = function (files) { //添加已经上传了的文件信息
                var cv = this.get(), nv = cv.concat(files);
                this.set(nv);
            }

            o.folderId = function (id) { //设置或获取目标编号
                var my = this;
                if (!empty(id)) {
                    my.para("folderId", id);
                    var form = my.fileForm;
                    form.find("input[name='folderId']").val(id);
                }
                else {
                    getFolderId(my);
                }
            }

            o.refreshTarget = function (cb) {
                var my = this, oj = my.getJquery();
                var form = oj.find("form.edit-files");
                var t = my.para("target");
                if (t) {
                    form.attr("action", t);
                    if(cb) cb(my);
                    return;
                }

                t = my.para("getTarget");
                if (!t) throw new Error("没有设置target或getTarget参数，无法使用服务器端磁盘组件处理器");

                var req = new $$request();
                req.success = function (r) {
                    //此处还可以根据服务器端传递来的密匙到期时间，自动刷新访问路径
                    form.attr("action", r.url);
                    if (cb) cb(my);
                };
                req.submit({ url: t });
            }

            init(o);

            o.refreshTarget(function (o) {  //这里是异步调用
                var fid = getFolderId(o);
                o.folderId(fid);
            });
        });

        function getFolderId(o){
            if (o.para("xaml")) {
                return o.attr("data-folderId");
            }
            return o.para("folderId");
        }

        function processExtension(ext) {
            if (ext.length > 0 && ext[0] != '.') ext = '.' + ext;
            return ext;
        }

        function init(o) {
            var oj = o.getJquery(), form = oj.find("form.edit-files");
            o.fileForm = form;
            form.fileupload({
                disableImageResize: false,
                disableImageResize: /Android(?!.*Chrome)|Opera/.test(window.navigator.userAgent),
                autoUpload: false,
                maxFileSize: (o.para("maxFileSize") || 5) * 1024, //外界指定的是KB单位
                acceptFileTypes: o.para("extensions") ? new RegExp(o.para("extensions")) : /(\.|\/)(gif|jpe?g|png)$/i,
                maxNumberOfFiles: o.para("max") || 1,
                messages: {
                    maxNumberOfFiles: '文件超过最大数量',
                    acceptFileTypes: '文件类型不正确',
                    maxFileSize: '文件所占空间太大',
                    minFileSize: '文件所占空间太小'
                }
                // Uncomment the following to send cross-domain cookies:
                //xhrFields: {withCredentials: true},
            });
            form.on('fileuploadfail', function (e, data) {
                if (data.jqXHR) data.errorThrown = data.jqXHR.responseText;
            });
            form.on('fileuploaddone', function (e, data) {
                var files = data.result.files, f = files[0];
                f.url = f.source;
                f.size = parseInt(f.size);
                f.extension = f.extension || f.ext;
                fillThumb(o, f);
            });

            if (o.para("disk")) {
                var diskModal = oj.__modal = oj.find("div[data-name='disk-modal']");

                $$(oj.find("div[data-name='disk']")[0]).on("select", function (disk, d, item) {
                    var file = { id: d.id, name: d.name, url: d.source, thumbnailUrl: d.thumb, size: parseInt(d.size), extension: processExtension(d.ext) };
                    o.add([file]);
                    oj.__modal.modal('toggle');
                });

                oj.__modal.on("shown.bs.modal", function (e) {
                    if (o.__loadedDisk) return;
                    o.__loadedDisk = true;
                    $$(oj.__modal.find("div[data-name='disk']")[0]).load();
                });

                oj.find(".fileinput-disk").on('click', function (e) {
                    oj.__modal.modal('toggle');
                });

                if (!diskModal.parent().is('body'))
                    $("body").append(diskModal);
            }
        }

        function fillThumb(o,t) {
            var e = getFileType(t.extension);
            if (e.img) t.thumbnailUrl = t.thumb;
            else if (e.video) t.videoUrl = t.thumb;
            else {
                var ext = t.extension;
                if (ext.length > 0 && ext[0] == '.') ext = ext.substr(1);
                t.thumbnailUrl = o.para("assetsPath") + "/icon_dark/" + ext + ".jpg";
            }
        }

        function getFileType(ext) {
            if (ext.indexOf('jpg') > -1 || ext.indexOf('jpeg') > -1 || ext.indexOf('png') > -1 || ext.indexOf('gif') > -1) return { img: true };
            if (ext.indexOf('mp4') > -1 || ext.indexOf('webm') > -1) return { video: true };
            return {};
        }

        $vir($upload, "execGet", function (o) {
            var form = o.fileForm, files = form.find(".files tr.template-download");
            var v = [];
            files.each(function () {
                var t = $(this), id = t.attr("data-id"), size = t.attr("data-size"), name = t.attr("data-name"), url = t.attr("data-url"), thumb = t.attr("data-thumbnailUrl");
                v.push({ id: id, size: size, name: name, thumb: thumb, url: url, extension: t.attr("data-extension"), error: (t.attr("data-error") || '') });
            });
            return v;
        });

        $vir($upload, "execSet", function (o, v) {
            for (var i = 0; i < v.length; i++) { //转换格式
                var t = v[i];
                t.extension = processExtension(t.extension);
                fillThumb(o,t);
            }
            checkError(o, v);

            var upload = getUpload(o);
            //以下采用内部方法来追加文件项
            var template = upload._renderDownload(v), tbody = o.fileForm.find("tbody.files"), temp = [];
            template.each(function () {
                temp.push(this);
            });
            template.splice(0, template.length);
            var rows = tbody.find("tr.template-download"), added = [];
            $(temp).each(function () {
                var newRow = $(this), dataId = newRow.attr("data-id"), finded = false;
                for (var i = 0; i < rows.length; i++) {
                    var oldRow = $(rows[i]);
                    if (oldRow.attr("data-id") == dataId) {
                        finded = true;
                        break;
                    }
                }
                if (!finded) {
                    tbody.append(newRow);
                    template.push(this);
                }
            });
            upload._transition(template);
        });

        function checkError(o, files) {
            var maxFileSize = (o.para("maxFileSize") || 5) * 1024;
            var acceptFileTypes = o.para("extensions") ? new RegExp(o.para("extensions")) : /(\.|\/)(gif|jpe?g|png)$/i
            var maxNumberOfFiles = o.para("max") || 1;
            var currentCount = o.fileForm.find("tbody.files").find("tr.template-upload").length;


            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                if (file.size > maxFileSize) file.error = "文件大小超出限制";
                else if (!acceptFileTypes.test(file.extension)) file.error = "文件类型错误";
                else {
                    var index = currentCount + i;
                    if (index >= maxNumberOfFiles) file.error = "超过最大文件数量";
                }
            }
        }

        $vir($upload, "execReset", function (o) {
            o.fileForm.find("tbody.files").find(".cancel").click();
            //o.execEvent("onchange", [o]);
        });

        var handlers = $input.validateHandlers;

        $upload.validator = function () {
            $o.inherit(this, $input.validator);
            this.addHandler(new handlers.required("{label}不能为空，请检查是否已上传所选文件"));
            this.addHandler(new handlers.length({ less: "{label}的文件不能小于{min}个", more: "{label}的文件不能大于{max}个" }));
            this.addHandler(new filesHandler());
        }

        function filesHandler() {//对文件列进行检查
            this.exec = function (o, n, v) {
                var error;
                for (var i = 0; i < v.length; i++) {
                    if (v[i].error) {
                        error = n + "中存在出错的文件";
                        break;
                    }
                }
                if (error) { throw new Error(error); }
            }
        }

        //单选绘制器的基类
        $upload.painter = function (methods) {
            $o.inherit(this, $input.painter, methods);
        }

        $$.wrapper.metronic.input.createUpload = function (painter) {
            if (!painter) painter = new $upload.painter();
            return new $upload(painter, new $upload.validator());
        }

        function getUpload(o) {
            return o.fileForm.data('blueimp-fileupload') || o.fileForm.data('fileupload');
        }

    })();

    //#endregion
});