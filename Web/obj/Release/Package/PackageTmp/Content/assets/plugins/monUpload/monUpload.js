

(function ($) {
    let dt = new DataTransfer();

    $.fn.monImageUpload = function (options) {
        var settings = $.extend({
            browseLabel: '<label class="text-muted drag-text">Drag image here or</label><br><span class="text-blue cursor-pointer">Browse images</span>',
            footerLabel: 'Maximum upload file size: 5 MB',
            allowMultiple: false,
            fileUploadCountLimit: 5
        }, options || {});

        var action = generateUniqueDigit();
        var fileInput = $(this);
        var parentDiv = fileInput.parent();
        fileInput.attr('action', action);

        var newDiv = $('<div id="container-' + action + '" class="div-upload-view image-container"></div>');
        var imageDetailDiv = $('<div class="image-upload item-details-image"></div>');

        parentDiv.append(newDiv);
        newDiv.append(imageDetailDiv);

        if (settings.allowMultiple) {
            fileInput.attr('multiple', 'multiple');

            var inputListDiv = $('<div class="image-list-container d-flex image-list-creation-container"></div>');
            newDiv.append(inputListDiv);
        } else {
            fileInput.removeAttr('multiple');
        }

        var noImageDiv = $('<div class="nofile-' + action + '"></div>');
        imageDetailDiv.append(noImageDiv);

        var imgIconSGV = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 472.7 386.4" class="icon icon-35 text-grey-shaded"><path d="M392 0H81C36 0 0 36 0 81v224a81 81 0 0081 81h311c44 0 81-36 81-81V81c0-45-37-81-81-81zM42 81c0-21 18-39 39-39h311c21 0 39 18 39 39v101l-112 76c-10 7-23 7-33-1l-94-66a72 72 0 00-82 1l-68 48V81zm389 224c0 22-18 39-39 39H81c-21 0-39-17-39-39v-14l92-65c10-7 24-7 34 0l94 66a71 71 0 0081 1l88-60v72z"></path><path d="M301 83a56 56 0 100 113 56 56 0 000-113zm0 78a21 21 0 110-43 21 21 0 010 43z"></path></svg>';
        var footerIconSGV = '<svg version="1.1" id="Layer_1" xmlns="http://www.w3.org/2000/svg" x="0" y="0" viewBox="0 0 512 512" xml:space="preserve" class="icon icon-xs align-middle"><path d="M256 172.3c-11.3 0-20.4 9.2-20.4 20.4v195.9c0 11.3 9.2 20.4 20.4 20.4s20.4-9 20.4-20.4V192.7c0-11.4-9.2-20.4-20.4-20.4z"></path><path d="M256 0C114.9 0 0 114.9 0 256s114.8 256 256 256c141.1 0 256-114.9 256-256S397.1 0 256 0zm0 471.3c-118.7 0-215.3-96.6-215.3-215.3S137.3 40.7 256 40.7 471.3 137.3 471.3 256 374.7 471.3 256 471.3z"></path><circle cx="256.4" cy="121.7" r="21.9"></circle></svg>';
        var footerDiv = $('<div class="filesize-info">' + footerIconSGV + '<span class="text-xs text-muted"> ' + settings.footerLabel + '</span></div>');
        var addImageDiv = $('<div><div class="hv-centered"> <div>' + imgIconSGV + '</div><div class="font-small">' + settings.browseLabel + '</div></div><span id="upload-' + action + '" class="span-upload"></div></div>');

        noImageDiv.append(addImageDiv);
        noImageDiv.append(footerDiv);
        $("#upload-" + action).append(fileInput);

        fileInput.on('change', async function () {
            var imageListContainer = $("#container-" + action).find('.image-list-container');
            //imageListContainer.empty();
            var elements = "";
            var addImage = '<div class="image-list-item cursor-pointer add-image-section"><div class="dropdown ember-view btn-group h-100"><button class="btn file-upload add-download-img-btn" type="button"> <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" class="cursor-pointer icon align-text-top action-icons icon-xlg fill-dropdown-blue"><path d="M256 15C122.9 15 15 122.9 15 256s107.9 241 241 241 241-107.9 241-241S389.1 15 256 15zm122 263H278v100c0 12.2-9.8 22-22 22s-22-9.8-22-22V278H134c-12.2 0-22-9.8-22-22s9.8-22 22-22h100V134c0-12.2 9.8-22 22-22s22 9.8 22 22v100h100c12.2 0 22 9.8 22 22s-9.8 22-22 22z"></path><path fill="#FFF" d="M378 234H278V134c0-12.2-9.8-22-22-22s-22 9.8-22 22v100H134c-12.2 0-22 9.8-22 22s9.8 22 22 22h100v100c0 12.2 9.8 22 22 22s22-9.8 22-22V278h100c12.2 0 22-9.8 22-22s-9.8-22-22-22z"></path></svg></button> <!----></div></div>';

            if (this.files.length > 0) {
                for (const file of this.files) {
                    dt.items.add(file);
                }

                this.files = dt.files;

                if ($(this).attr('multiple') !== "undefined") {

                    var item_elements = imageListContainer.find(".image-list-item").not(".add-image-section");
                    for (const elem of item_elements) {
                        elem.remove();
                    }

                    if (imageListContainer.length) {

                        var fileCounter = 0;

                        for (const file of this.files) {
                            fileCounter = fileCounter + 1;

                            if (file) {
                                await new Promise((resolve) =>
                                    fileToBase64(file, function (base64) {
                                        var itemAction = generateUniqueDigit("image-item-");
                                        var element = '<div class="image-list-item cursor-pointer" data-image-action="' + itemAction + '"><div id="ember454" class="ember-view"><div class=" hv-centered"><!----><img class="show-img img-fluid image-' + itemAction + '" src="' + base64 + '" alt=""></div></div></div>';

                                        if (fileCounter <= settings.fileUploadCountLimit) {
                                            imageListContainer.append(element);

                                            if (imageListContainer.find('.add-image-section').length) {
                                                imageListContainer.find('.add-image-section').appendTo(imageListContainer);
                                            } else {
                                                imageListContainer.append(addImage);
                                                imageListContainer.find('.add-download-img-btn').append(fileInput);
                                            }
                                        }

                                        resolve(base64);
                                    })
                                ).then((ret) => {

                                    var image_preview = imageDetailDiv.find("#preview-" + action);
                                    imageListContainer.find(".image-list-item:not(.add-image-section)").first().addClass("image-highlight");

                                    if (image_preview.length == 0) {
                                        imagePreview(ret, action);
                                    }

                                    elements = elements + ret;
                                    if (fileCounter >= settings.fileUploadCountLimit) {
                                        imageListContainer.find('.add-image-section').hide();
                                    }
                                });
                            }
                        }



                    }
                }
            }
        });

        $(document).on('click', '.image-list-item:not(.add-image-section)', function () {
            $('.image-list-item').removeClass("image-highlight");
            $(this).addClass("image-highlight");

            var base64 = $(this).find("img").attr("src");
            imagePreview(base64, action);
        });



    }
})(jQuery);





