using TauCode.Cqrs.Queries;

namespace TauCode.Mq.NHibernate.LocalTests.App.Core.Features.Notes.GetUserNotes;

public class GetUserNotesQuery : Query<GetUserNotesQueryResult>
{
    public string UserId { get; set; }
}