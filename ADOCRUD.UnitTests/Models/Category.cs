using ADOCRUD.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOCRUD.UnitTests.Models
{
    [Table("Category", "dbo")]
    public class Category
    {
        [PrimaryKey]
        [Member]
        public int Id { get; set; }

        [Member]
        public string Name { get; set; }

    }
}