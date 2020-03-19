namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "Details", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "Details");
        }
    }
}
