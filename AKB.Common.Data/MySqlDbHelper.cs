using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace AKB.Common.Data
{
    public class MySqlDbHelper : IDbHelper
    {
        public DbParameter CreateParameter(string name, object value)
        {
            if (value == null)
            {
                value = DBNull.Value;
            }

            return new MySqlParameter(name, value);
        }

        public DbConnection CreateConnection()
        {
            return new MySqlConnection(ConfigHelper.GetConnectionString());
        }

        public DbCommand CreateCommand(string query, DbConnection conn, DbTransaction tran)
        {
            DbCommand cmd = new MySqlCommand(query);
            cmd.Connection = conn;
            cmd.Transaction = tran;
            return cmd;
        }

        public DbDataAdapter CreateAdapter(DbCommand cmd)
        {
            DbDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            return da;
        }

        public string GetParameterSeperator()
        {
            return "?";
        }


        public void InsertBatch<T>(List<T> lst)
        {
            ParameterHelper.InsertBatch(this, lst);
        }

        public int ExecuteRawQuery(string query)
        {
            return ParameterHelper.ExecuteNonQuery(this, query);
        }

        public DataTable GetTable(string query, params KeyValuePair<string, object>[] parameters)
        {
            return ParameterHelper.GetTable(this, query, parameters);
        }


        #region Private methods

        #endregion
    }
}
