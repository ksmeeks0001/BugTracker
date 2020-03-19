namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_User_Data_Issues_Notes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Issues", "CreatedById", c => c.String(maxLength: 128));
            AddColumn("dbo.Issues", "DeveloperAssignedId", c => c.String(maxLength: 128));
            AddColumn("dbo.Notes", "PostedById", c => c.String(maxLength: 128));
            CreateIndex("dbo.Issues", "CreatedById");
            CreateIndex("dbo.Issues", "DeveloperAssignedId");
            CreateIndex("dbo.Notes", "PostedById");
            AddForeignKey("dbo.Issues", "CreatedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Issues", "DeveloperAssignedId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Notes", "PostedById", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notes", "PostedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Issues", "DeveloperAssignedId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Issues", "CreatedById", "dbo.AspNetUsers");
            DropIndex("dbo.Notes", new[] { "PostedById" });
            DropIndex("dbo.Issues", new[] { "DeveloperAssignedId" });
            DropIndex("dbo.Issues", new[] { "CreatedById" });
            DropColumn("dbo.Notes", "PostedById");
            DropColumn("dbo.Issues", "DeveloperAssignedId");
            DropColumn("dbo.Issues", "CreatedById");
        }
    }
}
