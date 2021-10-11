using Microsoft.Extensions.Configuration;
using System;

namespace Lab1_6.Models
{
    public class AppConfigs
    {
        public string UsersDB { get; private set; }
       
        public string Kafka { get; private set; }

        public static AppConfigs Init(IConfiguration configuration)
        {
            var config = new AppConfigs();

            config.UsersDB = GetConnStr(configuration);

            config.Kafka = configuration.GetSection("Kafka")?.Value;

            return config;
        }

        public static string GetConnStr(IConfiguration configuration)
        {
            var connStr = configuration.GetSection("UsersDB")?.Value;

            var pass = configuration.GetSection("USERS_DB_PASSWORD")?.Value;

            if (!string.IsNullOrEmpty(pass) && !string.IsNullOrEmpty(connStr))
                connStr = connStr.Replace("{passwrord_placeholder}", pass);

            if (string.IsNullOrEmpty(connStr))
                throw new InvalidOperationException("Connection string for UsersDbContext is empty.");

            return connStr;
        }
    }
}
