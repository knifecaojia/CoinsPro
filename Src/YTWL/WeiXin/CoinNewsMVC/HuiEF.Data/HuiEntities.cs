using HuiEF.Data.HuiEntity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace HuiEF.Data
{
    public class HuiEntities : DbContext
    {
     
        public HuiEntities() : base("name=huiefSqlite")
        {
            Database.SetInitializer<HuiEntities>(null);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
        }
        public DbSet<test01> Test { get; set; }
        public DbSet<Login_Log> Login_Log { get; set; }
        

    }
}

