using Libraries.ecommerce.Services.Models;
using Libraries.ecommerce.Services.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ecommerce.AzUtilityFunctions.Startup))]
namespace ecommerce.AzUtilityFunctions
{

    public class Startup : FunctionsStartup
    { 
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddOptions<WebsiteSetting>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("WebsiteSetting").Bind(settings);
                }); 
            builder.Services.AddHttpClient<Service>();

            builder.Services.AddScoped(typeof(IService<>), typeof(Libraries.ecommerce.Services.Repositories.Service<>));
           
        }
    }
}
