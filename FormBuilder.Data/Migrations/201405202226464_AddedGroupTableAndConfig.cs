namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGroupTableAndConfig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupName = c.String(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserGroupMap",
                c => new
                    {
                        GroupId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupId, t.UserId })
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserGroupMap", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserGroupMap", "GroupId", "dbo.Groups");
            DropIndex("dbo.UserGroupMap", new[] { "UserId" });
            DropIndex("dbo.UserGroupMap", new[] { "GroupId" });
            DropTable("dbo.UserGroupMap");
            DropTable("dbo.Groups");
        }
    }
}
