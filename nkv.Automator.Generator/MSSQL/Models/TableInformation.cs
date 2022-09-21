using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.MSSQL
{
    public class TableInformation
    {
        public string TABLE_CATALOG { get; set; }
        public string TABLE_SCHEMA { get; set; }
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public int ORDINAL_POSITION { get; set; }
        public string IS_NULLABLE { get; set; }
        public string DATA_TYPE { get; set; }
        public int NUMERIC_PRECISION { get; set; }
        public int NUMERIC_PRECISION_RADIX { get; set; }
        public int NUMERIC_SCALE { get; set; }
        public string COLUMN_DEFAULT { get; set; }
    }
}
