using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GrayDuck.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Rollbar;
using Rollbar.NetCore.AspNet;
using Rollbar.Telemetry;
using Microsoft.AspNetCore.Http;

namespace GrayDuck
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
                

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Add Response Compression
            services.AddResponseCompression();

            //GZip Compression
            services.Configure<GzipCompressionProviderOptions>(options => {
                options.Level = CompressionLevel.Optimal;
            });


            //Setup Database Services and Models
            services.Configure<databaseSettings>(
                Configuration.GetSection(nameof(databaseSettings)));

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            //Add as Singleton to Access from anywhere
            services.AddSingleton(Configuration);

            //Configure Cookie Sessio Auth
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            services.AddControllers();

            //Configure Rollbar
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddRollbarLogger(loggerOptions =>
            {
                loggerOptions.Filter = (loggerName, loglevel) => loglevel >= LogLevel.Error;
            });
            ConfigureRollbarSingleton();


            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "GrayDuck API",
                    Description = "Account, Contact, Contact Rating API using .NET Core",
                    TermsOfService = new Uri("https://thedevfactory.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "GrayDuck Free Software",
                        Email = string.Empty,
                        Url = new Uri("https://twitter.com/grayduck"),
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GrayDuck API");
                c.RoutePrefix = "api";
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //Allows us to use wwwroot content
            app.UseDefaultFiles();
            app.UseStaticFiles();

            //Rollbar Middleware
            app.UseRollbarMiddleware();
        }

        /// <summary>
        /// Configures the Rollbar singleton-like notifier.
        /// </summary>
        private void ConfigureRollbarSingleton()
        {
            const string rollbarAccessToken = "b64b52437e174943aa268c15d6c1be4a";
            const string rollbarEnvironment = "production_grayduck";

            RollbarConfig rollbarConfig = new RollbarConfig(rollbarAccessToken) { Environment = rollbarEnvironment };
            //rollbarConfig.ScrubFields = new string[] // data fields which values we want to mask as "***"
            //{
            //    //"url",
            //    //"method",
            //};

            //Important for GitHUb Integration
            rollbarConfig.Server = new Rollbar.DTOs.Server { Host = "localhost", Root = "C:\\Users\\nkrug\\OneDrive\\Desktop\\NETCoreGrayDuck\\", Branch = "master" };

            RollbarLocator.RollbarInstance
            // minimally required Rollbar configuration:
            // if you remove line below the logger's configuration will be auto-loaded from appsettings.json
            //.Configure(new RollbarConfig(rollbarAccessToken) { Environment = rollbarEnvironment })
            .Configure(rollbarConfig)
            // optional step if you would like to monitor Rollbar internal events within your application:
            //.InternalEvent += OnRollbarInternalEvent
            ;
        }
    }
}
