using ITManager.Infrastructure.Persistance;
using ITManager.Worker.Workers;
using Microsoft.EntityFrameworkCore;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, config) =>
    {
        config
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/worker-.txt", rollingInterval: RollingInterval.Day);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<ITManagerDbContext>(options =>
            options.UseSqlServer(
                context.Configuration.GetConnectionString("DefaultConnection")));

        services.AddHostedService<TareaPollingWorker>();
    })
    .Build();

await host.RunAsync();
