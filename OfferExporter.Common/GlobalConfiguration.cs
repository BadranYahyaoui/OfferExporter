using Microsoft.Extensions.Configuration;
using OfferExporter.Models;
using System;
using System.Data.Common;
using System.IO;
using System.Linq;

namespace OfferExporter.Common
{
    // ================== //
    // FIND DATABASE FILE //
    // ================== //

    public static class GlobalConfiguration
    {
        public static string GenerateConnectionString()
        {
            var dirPath = Environment.CurrentDirectory;
            var dbFilePath = string.Empty;
            try
            {
                while (dirPath is not null && !File.Exists(dbFilePath))
                {
                    var found = Directory.GetFiles(dirPath, "OFFERS.mdf", SearchOption.AllDirectories);
                    if (found.Any())
                    {
                        dbFilePath = found.Single();
                    }
                    else
                    {
                        dirPath = Directory.GetParent(dirPath)?.FullName;
                    }

                }
                if (dbFilePath is null || !File.Exists(dbFilePath))
                {
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Database OFFERS.mdf not found !");
                    throw new FileNotFoundException("Database OFFERS.mdf not found !");
                }
                var connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB; AttachDbFilename={dbFilePath}; Integrated Security=True;";
                return connectionString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetAllPromotionTargetsCommand() => GetDbCommand().PromotionTargets;
        public static string GetAllOffersCommand() => GetDbCommand().Offers;

        public static OfferExporterDbCommand GetDbCommand()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json");

            var config = configuration.Build();
            var dbCommand = config.GetSection("dbCommands").Get<OfferExporterDbCommand>();
            return dbCommand;
        }
    }
}
