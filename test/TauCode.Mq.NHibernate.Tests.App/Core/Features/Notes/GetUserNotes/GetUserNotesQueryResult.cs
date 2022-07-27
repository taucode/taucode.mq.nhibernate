using System.Collections.Generic;

namespace TauCode.Mq.NHibernate.Tests.App.Core.Features.Notes.GetUserNotes
{
    public class GetUserNotesQueryResult
    {
        public IList<NoteDto> Items { get; set; }

        public class NoteDto
        {
            public string Id { get; set; }
            public string UserId { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
        }
    }
}
