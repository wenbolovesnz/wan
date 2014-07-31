namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedGroupPhoto : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupPhotoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupPhotoes", "GroupId", "dbo.Groups");
            DropIndex("dbo.GroupPhotoes", new[] { "GroupId" });
            DropTable("dbo.GroupPhotoes");
        }
    }
}
