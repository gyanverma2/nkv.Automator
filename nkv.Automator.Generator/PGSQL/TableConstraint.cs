
namespace nkv.Automator.PGSQL
{
    public class TableConstraint
    {
        public string table_schema { get; set; } = null!;
        public string constraint_name { get; set; } = null!;
        public string table_name { get; set; } = null!;
        public string column_name { get; set; } = null!;
        public string foreign_table_schema { get; set; } = null!;
        public string foreign_table_name { get; set; } = null!;
        public string foreign_column_name { get; set; } = null!;
        public string contype { get; set; } = null!;
    }
}
