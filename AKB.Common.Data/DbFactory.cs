namespace AKB.Common.Data
{
    public class DbFactory : IAbstractFactory
    {
        public IDbHelper GetDbHelper(string dbName, string connectionString = null)
        {
            if (string.IsNullOrEmpty(dbName)) return null;

            switch (dbName.ToUpper())
            {
                case "MYSQL":
                    return new MySqlDbHelper(connectionString);
                case "SQLSERVER":
                    return new SqlServerDbHelper(connectionString);
                default: return null;
            }
        }
    }
}
