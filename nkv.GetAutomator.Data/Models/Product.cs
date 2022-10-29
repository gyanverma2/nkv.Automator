using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.GetAutomator.Data.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImages { get; set; }
        public string? ProductVideo { get; set; }
        public DateTime ProductLastUpdated { get; set; }
        public string ProductTags { get; set; }
        public string ProductVersion { get; set; }
        public string ProductNumber { get; set; }
        public string? ProductSubTitle { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public string? CodecanyonLink { get; set; }
        public int DatabaseTypeId { get; set; }
    }
    public class ProductSearchRequest
    {
        public int? DatabaseTypeId { get; set; }
        public int? CategoryID { get; set; }
        public string? SearchKey { get; set; }
    }
}
