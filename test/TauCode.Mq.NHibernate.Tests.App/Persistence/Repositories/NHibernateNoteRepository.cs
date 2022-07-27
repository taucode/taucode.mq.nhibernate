using NHibernate;
using System.Collections.Generic;
using System.Linq;
using TauCode.Mq.NHibernate.Tests.App.Domain.Notes;

namespace TauCode.Mq.NHibernate.Tests.App.Persistence.Repositories
{
    public class NHibernateNoteRepository : INoteRepository
    {
        private readonly ISession _session;

        public NHibernateNoteRepository(ISession session)
        {
            _session = session;
        }

        public IList<Note> GetUserNotes(string userId)
        {
            return _session
                .Query<Note>()
                .Where(x => x.UserId == userId)
                .ToList();
        }

        public void Save(Note note)
        {
            _session.SaveOrUpdate(note);
        }
    }
}
