
using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Commands;
using AllEvents.TicketManagement.Application.Features.Events.Handlers;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Persistance;
using AllEvents.TicketManagement.Persistance.Caching;
using AllEvents.TicketManagement.Persistance.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AllEvents.TicketManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<AllEventsDbContext>((serviceProvider, options) =>
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var threshold = TimeSpan.FromMilliseconds(20); 

                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                       .AddInterceptors(new QueryHandlerInterceptor(loggerFactory.CreateLogger<QueryHandlerInterceptor>(), threshold));
            });


            builder.Services.AddScoped<IEventQuery, EventQuery>(provider =>
            {
                var dbContext = provider.GetRequiredService<IAllEventsDbContext>();
                return new EventQuery(dbContext.Events.AsQueryable());
            });

            builder.Services.AddScoped<IAllEventsDbContext>(provider => provider.GetService<AllEventsDbContext>());
            builder.Services.AddLogging(configure => configure.AddConsole());
            builder.Services.AddMediatR(typeof(CreateEventCommandHandler).Assembly);
            builder.Services.AddMediatR(typeof(UpdateEventCommandHandler).Assembly);
            builder.Services.AddValidatorsFromAssemblyContaining<CreateEventCommandValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UpdateEventCommandValidator>();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policyBuilder => policyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                );
            });

            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();

            builder.Services.AddScoped<IEventQuery, EventQuery>(provider =>
            {
                var dbContext = provider.GetRequiredService<IAllEventsDbContext>();
                return new EventQuery(dbContext.Events.AsQueryable());
            });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
                options.InstanceName = "AllEvents:";
            });

            builder.Services.AddLogging(config =>
            {
                config.AddConsole(); 
                config.AddDebug(); 
            });

            builder.Services.AddMediatR(typeof(GetAllEventsQueryHandler).Assembly);


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Configuration.Bind("Security", new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Security"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
