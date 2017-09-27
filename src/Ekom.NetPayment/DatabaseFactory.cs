using NPoco;

namespace Umbraco.NetPayment
{
    class DatabaseFactory
    {
        ISettings _settings;
        public DatabaseFactory(ISettings settings)
        {
            _settings = settings;
        }

        public IDatabase GetDb()
        {
            return new Database(_settings.ConnectionStringName);
        }
    }

    /// <summary>
    /// Creates instances of the NPoco <see cref="Database"/> class.
    /// Must be disposed of after use.
    /// </summary>
    public interface IDatabaseFactory
    {
        /// <summary>
        /// Creates instances of the NPoco <see cref="Database"/> class.
        /// Must be disposed of after use.
        /// </summary>
        IDatabase GetDb();
    }
}
