namespace AKnightsTale.Migrations
{
    using AKnightsTale.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AKnightsTale.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "AKnightsTale.Models.ApplicationDbContext";
        }

        protected override void Seed(AKnightsTale.Models.ApplicationDbContext context)
        {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (!context.Users.Any(x => x.UserName == "admin@admin.com"))
            {
                var user = new ApplicationUser { UserName = "admin@admin.com", Email = "admin@admin.com" };
                userManager.Create(user, "Password@1");
                context.Roles.AddOrUpdate(x => x.Name, new IdentityRole { Name = "Admin" });
                context.SaveChanges();
                userManager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
