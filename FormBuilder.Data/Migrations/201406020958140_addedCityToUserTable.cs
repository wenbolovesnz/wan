namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedCityToUserTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "City", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "City");
        }
    }
}
