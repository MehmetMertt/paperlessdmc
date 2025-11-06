using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaperlessREST.Infrastructure; // here is the OCR-Worker.cs

Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<OcrWorker>();
    })
    .Build()
    .Run();
