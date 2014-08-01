namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAdurl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "AdUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "AdUrl");
        }
    }
}
