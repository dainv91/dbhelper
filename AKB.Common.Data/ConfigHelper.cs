using System;
using System.Configuration;

namespace AKB.Common.Data
{
    public static class ConfigHelper
    {
        private const string CONN_STR_NAME = "LocalDB";

        public static string GetConnectionString()
        {
            string connStr = null;
            var conn = ConfigurationManager.ConnectionStrings[CONN_STR_NAME];
            if (conn == null)
            {
                connStr = ConfigurationManager.AppSettings[CONN_STR_NAME];
            }
            if (string.IsNullOrEmpty(connStr))
            {
                throw new Exception("Please define ConnectionString name as LocalDB in App.config / Web.config");
            }
            return connStr;
        }

        public static string GetDbHelperName()
        {
            var str = ConfigurationManager.AppSettings["DB_HELPER"];
            return string.IsNullOrEmpty(str) ? "MySQL" : str;
        }
    }
}
