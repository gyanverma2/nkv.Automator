using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.Models
{
    internal class ProductModel
    {
        public int ProductID { get; set; }
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImages { get; set; }
        public string? ProductVideo { get; set; }
        public DateTime? ProductLastUpdated { get; set; }
        public string ProductTags { get; set; }
        public string ProductVersion { get; set; }
        public string ProductNumber { get; set; }
        public string ProductSubTitle { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string CategoryID { get; set; }
        public string? CodecanyonLink { get; set; }
        public int DatabaseTypeId { get; set; }
        public string DatabaseTypeName { get; set; }
    }
    public enum DataTypeEnum
    {
        MySQL = 1,
        MSSQL = 2,
        PostgreSQL = 3,
        MongoDB = 4,
    }
}
