using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Siscom.Agua.Api.Data;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Security;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using System.Text;

namespace Siscom.Agua.Api
{
    public class Startup
    {
      
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{ env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            var environment = Configuration["ApplicationSettings:Environment"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var settingsIdentity= Configuration.GetSection("AppIdentitySettings");
            var settingsJWT= Configuration.GetSection("ApplicationSettings");
            var settingsConnection = Configuration.GetSection("ConnectionStrings");
            var settings = settingsIdentity.Get<AppIdentitySettings>();
            var settingsApp = settingsJWT.Get<AppSettings>();
            var settingsCon = settingsConnection.Get<ConnectionString>();

            string assemblyNamespace = typeof(ApplicationDbContext).Namespace;


            // Inject AppSettings so that others can use too
            services.Configure<AppSettings>(Configuration.GetSection("ApplicationSettings"));
            services.Configure<ConnectionString>(settingsConnection);

            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS",
                    policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowCredentials().Build());
            });



            // ===== Add our DbContext ========
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                //options.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
                options.UseSqlServer(Configuration.GetConnectionString("SiscmomConnection"), optionsBuilder =>
               
                optionsBuilder.MigrationsAssembly(assemblyNamespace))
                    .EnableSensitiveDataLogging(true)
                    .UseLoggerFactory(new LoggerFactory()
                        .AddConsole((category, level) =>
                            level == LogLevel.Information && category == DbLoggerCategory.Database.Command.Name, true
                        )
                     );
             });

            // ===== Add Identity ========
            //===== Configure Identity Options ================
            // Inject AppIdentitySettings so that others can use too
            services.Configure<AppIdentitySettings>(settingsIdentity);

            services.AddIdentity<ApplicationUser, ApplicationRol>( cfg =>
                    {
                        //User Settings
                        cfg.User.RequireUniqueEmail = settings.User.RequireUniqueEmail;

                        //Password settings
                        cfg.Password.RequireDigit = settings.Password.RequireDigit;
                        cfg.Password.RequiredLength = settings.Password.RequiredLength;
                        cfg.Password.RequireNonAlphanumeric = settings.Password.RequireNonAlphanumeric;
                        cfg.Password.RequireUppercase = settings.Password.RequireUppercase;
                        cfg.Password.RequireLowercase = settings.Password.RequireLowercase;

                        //Lockout settings
                        cfg.Lockout.AllowedForNewUsers = settings.Lockout.AllowedForNewUsers;
                        cfg.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(settings.Lockout.DefaultLockoutTimeSpanInMins);
                        cfg.Lockout.MaxFailedAccessAttempts = settings.Lockout.MaxFailedAccessAttempts;
                    }
                ).AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultTokenProviders();

            // ===== Add Jwt Authentication ========
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = settingsApp.ValidAudience,
                    ValidIssuer = settingsApp.ValidIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settingsApp.IssuerSigningKey))
                };
            });

            // ===== Add MVC ========
            services.AddMvc()
                .AddJsonOptions(option =>
                    option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            ).SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            //======== Swagger Services ========
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Siscom API",
                    Version = "v1",
                    Description = "Sistema de Servicios Comerciales [App-SOSAPAC]"
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // ===== Add Authorization based in roles ========
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            SeedDatabase.Initialize(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);
            app.UseCors("EnableCORS");
            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Siscom API V1");
            });
            app.UseMvc();

        }
    }
}
