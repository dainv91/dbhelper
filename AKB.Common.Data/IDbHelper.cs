using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using AKB.Common.Data.Attr;

namespace AKB.Common.Data
{
    public interface IDbHelper
    {
        /// <summary>
        /// Create parameter object using name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        DbParameter CreateParameter(string name, object value);

        /// <summary>
        /// Create connection
        /// </summary>
        /// <returns></returns>
        DbConnection CreateConnection();

        /// <summary>
        /// Create command object
        /// </summary>
        /// <param name="query"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        DbCommand CreateCommand(string query, DbConnection conn, DbTransaction tran);

        /// <summary>
        /// Create Adapter for select query
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        DbDataAdapter CreateAdapter(DbCommand cmd);

        /// <summary>
        /// Get parameter seperator(MySQL - ?, SQL Server - @)
        /// </summary>
        /// <returns></returns>
        string GetParameterSeperator();

        /// <summary>
        /// Insert batch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        void InsertBatch<T>(List<T> lst);

        /// <summary>
        /// Executes a raw query and returns effected rows count
        /// If query is DDL script, return 0 when success, -2 when error occured.
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <returns>Effected rows count</returns>
        int ExecuteRawQuery(string query);

        DataTable GetTable(string query, params KeyValuePair<string, object>[] parameters);
    }

    /// <summary>
    /// Helper class
    /// </summary>
    public class ParameterHelper
    {
        public enum QueryType
        {
            INSERT = 1,
            UPDATE = 2,
            DELETE = 3,
            SELECT = 4
        }

        public const int QUERY_RESULT_SUCCESS = 0;
        public const int QUERY_RESULT_EXECUTED = -1;
        public const int QUERY_RESULT_EXCEPTION = -2;

        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Get table name of TableAttr instance
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetTableName(object obj)
        {
            var tableAttr =
                (TableAttr)Attribute.GetCustomAttribute(obj.GetType(), typeof(TableAttr));
            return tableAttr != null ? tableAttr.Name : null;
        }


        //public static List<string> GetDbParametersName(object obj, IDictionary<string, object> dictValues)
        //{
        //    var lst = new List<string>();
        //    if (obj == null) return lst;

        //    foreach (var propertyInfo in obj.GetType().GetProperties())
        //    {
        //        foreach (var customAttribute in propertyInfo.GetCustomAttributes(false))
        //        {
        //            if (!(customAttribute is ColumnAttr)) continue;

        //            var colNameInDb = (customAttribute as ColumnAttr).Name;
        //            lst.Add(colNameInDb);
        //            if (dictValues == null) continue;
        //            try
        //            {
        //                propertyInfo.SetValue(obj, Convert.ChangeType(dictValues[colNameInDb], propertyInfo.PropertyType), null);
        //            }
        //            catch
        //            {
        //                // ignored
        //            }
        //        }
        //    }

        //    return lst;
        //}

        public static bool IsThisColumnUsingForQueryType(ColumnAttr colAttr, QueryType queryType)
        {
            var result = true;
            switch (queryType)
            {
                case QueryType.INSERT:
                    if (!colAttr.UsingForInsert)
                        result = false;
                    break;
                case QueryType.UPDATE:
                    if (!colAttr.UsingForUpdate)
                        result = false;
                    break;
                case QueryType.DELETE:
                    if (!colAttr.UsingForDelete)
                        result = false;
                    break;
                case QueryType.SELECT:
                    if (!colAttr.UsingForSelect)
                        result = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("queryType", queryType, null);
            }
            return result;
        }

