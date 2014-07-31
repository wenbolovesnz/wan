using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBuilder.Business.Entities;
using FormBuilder.Business.EntityConfigurations;

namespace FormBuilder.Data
{  
    public class FormBuilderContext : DbContext
    {

        public FormBuilderContext()
            : base(ConnectionStringName)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroupRole> UserGroupRoles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventMessage> EventMessages { get; set; }
        public DbSet<JoinGroupRequest> JoinGroupRequests { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }
        public DbSet<GroupPhoto> GroupPhotos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new MembershipConfig());
            modelBuilder.Configurations.Add(new OAuthMemebershipConfig());
            modelBuilder.Configurations.Add(new RoleConfig());
            modelBuilder.Configurations.Add(new UserConfig());
            modelBuilder.Configurations.Add(new GroupConfig());
        }

        public static string ConnectionStringName
        {
            get
            {
                if (ConfigurationManager.AppSettings["ConnectionStringName"] != null)
                {
                    return ConfigurationManager.AppSettings["ConnectionStringName"];
                }

                return "FormBuilderDev";
            }
        }
    }
}
