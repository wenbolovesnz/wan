namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEventMessage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        EventId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventMessages", "UserId", "dbo.Users");
            DropForeignKey("dbo.EventMessages", "EventId", "dbo.Events");
            DropIndex("dbo.EventMessages", new[] { "UserId" });
            DropIndex("dbo.EventMessages", new[] { "EventId" });
            DropTable("dbo.EventMessages");
        }
    }
}
