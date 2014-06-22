namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedEventClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        GroupId = c.Int(nullable: false),
                        EventDateTime = c.DateTime(nullable: false),
                        EventLocation = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId);
            
            AddColumn("dbo.Users", "Event_Id", c => c.Int());
            CreateIndex("dbo.Users", "Event_Id");
            AddForeignKey("dbo.Users", "Event_Id", "dbo.Events", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.Events", "GroupId", "dbo.Groups");
            DropIndex("dbo.Users", new[] { "Event_Id" });
            DropIndex("dbo.Events", new[] { "GroupId" });
            DropColumn("dbo.Users", "Event_Id");
            DropTable("dbo.Events");
        }
    }
}
