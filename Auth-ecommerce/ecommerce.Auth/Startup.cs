using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using ecommerce.Auth.DataASPCoreIdentity;
using ecommerce.Data;
using IdentityServer4;
using IdentityServer4.Services;
using Libraries.ecommerce.GoogleReCaptcha.Models;
using Libraries.ecommerce.GoogleReCaptcha.Services;
using Libraries.ecommerce.RazorHtmlEmails.Services;
using Libraries.ecommerce.Services.Models;
using Libraries.ecommerce.Services.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ecommerce.Auth
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            var webSettings = Environment.IsDevelopment() ? Configuration.GetSection("websiteSettingLocal") : Configuration.GetSection("websiteSettingLive");
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            
            // migration assembly required as DbContext's are in a different assembly
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

           
            var connectionStr = Environment.IsDevelopment() ? Configuration.GetConnectionString("ConnectionStrLocal") : Configuration.GetConnectionString("ConnectionStrLive");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionStr);
            });
            services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = false;
                opt.Password.RequireUppercase = false;
                opt.User.RequireUniqueEmail = true;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
                opt.Lockout.MaxFailedAccessAttempts = 3;
            })
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(o =>
                     o.TokenLifespan = TimeSpan.FromMinutes(120));

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookies";
                config.LoginPath = "/Account/login";
                config.LogoutPath = "/Account/logout";
                config.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                config.SlidingExpiration = true;
                config.AccessDeniedPath = new PathString("/Account/AccessDenied");
            });

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.EmitStaticAudienceClaim = true;
                    options.Csp = new IdentityServer4.Configuration.CspOptions() { AddDeprecatedHeader = true };
                });

            builder.AddAspNetIdentity<ApplicationUser>();
            builder.AddProfileService<ProfileService>();

            builder.AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(connectionStr,
                    sql => sql.MigrationsAssembly(migrationsAssembly));
            });

            builder.AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(connectionStr,
                    sql => sql.MigrationsAssembly(migrationsAssembly));
                options.EnableTokenCleanup = true;
            });

            if (!Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                //Build Cert for live env. 
                X509Certificate2 cert = null;
                using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    certStore.Open(OpenFlags.ReadOnly);
                    foreach (X509Certificate2 certificate in certStore.Certificates)
                    {
                        if (!string.IsNullOrWhiteSpace(certificate?.SubjectName?.Name) &&
                            certificate.SubjectName.Name.Contains("CN=*.Example.com"))
                        { 
                            cert = certificate;
                            break; 
                        }
                    }
                }
                if (cert != null)
                {
                    builder.AddSigningCredential(cert);
                }
            }

            //http://docs.identityserver.io/en/latest/topics/add_apis.html
            services.AddLocalApiAuthentication();

            services.AddAuthentication()
            .AddGoogle("Google", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.ClientId = Environment.IsDevelopment() ?  Configuration["3rdPartyKeysLocal:GoogleClientId"] : Configuration["3rdPartyKeysLive:GoogleClientId"];
                options.ClientSecret = Environment.IsDevelopment() ? Configuration["3rdPartyKeysLocal:GoogleClientSecret"] : Configuration["3rdPartyKeysLive:GoogleClientId"];
            });

           

            services.AddTransient<IProfileService, ProfileService>();
            services.Configure<WebsiteSetting>(webSettings);
            services.AddHttpClient<Libraries.ecommerce.Services.Services.Service>();
            services.AddTransient<EmailSenderService>();
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.Configure<GoogleReCaptchaSettings>(Configuration.GetSection("GoogleReCaptcha"));
            services.AddTransient<IGoogleReCaptchaService, GoogleReCaptchaService>();
        }

     
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            DatabaseInitializer.Init(app);
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/robots.txt"))
                {
                    var robotsTxtPath = Path.Combine(env.ContentRootPath, "robots.txt");
                    string output = "User-agent: *  \nDisallow: /";
                    if (File.Exists(robotsTxtPath))
                    {
                        output = await File.ReadAllTextAsync(robotsTxtPath);
                    }
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(output);
                }
                else await next();
            });
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
   
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

    }
}
