using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System;

namespace AspNetIdentity2WebApiCustomize.Models
{
    //// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    //public class ApplicationUser : IdentityUser
    //{
    //    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
    //    {
    //        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
    //        var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
    //        // Add custom user claims here
    //        return userIdentity;
    //    }
    //}

    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    public ApplicationDbContext()
    //        : base("DefaultConnection", throwIfV1Schema: false)
    //    {
    //    }
        
    //    public static ApplicationDbContext Create()
    //    {
    //        return new ApplicationDbContext();
    //    }
    //}

  // You will not likely need to customize there, but it is necessary/easier to create our own 
  // project-specific implementations, so here they are:
  public class ApplicationUserLogin : IdentityUserLogin<string> { }
  public class ApplicationUserClaim : IdentityUserClaim<string> { }
  public class ApplicationUserRole : IdentityUserRole<string> { }

  // Must be expressed in terms of our custom Role and other types:
  public class ApplicationUser
      : IdentityUser<string, ApplicationUserLogin,
      ApplicationUserRole, ApplicationUserClaim> {
    public ApplicationUser() {
      this.Id = Guid.NewGuid().ToString();

      // Add any custom User properties/code here
    }


    public async Task<ClaimsIdentity>
        GenerateUserIdentityAsync(ApplicationUserManager manager) {
      var userIdentity = await manager
          .CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
      return userIdentity;
    }
  }


  // Must be expressed in terms of our custom UserRole:
  public class ApplicationRole : IdentityRole<string, ApplicationUserRole> {
    public ApplicationRole() {
      this.Id = Guid.NewGuid().ToString();
    }

    public ApplicationRole(string name)
      : this() {
      this.Name = name;
    }

    // Add any custom Role properties/code here
  }


  // Must be expressed in terms of our custom types:
  public class ApplicationDbContext
      : IdentityDbContext<ApplicationUser, ApplicationRole,
      string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim> {
    public ApplicationDbContext()
      : base("DefaultConnection") {
    }

    static ApplicationDbContext() {
      Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
    }

    public static ApplicationDbContext Create() {
      return new ApplicationDbContext();
    }

    // Add additional items here as needed
  }

  // Most likely won't need to customize these either, but they were needed because we implemented
  // custom versions of all the other types:
  public class ApplicationUserStore
      : UserStore<ApplicationUser, ApplicationRole, string,
          ApplicationUserLogin, ApplicationUserRole,
          ApplicationUserClaim>, IUserStore<ApplicationUser, string>,
      IDisposable {
    public ApplicationUserStore()
      : this(new IdentityDbContext()) {
      base.DisposeContext = true;
    }

    public ApplicationUserStore(DbContext context)
      : base(context) {
    }
  }


  public class ApplicationRoleStore
  : RoleStore<ApplicationRole, string, ApplicationUserRole>,
  IQueryableRoleStore<ApplicationRole, string>,
  IRoleStore<ApplicationRole, string>, IDisposable {
    public ApplicationRoleStore()
      : base(new IdentityDbContext()) {
      base.DisposeContext = true;
    }

    public ApplicationRoleStore(DbContext context)
      : base(context) {
    }
  }
}