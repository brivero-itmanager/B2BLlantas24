using Azure.Messaging.ServiceBus;
using ITManager.Domain.Interfaces;
using ITManager.Infrastructure.Persistance;
using ITManager.Infrastructure.Persistance.Repositories;
using ITManager.Worker.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;



var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, config) =>
    {
        config
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                "logs/worker-.txt",
                rollingInterval: RollingInterval.Day);
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
