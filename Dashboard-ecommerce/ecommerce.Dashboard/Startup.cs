using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using Libraries.ecommerce.RazorHtmlEmails.Services;
using Libraries.ecommerce.Services.Models;
using Libraries.ecommerce.Services.Repositories;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace ecommerce.Dashboard
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
        public void ConfigureServices(IServiceCollection services)
        {
            // The following line enables Application Insights telemetry collection.
            services.AddApplicationInsightsTelemetry();
            //string APIURL = Configuration.GetValue<string>("APIUrl");
            string authority = Configuration.GetValue<string>("AuthServerLiveUrl");
            var webSettings = Configuration.GetSection("websiteSettingLive");
            string secretKey = Configuration.GetValue<string>("AuthServerSecret");

            services.AddRazorPages();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            //Change settings if dev
            if (Environment.IsDevelopment())
            {
               // APIURL = Configuration.GetValue<string>("APIUrl");
                authority = Configuration.GetValue<string>("AuthServerLocalUrl");
                webSettings = Configuration.GetSection("websiteSettingLocal");
            }

            services.Configure<WebsiteSetting>(webSettings);
            services.AddHttpClient<Service>();
            services.AddTransient<FileUploadService>();
            services.AddTransient<PaymentService>();
            services.AddTransient<EmailSenderService>(); 
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddScoped(typeof(IService<>), typeof(Service<>));
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); 
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "DashCookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("DashCookies", opt =>
                {
                    // Configure the client application to use sliding sessions
                    opt.SlidingExpiration = true;
                    // Expire the session of 15 minutes of inactivity
                    opt.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = authority;
                    options.RequireHttpsMetadata = true;
                    options.ClientId = "mvcecommercedashboard";
                    options.ClientSecret = secretKey;
                    options.ResponseType = "code";
                    options.SignInScheme = "DashCookies";
                    options.SaveTokens = true;
                    options.Scope.Add("ecommerceapi");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("IdentityServerApi");
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role",
                    };
                    options.Events.OnTicketReceived = async (context) =>
                    {
                        context.Properties.ExpiresUtc = DateTime.UtcNow.AddHours(1);
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute().RequireAuthorization();
            });
        }
    }
}
