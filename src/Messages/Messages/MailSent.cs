using System;

namespace MassTransit.SmtpGateway.Messages
{
    public interface MailSent : CorrelatedBy<Guid>
    { }
}
