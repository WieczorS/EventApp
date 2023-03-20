
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using EventApp1.Config;
using EventApp1.Repositories;
using EventApp1.Services;

using interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;


var builder = WebApplication.CreateBuilder(args);
var connStrings = builder.Configuration.GetSection("connectionStrings").Get<DbO>();     //building connection to DB
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtConfig>();



//serilog moze zadziala



// Add services to the container.

// connStrings = builder.Configuration.GetSection("connectionStrings").Get<DbO>();
// jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtConfig>();

ConfigureLogging();
builder.Host.UseSerilog();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;

});
// builder.Services.AddBlazoredLocalStorage(config =>
// {
//     config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
//     config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
//     config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
//     config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
//     config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
//     config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
//     config.JsonSerializerOptions.WriteIndented = false;
// });

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
builder.Services.AddTransient<ITokenService, JwtTokenService >(); //dodanie repozytorium do konternera DI


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

// builder.Services.AddLogging(loggingBuilder => {
// });
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
app.UseHttpLogging();



app.MapControllers();

app.Run();

#region elastic
void ConfigureLogging()
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile(
            $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
            optional: true)
        .Build();

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
        .Enrich.WithProperty("Environment", environment)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
{
    return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
    };
}
#endregion