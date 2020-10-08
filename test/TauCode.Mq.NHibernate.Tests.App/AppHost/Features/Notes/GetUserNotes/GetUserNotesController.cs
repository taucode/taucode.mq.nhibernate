using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Cqrs.Queries;
using TauCode.Mq.NHibernate.Tests.App.Core.Features.Notes.GetUserNotes;

namespace TauCode.Mq.NHibernate.Tests.App.AppHost.Features.Notes.GetUserNotes
{
    [ApiController]
    public class GetUserNotesController : ControllerBase
    {
        private readonly IQueryRunner _queryRunner;

        public GetUserNotesController(IQueryRunner queryRunner)
        {
            _queryRunner = queryRunner;
        }

        [HttpGet]
        [Route("api/notes/by-user-id/{userId}")]
        public async Task<IActionResult> GetUserNotes([FromRoute]string userId)
        {
            var query = new GetUserNotesQuery
            {
                UserId = userId,
            };

            await _queryRunner.RunAsync(query, CancellationToken.None);

            return this.Ok(query.GetResult());
        }
    }
}
