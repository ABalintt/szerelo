using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SzervizAPI.Models;
using System.Data.Common;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SzervizAPI.Database
{
    public class SzervizContext:DbContext
    {
        public  DbSet<Ugyfelek> Ugyfelek { get; set; }
        public  DbSet<Jarmuvek> Jarmuvek { get; set; }
        public  DbSet<Dolgozo> Dolgozok { get; set; }
        public  DbSet<Javitasok> Javitasok { get; set; }
        public  DbSet<Idopont> Idopontok { get; set; }
        public  DbSet<Foglal> Foglal { get; set; }
        public  DbSet<Vegez> Vegez { get; set; }
        public  DbSet<Elvegzi> Elvegzi { get; set; }

        public SzervizContext() : base("name=SzervizContext") { }

        public SzervizContext(DbConnection existingConnection, bool contextOwnsConnection)
                : base(existingConnection, contextOwnsConnection) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}