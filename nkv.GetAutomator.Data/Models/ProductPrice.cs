using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.GetAutomator.Data.Models
{
    [Table("product_price")]
    public class ProductPrice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PriceID { get; set; }
        public int PriceTypeID { get; set; }
        public int ProductID { get; set; }
        public double Price { get; set; }
        public virtual string PriceTypeName { get; set; } = null!;
        public virtual int NumberOfDays { get; set; }
        public virtual int NumberOfClick { get; set; }
    }
    [Table("price_type")]
    public class PriceType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PriceTypeID { get; set; }
        public string PriceTypeName { get; set; } = null!;
        public int NumberOfDays { get; set; }
        public int NumberOfClick { get; set; }
    }
}
