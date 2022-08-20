using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.PGSQL
{
    internal class TableConstraint
    {
        public string table_schema { get; set; }
        public string constraint_name { get; set; }
        public string table_name { get; set; }
        public string column_name { get; set; }
        public string foreign_table_schema { get; set; }
        public string foreign_table_name { get; set; }
        public string foreign_column_name { get; set; }
        public string contype { get; set; }
    }
}
