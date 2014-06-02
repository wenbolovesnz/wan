namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMorePropertiesToUserTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "NickName", c => c.String());
            AddColumn("dbo.Users", "AboutMe", c => c.String());
            AddColumn("dbo.Users", "DOB", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "DOB");
            DropColumn("dbo.Users", "AboutMe");
            DropColumn("dbo.Users", "NickName");
        }
    }
}
