using HostedServices;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using MassTransit.Configuration;
using MassTransit.Options;
using Microsoft.Extensions.Logging;

namespace SmtpStorage
{
    class Program
    {
        static Task Main(string[] args)
        {
            return new HostBuilder()
                .ConfigureAppConfiguration((context, builder) => builder.AddEnvironmentVariables())
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddOptions()
                        .AddHostedService<MassTransitHostedService>()
                        .AddMassTransit(massTransit =>
                        {
                            massTransit.AddBus(provider => Bus.Factory.CreateUsingInMemory(inMemory =>
                            {
                                inMemory.UseBsonSerializer();
                                inMemory.UseExtensionsLogging(provider.GetRequiredService<ILoggerFactory>());

                                inMemory.UseSmtpStorage(smtpStorage =>
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
