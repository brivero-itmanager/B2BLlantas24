using ITManager.Application.Commands.ActualizarTarea;
using ITManager.Application.Commands.CrearTarea;
using ITManager.Application.Queries.GetTareaById;
using ITManager.Application.Queries.GetTareas;
using ITManager.Application.Commands.CreateSample;
using ITManager.Application.Commands.DeleteSample;
using ITManager.Application.Commands.UpdateSample;
using ITManager.Application.Queries.GetSampleById;
using ITManager.Application.Queries.GetSamples;
using ITManager.Domain.Entities;
using ITManager.Domain.Interfaces;
using ITManager.Infrastructure.Persistance;
using ITManager.Infrastructure.Persistance.Repositories;
using ITManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<ITManagerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IRepository<SampleEntity>, SampleRepository>();
builder.Services.AddScoped<ITareaRepository, TareaRepository>();

// Register command and query handlers
builder.Services.AddScoped<CreateSampleCommandHandler>();
builder.Services.AddScoped<CrearTareaCommandHandler>();
builder.Services.AddScoped<ActualizarTareaCommandHandler>();
builder.Services.AddScoped<GetTareasQueryHandler>();
builder.Services.AddScoped<GetTareaByIdQueryHandler>();
builder.Services.AddScoped<GetSamplesQueryHandler>();
builder.Services.AddScoped<GetSampleByIdQueryHandler>();
builder.Services.AddScoped<UpdateSampleCommandHandler>();
builder.Services.AddScoped<DeleteSampleCommandHandler>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for Blazor client
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7065", "http://localhost:5051")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("BlazorClient");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
