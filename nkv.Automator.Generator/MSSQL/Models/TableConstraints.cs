using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.MSSQL
{
    public class TableConstraints
    {
        public string COLUMN_NAME { get; set; }
        public string CONSTRAINT_TYPE { get; set; }
        public string CONSTRAINT_NAME { get; set; }
        public string CurrentTableName { get; set; }
        public string REFTable { get; set; }
        public string TargetColumn { get; set; }
    }
}
