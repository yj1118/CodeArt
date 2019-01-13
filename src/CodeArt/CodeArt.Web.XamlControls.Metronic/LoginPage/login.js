//== Class Definition
var SnippetLogin = function() {

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

    var displaySignUpForm = function() {
        login.removeClass('m-login--forget-password');
        login.removeClass('m-login--signin');

        login.addClass('m-login--signup');
        mUtil.animateClass(login.find('.m-login__signup')[0], 'flipInX animated');
    }

    var displaySignInForm = function() {
        login.removeClass('m-login--forget-password');
        login.removeClass('m-login--signup');

        login.addClass('m-login--signin');
        mUtil.animateClass(login.find('.m-login__signin')[0], 'flipInX animated');
    }

    var displayForgetPasswordForm = function() {
        login.removeClass('m-login--signin');
        login.removeClass('m-login--signup');

        login.addClass('m-login--forget-password');
        mUtil.animateClass(login.find('.m-login__forget-password')[0], 'flipInX animated');
    }

    var handleFormSwitch = function() {
        $('#m_login_forget_password').click(function(e) {
            e.preventDefault();
            displayForgetPasswordForm();
        });

        $('#m_login_forget_password_cancel').click(function(e) {
            e.preventDefault();
            displaySignInForm();
        });

        $('#m_login_signup').click(function(e) {
            e.preventDefault();
            displaySignUpForm();
        });

        $('#m_login_signup_cancel').click(function(e) {
            e.preventDefault();
            displaySignInForm();
        });
    }

    var handleSignInFormSubmit = function() {
        $('#m_login_signin_submit').click(function(e) {
            e.preventDefault();
            var btn = $(this);
            var form = $(this).closest('form');

            form.validate({
                rules: {
                    username: {
                        required: true
                    },
                    password: {
                        required: true
                    }
                },

                messages: {
                    username: {
                        required: $$strings.EnterUsername
                    },
                    password: {
                        required: $$strings.EnterPassword
                    }
                },
            });

            if (!form.valid()) {
                return;
            }

            btn.addClass('m-loader m-loader--right m-loader--light').attr('disabled', true);

            var view = new $$view("login");
            view.error = function (r) {
                $$('#loginForm').showError(r);
            };

            view.beforeSend = function () {

            }

            view.submit({ action: 'Login' });



        });


        $$('#loginForm').showError = function (msg) {
            var form = this.getJquery();
            showErrorMsg(form, 'danger', msg);

            $('#m_login_signin_submit').removeClass('m-loader m-loader--right m-loader--light').attr('disabled', false);
        }

        $$('#loginForm').show= function (msg) {
            var form = this.getJquery();
            showErrorMsg(form, 'success', msg);

            $('#m_login_signup_submit').removeClass('m-loader m-loader--right m-loader--light').attr('disabled', false);
            $('#m_login_signup_cancel').attr('disabled', false);
        }

    }

    var handleSignUpFormSubmit = function() {
        $('#m_login_signup_submit').click(function(e) {
            e.preventDefault();

            var btn = $(this);
            var form = $(this).closest('form');

            form.validate({
                rules: {
                    username: {
                        required: true
                    },
                    email: {
                        required: true,
                        email: true
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

    var handleForgetPasswordFormSubmit = function() {
        $('#m_login_forget_password_submit').click(function(e) {
            e.preventDefault();

            var btn = $(this);
            var form = $(this).closest('form');

            form.validate({
                rules: {
                    email: {
                        required: true,
                        email: true
                    }
                }
            });

            if (!form.valid()) {
                return;
            }

            btn.addClass('m-loader m-loader--right m-loader--light').attr('disabled', true);

            form.ajaxSubmit({
                url: '',
                success: function(response, status, xhr, $form) { 
                	// similate 2s delay
                	setTimeout(function() {
                		btn.removeClass('m-loader m-loader--right m-loader--light').attr('disabled', false); // remove 
	                    form.clearForm(); // clear form
	                    form.validate().resetForm(); // reset validation states

	                    // display signup form
	                    displaySignInForm();
	                    var signInForm = login.find('.m-login__signin form');
	                    signInForm.clearForm();
	                    signInForm.validate().resetForm();

	                    showErrorMsg(signInForm, 'success', 'Cool! Password recovery instruction has been sent to your email.');
                	}, 2000);
                }
            });
        });
    }

    //== Public Functions
    return {
        // public functions
        init: function() {
            handleFormSwitch();
            handleSignInFormSubmit();
            handleSignUpFormSubmit();
            handleForgetPasswordFormSubmit();
        },
        displaySignInForm: displaySignInForm
    };
}();

//== Class Initialization
jQuery(document).ready(function () {
    SnippetLogin.init();
});