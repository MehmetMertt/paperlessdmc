using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaperlessREST.Infrastructure;
using PaperlessREST.Infrastructure.Repositories;
using PaperlessREST.DataAccess.Service;

var builder = WebApplication.CreateBuilder(args); // an object which stores configurations (env-variables, etc.) to later build the webapp

builder.Services.AddDbContext<PaperlessRestContext>(options =>    // specifies context (connection to database, manages database entities, follows changes on objects) 
    options.UseNpgsql(                                            // usenpgsql ... uses postgres as database
        builder.Configuration.GetConnectionString("Default"),     // "Default" ... uses the connection specified in appsettings.json
        b => b.MigrationsAssembly("PaperlessREST.Infrastructure") // MigrationsAssembly ... specifies where ef-migrations are saved
    )
);

// scope ... lifetime of the service per HTTP request
builder.Services.AddScoped<IMetaDataService, MetaDataService>();       // MetaDataService ... consists of functions for metadata CRUD operations
builder.Services.AddScoped<IMetadataRepository, MetadataRepository>(); // Repository pattern for data access

builder.Services.AddControllers(); // registers MVC-controller, without this aps.net wouldnt know which classes to use for HTTP request (endpoint /api/metadata)
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// initializes the database here
using (var scope = app.Services.CreateScope()) // scope ... small container for services
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PaperlessRestContext>();
        context.Database.EnsureDeleted(); // deletes database on start (only good for development)
        context.Database.EnsureCreated(); // creates database on start
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database initialization.");
    }
}

/*if (app.Environment.IsDevelopment())
{*/
app.UseSwagger();
app.UseSwaggerUI();
/*}*/
app.UseHttpsRedirection(); // redirects HTTP to HTTPS
app.UseAuthorization();
app.MapControllers();      // connects controller endpoints through routing
app.Run();
