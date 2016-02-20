using ADOCRUD.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ADOCRUD.UnitTests.Models
{
    [Table("Product", "dbo")]
    public class Product
    {
        [PrimaryKey]
        [Member]
        public int Id { get; set; }

        [Member]
        public int CategoryId { get; set; }

        [Member]
        public string Name { get; set; }

        [Member]
        public decimal Price { get; set; }

        [Member]
        public string Description { get; set; }

    }
}