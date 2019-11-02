using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.Objects.Commands;

namespace MassTransit.Repositories
{
    public interface ISmtpInstancesRepository
    {
        Task SendCommand(CreateSmtpInstanceComand command, CancellationToken cancellationToken);
    }
}
