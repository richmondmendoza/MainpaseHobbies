(function ($) {

    function LoadCartCount() {
        $.ajax({
            url: '/mainsite/getcartitemcount',
            type: 'get',
            data: {},
            success: function (result) {
                $('#cart-count').text(result.Count);
            }
        });
    }

    function DeleteCartItem(options) {

        var settings = $.extend({
            id: '',
            header: 'Confirm',
            message: 'Content here',
        }, options);

        $('#confirm-modal').modalConfirm({
            url: '/home/deletecart',
            message: settings.message,
            param1: settings.id,
            header: settings.header
        });
    }

    function AddToCart(options) {
        var settings = $.extend({
            limit: 1,
            value: 1,
            cardId: ''
        }, options);

        $.ajax({
            url: '/home/addtocart',
            type: 'POST',
            data: {
                quantity: settings.value,
                cardId: settings.cardId
            },
            success: function (result) {
                LoadCartCount();

                $('#message-modal').modalMessage({
                    message: result.Message
                });
            }
        });
    }

    // expose function globally if needed
    window.LoadCartCount = LoadCartCount;
    window.AddToCart = AddToCart;
    window.DeleteCartItem = DeleteCartItem;

})(jQuery);