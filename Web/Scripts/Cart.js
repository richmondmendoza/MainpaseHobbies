

(function ($) {
    function AddToCart(options) {
        var settings = $.extend({
            limit: 1,
            value: 1,
            cardId: ''
        }, options);

        var count = parseInt($('#cart-count').text(), 10) || 0;

        $.ajax({
            url: '/home/addtocart',
            type: 'POST',
            data: {
                quantity: settings.value,
                cardId: settings.cardId
            },
            success: function (response) {
                if (response && response.Success) {
                    count += settings.value;
                    $('#cart-count').text(count);
                }
            }
        });
    }

    // expose function globally if needed
    window.AddToCart = AddToCart;

})(jQuery);