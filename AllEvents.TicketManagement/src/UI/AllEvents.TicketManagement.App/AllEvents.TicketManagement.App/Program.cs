using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Commands;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Persistance;
using AllEvents.TicketManagement.Persistance.Caching;
using AllEvents.TicketManagement.Persistance.Repositories;
using AllEvents.TicketManagement.Persistance.Seeding;
using AllEvents.TicketManagement.Persistence.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuration
            ConfigureServices(builder);

            var app = builder.Build();

            // Database Seeding
            await SeedDatabase(app);

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

            // Identity Services
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AllEventsDbContext>();

            // Scoped Services
            builder.Services.AddScoped<IEventQuery, EventQuery>(provider =>
            {
                var dbContext = provider.GetRequiredService<IAllEventsDbContext>();
                return new EventQuery(dbContext.Events.AsQueryable());
            });

            builder.Services.AddScoped<IAllEventsDbContext>(provider => provider.GetRequiredService<AllEventsDbContext>());
            builder.Services.AddScoped<ReadEventsServiceReader>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddTransient<DataSeeder>();

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

            // Razor Pages
            builder.Services.AddRazorPages();

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
        }

        private static async Task SeedDatabase(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AllEventsDbContext>();
                dbContext.Database.Migrate();

                var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                var basePath = AppContext.BaseDirectory;
                var relativePath = Path.GetRelativePath(basePath, "../../../Infrastructure/AllEvents.TicketManagement.Persistance/Data/EventsData.xlsx");
                var filePath = Path.Combine(basePath, relativePath);

                await seeder.SeedAsync(filePath);
            }
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                // Production-specific middleware
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // General Middleware
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
        }

        private static void ConfigureEndpoints(WebApplication app)
        {
            // Map Razor Pages
            app.MapRazorPages();
        }
    }
}
