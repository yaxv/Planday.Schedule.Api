using Microsoft.Data.Sqlite;
using Planday.Schedule.Commands;
using Planday.Schedule.Infrastructure.Commands;
using Planday.Schedule.Infrastructure.Providers;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Infrastructure.Queries;
using Planday.Schedule.Providers;
using Planday.Schedule.Queries;
using Planday.Schedule.UseCases;
using Planday.Schedule.UseCases.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// providers
builder.Services.AddSingleton<IConnectionStringProvider>(new ConnectionStringProvider(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddScoped<IPlandayEmployeeProvider, PlandayEmployeeProvider>();

// commands
builder.Services.AddScoped<ICreateShiftCommand, CreateShiftCommand>();
builder.Services.AddScoped<IUpdateShiftCommand, UpdateShiftCommand>();

// queries
builder.Services.AddScoped<IEmployeeExistsQuery, EmployeeExistsQuery>();
builder.Services.AddScoped<IGetAllShiftsQuery, GetAllShiftsQuery>();
builder.Services.AddScoped<IGetOverlappingShiftsQuery, GetOverlappingShiftsQuery>();
builder.Services.AddScoped<IGetShiftQuery, GetShiftQuery>();

// use cases
builder.Services.AddScoped<IAssignEmployeeToShiftService, AssignEmployeeToShiftService>();
builder.Services.AddScoped<ICreateShiftService, CreateShiftService>();
builder.Services.AddScoped<IGetShiftService, GetShiftService>();

builder.Services.AddTransient<SqliteConnection>(serviceProvider =>
{
    var cs = serviceProvider.GetRequiredService<IConnectionStringProvider>().GetConnectionString();
    var connection = new SqliteConnection(cs);
    return connection;
});

builder.Services.AddHttpClient("planday", (serviceProvider, client) =>
{
    var config = serviceProvider
        .GetRequiredService<IConfiguration>()
        .GetSection("PlanDayApi");

    client.BaseAddress = new Uri(config["BaseUrl"]);

    client.DefaultRequestHeaders.Add("Authorization",
        Environment.GetEnvironmentVariable("PlanDayAuthToken"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
