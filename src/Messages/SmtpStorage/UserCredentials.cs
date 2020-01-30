using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.Messages
{
    public interface UserCredentials
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
