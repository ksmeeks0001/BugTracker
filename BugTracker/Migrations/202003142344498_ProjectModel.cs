namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ManagerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ManagerId)
                .Index(t => t.ManagerId);
            
            CreateTable(
                "dbo.ProjectDevelopers",
                c => new
                    {
                        ProjectId = c.Int(nullable: false),
                        DeveloperId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ProjectId, t.DeveloperId })
                .ForeignKey("dbo.AspNetUsers", t => t.DeveloperId, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId)
                .Index(t => t.DeveloperId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "ManagerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProjectDevelopers", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ProjectDevelopers", "DeveloperId", "dbo.AspNetUsers");
            DropIndex("dbo.ProjectDevelopers", new[] { "DeveloperId" });
            DropIndex("dbo.ProjectDevelopers", new[] { "ProjectId" });
            DropIndex("dbo.Projects", new[] { "ManagerId" });
            DropTable("dbo.ProjectDevelopers");
            DropTable("dbo.Projects");
        }
    }
}
