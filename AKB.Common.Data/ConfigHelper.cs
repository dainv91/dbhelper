using System;
using System.Configuration;

namespace AKB.Common.Data
{
    public static class ConfigHelper
    {
        public static string GetConnectionString()
        {
            //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //AppSettingsSection app = config.AppSettings;
            //app.Settings.Add("x", "this is X");
            //config.Save(ConfigurationSaveMode.Modified);
            //var keys = app.Settings.AllKeys;
            var conn = ConfigurationManager.ConnectionStrings["LocalDB"];
            if (string.IsNullOrEmpty(conn.ConnectionString))
            {
                throw new Exception("Please define ConnectionString name as LocalDB in App.config");
            }
            return conn.ConnectionString;
        }

        public static string GetDbHelperName()
        {
            var str = ConfigurationManager.AppSettings["DB_HELPER"];
            return string.IsNullOrEmpty(str) ? "MySQL" : str;
        }
    }
}
