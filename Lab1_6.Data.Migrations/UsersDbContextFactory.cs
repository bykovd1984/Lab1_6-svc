using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace Lab1_6.Data
{
    public class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
    {
        public UsersDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<UsersDbContext>();

            var connStr = GetConnStr();

            builder.UseNpgsql(connStr, b => b.MigrationsAssembly("Lab1_6.Data.Migrations"));

            return new UsersDbContext(builder.Options);
        }

        private string GetConnStr()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

            var connStr = config.GetSection("UsersDB")?.Value;

            var pass = config.GetSection("USERS_DB_PASSWORD")?.Value;

            if (!string.IsNullOrEmpty(pass))
                connStr = connStr.Replace("{passwrord_placeholder}", pass);

            if (string.IsNullOrEmpty(connStr))
                throw new InvalidOperationException("Connection string for UsersDbContext is empty.");

            return connStr;
        }
    }
}
