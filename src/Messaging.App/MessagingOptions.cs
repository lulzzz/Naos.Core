﻿namespace Naos.Core.Messaging.App
{
    using Microsoft.Extensions.DependencyInjection;

    public class MessagingOptions
    {
        public MessagingOptions(INaosServicesContext context)
        {
            this.Context = context;
        }

        public INaosServicesContext Context { get; }
    }
}
