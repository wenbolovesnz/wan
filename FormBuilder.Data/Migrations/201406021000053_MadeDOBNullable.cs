namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeDOBNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "DOB", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "DOB", c => c.DateTime(nullable: false));
        }
    }
}
