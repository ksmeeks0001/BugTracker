namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrganizationOnPending : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PendingRegistrations", "OrganizationId", c => c.Int(nullable: false));
            CreateIndex("dbo.PendingRegistrations", "OrganizationId");
            AddForeignKey("dbo.PendingRegistrations", "OrganizationId", "dbo.Organizations", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PendingRegistrations", "OrganizationId", "dbo.Organizations");
            DropIndex("dbo.PendingRegistrations", new[] { "OrganizationId" });
            DropColumn("dbo.PendingRegistrations", "OrganizationId");
        }
    }
}
