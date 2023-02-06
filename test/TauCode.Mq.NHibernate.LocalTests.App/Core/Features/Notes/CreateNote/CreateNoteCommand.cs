using TauCode.Cqrs;

namespace TauCode.Mq.NHibernate.LocalTests.App.Core.Features.Notes.CreateNote;

public class CreateNoteCommand : ICommand
{
    public string UserId { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}