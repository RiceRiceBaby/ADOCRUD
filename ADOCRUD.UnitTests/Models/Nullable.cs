using ADOCRUD.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOCRUD.UnitTests.Models
{
    [Table("Nullable", "dbo")]
    public class Nullable
    {
        [PrimaryKey]
        [Member]
        public int Id { get; set; }

        [Member]
        public int? Integer32Value { get; set; }

        [Member]
        public DateTime? DateTimeValue { get; set; }

        [Member]
        public Decimal? DecimalValue { get; set; }

        [Member]
        public Double? DoubleValue { get; set; }

        [Member]
        public float? FloatValue { get; set; }

        [Member]
        public bool? BoolValue { get; set; }

        [Member]
        public byte? ByteValue { get; set; }

        [Member]
        public Guid? GuidValue { get; set; }

        [Member]
        public Int16? Integer16Value { get; set; }

        [Member]
        public Int64? Integer64Value { get; set; }

        [Member]
        public string StringValue { get; set; }
    }
}
