using MySql.Data.EntityFramework;
using PartsMySql.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PartsMysql.DataAccess
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))] // Specify MySQL EF configuration
    public class MySqlContext : DbContext
    {
        public DbSet<Inspection> Inspection { get; set; }
        public MySqlContext() : base("ConnString") // Pass the name of the connection string
        {
            // Additional configuration if needed
        }
    }
}
