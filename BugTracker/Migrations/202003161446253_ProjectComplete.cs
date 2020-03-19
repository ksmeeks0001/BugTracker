namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectComplete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "Complete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "Complete");
        }
    }
}
