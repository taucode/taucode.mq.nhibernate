using TauCode.Cqrs.Queries;
using TauCode.Mq.NHibernate.LocalTests.App.Domain.Notes;

namespace TauCode.Mq.NHibernate.LocalTests.App.Core.Features.Notes.GetUserNotes;

public class GetUserNotesQueryHandler : IQueryHandler<GetUserNotesQuery>
{
    private readonly INoteRepository _noteRepository;

    public GetUserNotesQueryHandler(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public void Execute(GetUserNotesQuery query)
    {
        throw new NotSupportedException("Use async overload");
    }

    public Task ExecuteAsync(GetUserNotesQuery query, CancellationToken cancellationToken = default)
    {
        var notes = _noteRepository.GetUserNotes(query.UserId);
        var result = new GetUserNotesQueryResult
        {
            Items = notes
                .Select(x => new GetUserNotesQueryResult.NoteDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Subject = x.Subject,
                    Body = x.Body,
                })
                .ToList(),
        };

        query.SetResult(result);
        return Task.CompletedTask;
    }
}