using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaperlessREST.Infrastructure;

namespace PaperlessREST.Test
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // replacing postgres database with an inmemory database
                var dbDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<PaperlessRestContext>)
                );

                if (dbDescriptor != null)
                    services.Remove(dbDescriptor);

                services.AddDbContext<PaperlessRestContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));

                // mocking rabbitmq
                services.RemoveAll<RabbitMqService>();
                services.AddSingleton<RabbitMqService>(_ => new FakeRabbitMqService());

                // mocking minio
                services.RemoveAll<IMinioClient>();
                services.AddSingleton<IMinioClient>(_ => new FakeMinioClient());
            });
        }
    }

}
