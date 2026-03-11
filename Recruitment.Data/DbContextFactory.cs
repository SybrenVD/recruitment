using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Recruitment.Data
{
    public class DbContextFactory : IDesignTimeDbContextFactory<RecruitmentDbContext>
    {
        public RecruitmentDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RecruitmentDbContext>();

            // Connection string voor design-time (migrations)
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=RecruitmentDb;Trusted_Connection=True;");

            return new RecruitmentDbContext(optionsBuilder.Options);
        }
    }
}
