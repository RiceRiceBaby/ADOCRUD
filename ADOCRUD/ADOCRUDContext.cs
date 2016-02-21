
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
        internal static IDbConnection sqlConnection;
        internal static IDbTransaction sqlTransaction;

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
            query.Append("where " + String.Join(" and ", primaryKeyProperties.Select(x => x.Name + " = @" + x.Name)));

            try
            {
                // Generates and executes sql command
                SqlCommand cmd = GenerateSqlCommand<T>(item, query.ToString(), null, primaryKeyProperties, ADOCRUDEnums.Action.Remove);
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
        internal SqlCommand GenerateSqlCommand<T>(T item, string query, PropertyInfo[] modelProperties, PropertyInfo[] primaryKeyProperties, ADOCRUDEnums.Action action)
        {
            // Initializes sql command
            SqlCommand cmd = new SqlCommand(query.ToString(), sqlConnection as SqlConnection, sqlTransaction as SqlTransaction);

            if (modelProperties != null && modelProperties.Count() > 0)
            { 
                // Adds model parameters to sql command
                for (int i = 0; i < modelProperties.Count(); i++)
                {
                    // Checks to see if the data type is in the list of handled data types
                    if (DataTypeMapper.DataTypes().ContainsKey(modelProperties[i].PropertyType))
                    {
                        SqlDbType dbType;

                        // Grabs value from property and checks if its null
                        // If its not null, the pass value into sqlcommand parameter
                        // Otherwise pass in DBNull.Value
                        DataTypeMapper.DataTypes().TryGetValue(modelProperties[i].PropertyType, out dbType);

                        object dbValue = modelProperties[i].GetValue(item, null);

                        if (dbValue != null)
                            cmd.Parameters.Add(modelProperties[i].Name, dbType).Value = modelProperties[i].GetValue(item, null);
                        else
                            cmd.Parameters.Add(modelProperties[i].Name, dbType).Value = DBNull.Value;
                    }
                    else
                    {
                        throw new Exception("One or more properties in your model include an unhandled data type");
                    }
                }
            }

            if (action == ADOCRUDEnums.Action.Insert)
            {
                // Grabs identity generated on insert
                cmd.Parameters.Add("primaryKey", SqlDbType.Int).Direction = ParameterDirection.Output;
            }
            else if (action  == ADOCRUDEnums.Action.Update || action == ADOCRUDEnums.Action.Remove)
            {
                // Add primary key parameters to sql command
                for (int i = 0; i < primaryKeyProperties.Count(); i++)
                {
                    if (primaryKeyProperties[i].PropertyType == typeof(int))
                        cmd.Parameters.Add(primaryKeyProperties[i].Name, SqlDbType.Int).Value = primaryKeyProperties[i].GetValue(item, null);
                    else if (primaryKeyProperties[0].PropertyType == typeof(Guid))
                        cmd.Parameters.Add(primaryKeyProperties[i].Name, SqlDbType.UniqueIdentifier).Value = primaryKeyProperties[i].GetValue(item, null);
                    else
                        throw new Exception("Primary key must be an integer or GUID");
                }
            }

            return cmd;
        }
    }
}
