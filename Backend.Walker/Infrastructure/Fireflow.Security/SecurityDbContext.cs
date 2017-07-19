using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Fireflow.Security
{
    public class SecurityDbContext : IdentityDbContext<ApplicationUser>
    {
        public SecurityDbContext()
            : base("SecurityConnection")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        //static SecurityDbContext()
        //{
        //    Database.SetInitializer(new SecurityDbInitializer());
        //}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public static SecurityDbContext Create()
        {
            return new SecurityDbContext();
        }
    }

    public class SecurityDbInitializer : DropCreateDatabaseAlways<SecurityDbContext>
    {
        protected override void Seed(SecurityDbContext context)
        {
            InitializeIdentity(context);
            base.Seed(context);
        }

        private static void InitializeIdentity(SecurityDbContext context)
        {
            //var cutpix = new ApplicationUser
            //{
            //    UserName = "cutpix",
            //    Email = "cutpix@outlook.com"
            //};

            //var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            //var result = await manager.CreateAsync(cutpix, "ghbdtndctv66");

            // add claims for user #1:
            //await manager.AddClaimAsync(cutpix.Id, new Claim(ClaimTypes.Name, "cutpix"));
            //await manager.AddClaimAsync(cutpix.Id, new Claim(ClaimTypes.Role, "Admin"));
        }
    }
}