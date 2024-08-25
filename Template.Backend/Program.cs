using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Template.Backend;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

// Configure the SwaggerUI in .Net 9
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
opciones.UseSqlServer("name=DefaultConnection"));

// Codigo para Accelerar la acciones
builder.Services.AddOutputCache(opciones =>
{
    opciones.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60);
});

// Variable para CORS
var origins =builder.Configuration.GetValue<string>("origins")!.Split(',');

// Cors Politic
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(opcionesCORS =>
    {
        opcionesCORS.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();

    // Configure the SwaggerUI in .Net 9
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseOutputCache();

app.UseAuthorization();

app.MapControllers();

app.Run();
