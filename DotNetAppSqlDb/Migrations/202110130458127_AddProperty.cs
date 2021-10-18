namespace DotNetAppSqlDb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Todoes", "Name", c => c.String());
            AddColumn("dbo.Todoes", "Brand", c => c.String());
            AddColumn("dbo.Todoes", "ImagePath", c => c.String());
            AddColumn("dbo.Todoes", "ThumbnailPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Todoes", "ThumbnailPath");
            DropColumn("dbo.Todoes", "ImagePath");
            DropColumn("dbo.Todoes", "Brand");
            DropColumn("dbo.Todoes", "Name");
        }
    }
}
