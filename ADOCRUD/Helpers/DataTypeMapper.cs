using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOCRUD.Helpers
{
    public class DataTypeMapper
    {
        public static Dictionary<Type, SqlDbType> DataTypes()
        {
            return new Dictionary<Type, SqlDbType>
            {
                // Non nullable
                [typeof(Int64)] = SqlDbType.BigInt,
                [typeof(byte)] = SqlDbType.TinyInt,
                [typeof(byte[])] = SqlDbType.VarBinary,
                [typeof(bool)] = SqlDbType.Bit,
                [typeof(char)] = SqlDbType.Char,
                [typeof(char[])] = SqlDbType.Char,
                [typeof(DateTime)] = SqlDbType.DateTime,
                [typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset,
                [typeof(decimal)] = SqlDbType.Decimal,
                [typeof(float)] = SqlDbType.Float,
                [typeof(double)] = SqlDbType.Float,
                [typeof(Int32)] = SqlDbType.Int,
                [typeof(Single)] = SqlDbType.Real,
                [typeof(Int16)] = SqlDbType.SmallInt,
                [typeof(Guid)] = SqlDbType.UniqueIdentifier,
                [typeof(object)] = SqlDbType.Variant,
                [typeof(TimeSpan)] = SqlDbType.Time,

                // Nullable
                [typeof(Int64?)] = SqlDbType.BigInt,
                [typeof(byte?)] = SqlDbType.TinyInt,
                [typeof(bool?)] = SqlDbType.Bit,
                [typeof(string)] = SqlDbType.VarChar,
                [typeof(char?)] = SqlDbType.Char,
                [typeof(DateTime?)] = SqlDbType.DateTime,
                [typeof(DateTimeOffset?)] = SqlDbType.DateTimeOffset,
                [typeof(decimal?)] = SqlDbType.Decimal,
                [typeof(float?)] = SqlDbType.Float,
                [typeof(double?)] = SqlDbType.Float,
                [typeof(Int32?)] = SqlDbType.Int,
                [typeof(Single?)] = SqlDbType.Real,
                [typeof(Int16?)] = SqlDbType.SmallInt,
                [typeof(Guid?)] = SqlDbType.UniqueIdentifier,
                [typeof(TimeSpan?)] = SqlDbType.Time
            };
        }
    }
}
