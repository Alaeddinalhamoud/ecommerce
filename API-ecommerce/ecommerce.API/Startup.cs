using System.IdentityModel.Tokens.Jwt;
using System.IO;
using ecommerce.DataAccess;
using ecommerce.Repositories;
using ecommerce.Services;
using Libraries.ecommerce.Services.Models;
using Libraries.ecommerce.Services.Repositories;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace API.ecommerce
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

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("ConnectionLiveStr");
            string authority = Configuration.GetValue<string>("AuthServerLiveUrl");
            var webSettings = Configuration.GetSection("WebsiteSettingLive");
            //APIURL => Auth AS API
            string APIURL = Configuration.GetValue<string>("APILiveUrl");
            //Change connection str if dev
            if (Environment.IsDevelopment())
            {
                connectionString = Configuration.GetConnectionString("ConnectionLocalStr");
                authority = Configuration.GetValue<string>("AuthServerLocalUrl");
                APIURL = Configuration.GetValue<string>("APILocalUrl");
                authority = Configuration.GetValue<string>("AuthServerLocalUrl");
                webSettings = Configuration.GetSection("WebsiteSettingLocal");
            }
            services.AddControllers().AddXmlSerializerFormatters();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "E-Commerce API", Version = "v1" });
            });
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
            services.AddCors(options => options.AddPolicy("Cors", builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = authority;
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            // adds an authorization policy to make sure the token is for scope 'api1'
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "ecommerceapi");
                });
            });

            services.Configure<WebsiteSetting>(webSettings);
            services.AddHttpClient<Service>();
            services.AddTransient<FileUploadService>();
            services.AddTransient<EmailSenderService>();
            services.AddScoped(typeof(IService<>), typeof(Service<>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce API v1"));
            }

            app.UseHttpsRedirection();
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
            app.UseCors("Cors");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization("ApiScope");
            });
        }
    }
}
