$$.createModule("Wrapper.metronic.disk", function (api, module) {
    api.requireModules(["Wrapper.metronic"]);
    api.requireModules(["Component"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component, $request = $$.ajax.request;

    var $disk = $$.wrapper.metronic.disk = function (painter) {
        $o.inherit(this, $component.ui, painter);
        var my = this;
        this.give = function (o) {
            $o.callvir(this, $component.ui, "give", o);
            o.itemContainer = $$(o.getJquery().find("ul[data-name='main-item-container']")[0]);
            o.load = function (v, p) {//v是folder的信息
                var my = this;
                if (!my.__inited) {
                    my.__inited = true;
                    my.refreshTarget(function (o) {
                        init(o);
                        o.context = new context(o);
                        o.page = new page(o);
                        executeLoad(o, v, p);
                    });
                    return;
                }
                executeLoad(my, v, p);
            }

            function executeLoad(o, v, p) {
                if (!v) v = o.context.current();
                else o.context.set(v);
                if (p) o.page.index(p.index);
                load(o);
            }

            o.drawPath = function () {
                var ctx = this.context, path = ctx.path(), my = this;
                var code = [], lastIndex = path.length - 1;
                if (path.length == 1) {
                    code.push("<li><i class='fa fa-home'></i><a href='javascript:;' data-name='home'>根目录</a></li>");
                } else {
                    path.each(function (i, p) {
                        if (i == 0) {
                            code.push("<li><i class='fa fa-home'></i><a href='javascript:;' data-name='home'>根目录</a><i class='fa fa-angle-right'></i></li>");
                        }
                        else if (i < lastIndex) {
                            code.push("<li><a href='javascript:;' data-name='path-item' data-id='" + p.id + "'>" + p.name + "</a><i class='fa fa-angle-right'></i></li>");
                        } else {
                            code.push("<li><a href='javascript:;'>" + p.name + "</a></li>");
                        }
                    });
                }
                var oj = my.getJquery(), bc = oj.find(".page-breadcrumb");
                bc.html(code.join(''));
                bc.off("click.disk", "a[data-name='home']");
                bc.on("click.disk", "a[data-name='home']", function (e) {
                    my.context.clear();
                    my.page.clear();
                    my.load();
                });

                bc.off("click.disk", "a[data-name='path-item']");
                bc.on("click.disk", "a[data-name='path-item']", function (e) {
                    var a = $(this);
                    my.context.set({ id: a.attr('data-id'), name: a.text() });
                    my.page.clear();
                    my.load();
                });

                oj.off("click.disk", ".disk-refresh");
                oj.on("click.disk", ".disk-refresh", function (e) {
                    my.page.clear();
                    my.load();
                });
            }

            o.drawPage = function (r) {
                var my = this, oj = my.getJquery();
                var dataCount = r.dataCount, pageIndex = my.page.index(), pageSize = my.page.size();
                var pageCount = parseInt(dataCount / pageSize);
                if (dataCount % pageSize > 0) pageCount++;
                my.page.count(pageCount); //更新页面总数

                var dis = 3, code = [];
                code.push('<li><a data-name="page-prev" href="javascript:;"><i class="fa fa-angle-left"></i></a></li>');
                for (var i = pageIndex - dis; i < pageIndex; i++) {
                    if (i >= 0) {
                        code.push('<li><a data-name="page-index" href="javascript:;">' + (i + 1) + '</a></li>');
                    }
                }
                code.push('<li class="active"><a href="javascript:;">' + (pageIndex + 1) + '</a></li>');
                for (var i = pageIndex + 1; i < (pageIndex + 1 + dis) ; i++) {
                    if (i < pageCount) {
                        code.push('<li><a data-name="page-index" href="javascript:;">' + (i + 1) + '</a></li>');
                    }
                }
                code.push('<li><a data-name="page-next" href="javascript:;"><i class="fa fa-angle-right"></i></a></li>');
                var page = oj.find(".pagination");
                page.html(code.join(''));

                page.off('click', "a[data-name='page-prev']");
                page.on('click', "a[data-name='page-prev']", { o: my }, function (e) {
                    if (my.page.prev()) my.load();
                });

                page.off('click', "a[data-name='page-next']");
                page.on('click', "a[data-name='page-next']", { o: my }, function (e) {
                    if (my.page.next()) my.load();
                });

                page.off('click', "a[data-name='page-index']");
                page.on('click', "a[data-name='page-index']", { o: my }, function (e) {
                    var pi = parseInt($(this).text()) - 1;
                    if (my.page.index(pi)) my.load();
                });

                //绘制其他关于翻页的元素
                var unitNext = oj.find("li[data-name='disk-next']");
                var unitPrev = oj.find("li[data-name='disk-prev']");
                var unitPage = oj.find(".pagination");

                unitNext.off('click');
                unitNext.on('click', { o: my }, function (e) {
                    if (my.page.next()) my.load();
                });

                unitPrev.off('click');
                unitPrev.on('click', { o: my }, function (e) {
                    if (my.page.prev()) my.load();
                });


                if (pageCount > 1) {
                    unitPage.show();
                    if (pageIndex < (pageCount - 1)) unitNext.show();
                    else unitNext.hide();

                    if (pageIndex > 0) unitPrev.show();
                    else unitPrev.hide();
                }
                else {
                    unitPage.hide();
                    unitNext.hide();
                    unitPrev.hide();
                }

            }

            function load(o) {
                var page = o.page;
                var req = new $$request();
                req.add("folderId", o.context.current().id);
                req.add("pageIndex", page.index());
                req.add("pageSize", page.size());
                req.success = function (r) {
                    o.itemContainer.bind(r);
                    o.drawPath();
                    o.drawPage(r);
                    initAjax(o);
                };
                req.getData = function (arg, paras) { return paras; };
                req.submit({ url: o.para("target"), action: "Load" });
            }

            function initAjax(o) {
                var oj = o.getJquery();
                if (o.context.path().length == 1) oj.find("li[data-name='disk-back']").hide();
                else oj.find("li[data-name='disk-back']").show();

                var cont = o.itemContainer.getJquery();
                cont.off('click', '.delete-file');
                cont.on('click', '.delete-file', function () { //删除文件
                    var _this = $(this);
                    bootbox.confirm({
                        buttons: {
                            confirm: {
                                label: '确定',
                            },
                            cancel: {
                                label: "取消"
                            }
                        },
                        message: _this.attr('data-confirm'),
                        callback: function (result) {
                            if (result) {
                                var file_container = _this.parent().parent().parent();
                                executeAction(o, 'DeleteFile', _this.attr('data-id'), '', file_container, 'apply_file_delete');
                            }
                        }
                    });
                });

                cont.off('click', '.delete-folder');
                cont.on('click', '.delete-folder', function () {
                    var _this = $(this);
                    bootbox.confirm({
                        buttons: {
                            confirm: {
                                label: '确定',
                            },
                            cancel: {
                                label: "取消"
                            }
                        },
                        message: _this.attr('data-confirm'),
                        callback: function (result) {
                            if (result) {
                                var folder_container = _this.parent().parent().parent();
                                executeAction(o, 'DeleteFolder', _this.attr('data-id'), '', folder_container, 'apply_folder_delete');
                            }
                        }
                    });
                });

                if (o.para("height")) {
                    var gird = oj.find("ul.grid");
                    cont.find(".lazy-loaded").lazyload({
                        placeholder: o.para("assetsPath") + "/icon_dark/jpg.jpg",
                        container: gird
                    });
                }
                else {
                    cont.find(".lazy-loaded").lazyload({
                        placeholder: o.para("assetsPath") + "/icon_dark/jpg.jpg"
                    });
                }

                cont.off('click', '.rename-folder');
                cont.on('click', '.rename-folder', function () { //修改文件夹名称
                    var _this = $(this);

                    var file_container = _this.parent().parent().parent();
                    var file_title = file_container.find('h4');
                    var old_name = $.trim(file_title.text());

                    bootbox.prompt({
                        buttons: {
                            confirm: {
                                label: '确定',
                            },
                            cancel: {
                                label: "取消"
                            }
                        },
                        title: "修改文件夹名称",
                        message: '',
                        callback: function (name) {
                            if (name !== null) {
                                name = fix_filename(name).replace('.', '');
                                if (name == '') {
                                    alert("请输入文件夹的名称");
                                    return false;
                                }
                                if (name != old_name) {
                                    executeAction(o, 'RenameFolder', _this.attr('data-id'), name, file_container, 'apply_folder_rename');
                                }
                            }
                        },
                        value: old_name
                    });
                });


                cont.off('click', '.rename-file');
                cont.on('click', '.rename-file', function () {  //修改文件名称
                    var _this = $(this);

                    var file_container = _this.parent().parent().parent();
                    var file_title = file_container.find('h4');
                    var old_name = $.trim(file_title.text());
                    bootbox.prompt({
                        buttons: {
                            confirm: {
                                label: '确定',
                            },
                            cancel: {
                                label: "取消"
                            }
                        },
                        title: "修改文件名称",
                        message: '',
                        callback: function (name) {
                            if (name !== null) {
                                name = fix_filename(name).replace('.', '');
                                if (name == '') {
                                    alert("请输入文件名称");
                                    return false;
                                }
                                if (name != old_name) {
                                    executeAction(o, 'RenameFile', _this.attr('data-id'), name, file_container, 'apply_file_rename');
                                }
                            }
                        },
                        value: old_name
                    });
                });

                cont.find('figure').off("mouseover");
                cont.find('figure').on('mouseover', function () {
                    $(this).find('.box:not(.no-effect)').animate({ top: "-26px" }, { queue: false, duration: 300 });
                });

                cont.find('figure').off("mouseout");
                cont.find('figure').on('mouseout', function () {
                    $(this).find('.box:not(.no-effect)').animate({ top: "0px" }, { queue: false, duration: 300 });
                });


                //图片预览
                cont.find(".preview").each(function () {
                    var _this = $(this), d = $$(this).data;
                    if (!d) return;
                    var s = d.source;
                    if (!d.isImg) return;
                    _this.attr("href", s);
                    _this.fancybox({
                        openEffect: 'elastic',
                        closeEffect: 'elastic',
                        title: d.name,
                        helpers: {
                            title: {
                                type: 'inside'
                            }
                        }
                    });
                });

                //文件选择
                if (o.getEvent("select")) {
                    cont.off("click", "a.link");
                    cont.on("click", "a.link", function (e) {
                        var d = $$($(this).find("img")[0]).data;
                        o.execEvent("onselect", [o, d, $(this)]);
                    });
                }
                cont.find('.tip').tooltip({ placement: "bottom" });
            }

            function context(o) {
                var _o = o, _path = [{ id: getRootId(o), name: '' }];
                var my = this;
                this.current = function () {
                    return _path.last()[0];
                }
                this.set = function (v) {
                    var index = -1;
                    _path.each(function (i, p) {
                        if (p.id == v.id) {
                            index = i;
                            return false;
                        }
                    });

                    if (index < 0) _path.push(v); //追加
                    else _path = _path.slice(0, index + 1); //截断
                    updateContext(_o, my);
                }
                this.path = function () {
                    return _path;
                }
                this.clear = function () {
                    _path = [_path[0]];
                    updateContext(_o, my);
                }
                this.pop = function () {
                    _path.pop();
                    updateContext(_o, my);
                }
                updateContext(_o, my);
            }

            function page(o) {
                var _index = 0, _size = o.para("pageSize") || 40, _count = 0;
                this.size = function () {
                    return _size;
                }
                this.count = function (v) { //页总数
                    if (!v) return _count;
                    _count = v;
                }

                this.index = function (v) {
                    if (empty(v)) return _index;
                    if (v < 0 || v >= _count) return false;
                    _index = v;
                    return true;
                }
                this.prev = function () {
                    if (_index <= 0) return false;
                    _index--;
                    return true;
                }
                this.next = function () {
                    if (_index >= (_count - 1)) return false;
                    _index++;
                    return true;
                }
                this.clear = function () {
                    _index = 0;
                }
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

            o.__inited = false;
        }

        function getRootId(o) {
            return o.para("rootId") || o.attr("data-rootId");
        }

        function init(o) {
            oj = o.getJquery();
            var exts = o.para("extensions");
            var allowed_ext = exts ? exts.split(',') : ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'tiff', 'svg', 'doc', 'docx', 'rtf', 'pdf', 'xls', 'xlsx', 'txt', 'csv', 'html', 'xhtml', 'psd', 'sql', 'log', 'fla', 'xml', 'ade', 'adp', 'mdb', 'accdb', 'ppt', 'pptx', 'odt', 'ots', 'ott', 'odb', 'odg', 'otp', 'otg', 'odf', 'ods', 'odp', 'css', 'ai', 'zip', 'rar', 'gz', 'tar', 'iso', 'dmg', 'mov', 'mpeg', 'mp4', 'avi', 'mpg', 'wma', 'flv', 'webm', 'mp3', 'm4a', 'ac3', 'aiff', 'mid', 'ogg', 'wav'];

            var uploadForm = oj.find("form[data-name='uploadForm']");
            o.uploadForm = uploadForm;
            uploadForm.prop("action", o.para("target"));
            uploadForm.myDropzone = new Dropzone(uploadForm[0], {
                dictInvalidFileType: "不允许上传的文件类型",
                dictFileTooBig: "上传文件的大小超过了允许的最大尺寸",
                dictResponseError: "服务器错误",
                paramName: "file", // The name that will be used to transfer the file
                maxFilesize: 100, // MB
                url: o.para("target"),
                accept: function (file, done) {
                    //var ext_img = new Array('jpg', 'jpeg', 'png', 'gif', 'bmp', 'tiff', 'svg');
                    var extension = file.name.split('.').pop();
                    extension = extension.toLowerCase();
                    if ($.inArray(extension, allowed_ext) > -1) {
                        done();
                    }
                    else {
                        done("不允许上传的文件类型");
                    }
                    this.paramName = file;
                }
                //,
                //sending: function (f) {
                //    this.options.paramName = f.name;
                //}
            });

            oj.find("a[data-name='btn-upload']").on('click', function () {
                var uploader = $(this).closest(".disk").find('.disk-uploader');
                uploader.show(500);
            });

            oj.find('.close-uploader').on('click', function () { //上传文件-返回文件列表按钮
                o = $(this).closest(".disk");
                var uploader = o.find('.disk-uploader');
                uploader.hide(500);
                o = $$(o);
                var drop = o.uploadForm.myDropzone;
                if (drop.getAcceptedFiles().length > 0) {
                    drop.removeAllFiles();
                    setTimeout(function () {
                        o.load();
                    }, 420);
                }
            });

            oj.find('.new-folder').on('click', function () {  //创建文件夹
                bootbox.prompt({
                    buttons: {
                        confirm: {
                            label: '确定',
                        },
                        cancel: {
                            label: "取消"
                        }
                    },
                    title: "创建文件夹",
                    message: '',
                    callback: function (name) {
                        if (name !== null) {
                            var folderId = o.context.current().id;
                            name = fix_filename(name).replace('.', '');
                            if (name == '') {
                                alert("请输入文件夹的名称");
                                return false;
                            }
                            return executeAction(o, 'CreateFolder', folderId, name, '', 'apply_folder_create');
                        }
                    }
                });
            });

            oj.find(".disk-filter-btn").on("click", function () {
                var val = oj.find(".disk-filter-input").val().toLowerCase();
                oj.find("ul.grid li:not(li[data-name='disk-back'])").each(function () {
                    var _this = $(this);
                    if (val != "" && _this.attr('data-name').toString().toLowerCase().indexOf(val) == -1) {
                        _this.hide(100);
                    } else {
                        _this.show(100);
                    }
                });
            });

            oj.find("li[data-name='disk-back']").on("click", function () {
                o.context.pop();
                o.page.clear();
                o.load();
            });

            if (o.para("height")) {
                var gird = oj.find("ul.grid");
                gird.css({ height: o.para("height"), "overflow": "auto" });
            }

            //右键菜单
            var copy_count = 0;
            oj.contextMenu({
                selector: 'figure:not(.back-directory)',
                autoHide: true,
                build: function ($trigger) {
                    $trigger.addClass('selected');
                    var options = {
                        callback: function (key, options) {
                            switch (key) {
                                case "copy_url":
                                    copy_count++;
                                    var m = $trigger.find('a.link').attr('data-preview');//encodeURL(m)

                                    bootbox.alert({
                                        buttons: {
                                            ok: {
                                                label: '确定',
                                            }
                                        },
                                        title: "文件地址",
                                        message: '<input class="bootbox-input bootbox-input-text form-control" id="url_text' + copy_count + '" type="text" style="height:30px;" value="' + m + '" />',
                                    });
                                    break;
                                case "move_file":
                                    move_file($trigger);
                                    break;
                            }
                        },
                        items: {}
                    };

                    options.items.copy_url = { name: "查看地址", icon: "url", disabled: false };
                    if (!$trigger.hasClass('directory')) {
                        options.items.move_file = { name: "移动文件", icon: "cut", disabled: false };
                    }

                    options.items.sep = '----';
                    options.items.info = { name: "<strong>详细信息</strong>", disabled: true };
                    options.items.name = { name: $trigger.attr('data-name'), icon: 'label', disabled: true };
                    options.items.size = { name: $trigger.find('.file-size').html(), icon: "size", disabled: true };
                    return options;
                },
                events: {
                    hide: function (opt) {
                        $('figure').removeClass('selected');
                    }
                }
            });

            $(document).on('contextmenu', function (e) {
                if (!$(e.target).is("figure"))
                    return false;
            });

            o.select_folder_modal = oj.find("*[data-name='selectFolders']");
            o.select_folder_list = oj.find("div[data-name='selectFoldersList']");

            var select_folder_paras = {};
            o.select_folder_modal.on("shown.bs.modal", function (e) {
                $$(o.select_folder_list[0]).load({ url: o.para("target"), paras: select_folder_paras });
            });

            function move_file(container) {
                var fileId = container.find('.rename-file').attr("data-id");

                select_folder_paras.rootId = getRootId(o);
                select_folder_paras.folderId = o.context.current().id;

                var list = $$(o.select_folder_list[0]);
                list.on("load", function () {
                    var j = list.getJquery();
                    j.off("click.select", "*[data-name='select']");
                    j.on("click.select", "*[data-name='select']", function (e) {
                        var a = $$(this), ad = a.data, targetId = ad.folderId;
                        execMoveFile(fileId, targetId, container);
                    });
                })
                o.select_folder_modal.modal('show');
            }

            function execMoveFile(fileId, folderId, container) {
                var req = new $$request();
                req.add("fileId", fileId);
                req.add("folderId", folderId);
                req.success = function (r) {
                    container.parent().hide();
                    o.select_folder_modal.modal('hide');
                }
                req.getData = function (arg, paras) { return paras; };
                req.submit({ url: o.para("target"), action: "MoveFileCommand" });
            }
        }

        function updateContext(o, ctx) { //更新上下文环境
            var uploadForm = o.uploadForm;
            uploadForm.find("input[name='folderId']").val(ctx.current().id);
        }

    }

    //function show_animation(oj) {
    //    oj.find("div[data-name='loading_container']").css('display', 'block');
    //    oj.find("div[data-name='loading']").css('opacity', '.7');
    //}


    //function hide_animation(oj) {
    //    oj.find("div[data-name='loading_container']").fadeOut();
    //}

    function executeAction(o, action, id, name, container, function_name) {
        if (!name) name = '';
        name = fix_filename(name);
        var req = new $$request();
        req.add("id", id);
        req.add("name", name.replace('/', ''));
        req.success = function (r) {
            $disk.__callbacks[function_name](o, container, name);
        };
        req.getData = function (arg, paras) { return paras; };
        req.submit({ url: o.para("target"), action: action + "Command" });
    }

    function fix_filename(stri) {
        if (stri != null) {
            //if ($('#transliteration').val() == "true") {
            //    stri = replaceDiacritics(stri);
            //    stri = stri.replace(/[^A-Za-z0-9\.\-\[\]\ \_]+/g, '');
            //}
            //if ($('#convert_spaces').val() == "true") {
            //    stri = stri.replace(/ /g, $('#replace_with').val());
            //    stri = stri.toLowerCase();
            //}
            stri = stri.replace('"', '');
            stri = stri.replace("'", '');
            stri = stri.replace("/", '');
            stri = stri.replace("\\", '');
            stri = stri.replace(/<\/?[^>]+(>|$)/g, "");
            return $.trim(stri);
        }
        return null;
    }

    var callbacks = $disk.__callbacks = {};

    callbacks.apply_file_delete = function (o, container) {
        container.parent().hide();
    }

    callbacks.apply_folder_create = function (o, container, name) {
        setTimeout(function () {
            o.load();
        }, 300);
    }

    callbacks.apply_folder_rename = function (o, container, name) {
        container.attr('data-name', name);
        container.find('figure').attr('data-name', name);
        container.find('h4 > a').text(name);
    }

    callbacks.apply_file_rename = function (o, container, name) {
        container.attr('data-name', name);
        container.parent().attr('data-name', name);
        container.find('h4').find('a').text(name);
    }


    callbacks.apply_folder_delete = function (o, container) {
        container.hide();
    }

    $disk.onbindFile = function (o, d) {
        var oj = o.getJquery();
        if (oj.is('li')) {
            oj.prop("class", "ff-item-type-" + d.extCode + " file");
        }
        else if (oj.is('figure')) {
            oj.prop("data-type", (d.isImg ? 'img' : 'file'));
        }
        else if (oj.is('div') && oj.prop('class') == 'filetype') {
            if (!d.isImg) oj.show(); else oj.hide(); //非图片文件，显示此项
        }
        else if (oj.is('div') && oj.prop('class') == 'cover') {
            if (!d.isImg) oj.show(); else oj.hide(); //非图片文件，显示此项
        }
        else if (oj.is('a') && oj.prop('class') == 'tip preview') {
            if (d.isImg) oj.show(); else oj.hide();
        }
        else if (oj.is('a') && oj.prop('class') == 'preview disabled') {
            //if (!d.isImg) o.show(); else o.hide(); //非图片文件，显示此项
            oj.hide();
        }
        else if (oj.is("img")) {
            if (!d.isImg) {
                var r = $$(oj.closest(".disk"));
                var thumb = r.para("assetsPath") + "/icon/" + d.ext + ".jpg";
                oj.attr("data-original", thumb);
                o.data.thumb = thumb;
            }
        }
    }
    $disk.onbindFolder = function (o, d) {
        o = o.getJquery();
        if (o.is('a') && o.prop('class') == 'folder-link') {
            o.off("click.disk");
            o.on("click.disk", function () {
                var disk = $$($(this).closest("div.disk")[0]);
                disk.load({ id: d.id, name: d.name }, { index: 0 });
            });

        }
    }

    $disk.painter = function (methods) { //methods自定义方法
        $o.inherit(this, $component.painter, methods);
        var my = this;
    }

    $disk.create = function (painter) {
        if (!painter) painter = new $disk.painter();
        return new $disk(painter);
    }

    Dropzone.autoDiscover = false;
});