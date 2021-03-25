using Microsoft.Extensions.Configuration;
using System;
using System.Threading;

namespace Lab1_6.Data.Migrations
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");



            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            var connStr = config.GetSection("UsersDB").Value;

            Console.WriteLine(connStr);

            Thread.Sleep(500000);
        }
    }
}
