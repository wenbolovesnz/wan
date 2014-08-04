namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedPersonalMessage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonalMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedByUserId = c.Int(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonalMessages", "UserId", "dbo.Users");
            DropIndex("dbo.PersonalMessages", new[] { "UserId" });
            DropTable("dbo.PersonalMessages");
        }
    }
}
