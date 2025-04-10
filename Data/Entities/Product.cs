using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoEfP26.Data.Entities
{
    public class Product
    {
        public Guid      Id          { get; set; }
        public Guid      GroupId     { get; set; }
        public String    Name        { get; set; } = null!;
        public String?   Description { get; set; }
        public String?   Slug        { get; set; }
        public String?   ImageUrl    { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal   Price       { get; set; }
                                     
        public int       Stock       { get; set; }
        public DateTime? DeletedAt   { get; set; }


        public ProductGroup ProductGroup { get; set; } = null!;
        public List<ItemImage> Images { get; set; } = [];
    }
}
// 101.101 = 1x2^2 + 0x2^1 + 1x2^0  + 1x2^-1  + 0 + 1x2^-3 = 4 + 1 + 1/2 + 1/8 = 5.625