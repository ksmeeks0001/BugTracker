namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectsToIssues : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Issues", "ProjectId", c => c.Int(nullable: false));
            CreateIndex("dbo.Issues", "ProjectId");
            AddForeignKey("dbo.Issues", "ProjectId", "dbo.Projects", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Issues", "ProjectId", "dbo.Projects");
            DropIndex("dbo.Issues", new[] { "ProjectId" });
            DropColumn("dbo.Issues", "ProjectId");
        }
    }
}
