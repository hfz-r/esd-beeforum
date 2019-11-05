using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using beeforum.Infrastructure.Mvc;
using beeforum.Infrastructure.Security;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Swashbuckle.AspNetCore.Swagger;

namespace beeforum.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }}
                });
                options.SwaggerDoc("v1", new Info {Title = "BeeForum API", Version = "v1"});
                options.CustomSchemaIds(y => y.FullName);
                options.DocInclusionPredicate((version, apiDescription) => true);
                options.TagActionsBy(y => new List<string>()
                {
                    y.GroupName
                });
            });
        }

        public static void AddMvcPipeline(this IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.Conventions.Add(new GroupByApiRootConvention());
                    options.Filters.Add(typeof(ValidatorActionFilter));
                }).AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                })
                .AddFluentValidation(config => { config.RegisterValidatorsFromAssemblyContaining<Startup>(); });
        }

        public static void AddAuthenticationPipeline(this IServiceCollection services)
        {
            services.AddOptions();

            const string issuer = "issuer";
            const string audience = "audience";

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("somethinglongerforthisdumbalgorithmisrequired"));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = issuer;
                options.Audience = audience;
                options.SigningCredentials = signingCredentials;
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingCredentials.Key,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = (context) =>
                        {
                            var token = context.HttpContext.Request.Headers["Authorization"];
                            if (token.Count > 0 && token[0].StartsWith("Token ", StringComparison.OrdinalIgnoreCase))
                            {
                                context.Token = token[0].Substring("Token ".Length).Trim();
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
        }

        public static void AddSerilog(this ILoggerFactory loggerFactory)
        {
            //attach the sink to the logger configuration
            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                //just for local debug
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {SourceContext} {Message}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            loggerFactory.AddSerilog(log);
            Log.Logger = log;
        }
    }
}