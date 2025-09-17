using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TheBestOfChuck.Application.Interfaces;
using TheBestOfChuck.Application.Services;
using TheBestOfChuck.Domain;
using TheBestOfChuck.Domain.Interfaces;
using TheBestOfChuck.Infrastructure;
using TheBestOfChuck.Infrastructure.Utils;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IJokeProvider, ChuckJokeProvider>();
builder.Services.AddScoped<IJokeRepository, JokeRepository>();
builder.Services.AddScoped<IJokeService, JokeService>();
builder.Services.AddScoped<IJokeFactory, JokeFactory>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddDbContext<JokeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("JokesDatabase")));

builder.Build().Run();
