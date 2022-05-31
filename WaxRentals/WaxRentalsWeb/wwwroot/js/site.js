(function ($) {

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

  $('body').on('click', '.wax-account-link', function open() {
    const account = $(this).data('account');
    if (account) {
      window.open('https://wax.bloks.io/account/' + account);
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

  $('body').on('click', '.external-link', function open() {
    const url = $(this).data('url');
    if (url) {
      window.open(url);
    }
  });

})(jQuery);

// ================
//    LOCAL STORAGE
// ================

const storage = {

  addresses: value => {
    if (value === undefined) {
      const stored = localStorage.getItem('MyRentals');
      return stored === null ? [] : JSON.parse(stored);
    } else {
      localStorage.setItem('MyRentals', JSON.stringify(value));
    }
  },

  remove: address => {
    if (address !== undefined && address !== null) {
      const value = storage.addresses();
      const index = value.indexOf(address.toLowerCase());
      if (index > -1) {
        value.splice(index, 1);
        storage.addresses(value);
      }
    }
    return storage.addresses();
  },

  add: address => {
    if (address !== undefined && address !== null) {
      const value = storage.addresses();
      if (!value.includes(address.toLowerCase())) {
        value.push(address.toLowerCase());
        storage.addresses(value);
      }
    }
    return storage.addresses();
  },

  addAll: addresses => {
    const value = storage.addresses();
    addresses.forEach(address => {
      if (!value.includes(address.toLowerCase())) {
        value.push(address.toLowerCase());
      }
    });
    storage.addresses(value);
    return storage.addresses();
  }

};

// ================
//    INITIAL LOAD
// ================

const load = {

  bananoAddress: function (address) {
    this._my.fetchRentals([address], false);
    this._my.fetchRentalDetails(address);
    this._load();
  },

  waxAccount: function (account) {
    this._my.fetchWaxRentals(account);
    this._load();
  },

  details: function (address) {
    this._my.fetchRentals(storage.addresses(), true);
    this._my.fetchRentalDetails(address);
    this._load();
  },

  default: function () {
    this._load('.menu .show-0');
  },

  sell: function () {
    this._load('.menu .show-1');
  },

  get: function () {
    this._load('.menu .show-2');
  },

  my: function () {
    this._load('.menu .show-3');
  },

  recents: function () {
    this._load('.menu .show-4');
  },

  open: function () {
    this._load('.menu .show-5');
  },

  _load: tab => {
    $(tab ?? '.menu .show-3').click();

    $('.menu a.show-3').on('click', function clear() {
      load._my.display = null;
      load._my.detail = null;
      load._my.fetchRentals(storage.addresses(), true);
    });

    $('.menu a.show-5').on('click', function clear() {
      load._open.package = null;
      load._open.loading = false;
    });
  }

};
