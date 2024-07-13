using System.Reflection;
using Common.Logging.Correlation;
using Discouint.Api.Services;
using Discount.Application.Handlers;
using Discount.Core.Repositories;
using Discount.Instructure.Extensions;
using Discount.Instructure.Repositories;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(typeof(CreateDiscountCommandHandler).GetTypeInfo().Assembly);
builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddAutoMapper(assembly);
builder.Services.AddGrpc();

var app = builder.Build();

if (app.Environment.IsEnvironment("Development"))
{
    app.UseDeveloperExceptionPage();
}

app.MapGrpcService<DiscountService>();

app.MigrateDatabase<Program>();

app.MapGet(
    "/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
);

app.Run();
