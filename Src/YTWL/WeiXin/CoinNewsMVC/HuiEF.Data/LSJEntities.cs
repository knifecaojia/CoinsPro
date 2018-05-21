

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace HuiEF.Data
{
    public class LSJEntities : DbContext
    {
        public LSJEntities() : base("name=nhibernatedemodb_sqlite")
        {
            Database.SetInitializer<HuiEntities>(null);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
        }
        public DbSet<Domain.Users> User { get; set; }

    }
}

