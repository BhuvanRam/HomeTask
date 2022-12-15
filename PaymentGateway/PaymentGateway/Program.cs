using Microsoft.EntityFrameworkCore;
using PaymentGateway.Domain;
using PaymentGateway.Middleware;
using PaymentGateway.Repositories;
using PaymentGateway.Services;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
string logFilePath = configuration["Serilog:LogFilePath"];
builder.Host.UseSerilog((context, configuration) => configuration.WriteTo.File(logFilePath, LogEventLevel.Information));

builder.Services.Configure<JwtConfiguration>(configuration.GetSection("JWT"));
builder.Services.AddDbContext<PaymentDbContext>(o => o.UseSqlServer(configuration["connectionStrings:PaymentDb"],
                                         b => b.MigrationsAssembly("PaymentGateway")));
builder.Services.RegisterPaymentGatewayServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandling>();
app.UseWhen(context => context.Request.Path.Value != null &&
                       !context.Request.Path.Value.StartsWith("/api/Payment/Token") &&
                       !context.Request.Path.Value.Contains("swagger"),
appBuilder =>
            {
                appBuilder.UseMiddleware<PaymentGatewayAuthorization>();
            }
           );

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/api/error");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }