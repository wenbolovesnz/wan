namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userImageToStringProperty : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "ProfileImage", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "ProfileImage", c => c.Binary());
        }
    }
}
