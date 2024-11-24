using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using SecretStore.Business.Interfaces;
using SecretStore.Business.Services;
using SecretStore.DataStore.Interface;
using SecretStore.DataStore.MsSqlStore;
using SecretStore.Options;

var builder = WebApplication.CreateBuilder(args);

string SecretStoreAllowSpecificOrigins = "_secretStoreAllowSpecificOrigins";

var services = builder.Services;
// Add services to the container.

// Add Newtonsoft.Json for JSON serialization and deserialization
services.AddControllers()
        .AddNewtonsoftJson(options =>
        {
            // Optional: Customize serializer settings
            options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Secret Store", Version = "v1" });
});

services.AddTransient<IPasswordEntriesBusinessService, PasswordEntriesBusinessService>();
services.AddTransient<ISecretStoreDataStore, SecretStoreDataStore>();

services.AddCors(options =>
{
    options.AddPolicy(name: SecretStoreAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var swaggerOptions = new SwaggerOptions();
    builder.Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

    app.UseSwagger(options =>
    {
        options.RouteTemplate = swaggerOptions.JsonRoute;
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description);
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(SecretStoreAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
