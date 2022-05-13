const formats = {
  currency: new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 0, maximumFractionDigits: 0 }),
  currencySmall: new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 2, maximumFractionDigits: 2 }),
  percent: new Intl.NumberFormat('en-US', { style: 'percent', signDisplay: 'always', minimumFractionDigits: 2, maximumFractionDigits: 2 }),
  units: new Intl.NumberFormat('en-US', { minimumFractionDigits: 4, maximumFractionDigits: 4 }),
};

const format = {
  currency: value => value < 10 ? formats.currencySmall.format(value) : formats.currency.format(value),
  percent: value => formats.percent.format(value / 100),
  units: value => formats.units.format(value)
}

const settings = {
  _bool: (name, value) => {
    return JSON.parse(settings._string(name, value));
  },
  _string: (name, value) => {
    if (value === null) {
      localStorage.removeItem(name);
    }
    else if (value !== undefined) {
      localStorage.setItem(name, value)
    }
    return localStorage.getItem(name);
  },

  ages      : show => settings._bool  ("settings.show.ages"      , show) ?? false     ,
  planned   : show => settings._bool  ("settings.show.planned"   , show) ?? true      ,
  kevin_only: show => settings._bool  ("settings.show.kevin_only", show) ?? false     ,
  footnotes : show => settings._bool  ("settings.show.footnotes" , show) ?? true      ,
  covered   : type => settings._string("settings.show.covered"   , type) ?? "FromZero",
  history   : show => settings._bool  ("settings.show.history"   , show) ?? true      ,
  charts    : show => settings._bool  ("settings.show.charts"    , show) ?? true      ,
}
