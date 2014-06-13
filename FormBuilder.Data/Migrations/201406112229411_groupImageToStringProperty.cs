namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class groupImageToStringProperty : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Groups", "GroupImage", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Groups", "GroupImage", c => c.Binary());
        }
    }
}
