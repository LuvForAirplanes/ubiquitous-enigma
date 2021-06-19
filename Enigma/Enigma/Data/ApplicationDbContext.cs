using Enigma.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Enigma.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Counselor> Counselors { get; set; }
        public DbSet<CounselorTemplate> CounselorTemplates { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
