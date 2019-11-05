using System;
using System.IO;
using System.Threading.Tasks;
using beeforum.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace beeforum.IntegrationTests
{
    public class SliceFixture : IDisposable
    {
        private static readonly IConfiguration Config;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ServiceProvider _provider;
        private readonly string _dbName = Guid.NewGuid() + ".db";

        static SliceFixture()
        {
            Config = new ConfigurationBuilder().Build();
        }

        public SliceFixture()
        {
            var startup = new Startup(Config);
            var services = new ServiceCollection();

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase(_dbName);
            services.AddSingleton(new ApplicationDbContext(optionsBuilder.Options));

            startup.ConfigureServices(services);

            _provider = services.BuildServiceProvider();

            GetDbContext().Database.EnsureCreated();
            _scopeFactory = _provider.GetRequiredService<IServiceScopeFactory>();
        }

        public ApplicationDbContext GetDbContext()
        {
            return _provider.GetRequiredService<ApplicationDbContext>();
        }

        public void Dispose()
        {
            File.Delete(_dbName);
        }

        //get scope
        public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                await action(scope.ServiceProvider);
            }
        }

        //get scope generic version
        public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                return await action(scope.ServiceProvider);
            }
        }

        //mock mediator
        public Task SendAsync(IRequest request)
        {
            return ExecuteScopeAsync(provider =>
            {
                var mediator = provider.GetService<IMediator>();

                return mediator.Send(request);
            });
        }

        //mock mediator with generic response
        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return ExecuteScopeAsync(provider =>
            {
                var mediator = provider.GetService<IMediator>();

                return mediator.Send(request);
            });
        }

        //mock dbcontext
        public Task ExecuteDbContextAsync(Func<ApplicationDbContext, Task> action)
        {
            return ExecuteScopeAsync(provider => action(provider.GetService<ApplicationDbContext>()));
        }

        //mock dbcontext with generic return
        public Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, Task<T>> action)
        {
            return ExecuteScopeAsync(provider => action(provider.GetService<ApplicationDbContext>()));
        }

        //mock dbcontext insert
        public Task InsertAsync(params object[] entities)
        {
            return ExecuteDbContextAsync(db =>
            {
                foreach (var entity in entities)
                {
                    db.Add(entity);
                }
                return db.SaveChangesAsync();
            });
        }
    }
}