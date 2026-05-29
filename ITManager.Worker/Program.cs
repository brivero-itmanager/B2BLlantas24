using ITManager.Domain.Interfaces;
using ITManager.Infrastructure.Persistance;
using ITManager.Infrastructure.Persistance.Repositories;
using ITManager.Worker.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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

        services.AddScoped<ITareaRepository, TareaRepository>();

        services.AddHostedService<TareaPollingWorker>();
    })
    .Build();

await host.RunAsync();
