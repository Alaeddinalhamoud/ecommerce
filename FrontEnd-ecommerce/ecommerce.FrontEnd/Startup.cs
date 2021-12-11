using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using AutoMapper;
using Libraries.ecommerce.GoogleReCaptcha.Models;
using Libraries.ecommerce.GoogleReCaptcha.Services;
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

namespace ecommerce.FrontEnd
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
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.Name = ".Example.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.IsEssential = true;
            });
            // The following line enables Application Insights telemetry collection.
            services.AddApplicationInsightsTelemetry();
            services.AddAutoMapper(typeof(Startup));
           // string APIURL = Configuration.GetValue<string>("APILiveUrl");
            string authority = Configuration.GetValue<string>("AuthServerLiveUrl");
            var webSettings = Configuration.GetSection("WebsiteSettingLive");
            string secretKey = Configuration.GetValue<string>("AuthServerSecret");
            //Change settings if dev
            if (Environment.IsDevelopment())
            {
                //APIURL = Configuration.GetValue<string>("APILocalUrl");
                authority = Configuration.GetValue<string>("AuthServerLocalUrl");
                webSettings = Configuration.GetSection("WebsiteSettingLocal");
            }
            services.AddHttpClient();
            services.Configure<WebsiteSetting>(webSettings);
            services.AddHttpClient<Service>();
            services.AddTransient<FileUploadService>();
            services.AddTransient<PaymentService>(); 
            services.AddTransient<EmailSenderService>(); 
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddScoped(typeof(IService<>), typeof(Service<>));
            services.Configure<GoogleReCaptchaSettings>(Configuration.GetSection("GoogleReCaptcha"));
            services.AddTransient<IGoogleReCaptchaService, GoogleReCaptchaService> ();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "ExampleCookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("ExampleCookies", opt =>
                {
                    // Configure the client application to use sliding sessions
                    opt.SlidingExpiration = true;
                    // Expire the session of 15 minutes of inactivity
                    opt.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                })
                .AddOpenIdConnect("oidc", options =>
                {

                    options.Authority = authority;
                    options.RequireHttpsMetadata = true;
                    
                    options.ClientId = "mvcecommercefront";
                    options.ClientSecret = secretKey;
                    options.ResponseType = "code";
                   // options.SignedOutCallbackPath = "/";
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied"); 
                    options.SignInScheme = "ExampleCookies";
                    options.SaveTokens = true;
                    options.Scope.Add("ecommerceapi");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("IdentityServerApi");
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

            services.AddRazorPages();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

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
                    string output = "User-agent: *  \nallow: /";
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
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
