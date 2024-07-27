using EventBus.Messages.Common;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application.Extensions;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add builder.Services to the container.

builder.Services.AddControllers();
builder.Services.AddApiVersioning();

builder.Services.AddApplicationServices();
builder.Services.AddInfraServices(builder.Configuration);

builder.Services.AddScoped<BasketOrderingConsumer>();

//builder.Services.AddScoped<BasketOrderingConsumerV2>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering.API", Version = "v1" });
});

builder.Services.AddHealthChecks().Services.AddDbContext<OrderContext>();

builder.Services.AddMassTransit(config =>
{
    //Mark this as consumer
    config.AddConsumer<BasketOrderingConsumer>();
    //config.AddConsumer<BasketOrderingConsumerV2>();
    config.UsingRabbitMq(
        (ctx, cfg) =>
        {
            cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
            //provide the queue name with consumer settings
            cfg.ReceiveEndpoint(
                EventBusConstant.BasketCheckoutQueue,
                c =>
                {
                    c.ConfigureConsumer<BasketOrderingConsumer>(ctx);
                }
            );
            ////V2 endpoint will pick items from here
            //cfg.ReceiveEndpoint(
            //    EventBusConstant.BasketCheckoutQueueV2,
            //    c =>
            //    {
            //        c.ConfigureConsumer<BasketOrderingConsumerV2>(ctx);
            //    }
            //);
        }
    );
});

builder.Services.AddMassTransitHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.MigrateDatabase<OrderContext>(
        (context, services) =>
        {
            var logger = services.GetService<ILogger<OrderContextSeed>>();
            OrderContextSeed.SeedAsync(context, logger!).Wait();
        }
    );

    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.API v1"));

    app.UseHttpsRedirection();
}

app.UseRouting();

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
