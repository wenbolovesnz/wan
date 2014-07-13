namespace FormBuilder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDeclineReasonToJoinGroupRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JoinGroupRequests", "DeclineReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.JoinGroupRequests", "DeclineReason");
        }
    }
}
