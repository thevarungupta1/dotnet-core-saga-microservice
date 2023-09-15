﻿using Microsoft.EntityFrameworkCore;
using Order.API.Models;

namespace Order.API.Database
{
    public class OrderingContext: DbContext    
    {
        public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) 
        { 
        
        }

        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
