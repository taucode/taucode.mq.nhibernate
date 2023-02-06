using FluentNHibernate.Mapping;
using TauCode.Mq.NHibernate.LocalTests.App.Domain.Notes;

namespace TauCode.Mq.NHibernate.LocalTests.App.Persistence.Maps;

public class NoteMap : ClassMap<Note>
{
    public NoteMap()
    {
        this.Id(x => x.Id);
        this.Map(x => x.UserId);
        this.Map(x => x.Subject);
        this.Map(x => x.Body);
    }
}