using StackExchange.Redis;
using KinoAPI.Services;
using KinoAPI.Middleware;
using KinoAPI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();


var redisMuxer = ConnectionMultiplexer.Connect(builder.Configuration.GetSection("RedisDatabase:ConnectionString").Value);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisMuxer);

builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<MovieCRUDService>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ForumService>();

builder.Services.AddScoped<ErrorHandlingMiddleware>();

builder.Services.AddScoped<SystemAccountSeeder>();


builder.Services.AddSwaggerGen(swagg =>
{
    swagg.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "basic",
        Type = SecuritySchemeType.Http,
        Description = "Basic Auth"
    });

    swagg.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthentication("BasicAuthentication").
            AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
            ("BasicAuthentication", null);

var app = builder.Build();

var scope = app.Services.CreateScope();
var accountSeeder = scope.ServiceProvider.GetRequiredService<SystemAccountSeeder>();
accountSeeder.Seed();

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");

    await next.Invoke();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ErrorHandlingMiddleware>();

CinemaSeedingService.ConfigureSeeder(app.Services.GetService<IConnectionMultiplexer>(),
                                     app.Configuration.GetSection("RedisDatabase:StoringKey").Value);
CinemaSeedingService.SeedDataBase();

app.MapControllers();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
