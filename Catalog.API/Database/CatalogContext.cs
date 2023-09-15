﻿using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Database
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options) :base(options) 
        {

        }

        public DbSet<CatalogItem> CatalogItems { get; set; }    
    }
}
