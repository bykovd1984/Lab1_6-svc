using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace IdentityServerAspNetIdentity.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var connStr = GetConnStr();

            builder.UseNpgsql(connStr, b => b.MigrationsAssembly("IdentityServerAspNetIdentity"));

            return new ApplicationDbContext(builder.Options);
        }

        private string GetConnStr()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

            return config.GetConnectionString("DefaultConnection");
        }
    }
}
