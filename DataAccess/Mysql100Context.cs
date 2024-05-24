using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using MySql.Data.EntityFramework;
using PartsMySql.Models;

namespace PartsMysql.DataAccess
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Mysql100Context : DbContext
    {
        public DbSet<Inspection> Inspection { get; set; }
        public Mysql100Context() : base("Mysql100") // Pass the name of the connection string
        {
            // Additional configuration if needed
        }
    }
}