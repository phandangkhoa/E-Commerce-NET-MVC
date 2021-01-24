namespace TMDT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit_comment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "Name", c => c.String());
            AddColumn("dbo.Comments", "Avatar", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "Avatar");
            DropColumn("dbo.Comments", "Name");
        }
    }
}
