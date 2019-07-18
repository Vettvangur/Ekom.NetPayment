using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;

namespace Umbraco.NetPayment.App_Start
{
    class MigrationCreateTables : MigrationBase
    {
        public MigrationCreateTables(IMigrationContext context)
            : base(context)
        { }

        public override void Migrate()
        {
            if (!TableExists("customNetPaymentOrder"))
            {
                Current.Logger.Debug<MigrationCreateTables>("Creating customNetPaymentOrder table");

                Create.Table<OrderStatus>().Do();
            }
            if (!TableExists("customNetPayments"))
            {
                Current.Logger.Debug<MigrationCreateTables>("Creating customNetPayments table");

                Create.Table<PaymentData>().Do();
            }
        }
    }

    class TranslationMigrationPlan : MigrationPlan
    {
        public TranslationMigrationPlan()
            : base("MyApplicationName")
        {
            From(string.Empty)
                .To<MigrationCreateTables>("first-migration");
        }
    }

    class EnsureTableExists : IComponent
    {
        private readonly IScopeProvider scopeProvider;
        private readonly IMigrationBuilder migrationBuilder;
        private readonly IKeyValueService keyValueService;
        private readonly ILogger logger;

        public EnsureTableExists(
            IScopeProvider scopeProvider,
            IMigrationBuilder migrationBuilder,
            IKeyValueService keyValueService,
            ILogger logger)
        {
            this.scopeProvider = scopeProvider;
            this.migrationBuilder = migrationBuilder;
            this.keyValueService = keyValueService;
            this.logger = logger;
        }

        public void Initialize()
        {
            logger.Debug<EnsureTableExists>("Ensuring NetPayment db tables exist");

            // perform any upgrades (as needed)
            var upgrader = new Upgrader(new TranslationMigrationPlan());
            upgrader.Execute(scopeProvider, migrationBuilder, keyValueService, logger);
            logger.Debug<EnsureTableExists>("Done");
        }

        public void Terminate()
        {
        }
    }
}
