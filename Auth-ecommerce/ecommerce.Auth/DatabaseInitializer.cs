using ecommerce.Auth.DataASPCoreIdentity;
using System;
using Serilog;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Auth.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using ecommerce.Data;

namespace ecommerce.Auth
{
    public class DatabaseInitializer
    {
        public static async Task Init(IApplicationBuilder app)
        {
            using (var provider = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                 provider.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
                provider.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                InitializeIdentityServer(provider);
                IdentityResult result = null;
                var userManager = provider.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = provider.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();               
               
                //Seeding the roles table
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    var admin = new IdentityRole("Admin");
                    await roleManager.CreateAsync(admin);
                }

                if (!await roleManager.RoleExistsAsync("vendor"))
                {
                    var vendor = new IdentityRole("Vendor");
                    await roleManager.CreateAsync(vendor);
                }

                if (!await roleManager.RoleExistsAsync("Customer"))
                {
                    var customer = new IdentityRole("Customer");
                    await roleManager.CreateAsync(customer);
                }

                var administratorUser =await userManager.FindByNameAsync("admin@Palmyra-Tech.com");

                if (administratorUser == null)
                {
                    administratorUser = new ApplicationUser
                    {
                        UserName = "admin@domain.com",
                        Email = "admin@domain.com",
                        isAdmin = true,
                        name = "Admin",
                        EmailConfirmed = true
                    };

                    result = await userManager.CreateAsync(administratorUser, "$Admin$User2020$");

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = await userManager.AddToRoleAsync(administratorUser, "Admin");

                    if (result.Succeeded) 
                    { 
                        result = await userManager.AddClaimsAsync(administratorUser, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Admin User"),
                        new Claim(JwtClaimTypes.Role, "Admin")});

                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        Console.WriteLine("administrator User created");
                     }
                }
                else
                {
                    Console.WriteLine("administrator User already exists");
                }
            }
        }

        private static void InitializeIdentityServer(IServiceScope app)
        {
            var context = app.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            context.Database.Migrate();
            if (!context.Clients.Any())
            {
                foreach (var client in Config.GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Clients already populated, update clients");
                foreach (var client in Config.GetClients().ToList())
                {
                    var item = context.Clients.Where(c => c.ClientId == client.ClientId).FirstOrDefault();
                    if (item == null)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    else
                    {
                        var model = client.ToEntity();
                        model.Id = item.Id;
                        context.Entry(item).CurrentValues.SetValues(model);
                    }
                }
                context.SaveChanges();
            } 
        

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.GetIdentityResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Log.Debug("IdentityResources already populated");
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in Config.GetApiScopes())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Log.Debug("ApiScopes already populated");
            }
        }
    }
}
