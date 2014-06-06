namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedProfileImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "ProfileImage", c => c.Binary());
            AddColumn("dbo.Users", "ContentType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "ContentType");
            DropColumn("dbo.Users", "ProfileImage");
        }
    }
}
