﻿namespace Microsoft.Extensions.DependencyInjection
{
    using Microsoft.Extensions.Configuration;

    public class ServiceConfigurationContext
    {
        public IServiceCollection Services { get; set; }

        public Naos.Core.Common.ServiceDescriptor Descriptor { get; set; }

        public IConfiguration Configuration { get; set; }
    }
}