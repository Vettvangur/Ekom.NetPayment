using NPoco;

namespace Umbraco.NetPayment
{
    class DatabaseFactory : IDatabaseFactory
    {
        readonly Settings _settings;
        public DatabaseFactory(Settings settings)
        {
            _settings = settings;
        }

        public Database GetDb()
        {
            return new Database(_settings.ConnectionStringName);
        }
    }

    /// <summary>
    /// Creates instances of the Petapoco <see cref="Database"/> class.
    /// Must be disposed of after use.
    /// </summary>
    public interface IDatabaseFactory
    {
        /// <summary>
        /// Creates instances of the Petapoco <see cref="Database"/> class.
        /// Must be disposed of after use.
        /// </summary>
        Database GetDb();
    }
}
