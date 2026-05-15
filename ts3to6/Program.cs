using ts3to6.Components;
using ts3to6.Services;
using Microsoft.AspNetCore.HttpOverrides;

namespace ts3to6
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Configure Forwarded Headers to support proxies like Nginx Proxy Manager
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // One instance per Blazor circuit (= per user tab)
            builder.Services.AddScoped<TsDbService>();
            builder.Services.AddScoped<MigrationService>();

            // Singleton: stores generated TS6 files in RAM for download
            builder.Services.AddSingleton<DownloadCache>();

            var app = builder.Build();

            // Forwarded Headers must be first in the pipeline
            app.UseForwardedHeaders();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection(); // Handled by the reverse proxy

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapControllers();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
