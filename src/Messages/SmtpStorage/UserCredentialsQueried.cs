﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.Messages
{
    public interface UserCredentialsQueried
    {
        UserCredentials[] UserCredentials { get; }
    }
}
