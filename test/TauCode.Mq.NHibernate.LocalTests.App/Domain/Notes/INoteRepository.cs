namespace TauCode.Mq.NHibernate.LocalTests.App.Domain.Notes;

// todo: async
public interface INoteRepository
{
    IList<Note> GetUserNotes(string userId);
    void Save(Note note);
}