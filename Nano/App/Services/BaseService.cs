using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nano.App.Controllers.Contracts;
using Nano.App.Controllers.Contracts.Extensions;
using Nano.App.Controllers.Contracts.Interfaces;
using Nano.App.Models.Interfaces;
using Nano.App.Services.Interfaces;
using Nano.Data.Interfaces;

namespace Nano.App.Services
{
    /// <inheritdoc />
    public abstract class BaseService<TContext> : IService
        where TContext : IDbContext
    {
        /// <summary>
        /// Data Context of type <typeparamref name="TContext"/>.
        /// </summary>
        protected virtual TContext Context { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The <see cref="IDbContext"/>.</param>
        protected BaseService(TContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.Context = context;
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> Get<TEntity>(object key, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return await this.Context
                .Set<TEntity>()
                .FindAsync(new[] { key }, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetAll<TEntity>(Query query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return await this.Context
                .Set<TEntity>()
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetMany<TEntity, TCriteria>(Query<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
            where TCriteria : class, ICriteria, new()
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return await this.Context
                .Set<TEntity>()
                .Where(query.Criteria)
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetMany<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return await this.Context
                .Set<TEntity>()
                .Where(expression)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task Add<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await this.Context
                .AddAsync(entity, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task AddMany<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await this.Context
                .AddRangeAsync(entities, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task Update<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await this.Context
                .UpdateAsync(entity, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task UpdateMany<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await this.Context
                .UpdateRangeAsync(entities, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task Delete<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await this.Context
                .RemoveAsync(entity, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeleteMany<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await this.Context
                .RemoveRangeAsync(entities, cancellationToken);
          
                await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Dispose (non-virtual).
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool).
        /// Override in derived classes as needed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            this.Context?.Dispose();
        }
    }
}