using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOCRUD.Attributes
{
    public class Table : Attribute
    {
        public Table(string tableName, string schemaName)
        {
            TableName = tableName;
            SchemaName = schemaName;
        }

        public string SchemaName { get; set; }
        public string TableName { get; set; }
    }
}
