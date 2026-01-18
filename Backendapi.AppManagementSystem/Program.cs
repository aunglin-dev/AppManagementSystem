using BackendAPI.Domain;
using BuildingBlocks.Exceptions.Handler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.BackendAPIDomin();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();



var app = builder.Build();

// Configure the HTTP request pipeline.



app.UseHttpsRedirection();


app.UseAuthentication();


app.MapControllers();


app.Run();

