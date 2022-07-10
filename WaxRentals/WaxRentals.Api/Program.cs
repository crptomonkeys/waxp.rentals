using System.Text.Json.Serialization;
using WaxRentals.Api.Config;
using WaxRentals.Service.Shared.Config;

#nullable disable

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
                             options.AddDefaultPolicy(policy => policy.AllowAnyOrigin()
                                                                      .AllowAnyHeader()
                                                                      .WithMethods("get", "post"))
                        );
builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureSwaggerGen(options =>
{
    options.CustomSchemaIds(type => SwaggerSchemaIds.Generate(type));
});

Dependencies.AddDependencies(builder.Services, (string)Environment.GetEnvironmentVariables()["SERVICE"]);
builder.Services.AddSingleton<Mapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
