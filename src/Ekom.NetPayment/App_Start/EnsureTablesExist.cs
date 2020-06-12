using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;

namespace Ekom.NetPayment.App_Start
{
    class MigrationCreateTables : MigrationBase
    {
        readonly ILogger _logger;

        public MigrationCreateTables(
            IMigrationContext context,
            ILogger logger)
            : base(context)
        {
            _logger = logger;
        }

        public override void Migrate()
        {
            if (!TableExists("customNetPaymentOrder"))
            {
                _logger.Debug<MigrationCreateTables>("Creating customNetPaymentOrder table");

                Create.Table<OrderStatus>().Do();
            }
            if (!TableExists("customNetPayments"))
            {
                _logger.Debug<MigrationCreateTables>("Creating customNetPayments table");

                Create.Table<PaymentData>().Do();
            }
        }
    }

    class TranslationMigrationPlan : MigrationPlan
    {
        public TranslationMigrationPlan()
            : base("NetPayment")
        {
            From(string.Empty)
                .To<MigrationCreateTables>("first-migration");
        }
    }

    class EnsureTablesExist : IComponent
    {
        private readonly IScopeProvider scopeProvider;
        private readonly IMigrationBuilder migrationBuilder;
        private readonly IKeyValueService keyValueService;
        private readonly ILogger logger;

        public EnsureTablesExist(
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
            logger.Debug<EnsureTablesExist>("Ensuring NetPayment db tables exist");

            // perform any upgrades (as needed)
            var upgrader = new Upgrader(new TranslationMigrationPlan());
            upgrader.Execute(scopeProvider, migrationBuilder, keyValueService, logger);
            logger.Debug<EnsureTablesExist>("Done");
        }

        public void Terminate()
        {
        }
    }
}
