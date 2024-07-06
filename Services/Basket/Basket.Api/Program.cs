using System.Reflection;
using Basket.Api.Swagger;
using Basket.Application.Commands;
using Basket.Core.Repositories;
using Basket.Infrastructure.Repositories;
using Common.Logging.Correlation;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;
builder.Services.AddControllers();

//Mvc.Versioning.ApiExplorer
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder
    .Services.AddVersionedApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    })
    .AddApiVersioning()
    .AddCors(options =>
    {
        options.AddPolicy(
            "CorsPolicy",
            policy =>
            {
                //TODO read the same from settings for prod deployment
                policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            }
        );
    });

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>(
        "CacheSettings:ConnectionString"
    );
});

builder.Services.AddMediatR(typeof(CreateShoppingCartCommand).GetTypeInfo().Assembly);

builder.Services.AddScoped<IBasketRepository, BasketRepository>();

builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

builder.Services.AddAutoMapper(assembly);

//builder.Services.AddScoped<DiscountGrpcService>();

//builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(o =>
//    o.Address = new Uri(Configuration["GrpcSettings:DiscountUrl"])
//);

//builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerDefaultValues>();
});

builder
    .Services.AddHealthChecks()
    .AddRedis(
        builder.Configuration["CacheSettings:ConnectionString"],
        "Redis Health",
        HealthStatus.Degraded
    );

//builder.Services.AddMassTransit(config =>
//{
//    config.UsingRabbitMq(
//        (ct, cfg) =>
//        {
//            cfg.Host(Configuration["EventBusSettings:HostAddress"]);
//        }
//    );
//});

//services.AddMassTransitHostedService();



var app = builder.Build();

if (app.Environment.IsEnvironment("Development"))
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant()
            );
        }
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks(
        "/health",
        new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        }
    );
});

app.Run();
