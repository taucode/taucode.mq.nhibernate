using TauCode.Mq.NHibernate.LocalTests.App.Client.Dto.Notes;

namespace TauCode.Mq.NHibernate.LocalTests.App.Client;

public interface IAppClient
{
    Task<IList<NoteDto>> GetUserNotes(string userId);
}