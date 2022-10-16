using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using nkv.GetAutomator.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.GetAutomator.Data.DBContext
{
    public class MySQLContext : DbContext
    {
        
        public MySQLContext(DbContextOptions<MySQLContext> options): base(options)
        { }
        public DbSet<Product> Products { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<DatabaseTypes> DatabaseTypes { get; set; }
    }
}
