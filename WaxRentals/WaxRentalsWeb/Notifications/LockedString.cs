using System.Threading;
using WaxRentalsWeb.Extensions;

namespace WaxRentalsWeb.Notifications
{
    internal class LockedString
    {

        private string _value = null;
        private readonly ReaderWriterLockSlim _deadbolt = new();

        public string Value
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
