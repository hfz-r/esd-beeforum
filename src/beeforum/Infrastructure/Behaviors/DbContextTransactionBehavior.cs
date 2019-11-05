using System;
using System.Threading;
using System.Threading.Tasks;
using beeforum.Infrastructure.Data;
using MediatR;

namespace beeforum.Infrastructure.Behaviors
{
    /// <summary>
    /// Add transaction to the processing pipeline
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class DbContextTransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ApplicationDbContext _dbContext;

        public DbContextTransactionBehavior(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            TResponse result;

            try
            {
                _dbContext.BeginTransaction();

                result = await next();

                _dbContext.CommitTransaction();
            }
            catch (Exception)
            {
                _dbContext.RollbackTransaction();
                throw;
            }

            return result;
        }
    }
}