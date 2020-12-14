using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Lupusec2Mqtt.Lupusec;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Serilog.Core;
using Serilog;

namespace Lupusec2Mqtt
{
    public class Startup
    {
        public readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            foreach (var kvp in Configuration.AsEnumerable())
            {
                Console.WriteLine($"{kvp.Key} => {kvp.Value}");
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<PollingHostedService>();

            services.AddHttpClient<LupusecTokenHandler>(client =>
            {
                client.BaseAddress = new Uri(Configuration["Lupusec:Url"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Configuration["Lupusec:Login"]}:{Configuration["Lupusec:Password"]}")));
            })
            .ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator })
            .AddTransientHttpErrorPolicy(c => 
            {
                return c.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                },
                (ex, timespan) => LupusecTokenHandler.ResetToken());
            });

            services.AddHttpClient<ILupusecService, LupusecService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["Lupusec:Url"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Configuration["Lupusec:Login"]}:{Configuration["Lupusec:Password"]}")));
            })
            .ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator })
            .AddHttpMessageHandler<LupusecTokenHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello Lupusec2Mqtt!");
                });
            });
        }
    }
}
