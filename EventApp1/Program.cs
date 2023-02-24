using System.Data;
using EventApp1.Config;
using EventApp1.Repositories;
using interfaces;
using Npgsql;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connStrings = builder.Configuration.GetSection("connectionStrings").Get<DbO>();     //building connection to DB


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<NpgsqlConnection>((sp) => new NpgsqlConnection(connStrings.Main));
builder.Services.AddTransient<IEventRepository, EventRepository>(); //dodanie repozytorium do konternera DI
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
Console.WriteLine("test");
//app.UseHttpsRedirection();

app.UseAuthorization();




app.MapControllers();

app.Run();