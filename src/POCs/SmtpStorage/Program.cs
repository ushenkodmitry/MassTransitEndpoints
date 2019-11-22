using System;
using System.Threading.Tasks;
using HostedServices;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Options.MassTransit;
using HostOptions = Options.RabbitMq.HostOptions;

namespace SmtpStorage
{
    class Program
    {
        static Task Main()
        {
            return new HostBuilder()
                .ConfigureAppConfiguration((context, builder) => builder.AddEnvironmentVariables())
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddHostedService<MassTransitHostedService>()
                        .AddMassTransit(massTransit =>
                        {
                            massTransit.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(rabbitMq =>
                            {
                                var hostOptions = new HostOptions();
                                context.Configuration.GetSection("Host").Bind(hostOptions);

                                var rabbitMqHost = rabbitMq.Host(hostOptions.Host, hostOptions.VirtualHost, host =>
                                {
                                    host.Username(hostOptions.Username);
                                    host.Password(hostOptions.Password);
                                });

                                var busOptions = new BusOptions();
                                context.Configuration.GetSection("Bus").Bind(busOptions);

                                if(string.Equals("bson", busOptions.Serializer, StringComparison.OrdinalIgnoreCase))
                                    rabbitMq.UseBsonSerializer();

                                rabbitMq.UseExtensionsLogging(provider.GetRequiredService<ILoggerFactory>());

                                rabbitMq.UseSmtpStorage(smtpStorage =>
                                {
                                    smtpStorage.Configure((ConnectionStringsOptions options) =>
                                    {
                                        context.Configuration.GetSection("ConnectionStrings").Bind(options);
                                    });
                                });
                            }));
                        });
                })
                .ConfigureLogging(builder => builder.AddConsole())
                .UseConsoleLifetime()
                .RunConsoleAsync();
        }
    }
}
