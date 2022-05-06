using System;

namespace WaxRentals.Monitoring
{
    public abstract class Updatable
    {

        public event EventHandler Updated;

        protected void RaiseEvent()
        {
            try
            {
                Updated?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Log errors.
            }
        }

    }
}
