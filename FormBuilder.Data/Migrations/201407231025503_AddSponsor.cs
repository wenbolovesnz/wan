namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSponsor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sponsors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PhotoUrl = c.String(),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.SponsorEvents",
                c => new
                    {
                        Sponsor_Id = c.Int(nullable: false),
                        Event_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Sponsor_Id, t.Event_Id })
                .ForeignKey("dbo.Sponsors", t => t.Sponsor_Id, cascadeDelete: false)
                .ForeignKey("dbo.Events", t => t.Event_Id, cascadeDelete: false)
                .Index(t => t.Sponsor_Id)
                .Index(t => t.Event_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sponsors", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.SponsorEvents", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.SponsorEvents", "Sponsor_Id", "dbo.Sponsors");
            DropIndex("dbo.SponsorEvents", new[] { "Event_Id" });
            DropIndex("dbo.SponsorEvents", new[] { "Sponsor_Id" });
            DropIndex("dbo.Sponsors", new[] { "GroupId" });
            DropTable("dbo.SponsorEvents");
            DropTable("dbo.Sponsors");
        }
    }
}
