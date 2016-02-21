using ADOCRUD.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOCRUD.UnitTests.Models
{
    [Table("User", "dbo")]
    public class User
    {
        [PrimaryKey]
        [Member]
        public int Id { get; set; }

        [Member]
        public string FirstName { get; set; }

        [Member]
        public string LastName { get; set; }

    }
}