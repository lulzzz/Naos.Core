﻿namespace Microsoft.Extensions.DependencyInjection
{
    public class ServiceOptions
    {
        public ServiceOptions(INaosServicesContext context)
        {
            this.Context = context;
        }

        public INaosServicesContext Context { get; }
    }
}
