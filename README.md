dbhelper
=============
AKB.Common.Data is a library for connect to multiple database. It supports SQL Server, MySQL and continue...

# Setup
* Add a Connection String name as _LocalDB_ in App.config
* Change DB_HELPER config name to correct database
* Create instance obj IDbHelper
```cs
private IDbHelper _dbHelper = FactoryProducer.GetFactory("DB").GetDbHelper(ConfigHelper.GetDbHelperName());
```