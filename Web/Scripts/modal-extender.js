
(function ($) {
    $.fn.modalForm = function (options) {
        var settings = $.extend({
            formId: '',
            beforeFormSubmitCallback: null,
            successCallback: null,
            withFileUpload: false,
            ajax: true,
            overlay: true,
            loading: true
        }, options || {});

        var thisId = "#" + $(this).attr('id');

        if (settings.ajax) {
            if (settings.withFileUpload === true) {
                var myModalWithFile = function () {
                    if (settings.beforeFormSubmitCallback)
                        settings.beforeFormSubmitCallback();

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
                            if (settings.successCallback)
                                settings.successCallback(data);

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

                    if (settings.beforeFormSubmitCallback)
                        settings.beforeFormSubmitCallback();

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
                                    if (settings.successCallback)
                                        settings.successCallback(data);
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
            beforeFormSubmitCallback: null,
            header: 'Confirm',
            message: 'Message Content',
            width: '450px',
            ajax: false,
            successCallback: null,
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
        $(thisId).find('.modal-body').html(settings.message);
        $(thisId).find('#form-confirm').attr('action', settings.url);

        $('#Param1').val(settings.param1);
        $('#Param2').val(settings.param2);
        $('#Param3').val(settings.param3);
        $('#Param4').val(settings.param4);



        if (settings.overlay === false) {
            $(this).on('shown.bs.modal', function () {
                $(".modal-backdrop.in").hide();
            })
        }

        if (settings.ajax) {
            var myModal = function () {
                if (settings.beforeFormSubmitCallback)
                    settings.beforeFormSubmitCallback();
                $('#form-confirm').attr('url', settings.url);
                $('#form-confirm').ajaxForm({
                    beforeSend: function () {
                        $(thisId).modal('hide');
                    },
                    success: function (data) {
                        if (settings.successCallback)
                            settings.successCallback(data);

                    }
                });
            };


            myModal();
        }
        $(thisId).modal('show');
    };
})(jQuery);


(function ($) {
    $.fn.modalMessage = function (options) {
        var settings = $.extend({
            header: 'Message',
            message: 'Message Content',
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