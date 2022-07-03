namespace WaxRentals.Data.Manager
{
    public interface IDataFactory
    {

        public IInsert   Insert   { get; }
        public ILog      Log      { get; }
        public IProcess  Process  { get; }
        public ITrackWax TrackWax { get; }
        public IExplore  Explore  { get; }

    }
}