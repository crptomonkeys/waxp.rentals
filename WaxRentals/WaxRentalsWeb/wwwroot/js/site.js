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

  $('body').on('click', '.wax-transaction-link', function open() {
    const hash = $(this).data('hash');
    if (hash) {
      window.open('https://wax.bloks.io/transaction/' + hash);
    }
  });

  $('body').on('click', '.banano-transaction-link', function open() {
    const hash = $(this).data('hash');
    if (hash) {
      window.open('https://yellowspyglass.com/hash/' + hash);
    }
  });

  $('body').on('click', '.banano-address-link', function open() {
    const address = $(this).data('address');
    if (address) {
      window.open('https://yellowspyglass.com/account/' + address);
    }
  });

})(jQuery);

