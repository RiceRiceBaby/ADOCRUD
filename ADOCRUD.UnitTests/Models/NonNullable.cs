using ADOCRUD.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOCRUD.UnitTests.Models
{
    [Table("NonNullable", "dbo")]
    public class NonNullable
    {
        [PrimaryKey]
        [Member]
        public int Id { get; set; }

        [Member]
        public Int64 Integer64Value { get; set; }

        [Member]
        public byte ByteValue { get; set; }

        [Member]
        public byte[] ByteArrayValue { get; set; }

        [Member]
        public bool BoolValue { get; set; }

        [Member]
        public string CharValue { get; set; }

        [Member]
        public DateTime DateTimeValue { get; set; }

        [Member]
        public DateTimeOffset DateTimeOffsetValue { get; set; }

        [Member]
        public decimal DecimalValue { get; set; }

        [Member]
        public float FloatValue { get; set; }

        [Member]
        public float DoubleValue { get; set; }

        [Member]
        public int Integer32Value { get; set; }

        [Member]
        public Single Single { get; set; }

        [Member]
        public Int16 Integer16Value { get; set; }

        [Member]
        public Guid GuidValue { get; set; }

        [Member]
        public string StringValue { get; set; }

    }
}