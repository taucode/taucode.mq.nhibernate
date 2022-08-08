using TauCode.Cqrs.Queries;

namespace TauCode.Mq.NHibernate.Tests.App.Core.Features.Notes.GetUserNotes;

public class GetUserNotesQuery : Query<GetUserNotesQueryResult>
{
    public string UserId { get; set; }
}