using AllEvents.TicketManagement.Persistance.Repositories;
using AllEvents.TicketManagement.Persistance.Seeding;

namespace AllEvents.TicketManagement.App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddScoped<ReadEventsServiceReader>();
            builder.Services.AddTransient<DataSeeder>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                await seeder.SeedAsync("C:\\Users\\KonstantinDinev\\source\\repos\\All-Events\\AllEvents.TicketManagement\\src\\Infrastructure\\AllEvents.TicketManagement.Persistance\\Data\\EventsData.xlsx");
            }


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
