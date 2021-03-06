﻿namespace Naos.Core.Infrastructure.Azure.KeyVault
{
    public class KeyVaultConfiguration
    {
        public bool Enabled { get; set; }

        public string Name { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}
