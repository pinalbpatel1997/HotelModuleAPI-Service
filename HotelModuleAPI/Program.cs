using HotelModuleAPI.Interface;
using HotelModuleAPI.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IUser, UserRepository>();
builder.Services.AddScoped<IDBConnection, DBConnectionRepository>();
builder.Services.AddScoped<ICommon, CommonRepository>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
