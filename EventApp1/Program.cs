using System.Data;
using System.Text;
using EventApp1.Config;
using EventApp1.Repositories;
using EventApp1.Services;
using interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connStrings = builder.Configuration.GetSection("connectionStrings").Get<DbO>();     //building connection to DB
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtConfig>();


builder.Services.AddControllers();
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    options.SwaggerDoc("v1",new OpenApiInfo
        {
            Version = "v1",
            Title = "EventApp",
            Description = "Aplikacja do obsługi wydarzeń",
            
        }
    ));
builder.Services.AddTransient<NpgsqlConnection>((sp) => new NpgsqlConnection(connStrings.Main));
builder.Services.AddTransient<IEventRepository, EventRepository>(); //dodanie repozytorium do konternera DI
builder.Services.AddTransient<IUserService, UserRepository>(); //dodanie repozytorium do konternera DI
builder.Services.AddTransient<IPasswordServices, PasswordServices>(); //dodanie repozytorium do konternera DI
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.Configuration = new OpenIdConnectConfiguration();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key))
    };
});
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();




app.MapControllers();

app.Run();