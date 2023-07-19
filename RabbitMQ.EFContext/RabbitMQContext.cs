using Microsoft.EntityFrameworkCore;
using RabbitMQ.Model;
using System.Collections.Generic; 
using System.Reflection.Emit;

namespace RabbitMQ.EFContext
{
    public class RabbitMQContext : DbContext
    {
        public RabbitMQContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=RabbitMQContext;Trusted_Connection=True;");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
        //entities
        public DbSet<Message> Messages { get; set; } 
    }
}