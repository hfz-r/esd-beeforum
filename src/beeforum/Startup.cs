using System.Reflection;
using AutoMapper;
using beeforum.Features.Profiles;
using beeforum.Infrastructure;
using beeforum.Infrastructure.Data;
using beeforum.Infrastructure.Behaviors;
using beeforum.Infrastructure.Errors;
using beeforum.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VueCliMiddleware;

namespace beeforum
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DbContextTransactionBehavior<,>));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("SqliteConnString")));

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddSwaggerGen();

            services.AddCors();
            services.AddMvcPipeline();
            services.AddAutoMapper(GetType().Assembly);

            // In production, js-files will be served from this directory
            services.AddSpaStaticFiles(options => options.RootPath = "ClientApp/dist");

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
            services.AddScoped<IProfileReader, ProfileReader>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthenticationPipeline();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });
            app.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.json", "BeeForum API V1"); });

            app.UseAuthentication();
            app.UseMvc();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    // run npm process with client app
                    spa.UseVueCli(npmScript: "serve", port: 8090);
                    // if prefer to proxy requests from client app, use proxy to SPA dev server instead:
                    // app should be already running before starting a .NET client
                    // spa.UseProxyToSpaDevelopmentServer("http://localhost:8080"); // Vue app port
                }
            });

            app.ApplicationServices.GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();
        }
    }
}