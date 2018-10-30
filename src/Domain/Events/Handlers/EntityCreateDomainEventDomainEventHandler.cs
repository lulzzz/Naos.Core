﻿namespace Naos.Core.Domain
{
    using System.Threading;
    using System.Threading.Tasks;
    using Naos.Core.Common;

    public class EntityCreateDomainEventDomainEventHandler
        : IDomainEventHandler<EntityCreateDomainEvent<IEntity>>
    {
        public async Task Handle(EntityCreateDomainEvent<IEntity> notification, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                if (notification?.Entity.Is<IStateEntity>() == true)
                {
                    var entity = notification.Entity.As<IStateEntity>();
                    entity.State?.SetCreated("[IDENTITY]", "domainevent"); // TODO
                    entity.State?.UpdateIdentifierHash(notification.Entity);
                }
            });
        }
    }
}
