namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEventCreatedById : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "CreatedByUserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "CreatedByUserId");
        }
    }
}
