using IdentityRolesFromScratch.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityRolesFromScratch.Data
{
	public class ApplicationDBContext : IdentityDbContext<AppUser>
	{
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            
        }

    }
}
