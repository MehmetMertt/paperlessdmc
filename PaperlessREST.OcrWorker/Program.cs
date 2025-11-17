using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaperlessREST.Infrastructure;
using PaperlessREST.DataAccess.Service;
using PaperlessREST.Infrastructure.Repositories;
using PaperlessREST.OcrWorker.Services;
using System;

namespace PaperlessREST.OcrWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new Exception("DB_CONNECTION is not set for OCRWorker!");

                services.AddDbContext<PaperlessRestContext>(options =>
                    options.UseNpgsql(connectionString));

                services.AddScoped<IMetadataRepository, MetadataRepository>();
                services.AddScoped<IMetaDataService, MetaDataService>();

                services.AddSingleton<GenAiService>();

                services.AddHostedService<OCRWorker>();
            });
    }
}
