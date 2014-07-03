namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class makeDecisionDateNullAble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.JoinGroupRequests", "DecisionDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.JoinGroupRequests", "DecisionDate", c => c.DateTime(nullable: false));
        }
    }
}
