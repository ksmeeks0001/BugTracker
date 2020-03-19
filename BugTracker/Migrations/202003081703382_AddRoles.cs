namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoles : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO AspNetRoles (Id, Name) VALUES (1, 'Admin')");
            Sql("INSERT INTO AspNetRoles (Id, Name) VALUES (2, 'Manager')");
            Sql("INSERT INTO AspNetRoles (Id, Name) VALUES (3, 'Developer')");

        }
        
        public override void Down()
        {
        }
    }
}
