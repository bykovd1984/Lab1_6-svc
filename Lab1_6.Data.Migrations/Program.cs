using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;

namespace Lab1_6.Data.Migrations
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var dbContext = new UsersDbContextFactory().CreateDbContext(args);

                var pendingMigrations = dbContext.Database.GetPendingMigrations();

                Console.WriteLine($"Pending migrations count: {pendingMigrations.Count()}");

                if (pendingMigrations.Count() > 0)
                {
                    dbContext.Database.Migrate();

                    Console.WriteLine($"Migrations applied.");
                }
            } 
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
