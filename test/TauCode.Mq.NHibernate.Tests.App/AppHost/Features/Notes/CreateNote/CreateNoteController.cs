using Microsoft.AspNetCore.Mvc;
using TauCode.Cqrs.Commands;
using TauCode.Mq.NHibernate.Tests.App.Core.Features.Notes.CreateNote;

namespace TauCode.Mq.NHibernate.Tests.App.AppHost.Features.Notes.CreateNote;

[ApiController]
public class CreateNoteController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public CreateNoteController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    [Route("api/notes")]
    public async Task<IActionResult> CreateNote(CreateNoteCommand command)
    {
        await _commandDispatcher.DispatchAsync(command, CancellationToken.None);
        return this.Ok();
    }
}