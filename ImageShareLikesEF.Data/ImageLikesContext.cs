using Microsoft.EntityFrameworkCore;

namespace ImageShareLikesEF.Data
{
    public class ImageLikesContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Image> Images { get; set; }

        public ImageLikesContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}