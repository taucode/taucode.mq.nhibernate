using TauCode.Mq.NHibernate.Tests.App.Client.Messages.Notes;
using TauCode.Mq.NHibernate.Tests.App.Domain.Notes;

namespace TauCode.Mq.NHibernate.Tests.App.Core.Handlers.Notes;

public class NewNoteHandler : AsyncMessageHandlerBase<NewNoteMessage>
{
    private readonly INoteRepository _noteRepository;

    public NewNoteHandler(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public override Task HandleAsync(NewNoteMessage message, CancellationToken cancellationToken)
    {
        var note = new Note(message.UserId, message.Subject, message.Body);
        _noteRepository.Save(note);

        return Task.CompletedTask;
    }
}