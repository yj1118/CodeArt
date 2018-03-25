var Login = function () {

    var handleLogin = function () {

        $('.login-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            rules: {
                username: {
                    required: true
                },
                password: {
                    required: true
                },
                remember: {
                    required: false
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

            invalidHandler: function (event, validator) { //display error alert on form submit
                var l = [];
                $(validator.errorList).each(function () {
                    l.push(this.message);
                });
                $(".alert-danger").find("span").text(l.join('；'));
                $('.alert-danger', $('.login-form')).show();
            },

            highlight: function (element) { // hightlight error inputs
                $(element)
                    .closest('.form-group').addClass('has-error'); // set error class to the control group
            },

            success: function (label) {
                label.closest('.form-group').removeClass('has-error');
                label.remove();
            },

            errorPlacement: function (error, element) {
                error.insertAfter(element.closest('.input-icon'));
            },

            submitHandler: function (form) {
                var view = new $$view();
                view.error = function (r) {
                    $(".alert-danger").find("span").text(r);
                    $('.alert-danger').show();
                };

                view.beforeSend = function () {
                    //$("#loginWrapper").addClass("flipped");
                }
                view.submit({ action: 'Login' });



                //var f = $$(form);
                //f.success = function (r) {
                //    if (r.status == "success") {
                //        f.reset();
                //        location.href = "/index.htm";
                //    } else {
                //        $(".alert-danger").find("span").text("用户名或密码错误");
                //        $('.alert-danger').show();
                //    };
                //};
                //f.error = function (r) {
                //    $(".alert-danger").find("span").text(r);
                //    $('.alert-danger').show();
                //};
                //f.submit({ action: 'Login' });
            }
        });

        $('.login-form input').keypress(function (e) {
            if (e.which == 13) {
                if ($('.login-form').validate().form()) {
                    $('.login-form').submit(); //form validation success, call ajax form submit
                }
                return false;
            }
        });
    }

    return {
        //main function to initiate the module
        init: function () {
            handleLogin();
        }

    };

}();