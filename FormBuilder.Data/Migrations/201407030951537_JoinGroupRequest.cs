namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JoinGroupRequest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JoinGroupRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Message = c.String(),
                        IsProcessed = c.Boolean(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                        RequestedDate = c.DateTime(nullable: false),
                        DecisionDate = c.DateTime(nullable: false),
                        DecisionUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.DecisionUserId)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.UserId)
                .Index(t => t.DecisionUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JoinGroupRequests", "UserId", "dbo.Users");
            DropForeignKey("dbo.JoinGroupRequests", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.JoinGroupRequests", "DecisionUserId", "dbo.Users");
            DropIndex("dbo.JoinGroupRequests", new[] { "DecisionUserId" });
            DropIndex("dbo.JoinGroupRequests", new[] { "UserId" });
            DropIndex("dbo.JoinGroupRequests", new[] { "GroupId" });
            DropTable("dbo.JoinGroupRequests");
        }
    }
}
