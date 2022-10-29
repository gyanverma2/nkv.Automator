using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.GetAutomator.Data.Models
{
    [Table("database_type")]
    public class DatabaseTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DatabaseTypeId { get; set; }
        public string DatabaseTypeName { get; set; }
        public bool DatabaseTypeIsActive { get; set; }
        public string DataTypeIconClass { get; set; }
    }
}
