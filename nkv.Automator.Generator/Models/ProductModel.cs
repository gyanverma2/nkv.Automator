using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.Models
{
    public class ProductModel
    {
        public int ProductID { get; set; }
        public string ProductTitle { get; set; } = null!;
        public string ProductDescription { get; set; } = null!;
        public string ProductImages { get; set; } = null!;
        public string? ProductVideo { get; set; }
        public DateTime? ProductLastUpdated { get; set; }
        public string ProductTags { get; set; } = null!;
        public string ProductVersion { get; set; } = null!;
        public string ProductNumber { get; set; } = null!;
        public string ProductSubTitle { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public int CategoryID { get; set; }
        public string? CodecanyonLink { get; set; }
        public int DatabaseTypeId { get; set; }
        public string DatabaseTypeName { get; set; } = null!;
    }
    
}
