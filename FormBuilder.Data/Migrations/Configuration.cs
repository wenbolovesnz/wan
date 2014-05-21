using FormBuilder.Business.Entities;

namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FormBuilder.Data.FormBuilderContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "FormBuilder.Data.FormBuilderContext";
        }

        protected override void Seed(FormBuilder.Data.FormBuilderContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //

            var user = context.Users.First();

            var group = new Group();
            group.GroupName = "Awesome one";
            group.CreatedDate = DateTime.Now;
            group.Description = "This is a good one";
            group.Users.Add(user);

            
            context.Groups.AddOrUpdate(
              p => p.GroupName, group             
            );
            
        }
    }
}
