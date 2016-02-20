
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ADOCRUD.Attributes;
using ADOCRUD.Helpers;
using System.Reflection;

namespace ADOCRUD
{
    public class ADOCRUDContext : IDisposable
    {
        protected static IDbConnection sqlConnection;
        protected static IDbTransaction sqlTransaction;

        public ADOCRUDContext(string connectionString)
        {            
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            sqlTransaction = sqlConnection.BeginTransaction();
        }

        /// <summary>
        /// Generates insert of an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Insert<T>(T item)
        {
            string tableName = ObjectTypeHelper.GetTableName<T>(item);
            PropertyInfo[] modelProperties = ObjectTypeHelper.GetModelProperties<T>(item, false);

            // Throws exception if there are no properties in the object or if all properties are missing Member attribute
            if (modelProperties == null || modelProperties.Count() == 0)
                throw new Exception("Class has no properties or are missing Member attribute");

            // Generates insert statement
            StringBuilder query = new StringBuilder();
            query.Append("insert into " + tableName + "(" + String.Join(",", modelProperties.Select(x => x.Name)) + ") ");
            query.Append("values (" + String.Join(",", modelProperties.Select(x => "@" + x.Name)) + ") ");
            query.Append("select @primaryKey = @@identity");

            try
            { 
                // Generates and executes sql command 
                SqlCommand cmd = GenerateSqlCommand<T>(item, query.ToString(), modelProperties, null, ADOCRUDEnums.Action.Insert);
                cmd.ExecuteNonQuery();

                // Loads db generated identity value into property with primary key attribute
                // if object only has 1 primary key
                // Multiple primary keys = crosswalk table which means property already contains the values
                PropertyInfo[] primaryKeyProperties = ObjectTypeHelper.GetPrimaryKeyProperties<T>(item);

                if (primaryKeyProperties != null && primaryKeyProperties.Count() == 1)
                    primaryKeyProperties[0].SetValue(item, cmd.Parameters["primaryKey"].Value as object);
            }
            catch (Exception ex)
            {
                sqlTransaction.Rollback();
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Generates update of an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Update<T>(T item)
        {
            string tableName = ObjectTypeHelper.GetTableName<T>(item);
            PropertyInfo[] modelProperties = ObjectTypeHelper.GetModelProperties<T>(item, false);
            PropertyInfo[] primaryKeyProperties = ObjectTypeHelper.GetPrimaryKeyProperties<T>(item);

            // Throws exception if there are no properties in the object or if all properties are missing Member attribute
            if (modelProperties == null || modelProperties.Count() == 0)
                throw new Exception("Class has no properties or are missing Member attribute");

            // Generates update statement
            StringBuilder query = new StringBuilder();
            query.Append("update " + tableName + " set ");
            query.Append(String.Join(",", modelProperties.Select(x => x.Name + " = @" + x.Name)));
            query.Append(" where " + primaryKeyProperties[0].Name + " = @" + primaryKeyProperties[0].Name);

            try
            {
                // Generates and executes sql command 
                SqlCommand cmd = GenerateSqlCommand<T>(item, query.ToString(), modelProperties, primaryKeyProperties, ADOCRUDEnums.Action.Update);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlTransaction.Rollback();
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Generates a removal of an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Remove<T>(T item)
        {
            string tableName = ObjectTypeHelper.GetTableName<T>(item);
            PropertyInfo[] modelProperties = ObjectTypeHelper.GetModelProperties<T>(item, false);
            PropertyInfo[] primaryKeyProperties = ObjectTypeHelper.GetPrimaryKeyProperties<T>(item);

            // Throws exception if there are no properties in the object or if all properties are missing Member attribute
            if (modelProperties == null || modelProperties.Count() == 0)
                throw new Exception("Class has no properties or are missing Member attribute");

            // Generates delete statement
            StringBuilder query = new StringBuilder();
            query.Append("delete from " + tableName + " ");
            query.Append("where " + String.Join(" and ", modelProperties.Select(x => x.Name + " = @" + x.Name)));

            try
            {
                // Generates and executes sql command
                SqlCommand cmd = GenerateSqlCommand<T>(item, query.ToString(), modelProperties, null, ADOCRUDEnums.Action.Remove);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlTransaction.Rollback();
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<T> QueryItems<T>(string query)
        {
            return sqlConnection.Query<T>(query, sqlTransaction);
        }

        public IEnumerable<T> QueryItems<T>(string query, object parameters)
        {
            return sqlConnection.Query<T>(query, parameters, sqlTransaction);
        }

        public void Commit()
        {
            sqlTransaction.Commit();
        }

        public void Dispose()
        {
            sqlTransaction.Dispose();
            sqlConnection.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">Model Property</param>
        /// <param name="query">Sql Statement</param>
        /// <param name="modelProperties">Properties of the model</param>
        /// <param name="primaryKeyProperties">Properties of the model that are associated with the primary key</param>
        /// <param name="isUpdate">Is the sql an update query</param>
        /// <returns></returns>
        private SqlCommand GenerateSqlCommand<T>(T item, string query, PropertyInfo[] modelProperties, PropertyInfo[] primaryKeyProperties, ADOCRUDEnums.Action action)
        {
            // Initializes sql command
            SqlCommand cmd = new SqlCommand(query.ToString(), sqlConnection as SqlConnection, sqlTransaction as SqlTransaction);

            // Adds parameters to sql command
            for (int i = 0; i < modelProperties.Count(); i++)
            {
                // Non nullable types
                if (modelProperties[i].PropertyType == typeof(int) || modelProperties[i].PropertyType == typeof(Int32))
                    cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Int).Value = modelProperties[i].GetValue(item, null);
                else if (modelProperties[i].PropertyType == typeof(DateTime))
                    cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.DateTime).Value = modelProperties[i].GetValue(item, null);
                else if (modelProperties[i].PropertyType == typeof(decimal))
                    cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Decimal).Value = modelProperties[i].GetValue(item, null);
                else if (modelProperties[i].PropertyType == typeof(double))
                    cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Float).Value = modelProperties[i].GetValue(item, null);
                else if (modelProperties[i].PropertyType == typeof(float))
                    cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Float).Value = modelProperties[i].GetValue(item, null);
                else if (modelProperties[i].PropertyType == typeof(bool))
                    cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Bit).Value = modelProperties[i].GetValue(item, null);
                else if (modelProperties[i].PropertyType == typeof(byte))
                    cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.TinyInt).Value = modelProperties[i].GetValue(item, null);
                else if (modelProperties[i].PropertyType == typeof(byte[]))
                    cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.VarBinary).Value = modelProperties[i].GetValue(item, null);
                else if (modelProperties[i].PropertyType == typeof(Guid))
                    cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.UniqueIdentifier).Value = modelProperties[i].GetValue(item, null);
                else if (modelProperties[i].PropertyType == typeof(Int16))
                    cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.SmallInt).Value = modelProperties[i].GetValue(item, null);
                else if (modelProperties[i].PropertyType == typeof(Int64))
                    cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.BigInt).Value = modelProperties[i].GetValue(item, null);

                // Nullable types
                else if (modelProperties[i].PropertyType == typeof(int?) || modelProperties[i].PropertyType == typeof(Int32?))
                {
                    int? value = modelProperties[i].GetValue(item, null) as int?;

                    if (value.HasValue)
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Int).Value = modelProperties[i].GetValue(item, null);
                    else
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Int).Value = DBNull.Value;
                }
                else if (modelProperties[i].PropertyType == typeof(DateTime?))
                {
                    DateTime? value = modelProperties[i].GetValue(item, null) as DateTime?;

                    if (value.HasValue)
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.DateTime).Value = modelProperties[i].GetValue(item, null);
                    else
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.DateTime).Value = DBNull.Value;
                }
                else if (modelProperties[i].PropertyType == typeof(decimal?))
                {
                    decimal? value = modelProperties[i].GetValue(item, null) as decimal?;

                    if (value.HasValue)
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Decimal).Value = modelProperties[i].GetValue(item, null);
                    else
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Decimal).Value = DBNull.Value;
                }
                else if (modelProperties[i].PropertyType == typeof(double?))
                {
                    double? value = modelProperties[i].GetValue(item, null) as double?;

                    if (value.HasValue)
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Float).Value = modelProperties[i].GetValue(item, null);
                    else
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Float).Value = DBNull.Value;
                }
                else if (modelProperties[i].PropertyType == typeof(float?))
                {
                    float? value = modelProperties[i].GetValue(item, null) as float?;

                    if (value.HasValue)
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Float).Value = modelProperties[i].GetValue(item, null);
                    else
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Float).Value = DBNull.Value;
                }
                else if (modelProperties[i].PropertyType == typeof(bool?))
                {
                    bool? value = modelProperties[i].GetValue(item, null) as bool?;

                    if (value.HasValue)
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Bit).Value = modelProperties[i].GetValue(item, null);
                    else
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.Bit).Value = DBNull.Value;
                }
                else if (modelProperties[i].PropertyType == typeof(byte?))
                {
                    byte? value = modelProperties[i].GetValue(item, null) as byte?;

                    if (value.HasValue)
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.TinyInt).Value = modelProperties[i].GetValue(item, null);
                    else
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.TinyInt).Value = DBNull.Value;
                }
                else if (modelProperties[i].PropertyType == typeof(Guid?))
                {
                    Guid? value = modelProperties[i].GetValue(item, null) as Guid?;

                    if (value.HasValue)
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.UniqueIdentifier).Value = modelProperties[i].GetValue(item, null);
                    else
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.UniqueIdentifier).Value = DBNull.Value;
                }
                else if (modelProperties[i].PropertyType == typeof(Int16?))
                {
                    Int16? value = modelProperties[i].GetValue(item, null) as Int16?;

                    if (value.HasValue)
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.SmallInt).Value = modelProperties[i].GetValue(item, null);
                    else
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.SmallInt).Value = DBNull.Value;
                }
                else if (modelProperties[i].PropertyType == typeof(Int64?))
                {
                    Int64? value = modelProperties[i].GetValue(item, null) as Int64?;

                    if (value.HasValue)
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.BigInt).Value = modelProperties[i].GetValue(item, null);
                    else
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.BigInt).Value = DBNull.Value;
                }
                else if (modelProperties[i].PropertyType == typeof(string))
                {
                    if (modelProperties[i].GetValue(item, null) != null)
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.VarChar).Value = modelProperties[i].GetValue(item, null);
                    else
                        cmd.Parameters.Add(modelProperties[i].Name, SqlDbType.VarChar).Value = DBNull.Value;
                }
            }


            if (action == ADOCRUDEnums.Action.Insert)
            {
                // Grabs identity generated on insert
                cmd.Parameters.Add("primaryKey", SqlDbType.Int).Direction = ParameterDirection.Output;
            }
            else if (action  == ADOCRUDEnums.Action.Update)
            {
                if (primaryKeyProperties[0].PropertyType == typeof(int) || primaryKeyProperties[0].PropertyType == typeof(Int32))
                    cmd.Parameters.Add(primaryKeyProperties[0].Name, SqlDbType.Int).Value = primaryKeyProperties[0].GetValue(item, null);
                else if (primaryKeyProperties[0].PropertyType == typeof(Guid))
                    cmd.Parameters.Add(primaryKeyProperties[0].Name, SqlDbType.UniqueIdentifier).Value = primaryKeyProperties[0].GetValue(item, null);
                else
                    throw new Exception("Primary key must be an integer or GUID");
            }

            return cmd;
        }
    }
}
