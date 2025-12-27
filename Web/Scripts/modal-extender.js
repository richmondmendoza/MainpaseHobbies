
(function ($) {
    $.fn.modalForm = function (options) {
        var settings = $.extend({
            formId: '',
            beforeSend: null,
            success: null,
            withFileUpload: false,
            ajax: true,
            overlay: true,
            loading: true
        }, options || {});

        var thisId = "#" + $(this).attr('id');

        if (settings.ajax) {
            if (settings.withFileUpload === true) {
                var myModalWithFile = function () {
                    if (settings.beforeSend)
                        settings.beforeSend();

                    $('#' + settings.formId).ajaxForm({
                        beforeSend: function () {
                            if (settings.loading) {
                                // $('body').loading('start');
                            }
                            $(thisId).modal('hide');
                        },
                        success: function (data) {
                            $(thisId).modal('hide');
                            if (settings.loading) {
                                //$('body').loading('stop');
                            }
                            $('#' + settings.formId)[0].reset();
                            if (settings.success)
                                settings.success(data);

                        }
                    });
                };
                $.validator.unobtrusive.parse('#' + settings.formId);
                myModalWithFile();
            } else {
                var myModal = function () {

                    if (settings.overlay === false) {
                        $(this).on('shown.bs.modal', function () {
                            $(".modal-backdrop.in").hide();
                        })
                    }

                    if (settings.beforeSend)
                        settings.beforeSend();

                    $('#' + settings.formId).unbind("submit").submit(function () {

                        if ($(this).valid()) {
                            $.ajax({
                                type: "Post",
                                url: $('#' + settings.formId).attr('action'),
                                data: $('#' + settings.formId).serialize(),
                                beforeSend: function () {
                                    if (settings.loading) {
                                        //$('body').loading('start');
                                    }
                                    $(thisId).modal('hide');
                                },
                                success: function (data) {
                                    $(thisId).modal('hide');
                                    if (settings.loading) {
                                        //$('body').loading('stop');
                                    }
                                    $('#' + settings.formId)[0].reset();
                                    if (settings.success)
                                        settings.success(data);
                                }
                            });
                        }
                        return false;
                    });
                };

                myModal();
            }
        }



        $(thisId).modal('show');
    };
})(jQuery);

(function ($) {
    $.fn.modalConfirm = function (options) {
        var settings = $.extend({
            url: '',
            form: '',
            otherFormToSubmit: '',
            beforeSend: null,
            header: 'Confirm',
            message: null,
            width: '450px',
            ajax: false,
            success: null,
            param1: '',
            param2: '',
            param3: '',
            param4: '',
            overlay: true,
        }, options || {});

        var thisId = "#" + $(this).attr('id');

        //if (settings.url === '' && settings.form === '') {
        //    alert('Action Url or Form is missing');
        //    return;
        //}

        $(thisId).find('.modal-title').html(settings.header);
        $(thisId).find('form').attr('action', settings.url);
        if (settings.message != null) {
            $(thisId).find('.modal-body').html(settings.message);
        }

        $(thisId).find('#Param1').val(settings.param1);
        $(thisId).find('#Param2').val(settings.param2);
        $(thisId).find('#Param3').val(settings.param3);
        $(thisId).find('#Param4').val(settings.param4);

        $(thisId).find('#Remarks').removeAttr('readonly').val('');

        if (settings.overlay === false) {
            $(this).on('shown.bs.modal', function () {
                $(".modal-backdrop.in").hide();
            })
        }

        if (settings.ajax) {
            var myModal = function () {

                if (settings.beforeSend)
                    settings.beforeSend();

                $(thisId).find('form').attr('url', settings.url);
                $(thisId).find('form').ajaxForm({
                    beforeSend: function () {
                        $(thisId).modal('hide');
                    },
                    success: function (data) {
                        if (settings.success)
                            settings.success(data);

                    }
                });
            };


            myModal();

        }
        else if (settings.otherFormToSubmit !== '') {

            $(thisId).find('.btn-confirm').off('click').on('click', function (e) {
                e.preventDefault();

                const $form = $('#' + settings.otherFormToSubmit);

                // Remove validation attributes temporarily
                $form.find('[required], [pattern]').each(function () {
                    $(this).data('required', $(this).attr('required')).removeAttr('required');
                    $(this).data('pattern', $(this).attr('pattern')).removeAttr('pattern');
                });

                // Optional: skip client-side validation
                const formElement = $form[0];
                formElement.action = settings.url;
                formElement.submit(); // native DOM method = no validation
            });
        }

        $(thisId).modal('show');

    };
})(jQuery);



