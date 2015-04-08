using AspNetIdentity2WebApiCustomize.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Data.Entity;
using System.Web;

namespace AspNetIdentity2WebApiCustomize
{
    public class ApplicationUserManager 
        : UserManager<ApplicationUser, string>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, string> store)
        public ApplicationUserManager(ApplicationUserStore store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(
            IdentityFactoryOptions<ApplicationUserManager> options, 
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(
                new UserStore<ApplicationUser, ApplicationRole, string, 
                    ApplicationUserLogin, ApplicationUserRole, 
                    ApplicationUserClaim>(context.Get<ApplicationDbContext>()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(
                        dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }


    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(
            IdentityFactoryOptions<ApplicationRoleManager> options, 
            IOwinContext context)
        {
            return new ApplicationRoleManager(
                new ApplicationRoleStore(context.Get<ApplicationDbContext>()));
        }
    }


    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer 
        : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            var userManager = HttpContext.Current
                .GetOwinContext().GetUserManager<ApplicationUserManager>();

            var roleManager = HttpContext.Current
                .GetOwinContext().Get<ApplicationRoleManager>();

            // Initial Admin user:
            const string name = "admin@example.com";
            const string password = "Admin@123456";

            // Some initial values for custom properties:
            const string address = "1234 Sesame Street";
            const string city = "Portland";
            const string state = "OR";
            const string postalCode = "97209";

            const string roleName = "Admin";
            const string roleDescription = "All access pass";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new ApplicationRole(roleName);

                // Set the new custom property:
                role.Description = roleDescription;
                var roleresult = roleManager.Create(role);
            }

            // Create Admin User:
            var user = userManager.FindByName(name);
            if (user == null)
            {
                user = new ApplicationUser { UserName = name, Email = name };

                // Set the new custom properties:
                user.Address = address;
                user.City = city;
                user.State = state;
                user.PostalCode = postalCode;

                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                userManager.AddToRole(user.Id, role.Name);
            }

            // Initial Vanilla User:
            const string vanillaUserName = "vanillaUser@example.com";
            const string vanillaUserPassword = "Vanilla@123456";

            // Add a plain vannilla Users Role:
            const string usersRoleName = "Users";
            const string usersRoleDescription = "Plain vanilla User";

            //Create Role Users if it does not exist
            var usersRole = roleManager.FindByName(usersRoleName);
            if (usersRole == null)
            {
                usersRole = new ApplicationRole(usersRoleName);

                // Set the new custom property:
                usersRole.Description = usersRoleDescription;
                var userRoleresult = roleManager.Create(usersRole);
            }

            // Create Vanilla User:
            var vanillaUser = userManager.FindByName(vanillaUserName);
            if (vanillaUser == null)
            {
                vanillaUser = new ApplicationUser { UserName = vanillaUserName, Email = vanillaUserName };

                // Set the new custom properties:
                vanillaUser.Address = address;
                vanillaUser.City = city;
                vanillaUser.State = state;
                vanillaUser.PostalCode = postalCode;

                var result = userManager.Create(vanillaUser, vanillaUserPassword);
                result = userManager.SetLockoutEnabled(vanillaUser.Id, false);
            }

            // Add vanilla user to Role Users if not already added
            var rolesForVanillaUser = userManager.GetRoles(vanillaUser.Id);
            if (!rolesForVanillaUser.Contains(usersRole.Name))
            {
                userManager.AddToRole(vanillaUser.Id, usersRole.Name);
            }
        }
    }

}
