﻿namespace Naos.Core.Domain.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EnsureThat;
    using MediatR;
    using Naos.Core.Common;
    using Naos.Core.Domain.Specifications;

    /// <summary>
    /// Represents an InMemoryRepository
    /// </summary>
    /// <typeparam name="T">The type of the domain entity</typeparam>
    /// <seealso cref="Domain.IRepository{T, TId}" />
    public class InMemoryRepository<T> : IRepository<T>
        where T : class, IEntity, IAggregateRoot
    {
        protected IEnumerable<T> entities;
        private readonly IMediator mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{T}" /> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="options">The options.</param>
        public InMemoryRepository(
            IMediator mediator,
            IRepositoryOptions options = null)
            : this(mediator, null, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{T}" /> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="entities">The entities.</param>
        /// <param name="options">The options.</param>
        public InMemoryRepository(IMediator mediator, IEnumerable<T> entities = null, IRepositoryOptions options = null)
        {
            EnsureArg.IsNotNull(mediator, nameof(mediator));

            this.mediator = mediator;
            this.entities = entities.NullToEmpty();
            this.Options = options;
        }

        protected IRepositoryOptions Options { get; }

        /// <summary>
        /// Finds all asynchronous.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindAllAsync(IFindOptions<T> options = null)
        {
            return await this.FindAllAsync(specifications: null, options: options);
        }

        /// <summary>
        /// Finds all asynchronous.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindAllAsync(ISpecification<T> specification, IFindOptions<T> options = null)
        {
            return specification == null
                ? await this.FindAllAsync(specifications: null, options: options)
                : await this.FindAllAsync(new[] { specification }, options);
        }

        /// <summary>
        /// Finds all asynchronous.
        /// </summary>
        /// <param name="specifications">The specifications.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> FindAllAsync(IEnumerable<ISpecification<T>> specifications, IFindOptions<T> options = null)
        {
            var result = this.entities;

            foreach (var specification in specifications.NullToEmpty())
            {
                result = result.Where(this.EnsurePredicate(specification));
            }

            return await Task.FromResult(this.FindAll(result, options));
        }

        /// <summary>
        /// Finds the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">id</exception>
        public async Task<T> FindOneAsync(object id)
        {
            if (id.IsDefault())
            {
                return null;
            }

            var result = this.entities.FirstOrDefault(x => x.Id.Equals(id));

            if (this.Options?.Mapper != null && result != null)
            {
                return this.Options.Mapper.Map<T>(result);
            }

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Asynchronous checks if element exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(object id)
        {
            if (id.IsDefault())
            {
                return false;
            }

            return await this.FindOneAsync(id) != null;
        }

        /// <summary>
        /// Adds or updates asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Method for generating new Ids not provided</exception>
        public async Task<T> AddOrUpdateAsync(T entity)
        {
            if (entity == null)
            {
                return null;
            }

            bool isTransient = false;
            if (entity.Id.IsDefault())
            {
                // TODO: move this to seperate class (IdentityGenerator)
                if (entity is IEntity<int>)
                {
                    (entity as IEntity<int>).Id = this.entities.Count() + 1;
                }
                else if (entity is IEntity<string>)
                {
                    (entity as IEntity<string>).Id = Guid.NewGuid().ToString();
                }
                else if (entity is IEntity<Guid>)
                {
                    (entity as IEntity<Guid>).Id = Guid.NewGuid();
                }
                else
                {
                    throw new NotSupportedException($"Entity Id type {entity.Id.GetType().Name}");
                    // TODO: or just set Id to null?
                }

                isTransient = true;
            }

            if (isTransient && entity is IStateEntity)
            {
                // TODO: do this in an notification handler (EntityAddDomainEvent), the handler has access to the requestcontext and can set the created user (State.CreatedBy)
                entity.As<IStateEntity>().State.CreatedDate = new DateTimeEpoch();
            }

            if (!isTransient && entity is IStateEntity)
            {
                // TODO: do this in an notification handler (EntityUpdateDomainEvent)
                entity.As<IStateEntity>().State.UpdatedDate = new DateTimeEpoch();
            }

            if (this.Options?.PublishEvents == true)
            {
                if (isTransient)
                {
                    await this.mediator.Publish(new EntityCreateDomainEvent<T>(entity)).ConfigureAwait(false);
                }
                else
                {
                    await this.mediator.Publish(new EntityUpdateDomainEvent<T>(entity)).ConfigureAwait(false);
                }
            }

            // TODO: map to destination
            this.entities = this.entities.Concat(new[] { entity }.AsEnumerable());

            if (this.Options?.PublishEvents == true)
            {
                if (isTransient)
                {
                    await this.mediator.Publish(new EntityCreatedDomainEvent<T>(entity)).ConfigureAwait(false);
                }
                else
                {
                    await this.mediator.Publish(new EntityUpdatedDomainEvent<T>(entity)).ConfigureAwait(false);
                }
            }

            return entity;
        }

        /// <summary>
        /// Deletes asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">id</exception>
        public async Task DeleteAsync(object id)
        {
            if (id.IsDefault())
            {
                return;
            }

            var entity = this.entities.FirstOrDefault(x => x.Id.Equals(id));
            if (entity != null)
            {
                if (this.Options?.PublishEvents == true)
                {
                    await this.mediator.Publish(new EntityDeleteDomainEvent<T>(entity)).ConfigureAwait(false);
                }

                this.entities = this.entities.Where(x => !x.Id.Equals(entity.Id));

                if (this.Options?.PublishEvents == true)
                {
                    await this.mediator.Publish(new EntityDeletedDomainEvent<T>(entity)).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Deletes asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Id</exception>
        public async Task DeleteAsync(T entity)
        {
            if (entity?.Id.IsDefault() != false)
            {
                return;
            }

            this.entities = this.entities.Where(x => !x.Id.Equals(entity.Id));
            if (this.Options?.PublishEvents == true)
            {
                await this.mediator.Publish(new EntityDeletedDomainEvent<T>(entity)).ConfigureAwait(false);
            }
        }

        protected virtual Func<T, bool> EnsurePredicate(ISpecification<T> specification)
        {
            return specification.ToPredicate();
        }

        protected virtual IEnumerable<T> FindAll(IEnumerable<T> entities, IFindOptions<T> options = null)
        {
            var result = entities;

            if (options?.Skip.HasValue == true && options.Skip.Value > 0)
            {
                result = result.Skip(options.Skip.Value);
            }

            if (options?.Take.HasValue == true && options.Take.Value > 0)
            {
                result = result.Take(options.Take.Value);
            }

            if (this.Options?.Mapper != null && result != null)
            {
                return result.Select(r => this.Options.Mapper.Map<T>(r));
            }

            return result;
        }
    }
}