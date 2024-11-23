using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthMicroService.Entities;

namespace AuthMicroService.DataAccess
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }
        
        public DbSet<RefreshToken> RefreshTokens { get; set; }

    }
}