function generateUniqueDigit(checkname) {
    const digits = []

    while (digits.length < 3) {
        const random = Math.floor(Math.random() * 10);
        digits.push(random);

        if (digits.length == 3) {
            var id = checkname + digits.join("");
            if ($("#" + id).length) {
                digits = [];
            }
        }
    }

    return digits.join("");
}

function fileToBase64(file, callback) {
    const reader = new FileReader();
    reader.onload = function (e) {
        callback(e.target.result);
    };

    reader.readAsDataURL(file);
}

function imagePreview(base64, action) {
    var container = $("#container-" + action).find(".item-details-image");
    var toRemove = container.find(".nofile-" + action);
    var previewToRemove = container.find("#preview-" + action);

    if (toRemove.length) {
        toRemove.remove();
    }

    if (previewToRemove.length) {
        previewToRemove.remove();
    }

    var imagePreviewContainer = $('<div id="preview-' + action + '"></div>');
    container.append(imagePreviewContainer);

    var imagePreviewContent = $('<div class="preview-img cursor-pointer"></div>');
    var previewImageFooter = $('<div></div>');
    imagePreviewContainer.append(imagePreviewContent);
    imagePreviewContainer.append(previewImageFooter);

    var previewImage = $('<div class="hv-centered"><img class="show-img img-fluid" src="' + base64 + '" alt=""></div>');
    var previewImageOverlay = $('<div class="image-overlay"></div>');
    var previewImageOverlayDetails = $('<div class="image-overlay-details v-centered"></div>');
    var svgMagnifyingGlass = '<svg version="1.1" id="Layer_1" xmlns="http://www.w3.org/2000/svg" x="0" y="0" viewBox="0 0 512 512" xml:space="preserve" class="icon icon-lg fill-white"><path d="M283.9 186.4h-64.6l-.4-71.1c-.1-8.8-7.2-15.9-16-15.9h-.1c-8.8.1-16 7.3-15.9 16.1l.4 70.9h-64.4c-8.8 0-16 7.2-16 16s7.2 16 16 16h64.6l.4 71.1c.1 8.8 7.2 15.9 16 15.9h.1c8.8-.1 16-7.3 15.9-16.1l-.4-70.9h64.4c8.8 0 16-7.2 16-16s-7.1-16-16-16z"></path><path d="M511.3 465.3L371.2 325.2c-1-1-2.6-1-3.6 0l-11.5 11.5c31.6-35.9 50.8-82.9 50.8-134.3C406.9 90.3 315.6-1 203.4-1 91.3-1 0 90.3 0 202.4s91.3 203.4 203.4 203.4c51.4 0 98.5-19.2 134.3-50.8l-11.5 11.5c-1 1-1 2.6 0 3.6l140.1 140.1c1 1 2.6 1 3.6 0l41.4-41.4c.9-.9.9-2.5 0-3.5zm-307.9-92.5C109.5 372.8 33 296.4 33 202.4S109.5 32.1 203.4 32.1s170.4 76.4 170.4 170.4-76.4 170.3-170.4 170.3z"></path></svg>';
    

    imagePreviewContent.append(previewImage);
    imagePreviewContent.append(previewImageOverlay);
    imagePreviewContent.append(previewImageOverlayDetails);
    previewImageOverlayDetails.append(svgMagnifyingGlass);



}

function onImageAdd() {

}