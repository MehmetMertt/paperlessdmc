using Microsoft.EntityFrameworkCore;
using PaperlessREST.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessREST.Infrastructure
{
    public class PaperlessRestContext : DbContext
    {
        public PaperlessRestContext()
        {
            
        }

        public PaperlessRestContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<MetaData> MetaDatas => Set<MetaData>();
        public DbSet<User> Users => Set<User>();

    }
}
