using FluentMigrator;

namespace TauCode.Mq.NHibernate.Tests.App.DbMigrations
{
    [Migration(0)]
    public class M0_Baseline : ForwardOnlyMigration
    {
        public override void Up()
        {
            this.Create.Table("note")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey()
                .WithColumn("user_id")
                    .AsString()
                    .NotNullable()
                .WithColumn("subject")
                    .AsString()
                    .NotNullable()
                .WithColumn("body")
                    .AsString()
                    .NotNullable();
        }
    }
}
