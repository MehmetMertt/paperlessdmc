using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaperlessREST.Infrastructure;
using PaperlessREST.Infrastructure.Repositories;
using PaperlessREST.Infrastructure.Service;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<PaperlessRestContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default"),
        b => b.MigrationsAssembly("PaperlessREST.Infrastructure")
    )
);


builder.Services.AddScoped<IMetaDataService, MetaDataService>();


builder.Services.AddScoped<IMetadataRepository, MetadataRepository>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PaperlessRestContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database initialization.");
    }
}



// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{*/
app.UseSwagger();
app.UseSwaggerUI();
/*}*/

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
