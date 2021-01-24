namespace TMDT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class new_db : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "BookID", "dbo.Books");
            DropIndex("dbo.Comments", new[] { "BookID" });
            CreateTable(
                "dbo.Prices",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        price = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Books", "Comment_CommentId", c => c.Int());
            CreateIndex("dbo.Books", "Comment_CommentId");
            AddForeignKey("dbo.Books", "Comment_CommentId", "dbo.Comments", "CommentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Books", "Comment_CommentId", "dbo.Comments");
            DropIndex("dbo.Books", new[] { "Comment_CommentId" });
            DropColumn("dbo.Books", "Comment_CommentId");
            DropTable("dbo.Prices");
            CreateIndex("dbo.Comments", "BookID");
            AddForeignKey("dbo.Comments", "BookID", "dbo.Books", "BookID", cascadeDelete: true);
        }
    }
}
