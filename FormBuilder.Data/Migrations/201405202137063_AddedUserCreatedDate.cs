namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserCreatedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "CreatedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "CreatedDate");
        }
    }
}
