using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Commands;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Persistance;
using AllEvents.TicketManagement.Persistance.Caching;
using AllEvents.TicketManagement.Persistance.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuration
            ConfigureServices(builder);

            var app = builder.Build();

            // Middleware Configuration
            ConfigureMiddleware(app);

            // Endpoint Mapping
            ConfigureEndpoints(app);

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Database Context and Interceptors
            builder.Services.AddDbContext<AllEventsDbContext>((serviceProvider, options) =>
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var threshold = TimeSpan.FromMilliseconds(20);

                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                       .AddInterceptors(new QueryHandlerInterceptor(loggerFactory.CreateLogger<QueryHandlerInterceptor>(), threshold));
            });

            // Scoped Services
            builder.Services.AddScoped<IEventQuery, EventQuery>(provider =>
            {
                var dbContext = provider.GetRequiredService<IAllEventsDbContext>();
                return new EventQuery(dbContext.Events.AsQueryable());
            });

            builder.Services.AddScoped<IAllEventsDbContext>(provider => provider.GetService<AllEventsDbContext>());
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();

            // MediatR and Validators
            builder.Services.AddMediatR(typeof(CreateEventCommandHandler).Assembly);
            builder.Services.AddMediatR(typeof(UpdateEventCommandHandler).Assembly);
            builder.Services.AddValidatorsFromAssemblyContaining<CreateEventCommandValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UpdateEventCommandValidator>();

            // Caching
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
                options.InstanceName = "AllEvents:";
            });

            // Logging
            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddDebug();
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policyBuilder => policyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                );
            });

            // Controllers
            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configuration Binding
            builder.Configuration.Bind("Security", new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Security"));
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                // Development-specific middleware
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // General Middleware
            app.UseHttpsRedirection();
            app.UseCors();
            app.UseAuthorization();
        }

        private static void ConfigureEndpoints(WebApplication app)
        {
            // Map Controllers
            app.MapControllers();
        }
    }
}
