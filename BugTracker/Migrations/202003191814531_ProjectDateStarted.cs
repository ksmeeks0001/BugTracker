namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectDateStarted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "DateStarted", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "DateStarted");
        }
    }
}
