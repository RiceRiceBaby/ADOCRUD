using ADOCRUD.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOCRUD.UnitTests.Models
{
    [Table("UserProduct", "dbo")]
    public class UserProduct
    {
        [PrimaryKey]
        [Member]
        public int UserId { get; set; }

        [PrimaryKey]
        [Member]
        public int ProductId { get; set; }

    }
}