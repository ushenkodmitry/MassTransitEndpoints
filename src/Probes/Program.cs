using GreenPipes.Internals.Extensions;
using MassTransit;
using MassTransit.RazorRenderer.Messages;
using MassTransit.SmtpGateway;
using MassTransit.SmtpGateway.Configuration;
using MassTransit.SmtpGateway.Options;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MassTransit.RazorRenderer.Configuration;
using RazorRendererBehaviorOptions = MassTransit.RazorRenderer.Options.BehaviorOptions;
using SmtpGatewayBehaviorOptions = MassTransit.SmtpGateway.Options.BehaviorOptions;

namespace Probes
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingInMemory(inMemory =>
            {
                //inMemory.ReceiveEndpoint(endpoint =>
                //{
                //    SmtpGatewayConfigurationExtensions.UseSmtpGateway(endpoint, smtp =>
                //    {
                //        smtp.UseOptions((ServerOptions options) =>
                //        {
                //            options.Host = "";
                //            options.Port = 465;
                //            options.Username = "";
                //            options.Password = "";
                //            options.UseSsl = true;
                //        });
                //        smtp.UseOptions((SmtpGatewayBehaviorOptions options) =>
                //        {
                //        });
                //    });
                //});

                inMemory.ReceiveEndpoint(endpoint =>
                {
                    endpoint.UseRazorRenderer(renderer =>
                    {
                        renderer.UseOptions((RazorRendererBehaviorOptions options) =>
                        {
                            options.TemplatesFolder = "Templates";
                        });
                    });
                });
            });

            await bus.StartAsync();

            //await SendMail(bus);
            await RenderKeyedTemplate(bus);

            await bus.StopAsync();

            await Console.In.ReadLineAsync();
        }

        async static Task SendMail(IBus bus)
        {
            using (MemoryStream ms1 = new MemoryStream())
            using (MemoryStream ms2 = new MemoryStream())
            {
                var buffer1 = Encoding.UTF8.GetBytes("1,text,column,age,none");
                await ms1.WriteAsync(buffer1, 0, buffer1.Length);

                var buffer2 = Encoding.UTF8.GetBytes("123456789012345667890");
                await ms2.WriteAsync(buffer2, 0, buffer2.Length);

                await bus.SendMail(mail =>
                {
                    mail.To(to => to.Mailbox("Me", "me@mail.com"));
                    mail.From(from => from.Mailbox("Me", "me@mail.com"));
                    mail.WithSubject("Probes of SmtpGateway");
                    mail.WithImportance.High();
                    mail.WithBody(body => body.TextBody("Priem! Prinimau vas. Nemnozhechko rastyt peregryzki..."));
                    mail.WithAttachments(attachments =>
                    {
                        attachments.Attach("file.csv", ms1);
                        attachments.Attach("file.txt", ms2);
                    });
                });
            }

        }

        async static Task RenderKeyedTemplate(IBus bus)
        {
            RenderKeyedTemplate renderKeyedTemplate = TypeCache<RenderKeyedTemplate>.InitializeFromObject(new
            {
                TemplateKey = "Template.cshtml",
                Model = "{ \"Who\": \"everybody\"}"
            });

            await bus.Publish(renderKeyedTemplate);
        }
    }
}
