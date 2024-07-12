using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Persistance;
using AllEvents.TicketManagement.Persistance.Repositories;
using AllEvents.TicketManagement.Persistance.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AllEvents.TicketManagement.App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AllEventsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AllEventsDbContext>();

            builder.Services.AddRazorPages();

            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<ReadEventsServiceReader>();

            builder.Services.AddTransient<DataSeeder>();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var app = builder.Build();

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

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
