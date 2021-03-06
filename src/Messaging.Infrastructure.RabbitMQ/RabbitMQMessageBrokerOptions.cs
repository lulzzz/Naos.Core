﻿namespace Naos.Core.Messaging.Infrastructure.RabbitMQ
{
    using MediatR;
    using Naos.Core.Common;
    using Naos.Core.Messaging.Domain;

    public class RabbitMQMessageBrokerOptions : BaseOptions
    {
        public IMediator Mediator { get; set; }

        public ISerializer Serializer { get; set; }

        public IMessageHandlerFactory HandlerFactory { get; set; }

        public ISubscriptionMap Map { get; set; }

        public string Host { get; set; }
    }
}
