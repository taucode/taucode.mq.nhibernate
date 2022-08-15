namespace TauCode.Mq.NHibernate.Tests.App.Domain.Notes;

// todo: async
public interface INoteRepository
{
    IList<Note> GetUserNotes(string userId);
    void Save(Note note);
}