using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Auth.DataASPCoreIdentity;
using ecommerce.Data;
using ecommerce.Data.MVData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static IdentityServer4.IdentityServerConstants;

namespace ecommerce.Auth.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(LocalApi.PolicyName)]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext context;
        public UserController(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager, ApplicationDbContext _context)
        {
            userManager = _userManager;
            roleManager = _roleManager;
            context = _context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = from ur in context.UserRoles
                        join u in context.Users on ur.UserId equals u.Id
                        join r in context.Roles on ur.RoleId equals r.Id
                        select new User
                        {
                            userId = u.Id,
                            name = u.name,
                            role = r.Name,
                            roleId = r.Id,
                            email = u.Email,
                            isBlocked = u.isBlocked
                        };
            return Ok(users);
        } 

        [HttpGet("{id}"), AllowAnonymous]
        public async Task<IActionResult> Get(string Id)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            try
            {
                 applicationUser = await context.Users.FindAsync(Id);
                //Get the user role
                var userRoleModel = context.UserRoles.FirstOrDefault(x => x.UserId.Equals(applicationUser.Id));
                var roleModel = context.Roles.FirstOrDefault(x => x.Id.Equals(userRoleModel.RoleId));
                if (roleModel.Name.Equals("Vendor"))
                {
                    applicationUser.isVendor = true;
                }
                else if (roleModel.Name.Equals("Admin"))
                {
                    applicationUser.isAdmin = true;
                }

                applicationUser.isBlocked = applicationUser.isBlocked;

                return Ok(applicationUser);
            }
            catch (Exception)
            {
                return Ok(applicationUser);
            }           
        }

        [HttpPost]
        public async Task<IActionResult> Post(ApplicationUser applicationUser)
        {
            POJO model = new POJO();

            if (applicationUser == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            if (!String.IsNullOrEmpty(applicationUser.Id))
            {
                ApplicationUser _Entity = await context.Users.FindAsync(applicationUser.Id);
                _Entity.name = String.IsNullOrEmpty(applicationUser.name) ? _Entity.name : applicationUser.name;
                _Entity.Email = String.IsNullOrEmpty(applicationUser.Email) ? _Entity.Email : applicationUser.Email;
                _Entity.NormalizedEmail = String.IsNullOrEmpty(applicationUser.Email) ? _Entity.Email : applicationUser.Email.ToUpper();
                _Entity.PhoneNumber = String.IsNullOrEmpty(applicationUser.PhoneNumber) ? _Entity.PhoneNumber : applicationUser.PhoneNumber;
                _Entity.dOB = applicationUser.dOB.Equals("0001-01-01T00:00:00") ? _Entity.dOB : applicationUser.dOB;
                _Entity.gender = applicationUser.gender == 0 ? _Entity.gender : applicationUser.gender;
                _Entity.isBlocked = applicationUser.isBlocked;
                _Entity.updateDate = DateTime.Now;
                _Entity.modifiedBy = String.IsNullOrEmpty(applicationUser.modifiedBy) ? _Entity.modifiedBy : applicationUser.modifiedBy;

                try
                {
                    await UpdateUserRole(applicationUser);
                    await context.SaveChangesAsync();
                    model.id = _Entity.Id;
                    model.flag = true;
                    model.message = "Has Been Updated.";

                }
                catch (Exception ex)
                {
                    model.flag = false;
                    model.message = ex.ToString();
                }
            }
            return Ok(model);
        }

        private async Task UpdateUserRole(ApplicationUser applicationUser)
        {
            //Update the Role or the user.
            //Get the current role
            var userRoleModel = context.UserRoles.FirstOrDefault(x => x.UserId.Equals(applicationUser.Id));
            IdentityRole roleModel = new IdentityRole();

            if (applicationUser.isVendor)
            {
                //Get vendor Id
                roleModel = context.Roles.FirstOrDefault(x => x.Name.Equals("Vendor"));
            }
            else if (applicationUser.isAdmin)
            {
                roleModel = context.Roles.FirstOrDefault(x => x.Name.Equals("Admin"));
            }
            else
            {
                roleModel = context.Roles.FirstOrDefault(x => x.Name.Equals("Customer"));
            }
            //If the new role = old role don't update
            if (!userRoleModel.RoleId.Equals(roleModel.Id))
            {
                //Remove old role
                context.UserRoles.Remove(userRoleModel);
                //Add new role
                IdentityUserRole<string> updatedRole = new IdentityUserRole<string>
                {
                    UserId = applicationUser.Id,
                    RoleId = roleModel.Id
                };
                await context.UserRoles.AddAsync(updatedRole);
                await context.SaveChangesAsync();
            } 
        }

        [HttpPost("BlockUser")]
        public async Task<IActionResult> BlockUser(ApplicationUser applicationUser)
        {
            POJO model = new POJO();

            if (applicationUser == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            if (!String.IsNullOrEmpty(applicationUser.Id))
            {
                ApplicationUser _Entity = await context.Users.FindAsync(applicationUser.Id);

                _Entity.isBlocked = _Entity.isBlocked.Equals(false) ? true : false;
                _Entity.updateDate = DateTime.Now;
                _Entity.modifiedBy = String.IsNullOrEmpty(applicationUser.modifiedBy) ? _Entity.modifiedBy : applicationUser.modifiedBy;
                try
                {
                    await context.SaveChangesAsync();
                    model.id = _Entity.Id;
                    model.flag = true;
                    var status = _Entity.isBlocked ? "Blocked" : "Unblocked";
                    model.message = $"{_Entity.name} has been {status} successfully.";
                }
                catch (Exception ex)
                {
                    model.flag = false;
                    model.message = ex.ToString();
                }
            }
            return Ok(model);
        }



        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            ApplicationUser user = await context.Users.FindAsync(model.UserId);
            // var user = await userManager.GetUserAsync(User);

            POJO pojoModel = new POJO();

            if (user == null)
            {
                pojoModel.flag = false;
                pojoModel.message = "Empty request.";
                return Ok(pojoModel);
            }
            // ChangePasswordAsync changes the user password
            var result = await userManager.ChangePasswordAsync(user,
                model.CurrentPassword, model.NewPassword);

            // The new password did not meet the complexity rules or
            // the current password is incorrect. Add these errors to
            // the ModelState and rerender ChangePassword view
            if (!result.Succeeded)
            {
                pojoModel.flag = false;
                pojoModel.message = "Empty request.";

                foreach (var error in result.Errors)
                {
                    pojoModel.message = pojoModel.message + error.Description;
                }
                return Ok(pojoModel);
            }
            pojoModel.flag = true;
            pojoModel.message = "Password Has been changed";
            return Ok(pojoModel);
        }

        [HttpPost("SaveUserDetails")]
        public async Task<IActionResult> SaveUserDetails(ApplicationUser applicationUser)
        {
            POJO model = new POJO();

            if (applicationUser == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            if (!String.IsNullOrEmpty(applicationUser.Id))
            {
                ApplicationUser _Entity = await context.Users.FindAsync(applicationUser.Id);
                _Entity.name = String.IsNullOrEmpty(applicationUser.name) ? _Entity.name : applicationUser.name;
                _Entity.Email = String.IsNullOrEmpty(applicationUser.Email) ? _Entity.Email : applicationUser.Email;
                _Entity.NormalizedEmail = String.IsNullOrEmpty(applicationUser.Email) ? _Entity.Email : applicationUser.Email.ToUpper();
                _Entity.PhoneNumber = String.IsNullOrEmpty(applicationUser.PhoneNumber) ? _Entity.PhoneNumber : applicationUser.PhoneNumber;
                _Entity.dOB = applicationUser.dOB.Equals("0001-01-01T00:00:00") ? _Entity.dOB : applicationUser.dOB;
                _Entity.gender = applicationUser.gender == 0 ? _Entity.gender : applicationUser.gender;
                _Entity.updateDate = DateTime.Now;
                _Entity.modifiedBy = String.IsNullOrEmpty(applicationUser.modifiedBy) ? _Entity.modifiedBy : applicationUser.modifiedBy;

                try
                {
                    await context.SaveChangesAsync();
                    model.id = _Entity.Id;
                    model.flag = true;
                    model.message = "Has Been Updated.";

                }
                catch (Exception ex)
                {
                    model.flag = false;
                    model.message = ex.ToString();
                }
            }
            return Ok(model);
        }

    }
}