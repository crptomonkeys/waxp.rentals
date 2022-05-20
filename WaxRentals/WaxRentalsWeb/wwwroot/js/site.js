(function ($) {

  setTimeout(() => $('.menu .show-0').click(), 500);

  $('.buy-wax').on('click', () => {
    $('.menu .show-1').click();
    return false;
  });

})(jQuery);

