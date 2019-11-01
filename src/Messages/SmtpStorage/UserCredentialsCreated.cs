using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.Messages
{
    public interface UserCredentialsCreated
    {
        public int Id { get; }
    }
}