        /// <summary>
        /// Insert Batch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="helper"></param>
        /// <param name="lst"></param>
        public static void InsertBatch<T>(IDbHelper helper, List<T> lst)
        {
            if (lst == null)
            {
                throw new Exception("List object to insert has no value");
            }

            var query = GetQueryWithParameters(lst[0], QueryType.INSERT, ",", helper.GetParameterSeperator());
            using (var connection = helper.CreateConnection())
            {
                connection.Open();
                using (var tran = connection.BeginTransaction(IsolationLevel.Serializable))
                using (var command = helper.CreateCommand(query, connection, tran))
                {
                    command.CommandTimeout = 20 * 60; // 20 minutes
                    foreach (var tblImportExcelRow in lst)
                    {
                        command.Parameters.Clear();
                        var parameters =
                            GetListParameterOfObject(helper, tblImportExcelRow,
                                QueryType.INSERT, helper.GetParameterSeperator());
                        foreach (var p in parameters)
                        {
                            command.Parameters.Add(p);
                        }
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Execute insert query exception", ex);
                            //sbErr.Append(ex.Message).Append(Environment.NewLine);
                        }
                    } // End for in list
                    try
                    {
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        //sbErr.Append(ex.Message).Append(Environment.NewLine);
                        Logger.Error("Commit exception", ex);
                        throw new Exception("Commit exception: " + ex.Message, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Executes a query and returns effected rows count. If query is DDL script, return 0 when success, -2 when error occured.
        /// </summary>
        /// <param name="helper">Query to execute</param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(IDbHelper helper, string query)
        {
            using (var connection = helper.CreateConnection())
            {
                connection.Open();
                using (var command = helper.CreateCommand(query, connection, null))
                {
                    command.CommandText = query;
                    command.CommandTimeout = 20 * 60; // 10 minutes
                    int result;
                    try
                    {
                        result = command.ExecuteNonQuery();
                        if (result == QUERY_RESULT_EXECUTED) result = QUERY_RESULT_SUCCESS;
                    }
                    catch (Exception ex)
                    {
                        //result = -1;
                        result = QUERY_RESULT_EXCEPTION;
                        Logger.Debug("Query: " + query);
                        Logger.Error("Execute Query exception", ex);
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// Runs a query and returns a DataTable.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="query">Query to execute</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        public static DataTable GetTable(IDbHelper helper, string query, params KeyValuePair<string, object>[] parameters)
        {
            var table = new DataTable();
            using (var connection = helper.CreateConnection())
            {
                connection.Open();
                using (var command = helper.CreateCommand(query, connection, null))
                {
                    foreach (var parameter in parameters)
                    {
                        var p = helper.CreateParameter(parameter.Key, parameter.Value);
                        command.Parameters.Add(p);
                    }

                    using (var adapter = helper.CreateAdapter(command))
                    {
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }

        #region Private method

        /// <summary>
        /// Get query include parameters name and annotation
        /// </summary>
        /// <param name="obj">Entity object with attributes</param>
        /// <param name="type">Query type</param>
        /// <param name="nameSeperator">Seperator between parameters name</param>
        /// <param name="valueSeperator">Prefix sperator between parameters</param>
        /// <returns></returns>
        private static string GetQueryWithParameters(object obj, QueryType type, string nameSeperator = ",", string valueSeperator = "?")
        {
            var table = GetTableName(obj);
            if (table == null)
            {
                throw new Exception("Invalid attributes.");
            }

            var sb = new StringBuilder();
            var parameters = GetParametersName(obj, type);
            var parametersName = string.Join(nameSeperator, parameters);
            var parametersValue = string.Join(nameSeperator + valueSeperator, parameters);
            switch (type)
            {
                case QueryType.INSERT:
                    sb.Append(@"INSERT INTO ");
                    sb.Append(table);
                    sb.Append("(");
                    sb.Append(parametersName);
                    sb.Append(") VALUES (").Append(valueSeperator);
                    sb.Append(parametersValue);
                    sb.Append(")");
                    break;
                case QueryType.UPDATE:
                    break;
                case QueryType.DELETE:
                    break;
                case QueryType.SELECT:
                    break;

            }

            return sb.ToString();
        }

        /// <summary>
        /// Get list parameter of object as string
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        private static List<string> GetParametersName(object obj, QueryType queryType)
        {
            var lst = new List<string>();
            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                foreach (var customAttribute in propertyInfo.GetCustomAttributes(false))
                {
                    if (!(customAttribute is ColumnAttr)) continue;

                    var colAttr = customAttribute as ColumnAttr;
                    //switch (queryType)
                    //{
                    //    case QueryType.INSERT:
                    //        if (!colAttr.UsingForInsert)
                    //            continue;
                    //        break;
                    //    case QueryType.UPDATE:
                    //        if (!colAttr.UsingForUpdate)
                    //            continue;
                    //        break;
                    //    case QueryType.DELETE:
                    //        if (!colAttr.UsingForDelete)
                    //            continue;
                    //        break;
                    //    case QueryType.SELECT:
                    //        if (!colAttr.UsingForSelect)
                    //            continue;
                    //        break;
                    //    default:
                    //        throw new ArgumentOutOfRangeException("queryType", queryType, null);
                    //}
                    if (!IsThisColumnUsingForQueryType(colAttr, queryType))
                    {
                        continue;
                    }

                    var colNameInDb = (customAttribute as ColumnAttr).Name;
                    lst.Add(colNameInDb);
                }
            }

            return lst;
        }

        /// <summary>
        /// Get list parameters
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="obj"></param>
        /// <param name="queryType"></param>
        /// <param name="valueSeperator"></param>
        /// <returns></returns>
        private static List<DbParameter> GetListParameterOfObject(IDbHelper helper, object obj, QueryType queryType, string valueSeperator = "?")
        {
            var lst = new List<DbParameter>();

            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                foreach (var customAttribute in propertyInfo.GetCustomAttributes(false))
                {
                    if (!(customAttribute is ColumnAttr)) continue;

                    var colAttr = customAttribute as ColumnAttr;

                    if (!IsThisColumnUsingForQueryType(colAttr, queryType))
                    {
                        continue;
                    }

                    var colNameInDb = (customAttribute as ColumnAttr).Name;
                    var value = propertyInfo.GetValue(obj, null);

                    var parameter = helper.CreateParameter(valueSeperator + colNameInDb, value);
                    lst.Add(parameter);
                }
            }
            return lst;
        }

        #endregion
    }
}
