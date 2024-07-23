using Microsoft.EntityFrameworkCore;

namespace Task_Cat_ProMvc.Models.Data
{
    public class Cat_ProductDbContext:DbContext
    {
       

        public Cat_ProductDbContext(DbContextOptions<Cat_ProductDbContext> options) : base(options)
        {
         
          
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApiCalling> ApiCallings { get; set; }
        //public DbSet<CallApi> CallApi { get; set; } 
    }
}
