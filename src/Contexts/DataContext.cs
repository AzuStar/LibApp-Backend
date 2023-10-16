using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Contexts
{
    /// <summary>
    /// Primaary data context for lambda
    /// </summary>
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Branch> Branches { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
    }
}