using Microsoft.EntityFrameworkCore;

namespace Backend.Contexts
{
    /// <summary>
    /// Primaary data context for lambda
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
    }
}