using ADOCRUD.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADOCRUD.Helpers
{
    public class ObjectTypeHelper
    {
        /// <summary>
        /// Get name of table model is mapped to
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetTableName<T>(T item)
        {
            Table tableAttr = System.Attribute.GetCustomAttribute(item.GetType(), typeof(Table)) as Table;
            return tableAttr.SchemaName + "." + tableAttr.TableName;
        }

        /// <summary>
        /// Get all the properties in the model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="includePrimaryKey"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetModelProperties<T>(T item, bool includePrimaryKey = true)
        {
            if (includePrimaryKey)
                return item.GetType().GetProperties()
                    .Where(x =>
                        Attribute.IsDefined(x, typeof(Member))).ToArray();
            else
                return item.GetType().GetProperties()
                    .Where(x =>
                        Attribute.IsDefined(x, typeof(Member)) &&
                        !Attribute.IsDefined(x, typeof(PrimaryKey))).ToArray();
        }

        public static PropertyInfo[] GetPrimaryKeyProperties<T>(T item)
        {
                return item.GetType().GetProperties()
                    .Where(x =>
                        Attribute.IsDefined(x, typeof(PrimaryKey))).ToArray();
        }
    }
}
