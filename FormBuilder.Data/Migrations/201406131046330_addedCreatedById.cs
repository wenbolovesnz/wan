namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedCreatedById : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "CreatedById", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "CreatedById");
        }
    }
}