(function ($) {
    $.fn.modalConfirmWithComments = function (options) {
        var settings = $.extend({
            url: '',
            beforeSend: null,
            header: 'Confirm',
            message: null,
            width: '450px',
            ajax: false,
            requireRemarks: false,
            success: null,
            param1: '',
            param2: '',
            param3: '',
            param4: '',
            overlay: true,
        }, options || {});

        var thisId = "#" + $(this).attr('id');

        if (settings.url === '') {
            alert('Action Url is missing');
            return;
        }

        $(thisId).find('.modal-title').html(settings.header);
        $(thisId).find('form').attr('action', settings.url);
        if (settings.message != null) {
            $(thisId).find('.modal-body').html(settings.message);
        }



        $(thisId).find('#Param1').val(settings.param1);
        $(thisId).find('#Param2').val(settings.param2);
        $(thisId).find('#Param3').val(settings.param3);
        $(thisId).find('#Param4').val(settings.param4);


        $(thisId).find('#Remarks').removeAttr('readonly').removeAttr('disabled').val('');
        $(thisId).find('#ReasonForExceeding').removeAttr('readonly').removeAttr('disabled').val('');

        

        if (!settings.requireRemarks) {
            $(thisId).find('#Remarks, #ReasonForExceeding')
                .prop('readonly', false)
                .removeAttr('required')
                .removeClass('input-validation-error is-invalid')
                .removeAttr('data-val-required data-val');


            $(thisId).find('#Remarks, #ReasonForExceeding').next('span').remove();

            $(thisId).find('form').removeData("validator");
            $(thisId).find('form').removeData("unobtrusiveValidation");
        }
        else {

            $(thisId).find('#Remarks, #ReasonForExceeding')
                .prop('required', true)
                .attr('data-val', 'true')
                .attr('data-val-required', 'Remarks is required');


            if ($(thisId).find('span[data-valmsg-for="Remarks"]').length === 0) {
                $(thisId).find('#Remarks').after('<span class="text-danger field-validation-valid" data-valmsg-for="Remarks" data-valmsg-replace="true"></span>');
            }
            if ($(thisId).find('span[data-valmsg-for="ReasonForExceeding"]').length === 0) {
                $(thisId).find('#Remarks').after('<span class="text-danger field-validation-valid" data-valmsg-for="ReasonForExceeding" data-valmsg-replace="true"></span>');
            }

            $(thisId).find('form').removeData("validator");
            $(thisId).find('form').removeData("unobtrusiveValidation");
            $.validator.unobtrusive.parse('#' + $(thisId).find('form').attr('id'));
        }



        if (settings.overlay === false) {
            $(this).on('shown.bs.modal', function () {
                $(".modal-backdrop.in").hide();
            })
        }

        if (settings.ajax) {
            var myModal = function () {

                if (settings.beforeSend)
                    settings.beforeSend();

                $(thisId).find('form').attr('url', settings.url);
                $(thisId).find('form').ajaxForm({
                    beforeSend: function () {
                        $(thisId).modal('hide');
                    },
                    success: function (data) {
                        if (settings.success)
                            settings.success(data);

                    }
                });
            };


            myModal();
        }

        $.validator.unobtrusive.parse($(thisId).find('form'));
        $(thisId).modal('show');
    };
})(jQuery);


(function ($) {
    $.fn.modalMessage = function (options) {
        var settings = $.extend({
            header: 'Message',
            message: 'Message Content',
            width: '500px',
            overlay: true
        }, options || {});

        var thisId = "#" + $(this).attr('id');

        $(thisId).find('.modal-title').html(settings.header);
        $(thisId).find('.modal-body').html(settings.message);
        if (settings.overlay === false) {
            $(this).on('shown.bs.modal', function () {
                $(".modal-backdrop.in").hide();
            })
        }

        $(thisId).modal('show');
    };
})(jQuery);


(function ($) {
    $.fn.modalView = function (options) {
        var settings = $.extend({
            width: '400px',
            overlay: true
        }, options || {});

        var thisId = "#" + $(this).attr('id');

        $(thisId).find('.modal-title').html(settings.header);
        $(thisId).find('.modal-body').html(settings.message);

        if (settings.overlay === false) {
            $(this).on('shown.bs.modal', function () {
                $(".modal-backdrop.in").hide();
            })
        }

        $(thisId).modal('show');

    };
})(jQuery);