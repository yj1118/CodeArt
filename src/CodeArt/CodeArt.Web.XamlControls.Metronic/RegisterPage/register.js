//== Class Definition
var SnippetRegister = function() {

    var login = $('#m_login');

    var showErrorMsg = function(form, type, msg) {
        var alert = $('<div class="m-alert m-alert--outline alert alert-' + type + ' alert-dismissible" role="alert">\
			<button type="button" class="close" data-dismiss="alert" aria-label="Close"></button>\
			<span></span>\
		</div>');

        form.find('.alert').remove();
        alert.prependTo(form);
        mUtil.animateClass(alert[0], 'flipInX animated');
        alert.find('span').html(msg);
    }

    //== Private Functions

    var handleSignUpFormSubmit = function () {

        var coding = false;
        $("#m_login_signup_code").click(function (e) {
            e.preventDefault();
            var mobileNumber = $("#username").val();
            if (!mobileNumber) {
                $$('#registerForm').showError("请输入手机号");
                return;
            }

            if (coding) return;
            coding = true;
            var btn = $(this), interval, count = 60;
            btn.attr('disabled', true);
            btn.text(count + "秒后重新获取");
            interval = setInterval(() => {
                count--;
                if (count <= 0) {
                    clearInterval(interval);
                    btn.attr('disabled', false);
                    btn.text("获取验证码");
                    coding = false;
                }
                else {
                    btn.text(count + "秒后重新获取");
                }
            }, 1000);

            var sender = {
                data: {
                    mobileNumber: mobileNumber
                }
            }

            
            var view = new $$view(sender);
            view.error = function (r) {
                $$('#registerForm').showError(r);
            };

            view.beforeSend = function () {

            }

            view.submit({ action: 'GetVerificationCode' });

        });

        $('#m_login_signup_submit').click(function(e) {
            e.preventDefault();

            var btn = $(this);
            var form = $(this).closest('form');

            form.validate({
                rules: {
                    username: {
                        required: true
                    },
                    code: {
                        required: true,
                    },
                    password: {
                        required: true,
                    },
                    rpassword: {
                        required: true,
                        equalTo: "#register_password"
                    },
                    agree: {
                        required: true
                    }
                },
                messages: {
                    username: {
                        required: $$strings.EnterUsername
                    },
                    code: {
                        required: "请输入验证码",
                    },
                    email: {
                        required: $$strings.EnterEmail,
                        email: $$strings.EmailError
                    },
                    password: {
                        required: $$strings.EnterPassword
                    },
                    rpassword: {
                        required: $$strings.EnterConfirmationPassword,
                        equalTo: $$strings.TwicePasswordDifferent
                    },

                },
            });

            if (!form.valid()) {
                return;
            }

            btn.addClass('m-loader m-loader--right m-loader--light').attr('disabled', true);
            $('#m_login_signup_cancel').attr('disabled', true);

            var view = new $$view("register");
            view.error = function (r) {
                $$('#registerForm').showError(r);
            };

            view.beforeSend = function () {
               
            }

            view.submit({ action: 'Register' });
        });

        $$('#registerForm').showError = function (msg) {
            var form = this.getJquery();
            showErrorMsg(form, 'danger', msg);

            $('#m_login_signup_submit').removeClass('m-loader m-loader--right m-loader--light').attr('disabled', false);
            $('#m_login_signup_cancel').attr('disabled', false);
        }

            //form.ajaxSubmit({
            //    url: '',
            //    success: function(response, status, xhr, $form) {
            //    	// similate 2s delay
            //    	setTimeout(function() {
	           //         btn.removeClass('m-loader m-loader--right m-loader--light').attr('disabled', false);
	           //         form.clearForm();
	           //         form.validate().resetForm();

	           //         // display signup form
	           //         displaySignInForm();
	           //         var signInForm = login.find('.m-login__signin form');
	           //         signInForm.clearForm();
	           //         signInForm.validate().resetForm();

	           //         showErrorMsg(signInForm, 'success', 'Thank you. To complete your registration please check your email.');
	           //     }, 2000);
            //    }
            //});
        //});
    }


    //== Public Functions
    return {
        // public functions
        init: function() {
            handleSignUpFormSubmit();
        }
    };
}();

//== Class Initialization
jQuery(document).ready(function () {
    SnippetRegister.init();
});