using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBuilder.Business.Entities;

namespace FormBuilder.Business.EntityConfigurations
{
    public class GroupConfig: EntityTypeConfiguration<Group>
    {
        public GroupConfig()
        {
            this.HasKey(m => m.Id);
            this.Property(m => m.CreatedDate).IsRequired();
            this.Property(m => m.Description).IsRequired();
            this.Property(m => m.GroupName).IsRequired();

            this.HasMany(m => m.Users).WithMany(b => b.Groups).Map(m =>
                {
                    m.MapLeftKey("GroupId");
                    m.MapRightKey("UserId");
                    m.ToTable("UserGroupMap");
                });
        }
    }
}
