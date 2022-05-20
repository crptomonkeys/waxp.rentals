(function ($) {

  setTimeout(() => $('.menu .show-0').click(), 500);

  $('.buy-wax').on('click', () => {
    $('.menu .show-1').click();
    return false;
  });

  $('body').on('click', '.clipboard', function copy() {
    $(this).toggleClass('fa-copy fa-check clipboard');
    navigator.clipboard.writeText($(this).data('copy'));
    setTimeout(() => $(this).toggleClass('fa-copy fa-check clipboard'), 500);
  });

})(jQuery);

