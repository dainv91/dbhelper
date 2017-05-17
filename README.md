# dbhelper
# Setup
## Add ConnectionString name LocalDB in App.config
## Change DB_HELPER config name to correct database

# Create instance obj IDbHelper
private static readonly IDbHelper _dbHelper = FactoryProducer.GetFactory("DB").GetDbHelper(ConfigHelper.GetDbHelperName());
