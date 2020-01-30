using System;
using HostedServices;
using MassTransit;
using MassTransit.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Options.MassTransit;
using HostOptions = Options.RabbitMq.HostOptions;

namespace SmtpManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services
                .AddHostedService<MassTransitHostedService>()
                .AddMassTransit(massTransit =>
                {
                    massTransit.AddRequestClient<QuerySmtpConnections>();
                    massTransit.AddRequestClient<QueryUserCredentials>();

                    massTransit.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(rabbitMq =>
                    {
                        var hostOptions = new HostOptions();
                        Configuration.GetSection("Host").Bind(hostOptions);

                        var rabbitMqHost = rabbitMq.Host(hostOptions.Host, hostOptions.VirtualHost, host =>
                        {
                            host.Username(hostOptions.Username);
                            host.Password(hostOptions.Password);
                        });

                        var busOptions = new BusOptions();
                        Configuration.GetSection("Bus").Bind(busOptions);

                        if (string.Equals("bson", busOptions.Serializer, StringComparison.OrdinalIgnoreCase))
                            rabbitMq.UseBsonSerializer();
                    }));
                });

            services
                .AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBus>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
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
