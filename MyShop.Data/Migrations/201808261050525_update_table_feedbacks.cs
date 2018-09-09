namespace MyShop.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_table_feedbacks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feedbacks", "Phone", c => c.String(maxLength: 15));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Feedbacks", "Phone");
        }
    }
}
