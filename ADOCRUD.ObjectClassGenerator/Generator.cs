using ADOCRUD.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOCRUD.ObjectClassGenerator
{
    public class Generator
    {
        /// <summary>
        /// Creates text for each property
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private static string CreateProperty(SqlDbType dbType, bool isNullable, string columnName)
        {
            string propertyType = string.Empty;

            if (!isNullable)
            {
                switch (dbType)
                {
                    // Non nullable types
                    case SqlDbType.BigInt:
                        propertyType = "Int64";
                        break;

                    case SqlDbType.TinyInt:
                        propertyType = "byte";
                        break;

                    case SqlDbType.VarBinary:
                        propertyType = "byte[]";
                        break;

                    case SqlDbType.Bit:
                        propertyType = "bool";
                        break;

                    case SqlDbType.Char:
                        propertyType = "string";
                        break;

                    case SqlDbType.DateTime:
                        propertyType = "DateTime";
                        break;

                    case SqlDbType.DateTimeOffset:
                        propertyType = "DateTimeOffset";
                        break;

                    case SqlDbType.Decimal:
                        propertyType = "decimal";
                        break;

                    case SqlDbType.Float:
                        propertyType = "float";
                        break;

                    case SqlDbType.Int:
                        propertyType = "int";
                        break;

                    case SqlDbType.Real:
                        propertyType = "Single";
                        break;

                    case SqlDbType.SmallInt:
                        propertyType = "Int16";
                        break;

                    case SqlDbType.UniqueIdentifier:
                        propertyType = "Guid";
                        break;

                    case SqlDbType.Variant:
                        propertyType = "object";
                        break;

                    case SqlDbType.VarChar:
                        propertyType = "string";
                        break;
                }
            }
            else
            {
                switch (dbType)
                {
                    // Nullable types
                    case SqlDbType.BigInt:
                        propertyType = "Int64?";
                        break;

                    case SqlDbType.TinyInt:
                        propertyType = "byte?";
                        break;

                    case SqlDbType.VarBinary:
                        propertyType = "byte[]";
                        break;

                    case SqlDbType.Bit:
                        propertyType = "bool?";
                        break;

                    case SqlDbType.Char:
                        propertyType = "string";
                        break;

                    case SqlDbType.DateTime:
                        propertyType = "DateTime?";
                        break;

                    case SqlDbType.DateTimeOffset:
                        propertyType = "DateTimeOffset?";
                        break;

                    case SqlDbType.Decimal:
                        propertyType = "decimal?";
                        break;

                    case SqlDbType.Float:
                        propertyType = "float?";
                        break;

                    case SqlDbType.Int:
                        propertyType = "int?";
                        break;

                    case SqlDbType.Real:
                        propertyType = "Single?";
                        break;

                    case SqlDbType.SmallInt:
                        propertyType = "Int16?";
                        break;

                    case SqlDbType.UniqueIdentifier:
                        propertyType = "Guid?";
                        break;

                    case SqlDbType.Variant:
                        propertyType = "object";
                        break;

                    case SqlDbType.VarChar:
                        propertyType = "string";
                        break;
                }
            }

            return String.Format("public {0} {1} {{ get; set; }}", propertyType, columnName);
        }

        /// <summary>
        /// Creates text for the whole class
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        public static string CreateClass(DataTable dt, string nameSpace)
        {
            StringBuilder textForClassCreation = new StringBuilder();

            if (dt != null)
            {
                textForClassCreation.Append("using ADOCRUD.Attributes;" + Environment.NewLine);
                textForClassCreation.Append("using System;" + Environment.NewLine);
                textForClassCreation.Append("using System.Collections.Generic;" + Environment.NewLine);
                textForClassCreation.Append("using System.Linq;" + Environment.NewLine);
                textForClassCreation.Append("using System.Text;" + Environment.NewLine);
                textForClassCreation.Append("using System.Threading.Tasks;" + Environment.NewLine+Environment.NewLine + Environment.NewLine);
                textForClassCreation.Append("namespace " + nameSpace + Environment.NewLine);
                textForClassCreation.Append("{" + Environment.NewLine);
                textForClassCreation.Append("[Table(\"".PadLeft(12, ' ') + dt.TableName + "\", \"" + dt.Prefix + "\")]" + Environment.NewLine);
                textForClassCreation.Append(("public class " + dt.TableName).PadLeft(+dt.TableName.Length + 17, ' ') + Environment.NewLine);
                textForClassCreation.Append("{".PadLeft(5, ' ') + Environment.NewLine);

                foreach (DataColumn column in dt.Columns)
                {
                    if (dt.PrimaryKey.Contains(column))
                        textForClassCreation.Append("[PrimaryKey]".PadLeft(20, ' ') + Environment.NewLine);

                    textForClassCreation.Append("[Member]".PadLeft(16, ' ') + Environment.NewLine);
                    Type t = column.DataType;

                    SqlDbType dbType = new SqlDbType();
                    DataTypeMapper.DataTypes().TryGetValue(column.DataType, out dbType);

                    string propertyLine = CreateProperty(dbType, column.AllowDBNull, column.ColumnName);
                    textForClassCreation.Append(propertyLine.PadLeft(propertyLine.Length + 8, ' ') + Environment.NewLine + Environment.NewLine);
                }

                textForClassCreation.Append("}".PadLeft(5, ' ') + Environment.NewLine);
                textForClassCreation.Append("}");
            }

            return textForClassCreation.ToString();
        }
    }
}
