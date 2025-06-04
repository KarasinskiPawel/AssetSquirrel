using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AssetSquirrel.WebApp.Data
{
    public class AssetSquirrelUserIdentityContext(DbContextOptions<AssetSquirrelUserIdentityContext> options) : IdentityDbContext<IdentityUser>(options)
    {
    }
}
