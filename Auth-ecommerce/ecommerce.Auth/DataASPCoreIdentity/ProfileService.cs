using ecommerce.Data;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ecommerce.Auth.DataASPCoreIdentity
{
    public class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProfileService(UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
            _roleManager = roleManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();

            var user = await _userManager.FindByIdAsync(sub);
            var principal = await _claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();
            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
            claims.Add(new Claim(JwtClaimTypes.GivenName, user.UserName));
            claims.Add(new Claim(JwtClaimTypes.NickName, user.name));

            context.IssuedClaims.AddRange(context.Subject.Claims);

            //var user = await _userManager.GetUserAsync(context.Subject);

            //var roles = await _userManager.GetRolesAsync(user);

            //foreach (var role in roles)
            //{
            //    context.IssuedClaims.Add(new Claim(JwtClaimTypes.Role, role));
            //}

           // Get the current role for the user.
            var xx = await _userManager.GetRolesAsync(user);
            var role = xx.FirstOrDefault();
            claims.Add(new Claim(JwtClaimTypes.Role, role));
            claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
