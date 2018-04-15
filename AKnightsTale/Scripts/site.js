$('#myCarousel').carousel({
    pause: true,
    interval: false
});

// handles the carousel thumbnails
$('[id^=carousel-selector-]').click(function () {
    var id_selector = $(this).attr("id");
    var id = id_selector.substr(id_selector.length - 1);
    id = parseInt(id);
    $('#myCarousel').carousel(id);
    $('[id^=carousel-selector-]').removeClass('selected');
    $(this).addClass('selected');
});

$('#myCarousel').bind('slide.bs.carousel', function (e) {
    var slideFrom = $(this).find('.active').index();
    var videoContent = $('.item[data-slide-number=' + slideFrom + '] .embed-responsive');
    videoContent.html(videoContent.html());
});

// when the carousel slides, auto update
$('#myCarousel').on('slid.bs.carousel', function (e) {
    var id = $('.item.active').data('slide-number');
    id = parseInt(id);
    $('[id^=carousel-selector-]').removeClass('selected');
    $('[id=carousel-selector-' + id + ']').addClass('selected');
});