namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedUserGroupRoleTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserGroupRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.webpages_Roles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.GroupId)
                .Index(t => t.RoleId);
            
            AddColumn("dbo.Groups", "GroupImage", c => c.Binary());
            AddColumn("dbo.Groups", "ContentType", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserGroupRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserGroupRoles", "RoleId", "dbo.webpages_Roles");
            DropForeignKey("dbo.UserGroupRoles", "GroupId", "dbo.Groups");
            DropIndex("dbo.UserGroupRoles", new[] { "RoleId" });
            DropIndex("dbo.UserGroupRoles", new[] { "GroupId" });
            DropIndex("dbo.UserGroupRoles", new[] { "UserId" });
            DropColumn("dbo.Groups", "ContentType");
            DropColumn("dbo.Groups", "GroupImage");
            DropTable("dbo.UserGroupRoles");
        }
    }
}
