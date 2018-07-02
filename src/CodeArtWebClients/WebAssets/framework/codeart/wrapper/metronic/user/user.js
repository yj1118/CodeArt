$$.createModule("Wrapper.metronic.user", function (api, module) {
    var metro = $$metronic = $$.wrapper.metronic;
    var util = api.util, empty = util.empty;
    
    var user = metro.user = {};

    (function () {
        user.profile = function () {
            this.give = function (o) {
                initPhoto(o);

                o.scriptElement = function () { //参与视图的能力

                    var o = this, oj = o.getJquery();
                    var value = {
                        userId: oj.attr("data-userId"),
                        photoId: oj.attr("data-photoId"),
                    };

                    return {
                        id: oj.attr("id"),
                        metadata: { value: value }
                    };
                }
            }
        }

        function submitChanged(o, cb) {
            var view = new $$view();
            view.append(o);

            view.success = function () {
                cb();
            }

            view.submit({ component: o.attr("name"), action: 'UpdateUser' });
        }

        function initPhoto(o) {
            var oj = o.getJquery(), btn = oj.find(".btn-user-photo");
            var modal = getModal(o, "photo");
            var cropper = $$(modal.find("div[data-name='cropper']"));
            cropper.config({
                width: oj.attr("data-photoWidth"),
                height: oj.attr("data-photoHeight"),
            });
            cropper.on("upload", function (file) {
                if (!file) {
                    modal.modal('hide');
                }
                else {
                    oj.attr("data-photoId", file.id);
                    submitChanged($$(oj), function () {
                        setImage(oj.find(".img-userpic"), { url: file.source, id: file.id });
                        modal.modal('hide');
                    });
                }
            });

            function setImage(g, d) {
                g = $(g);
                g.attr("src", d.url);
                $$(g).data = d;
            }

            btn.off("click.photo");
            btn.on("click.photo", function () {
                modal.modal({ backdrop: 'static' });
            });


            var uploadBtn = modal.find(".userprofile-cropper-btn-ok");
            uploadBtn.off("click.upload");
            uploadBtn.on("click.upload", function () {
                cropper.upload();
            });
        }

        function getModal(o, t) {
            var modalId = "#userProfileModal_" + t + "_" + o.attr("id");
            return $(modalId);
        }

    })();

    (function () {
        user.security = function () {
            this.give = function (o) {
                initSecurity(o);
            }
        }

        function setLevelText(oj, value) {
            var font = oj.find(".font-levelname");
            var result = getLevel(value);
            font.text(result.name);
            font.attr("color", result.color);
        }

        function getLevel(value) {
            var _value = value;
            //获取安全级别的信息
            var level = { value: _value * 100 }; //level.value是百分比
            if (level.value < 50) {
                level.color = "#f00f00";
                level.name = "低";
            } else if (level.value < 75) {
                level.color = "#89c4f4";
                level.name = "中";
            }
            else {
                level.color = "#45b6af";
                level.name = "高";
            }
            return level;
        }

        function initSecurity(o) {

            var oj = o.getJquery();
            var level = oj.attr("data-securityLevel");

            // 设置安全级别的文字和颜色
            var progbar = oj.find(".progbar-level");
            $$(progbar).update(level * 100);

            setLevelText(oj, level);

            // 各弹出Model
            var modalMobile = getModal(o, "mobile");
            var modalPassword = getModal(o, "password");

            var btnGetVC = modalMobile.find(".btn-mobileModel-getVC");
            btnGetVC.off("click.mobileModel.getvc");
            btnGetVC.on("click.mobileModel.getvc", function () {
                getVC();
            });

            function getVC() {

                var mobileNumber = modalMobile.find(".mobileModel-mobile");
                var result = ($$(mobileNumber)).validate(true);

                if (result.status() == "success") {
                    var view = new $$view("vc");

                    view.error = function (r) {
                        bootbox.alert({
                            buttons: {
                                ok: {
                                    label: '确认',
                                }
                            },
                            message: r
                        });
                    };

                    view.beforeSend = function () {
                        disabledGetVC(60);
                    }

                    view.submit({ action: 'GetSMSVC' });
                }
            }

            function disabledGetVC(waitCount) {

                if (!waitCount) waitCount = btnGetVC.waitCount;
                if (waitCount == 0) {
                    btnGetVC.html("获取验证码");
                    btnGetVC.removeAttr("disabled");
                    return;
                }
                btnGetVC.waitCount = --waitCount;
                btnGetVC.attr("disabled", "disabled");
                btnGetVC.html("（" + (waitCount + 1) + "秒）获取验证码");
                setTimeout(disabledGetVC, 1000);
            }

            // 各链接的事件
            var a_passwordSet = oj.find(".a-password-set");
            a_passwordSet.off("click.password.set");
            a_passwordSet.on("click.password.set", function () {
                $$(modalPassword).title("设置密码");
                modalPassword.modal({ backdrop: 'static' });
            });
            var a_passwordUpdate = oj.find(".a-password-update");
            a_passwordUpdate.off("click.password.update");
            a_passwordUpdate.on("click.password.update", function () {
                $$(modalPassword).title("修改密码");
                modalPassword.modal({ backdrop: 'static' });
            });

            var a_mobileSet = oj.find(".a-mobile-set");
            a_mobileSet.off("click.mobile.set");
            a_mobileSet.on("click.mobile.set", function () {
                $$(modalMobile).title("绑定手机");
                modalMobile.modal({ backdrop: 'static' });
            });
            var a_mobileUpdate = oj.find(".a-mobile-update");
            a_mobileUpdate.off("click.mobile.update");
            a_mobileUpdate.on("click.mobile.update", function () {
                $$(modalMobile).title("修改手机");
                modalMobile.modal({ backdrop: 'static' });
            });

            var a_emailSet = oj.find(".a-email-set");
            a_emailSet.off("click.email.set");
            a_emailSet.on("click.email.set", function () {
                $$bootbox.alert("设置邮箱功能暂时还没有开放!");
            });
            var a_emailUpdate = oj.find(".a-email-update");
            a_emailUpdate.off("click.email.update");
            a_emailUpdate.on("click.email.update", function () {
                $$bootbox.alert("修改邮箱功能暂时还没有开放!");
            });

            var a_questionSet = oj.find(".a-question-set");
            a_questionSet.off("click.question.set");
            a_questionSet.on("click.question.set", function () {
                $$bootbox.alert("设置密保问题功能暂时还没有开放!");
            });
            var a_questionUpdate = oj.find(".a-question-update");
            a_questionUpdate.off("click.question.update");
            a_questionUpdate.on("click.question.update", function () {
                $$bootbox.alert("修改密保问题功能暂时还没有开放!");
            });

        }

        function getModal(o, t) {
            var modalId = "#userSecurityModal_" + t + "_" + o.attr("id");
            return $(modalId);
        }

    })();



});