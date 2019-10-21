using Akka.Actor;
using ControlTower.Printer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ControlTower
{
    /// <summary>
    /// Contains the startup logic for the application
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configures runtime services for the application
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            var printerStatus = new PrinterStatus();
            var actorSystem = ActorSystem.Create("Printer");

            var monitor = actorSystem.ActorOf(PrinterMonitor.Props(),"printer-monitor");
            var printer = actorSystem.ActorOf(PrinterDevice.Props(monitor), "printer");

            services.AddSingleton(printerStatus);
            services.AddSingleton<IPrinterService>(new PrinterService(printer));

            services.AddServerSideBlazor();
            services.AddRazorPages();
        }

        /// <summary>
        /// Configures the HTTP pipeline for the application
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
