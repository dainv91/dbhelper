namespace AKB.Common.Data
{
    public class DbFactory : IAbstractFactory
    {
        public IDbHelper GetDbHelper(string dbName)
        {
            if (string.IsNullOrEmpty(dbName)) return null;

            switch (dbName.ToUpper())
            {
                case "MYSQL":
                    return new MySqlDbHelper();
                case "SQLSERVER":
                    return new SqlServerDbHelper();
                default: return null;
            }
        }
    }
}
