using System.Threading;
using WaxRentals.Monitoring.Extensions;

namespace WaxRentalsWeb.Data
{
    public class VolatileDecimal
    {

        private decimal _value = 0;
        private readonly ReaderWriterLockSlim _deadbolt = new();

        public decimal Value
        {
            get
            {
                var @this = this;
                return _deadbolt.SafeRead(() => @this._value);
            }
            set
            {
                var @this = this;
                _deadbolt.SafeWrite(() => @this._value = value);
            }
        }

    }
}
