namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adSiteUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "AdSiteUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "AdSiteUrl");
        }
    }
}
