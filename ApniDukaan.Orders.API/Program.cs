using ApniDukaan.Orders.API.Middleware;
using ApniDukaan.Orders.Business;
using ApniDukaan.Orders.Business.HttpClients;
using ApniDukaan.Orders.DataAccessLayer;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Adding Data Access layer and Business layer
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLayer(builder.Configuration);

builder.Services.AddControllers();

// Fluent Validations
builder.Services.AddFluentValidationAutoValidation();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Http Client service
builder.Services.AddHttpClient<UsersMicroserviceClient>(client =>
{
    client.BaseAddress = new Uri($"{builder.Configuration["UsersMicroserviceName"]}:{builder.Configuration["UsersMicroservicePort"]}");
});

var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseRouting();

// Cors
app.UseCors();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Auth
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
